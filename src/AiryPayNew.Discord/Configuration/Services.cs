﻿using AiryPayNew.Application;
using AiryPayNew.Application.Common;
using AiryPayNew.Discord.Services;
using AiryPayNew.Discord.Services.Messaging;
using AiryPayNew.Infrastructure;
using AiryPayNew.Shared.Settings.AppSettings;
using GenericRateLimiter.Configuration;
using Sqids;

namespace AiryPayNew.Discord.Configuration;

public static class Services
{
    public static IServiceCollection AddServices(
        this IServiceCollection serviceCollection,
        AppSettings appSettings)
    {
        serviceCollection.AddApplication();
        serviceCollection.AddInfrastructure(appSettings);

        serviceCollection.AddRateLimiter<ulong>(options =>
        {
            foreach (var rateLimit in appSettings.Discord.RateLimiters)
            {
                options.AddRateLimiter(rateLimit.Limit, rateLimit.Period, rateLimit.BanPeriod);
            }
        });

        serviceCollection.AddSingleton(new SqidsEncoder<long>(new()
        {
            MinLength = 8
        }));
        
        serviceCollection.AddSingleton(appSettings);

        #region Configure messaging consumers

        serviceCollection.AddSingleton<RoleAssignmentConsumer>();
        serviceCollection.AddHostedService<RoleAssignmentListenerService>();

        #endregion
        
        return serviceCollection;
    }
}