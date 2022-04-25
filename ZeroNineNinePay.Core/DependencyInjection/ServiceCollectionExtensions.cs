using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Tgs.Payment.ZeroNineNinePay;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZeroNineNinePay(
        this IServiceCollection services,
        Action<ZeroNineNinePayOptions, IServiceProvider> configure,
        Func<IServiceProvider, DelegatingHandler> httpClientMessageHandlerFactory)
        => services
            .AddOptions<ZeroNineNinePayOptions>()
            .Configure(configure)
            .Services
            .AddHttpClient<ZeroNineNinePayClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<ZeroNineNinePayOptions>>().Value;

                httpClient.BaseAddress = options.PaymentApiUrl;
            })
            .AddHttpMessageHandler(httpClientMessageHandlerFactory)
            .Services
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
            .AddPaymentClient()
            .AddPaymentServiceClient((options, sp) =>
            {
                var ZeroNineNinePayOptions = sp.GetRequiredService<IOptions<ZeroNineNinePayOptions>>().Value;

                options.PaymentServiceBaseUrl = ZeroNineNinePayOptions.PaymentServiceBaseUrl;

                options.JsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = {
                        new JsonStringEnumConverter()
                    }
                };
            })
            .AddTransient<ZeroNineNinePayHasher>()
            .AddSingleton<ZeroNineNinePaySerializer>();
}