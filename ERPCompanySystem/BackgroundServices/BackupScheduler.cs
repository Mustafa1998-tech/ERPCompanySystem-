using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace ERPCompanySystem.BackgroundServices
{
    public class BackupScheduler
    {
        public static async Task ScheduleBackups()
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();

            // نسخ احتياطي كامل يومياً في الساعة 2 صباحاً
            var fullBackupJob = JobBuilder.Create<FullBackupJob>()
                .WithIdentity("fullBackupJob", "group1")
                .Build();

            var fullBackupTrigger = TriggerBuilder.Create()
                .WithIdentity("fullBackupTrigger", "group1")
                .StartNow()
                .WithDailyTimeIntervalSchedule
                (
                    x => x.OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(2, 0))
                )
                .Build();

            await scheduler.ScheduleJob(fullBackupJob, fullBackupTrigger);

            // نسخ احتياطي تفاضلي كل 6 ساعات
            var diffBackupJob = JobBuilder.Create<DifferentialBackupJob>()
                .WithIdentity("diffBackupJob", "group1")
                .Build();

            var diffBackupTrigger = TriggerBuilder.Create()
                .WithIdentity("diffBackupTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule
                (
                    x => x
                        .WithIntervalInHours(6)
                        .RepeatForever()
                )
                .Build();

            await scheduler.ScheduleJob(diffBackupJob, diffBackupTrigger);

            // نسخ احتياطي للسجلات كل ساعة
            var logBackupJob = JobBuilder.Create<TransactionLogBackupJob>()
                .WithIdentity("logBackupJob", "group1")
                .Build();

            var logBackupTrigger = TriggerBuilder.Create()
                .WithIdentity("logBackupTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule
                (
                    x => x
                        .WithIntervalInHours(1)
                        .RepeatForever()
                )
                .Build();

            await scheduler.ScheduleJob(logBackupJob, logBackupTrigger);
        }
    }

    public class FullBackupJob : IJob
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<FullBackupJob> _logger;

        public FullBackupJob(IBackupService backupService, ILogger<FullBackupJob> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation("Starting full backup");
                await _backupService.CreateBackupAsync(BackupType.Full, "Scheduled full backup");
                _logger.LogInformation("Full backup completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in full backup");
            }
        }
    }

    public class DifferentialBackupJob : IJob
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<DifferentialBackupJob> _logger;

        public DifferentialBackupJob(IBackupService backupService, ILogger<DifferentialBackupJob> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation("Starting differential backup");
                await _backupService.CreateBackupAsync(BackupType.Differential, "Scheduled differential backup");
                _logger.LogInformation("Differential backup completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in differential backup");
            }
        }
    }

    public class TransactionLogBackupJob : IJob
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<TransactionLogBackupJob> _logger;

        public TransactionLogBackupJob(IBackupService backupService, ILogger<TransactionLogBackupJob> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation("Starting transaction log backup");
                await _backupService.CreateBackupAsync(BackupType.TransactionLog, "Scheduled transaction log backup");
                _logger.LogInformation("Transaction log backup completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in transaction log backup");
            }
        }
    }
}
