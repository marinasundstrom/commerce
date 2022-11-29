﻿using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.CustomerService.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomerServiceClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddTicketsClient(configureClient, builder);

        return services;
    }
    public static IServiceCollection AddTicketsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(TicketsClient), configureClient)
            .AddTypedClient<ITicketsClient>((http, sp) => new TicketsClient(http));

        builder?.Invoke(b);

        return services;
    }
}