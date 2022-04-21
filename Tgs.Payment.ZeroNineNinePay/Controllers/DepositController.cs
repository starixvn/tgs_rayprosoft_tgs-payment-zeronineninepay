using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using Tgs.Payment.ZeroNineNinePay.Services;
using Tgs.Payment.ZeroNineNinePay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.ZeroNineNinePay.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly ILogger<DepositController> _logger;

        public DepositController(ILogger<DepositController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("callback")]
        [Produces("text/plain")]
        public async Task<string> DepositCallback(
           [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo?> getDepositFormService,
           [FromServices] ICommandService<CheckSigningCodeCommand> checkSigningCodeService,
           [FromServices] ICommandService<SaveDepositReturnCommand> saveReturnService,
           [FromServices] ICommandService<ApproveDepositFormCommand> approveService,
           [FromServices] ICommandService<RejectDepositFormCommand> rejectService,
           [FromBody] Dictionary<string, object> response)
        {
            var json = JsonSerializer.Serialize(response);

            _logger.LogInformation(json);

            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
            _ = dict.Remove("sign");

            var depositResponse = JsonSerializer.Deserialize<CallbackDepositViewModel>(json)!;

            var depositForm = await getDepositFormService
                .GetAsync(new GetDepositFormQuery(depositResponse.OutTradeNo))
                .ConfigureAwait(false);

            var checkSigningCodeCommand = new CheckSigningCodeCommand(
                dict,
                depositResponse.Sign,
                depositForm!.ApplyInfo!.Settings.MerchantKey);

            await checkSigningCodeService
                .ExecuteAsync(checkSigningCodeCommand)
                .ConfigureAwait(false);

            if (await checkSigningCodeCommand.CommandResultSource.Task.ConfigureAwait(false))
            {
                if (depositForm.Status < PaymentFlowState.WaitApprove && depositResponse.State == 1)
                {
                    var returnData = new ZeroNineNinePayDepositReturn()
                    {
                        MerchantId = depositResponse.MerchantId,
                        PayType = depositResponse.PayType,
                        TradeNo = depositResponse.TradeNo,
                        OutTradeNo = depositResponse.OutTradeNo,
                        Money = depositResponse.Money,
                        MoneyTrue = depositResponse.MoneyTrue,
                        Fee = depositResponse.Fee,
                        State = depositResponse.State,
                        Sign = depositResponse.Sign,
                        SignType = depositResponse.SignType
                    };

                    await saveReturnService
                        .ExecuteAsync(new SaveDepositReturnCommand(depositForm.Id, returnData))
                        .ConfigureAwait(false);

                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    _logger.LogInformation(JsonSerializer.Serialize(returnData, jsonSerializerOptions));

                    await approveService
                        .ExecuteAsync(new ApproveDepositFormCommand(
                            depositForm.Id,
                            depositForm.BrandId,
                            returnData.MoneyTrue))
                        .ConfigureAwait(false);
                }
                else if (depositForm.Status < PaymentFlowState.WaitApprove && depositResponse.State == -1) 
                {
                    var returnData = new ZeroNineNinePayDepositReturn()
                    {
                        MerchantId = depositResponse.MerchantId,
                        PayType = depositResponse.PayType,
                        TradeNo = depositResponse.TradeNo,
                        OutTradeNo = depositResponse.OutTradeNo,
                        Money = depositResponse.Money,
                        MoneyTrue = depositResponse.MoneyTrue,
                        Fee = depositResponse.Fee,
                        State = depositResponse.State,
                        Sign = depositResponse.Sign,
                        SignType = depositResponse.SignType
                    };

                    await saveReturnService
                        .ExecuteAsync(new SaveDepositReturnCommand(depositForm.Id, returnData))
                        .ConfigureAwait(false);

                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    _logger.LogInformation(JsonSerializer.Serialize(returnData, jsonSerializerOptions));

                    await rejectService
                        .ExecuteAsync(new RejectDepositFormCommand(
                            depositForm.Id,
                            depositForm.BrandId))
                        .ConfigureAwait(false);
                }

                _logger.LogInformation("callback executed");

                return "success";
            }
            else
                throw new Exception();
        }
    }
}