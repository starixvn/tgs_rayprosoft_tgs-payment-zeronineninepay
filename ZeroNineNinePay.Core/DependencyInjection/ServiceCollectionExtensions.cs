using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Tgs.Payment.ZeroNineNinePay;
using Tgs.Payment.ZeroNineNinePay.MessageQueue;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZeroNineNinePay(
            this IServiceCollection services,
            Action<ZeroNineNinePayOptions,
            IServiceProvider> configure)
            => services
                .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
                .AddOptions<ZeroNineNinePayOptions>()
                .Configure(configure)
                .Services
                .AddHttpClient<ZeroNineNinePayClient>()
                .ConfigureHttpClient((sp, httpClient) =>
                {
                    var options = sp.GetRequiredService<IOptions<ZeroNineNinePayOptions>>().Value;

                    httpClient.BaseAddress = options.PaymentApiUrl;
                })
                .Services
                //.AddSingleton<ZeroNineNinePaySerializer>()
                .AddSingleton<ZeroNineNinePayHasher>()
                .AddPaymentClient()
                .AddPaymentServiceClient((options, sp) =>
                {
                    var ZeroNineNinePayOptions = sp.GetRequiredService<IOptions<ZeroNineNinePayOptions>>().Value;

                    options.PaymentServiceBaseUrl = ZeroNineNinePayOptions.PaymentServiceBaseUrl;

                    options.JsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                    };
                })
                .AddMessageQueue()
                .AddNatsMessageQueue(config =>
                {
                    _ = config
                        .ConfigQueueOptions((options, sp) =>
                        {
                            var zeroNineNinePayOptions = sp.GetRequiredService<IOptions<ZeroNineNinePayOptions>>().Value;

                            options.Url = zeroNineNinePayOptions.NatsUrl;
                        })
                        .AddProcessor<DepositProcessor>("tgs.payment.zeronineninepay.deposit", "tgs.payment.zeronineninepay.deposit")
                        .AddProcessor<WithdrawProcessor>("tgs.payment.zeronineninepay.withdraw", "tgs.payment.zeronineninepay.withdraw");
                })
                .ClearAllExchanges()
                .AddNatsGlobPatternExchange("*")
                .Services;
    }
}