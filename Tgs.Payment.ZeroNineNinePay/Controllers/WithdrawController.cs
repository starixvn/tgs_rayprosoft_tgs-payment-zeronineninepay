using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using Tgs.Payment.ZeroNineNinePay.Services;
using Tgs.Payment.ZeroNineNinePay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.ZeroNineNinePay.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        private readonly ILogger<WithdrawController> _logger;

        public WithdrawController(ILogger<WithdrawController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("callback")]
        [Produces("text/plain")]
        public async Task<string> WithdrawCallback(
            [FromServices] IGetService<GetWithdrawFormQuery, Services.WithdrawFormInfo?> getWithdrawFormService,
            [FromServices] ICommandService<CheckSigningCodeCommand> checkSigningCodeService,
            [FromServices] ICommandService<SaveWithdrawReturnCommand> saveReturnService,
            [FromServices] IPaymentClient paymentClient,
            [FromBody] Dictionary<string, object> response)
        {
            var json = JsonSerializer.Serialize(response);

            _logger.LogInformation("Withdraw callback:{json}", json);

            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;

            var withdrawResponse = JsonSerializer.Deserialize<CallbackWithdrawViewModel>(json)!;

            var withdrawForm = await getWithdrawFormService
                .GetAsync(new GetWithdrawFormQuery(withdrawResponse.OutTradeNo))
                .ConfigureAwait(false);

            if (withdrawForm == null)
                return "Withdraw form not found.";

            var checkSigningCodeCommand = new CheckSigningCodeCommand(
                dict,
                withdrawResponse.Sign,
                withdrawForm!.ApplyInfo!.Settings.MerchantKey
                );

            await checkSigningCodeService
                .ExecuteAsync(checkSigningCodeCommand)
                .ConfigureAwait(false);

            if (await checkSigningCodeCommand.CommandResultSource.Task.ConfigureAwait(false))
            {
                var returnData = new ZeroNineNinePayWithdrawReturn()
                {
                    MerchantId = withdrawResponse.MerchantId,
                    TradeNo = withdrawResponse.TradeNo,
                    OutTradeNo = withdrawResponse.OutTradeNo,
                    Money = withdrawResponse.Money,
                    Fee = withdrawResponse.Fee,
                    State = withdrawResponse.State,
                    Sign = withdrawResponse.Sign,
                    SignType = withdrawResponse.SignType
                };

                if (withdrawResponse.State == 1)
                {
                    var formStatus = withdrawForm.Status;
                    if (withdrawForm.Status == PaymentFlowState.Approved)
                        formStatus = PaymentFlowState.Paid;

                    var saveWithdrawCommand = new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        returnData,
                        returnData.Money);

                    await saveReturnService
                        .ExecuteAsync(saveWithdrawCommand)
                        .ConfigureAwait(false);

                    await paymentClient
                        .PaidWithdrawAsync(
                            withdrawForm.Id,
                            withdrawForm.BrandId,
                            HttpContext.RequestAborted)
                        .ConfigureAwait(false);
                }
                else if (withdrawResponse.State == -1)
                {
                    var saveWithdrawCommand = new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        returnData,
                        null);

                    await saveReturnService
                        .ExecuteAsync(saveWithdrawCommand)
                        .ConfigureAwait(false);

                    await paymentClient
                        .RejectWithdrawAsync(
                            withdrawForm.Id,
                            withdrawForm.BrandId)
                        .ConfigureAwait(false);
                }
                else
                {
                    var saveWithdrawCommand = new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        returnData,
                        null);

                    await saveReturnService
                        .ExecuteAsync(saveWithdrawCommand)
                        .ConfigureAwait(false);
                }

                _logger.LogInformation(JsonSerializer.Serialize(returnData));

                _logger.LogInformation("callback executed");

                return "success";
            }
            else
                throw new Exception();
        }
    }
}