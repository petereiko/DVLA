using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.BackgroundJobModule
{
    public static class JobRegistrations
    {
        public static IServiceCollection AddBackgroundJobRegistrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(q =>
            {
                var visualAssessmentResultJob = new JobKey(nameof(VisualAssessmentResultJob));

                q.AddJob<VisualAssessmentResultJob>(opts => opts
                    .WithIdentity(visualAssessmentResultJob)
                    .WithDescription("Auto debit Savings")
                );

                q.AddTrigger(opts => opts
                    .ForJob(visualAssessmentResultJob)
                    .WithIdentity("VisualAssessmentResultJob-Trigger")
                    .WithCronSchedule("0 */2 * * * ?") // every 2 mins (Quartz uses 6-part cron)
                    .WithDescription("Runs every 2 minutes")
                );

                var hardDeleteVisualAssessmentResultjobKey = new JobKey(nameof(HardDeleteVisualAssessmentResultJob));
                q.AddJob<HardDeleteVisualAssessmentResultJob>(opts => opts
                    .WithIdentity(hardDeleteVisualAssessmentResultjobKey)
                    .WithDescription("Queries and processes pending deposits")
                );
                q.AddTrigger(opts => opts
                    .ForJob(hardDeleteVisualAssessmentResultjobKey)
                    .WithIdentity("HardDeleteVisualAssessmentResultJob-Trigger")
                    .WithCronSchedule("0 0 0 * * ?") //daily // every 2 mins (Quartz uses 6-part cron)
                    .WithDescription("Runs every day")
                );

                var sendEmailJobKey = new JobKey(nameof(SendEmailJob));
                q.AddJob<SendEmailJob>(opts => opts
                    .WithIdentity(sendEmailJobKey)
                    .WithDescription("Processes pending Email kyc from Dojah Webhook")
                );
                q.AddTrigger(opts => opts
                    .ForJob(sendEmailJobKey)
                    .WithIdentity("SendEmailJob-Trigger")
                    .WithCronSchedule("0 */2 * * * ?") // every 2 mins (Quartz uses 6-part cron)
                    .WithDescription("Runs every 2 minutes")
                );


                var updateAuthDocJobKey = new JobKey(nameof(UpdateAuthDocJob));

                q.AddJob<UpdateAuthDocJob>(opts => opts
                    .WithIdentity(updateAuthDocJobKey)
                    .WithDescription("update Authenticating Doctor details")
                );

                q.AddTrigger(opts => opts
                    .ForJob(updateAuthDocJobKey)
                    .WithIdentity("UpdateAuthDocJob-Trigger")
                    .WithCronSchedule("0 */2 * * * ?") // every 2 mins (Quartz uses 6-part cron)
                    .WithDescription("Runs every 2 minutes")
                );


                var assessmentResultBackJobbKey = new JobKey(nameof(BackupVisualAssessmentResultJob));

                q.AddJob<BackupVisualAssessmentResultJob>(opts => opts
                    .WithIdentity(assessmentResultBackJobbKey)
                    .WithDescription("Query BudPay")
                );

                q.AddTrigger(opts => opts
                    .ForJob(assessmentResultBackJobbKey)
                    .WithIdentity("BackupVisualAssessmentResultJob-Trigger")
                    .WithCronSchedule("0 0 0 * * ?") // daily
                );

                var syncOptometrsitFirmJobKey = new JobKey(nameof(SyncOptometristFirmJob));
                q.AddJob<SyncOptometristFirmJob>(opts => opts
                    .WithIdentity(syncOptometrsitFirmJobKey)
                    .WithDescription("Syncs Optometrist Firms")
                );

                q.AddTrigger(opts => opts
                    .ForJob(syncOptometrsitFirmJobKey)
                    .WithIdentity("SyncOptometristFirmJob-Trigger")
                    .WithCronSchedule("0 0 0 * * ?") // daily
                );

                //var maturedSavingJobKey = new JobKey(nameof(MaturedSavingJob));
                //q.AddJob<MaturedSavingJob>(opts => opts
                //    .WithIdentity(maturedSavingJobKey)
                //    .WithDescription("Processes Interests for active savings only")
                //);
                //q.AddTrigger(opts => opts
                //    .ForJob(maturedSavingJobKey)
                //    .WithIdentity("MaturedSavingJob-Trigger")
                //    .WithCronSchedule("0 0 0 * * ?") //daily
                //);

                //var savingsInterestJobKey = new JobKey(nameof(SavingsInterestJob));
                //q.AddJob<SavingsInterestJob>(opts => opts
                //    .WithIdentity(savingsInterestJobKey)
                //    .WithDescription("Processes Interests for active savings only")
                //);
                //q.AddTrigger(opts => opts
                //    .ForJob(savingsInterestJobKey)
                //    .WithIdentity("SavingsInterestJob-Trigger")
                //    .WithCronSchedule("0 0 0 * * ?") //daily
                //);
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }
    }
}
