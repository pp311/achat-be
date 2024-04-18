using Quartz;
using AChat.Application.BackgroundServices;

namespace AChat.Extensions;

public static class BackgroundJobExtension
{
	public static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            services.AddQuartz(options =>
            {
                // Run at 12:00 PM every day
                var removeRefreshTokenJob = new JobKey("RemoveRefreshTokenJob");
                options.AddJob<ClearRefreshTokenJob>(removeRefreshTokenJob)
                       .AddTrigger(trigger =>
                            trigger.ForJob(removeRefreshTokenJob)
                                .WithCronSchedule("0 0 12 * * ? *"));
            });
    
            services.AddQuartzHostedService(options =>
            {
                options.AwaitApplicationStarted = true;
                options.WaitForJobsToComplete = true;
            });
                
            return services;
        }
}