using Elogbook.Core;
using Elogbook.Core.Enums;
using Elogbook.Core.Log;
using Elogbook.Data;
using Quartz;
using Quartz.Logging;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Elogbook.Api.Jobs
{
    /// <summary>
    /// JobInitial
    /// </summary>
    public static class JobInitial
    {
        /// <summary>
        /// InitJob
        /// </summary>
        /// <param name="scheduler"></param>
        public static async Task InitJobAsync(IScheduler scheduler)
        {
            using (var context = new ElogbookDataContext())
            {
                var schedule = context.Schedule.ToListAsync().Result;
                _ = scheduler.Clear();

                // Initial Job Default
                foreach (var item in schedule)
                {
                    if (item.Active == 1 && !item.IsDeleted)
                    {
                        switch (item.ScheduleName.ToLower())
                        {
                            case "pmis":
                                //Trigger job to sync data from elogbook to PMIS
                                _ = TriggerJobPmis(scheduler, item.ScheduleValue);
                                break;
                            case "resetticket":
                                //Trigger job to reset ticket number not use
                                _ = TriggerJobResetTicketNumber(scheduler, item.ScheduleValue);
                                break;
                            case "deletelog":
                                //Trigger job to delete log file
                                _ = TriggerJobDeleteLogFiles(scheduler, item.ScheduleValue);
                                break;
                            case "dcs":
                                //Trigger job to import data from DCS to Elogbook
                                _ = TriggerJobDcs(scheduler, item.ScheduleValue);
                                break;
                            case "mailservices":
                                _ = TriggerJobMailServices(scheduler, item.ScheduleValue);
                                break;
                            case "workticket":
                                _ = TriggerJobCreateWorkTicket(scheduler, item.ScheduleValue);
                                break;
                            case "pmisdevice":
                                //Trigger job to sync device data from PMIS to elogbook
                                _ = TriggerJobPmisToElogbook(scheduler, item.ScheduleValue);
                                break;
                            case "autonomousmaintenance":
                                //Trigger job to create data autonomousmaintenance
                                _ = TriggerJobCreateAutonomousMaintenance(scheduler, item.ScheduleValue);
                                break;
                            case "amsendmailnotification":
                                //Trigger job to send mail notification to user
                                _ = TriggerJobSendMailNotificationAM(scheduler, item.ScheduleValue);
                                break;
                            case "autocompleteam":
                                //Trigger job auto complete am
                                _ = TriggerJobAutoCompleteAM(scheduler, item.ScheduleValue);
                                break;
                            case "sendmailreport":
                                //Trigger send mail report
                                _ = TriggerJobSendMailReport(scheduler, item.ScheduleValue);
                                break;
                            case "backupdata":
                                //Trigger send mail report
                                _ = TriggerJobBackupData(scheduler, item.ScheduleValue);
                                break;
                            case "operationparameterwarning":
                                //Trigger sync data operation parameter warning
                                _ = TriggerJobSyncDataOperationParameterWarning(scheduler, item.ScheduleValue);
                                break;
                            case "historycall":
                                // Trigger archive dữ liệu cũ OperationParameters sang HistoryCall
                                _ = TriggerJobHistoryCall(scheduler, item.ScheduleValue);
                                break;
                            case "workticketcompletionreminder":
                                //Trigger gửi mail nhắc nhở, cảnh báo phiếu công tác sắp đến, quá thời hạn theo kế hoạch nhưng chưa hoàn thành
                                _ = TriggerJobWorkTicketCompletionReminder(scheduler, item.ScheduleValue);
                                break;
                            case "registworkticket":
                                //Trigger tạo phiếu đăng ký công việc từ file excel theo lịch trình
                                _ = TriggerJobRegistWorkTicket(scheduler, item.ScheduleValue);
                                break;
                        }
                    }
                }

                await scheduler.Start();
            }
        }

        /// <summary>
        /// Tự động hoàn thành bảo trì tự quả, sau 1h khi người dùng chưa ký hoàn thành
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobAutoCompleteAM(IScheduler scheduler, string cronSchedule)
        {
            if (!string.IsNullOrWhiteSpace(cronSchedule))
            {
                LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "Start auto complete AM");

                //Disable log for Quartz
                LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;

                //Start job
                await scheduler.Start();

                //Define job name and job group
                IJobDetail job = JobBuilder.Create<AmAutoCompleteJob>()
                    .WithIdentity("amAutoCompleteJob", "groupAutoAm")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("amAutoCompleteJob", "groupAutoAm")
                    .StartNow()
                    .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                    .Build();

                //Tell Quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "End auto complete AM");
            }
        }

        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="scheduleValue"></param>
        /// <returns></returns>
        private static async Task TriggerJobMailServices(IScheduler scheduler, string scheduleValue)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<MailServicesJob>()
                .WithIdentity("mailservices", "groupMailServices")
                .Build();

            //Trigger the job to run now, and then repeat by schedule config
            if (string.IsNullOrWhiteSpace(scheduleValue))
            {
                scheduleValue = AppSettings.CRON_SCHEDULE_SEND_MAIL;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("mailservices", "groupMailServices")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(scheduleValue))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Sync data PMIS
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobPmis(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<PmisJob>()
                .WithIdentity("pmis", "groupPmis")
                .Build();

            //Trigger the job to run now, and then repeat every 30 minutes
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_PMIS;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("pmis", "groupPmis")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                //.WithSchedule(CronScheduleBuilder.CronSchedule("0 0/2 * * * ?"))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Sync data PMIS to Elogbook
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobPmisToElogbook(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<PmisToElogbookJob>()
                .WithIdentity("pmisdevice", "groupPmis2")
                .Build();

            //Trigger the job to run now, and then repeat every 30 minutes
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_PMIS_ELOGBOOK;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("pmisdevice", "groupPmis2")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                //.WithSchedule(CronScheduleBuilder.CronSchedule("0 0 1 * * ?"))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Reset ticket
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobResetTicketNumber(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<ResetTicketNumberJob>()
                .WithIdentity("ticketnumber", "tsvh")
                .Build();

            //Trigger the job to run now, and then repeat 1:00 AM everyday
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_TICKET_NUMBER;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("ticketnumber", "tsvh")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                //.WithSchedule(CronScheduleBuilder.CronSchedule("0 0/4 * * * ?"))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Delete log
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobDeleteLogFiles(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<DeleteLogFileJob>()
                .WithIdentity("deletelog", "groupDeleteLog")
                .Build();

            //Trigger the job to run now, and then repeat by schedule config
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_DELETE_LOG;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("deletelog", "groupDeleteLog")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Import data DCS
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobDcs(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<DcsJob>()
                .WithIdentity("dcs", "groupDcs")
                .Build();

            //Trigger the job to run now, and then repeat by schedule config
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_DELETE_LOG;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("dcs", "groupDcs")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Sync data PMIS
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobCreateWorkTicket(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<WorkTicketJob>()
                .WithIdentity("workticket", "groupWorkTicket")
                .Build();

            //Trigger the job to run now, and then repeat every 1 day
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_WORK_TICKET;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("workticket", "groupWorkTicket")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                //.WithSchedule(CronScheduleBuilder.CronSchedule("*/5 * * * * ?"))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Create AutonomousMaintenance
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobCreateAutonomousMaintenance(IScheduler scheduler, string cronSchedule)
        {
            if (!string.IsNullOrWhiteSpace(cronSchedule))
            {
                LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "Start run job create autonomous maintenance");

                //Disable log for Quartz
                LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;

                //Start job
                await scheduler.Start();

                //Define job name and job group
                IJobDetail job = JobBuilder.Create<AmCreateJob>()
                    .WithIdentity("amCreateJob", "groupAmTask")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("amCreateJob", "groupAmTask")
                    .StartNow()
                    .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                    .Build();

                //Tell Quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "End run job create autonomous maintenance");
            }
        }

        /// <summary>
        /// Send mail notification AM
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobSendMailNotificationAM(IScheduler scheduler, string cronSchedule)
        {
            if (!string.IsNullOrWhiteSpace(cronSchedule))
            {
                LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "Start run job create send mail notification AM");

                //Disable log for Quartz
                LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;

                //Start job
                await scheduler.Start();

                //Define job name and job group
                IJobDetail job = JobBuilder.Create<AmNotificationsJob>()
                    .WithIdentity("amSendMailJob", "groupSendMailAm")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("amSendMailJob", "groupSendMailAm")
                    .StartNow()
                    .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                    .Build();

                //Tell Quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                LogCustom.LogMessage(TrackingLogLevel.INFO, "Autonomous Maintenance", "End run job create send mail notification AM");
            }
        }

        /// <summary>
        /// Tự động gửi mail thống kê số liệu 9h chủ nhật hằng tuần
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobSendMailReport(IScheduler scheduler, string cronSchedule)
        {
            if (!string.IsNullOrWhiteSpace(cronSchedule))
            {
                LogCustom.LogMessage(TrackingLogLevel.INFO, "Am Report", "Start send mail report");

                //Disable log for Quartz
                LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;

                //Start job
                await scheduler.Start();

                //Define job name and job group
                IJobDetail job = JobBuilder.Create<SendMailReportJob>()
                    .WithIdentity("sendMailReportJob", "groupAmMailReport")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("sendMailReportJob", "groupAmMailReport")
                    .StartNow()
                    .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                    .Build();

                //Tell Quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                LogCustom.LogMessage(TrackingLogLevel.INFO, "Am Report", "End send mail report");
            }
        }

        /// <summary>
        /// Backup data
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobBackupData(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<BackupDataJob>()
                .WithIdentity("backupDataJob", "groupBackupDataJob")
                .Build();

            //Trigger the job to run now, and then repeat every 1 day
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_BACKUP_DATA;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("backupDataJob", "groupBackupDataJob")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                //.WithSchedule(CronScheduleBuilder.CronSchedule("*/5 * * * * ?"))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Reset ticket
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobSyncDataOperationParameterWarning(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<OperationParameterWarningJob>()
                .WithIdentity("operationparameterwarning", "opw")
                .Build();

            //Trigger the based on time setting
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_SYNC_DATA_OPERATION_PARAMETER_WARNING;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("operationparameterwarning", "opw")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Archive dữ liệu cũ OperationParameters sang OperationParametersHistoryCall
        /// </summary>
        private static async Task TriggerJobHistoryCall(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<HistoryCallJob>()
                .WithIdentity("historycall", "groupHistoryCall")
                .Build();

            //Fallback về AppSettings nếu DB không có cron
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_HISTORY_CALL;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("historycall", "groupHistoryCall")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Gửi mail nhắc nhở, cảnh báo phiếu công tác sắp đến, quá thời hạn theo kế hoạch nhưng chưa hoàn thành
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobWorkTicketCompletionReminder(IScheduler scheduler, string cronSchedule)
        {
            //Disable log for Quartz
            LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;
            //Start job
            await scheduler.Start();

            //Define job name and job group
            IJobDetail job = JobBuilder.Create<WorkTicketCompletionReminder>()
                .WithIdentity("workTicketCompletionReminder", "groupWorkTicketCompletionReminder")
                .Build();

            //Trigger the job to run now, and then repeat 1 minute every hour
            if (string.IsNullOrWhiteSpace(cronSchedule))
            {
                cronSchedule = AppSettings.CRON_SCHEDULE_WORK_TICKET_COMPLETION_NOTIFICATION_HOURS;
            }

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("workTicketCompletionReminder", "groupWorkTicketCompletionReminder")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                .Build();

            //Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        /// <summary>
        /// Tự động tạo phiếu đăng ký công việc từ file excel theo lịch trình
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="cronSchedule"></param>
        /// <returns></returns>
        private static async Task TriggerJobRegistWorkTicket(IScheduler scheduler, string cronSchedule)
        {
            if (!string.IsNullOrWhiteSpace(cronSchedule))
            {
                LogCustom.LogMessage(TrackingLogLevel.INFO, "Schedule Job", "Create Registration Ticket from Excel");

                //Disable log for Quartz
                LogProvider.IsDisabled = AppSettings.IS_TURN_OFF_LOG_QUARTZ;

                //Start job
                await scheduler.Start();

                //Define job name and job group
                IJobDetail job = JobBuilder.Create<RegistWorkTicketJob>()
                    .WithIdentity("RegistWorkTicket", "groupRegistWorkTicket")
                    .Build();

                //Trigger the job to run now
                if (string.IsNullOrWhiteSpace(cronSchedule))
                {
                    cronSchedule = AppSettings.CRON_SCHEDULE_REGIST_WORK_TICKET;
                }

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("RegistWorkTicket", "groupRegistWorkTicket")
                    .StartNow()
                    .WithSchedule(CronScheduleBuilder.CronSchedule(cronSchedule))
                    .Build();

                //Tell Quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                LogCustom.LogMessage(TrackingLogLevel.INFO, "Schedule Job", "Create Registration Ticket from Excel");
            }
        }

    }
}