using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace ERPCompanySystem.BackgroundServices
{
    public class JobScheduler
    {
        public static async Task ScheduleJobs()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            // Job 1: Daily Sales Report
            var dailyReportJob = JobBuilder.Create<DailyReportJob>()
                .WithIdentity("dailyReportJob", "group1")
                .Build();

            var dailyReportTrigger = TriggerBuilder.Create()
                .WithIdentity("dailyReportTrigger", "group1")
                .StartNow()
                .WithDailyTimeIntervalSchedule
                (
                    x => x.OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                )
                .Build();

            await scheduler.ScheduleJob(dailyReportJob, dailyReportTrigger);

            // Job 2: Weekly Backup
            var backupJob = JobBuilder.Create<WeeklyBackupJob>()
                .WithIdentity("backupJob", "group1")
                .Build();

            var backupTrigger = TriggerBuilder.Create()
                .WithIdentity("backupTrigger", "group1")
                .StartNow()
                .WithWeeklySchedule
                (
                    x => x.OnDayOfWeek(DayOfWeek.Sunday)
                        .StartingWeeklyAt(TimeOfDay.HourAndMinuteOfDay(2, 0))
                )
                .Build();

            await scheduler.ScheduleJob(backupJob, backupTrigger);
        }
    }

    public class DailyReportJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            // تنفيذ منطق إنشاء تقرير المبيعات اليومي
            return Task.CompletedTask;
        }
    }

    public class WeeklyBackupJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            // تنفيذ منطق نسخ احتياطي قاعدة البيانات
            return Task.CompletedTask;
        }
    }
}
