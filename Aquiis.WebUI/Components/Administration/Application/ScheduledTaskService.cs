using Aquiis.WebUI.Components.PropertyManagement;
using Microsoft.Extensions.Logging;

namespace Aquiis.WebUI.Components.Administration.Application
{
    public class ScheduledTaskService : BackgroundService
    {
        private readonly ILogger<ScheduledTaskService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _dailyTimer;
        private Timer? _hourlyTimer;

        public ScheduledTaskService(
            ILogger<ScheduledTaskService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Task Service is starting.");

            // Calculate time until next midnight for daily tasks
            var now = DateTime.Now;
            var nextMidnight = now.Date.AddDays(1);
            var timeUntilMidnight = nextMidnight - now;

            // Start daily timer (executes at midnight)
            _dailyTimer = new Timer(
                async _ => await ExecuteDailyTasks(),
                null,
                timeUntilMidnight,
                TimeSpan.FromDays(1));

            // Start hourly timer (executes every hour)
            _hourlyTimer = new Timer(
                async _ => await ExecuteHourlyTasks(),
                null,
                TimeSpan.Zero, // Start immediately
                TimeSpan.FromHours(1));

            _logger.LogInformation("Scheduled Task Service started. Daily tasks will run at midnight, hourly tasks every hour.");

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ExecuteDailyTasks()
        {
            _logger.LogInformation("Executing daily tasks at {Time}", DateTime.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var propertyManagementService = scope.ServiceProvider.GetRequiredService<PropertyManagementService>();

                // Calculate daily payment totals
                var today = DateTime.Today;
                var todayPayments = await propertyManagementService.GetPaymentsAsync();
                var dailyTotal = todayPayments
                    .Where(p => p.PaymentDate.Date == today && !p.IsDeleted)
                    .Sum(p => p.Amount);

                _logger.LogInformation("Daily Payment Total for {Date}: ${Amount:N2}", 
                    today.ToString("yyyy-MM-dd"), 
                    dailyTotal);

                // You can add more daily tasks here:
                // - Generate daily reports
                // - Send payment reminders
                // - Check for overdue invoices
                // - Archive old records
                // - Send summary emails to property managers
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing daily tasks");
            }
        }

        private async Task ExecuteHourlyTasks()
        {
            _logger.LogInformation("Executing hourly tasks at {Time}", DateTime.Now);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var propertyManagementService = scope.ServiceProvider.GetRequiredService<PropertyManagementService>();

                // Example hourly task: Check for upcoming lease expirations
                var upcomingLeases = await propertyManagementService.GetLeasesAsync();
                var expiringIn30Days = upcomingLeases
                    .Where(l => l.EndDate >= DateTime.Today && 
                               l.EndDate <= DateTime.Today.AddDays(30) && 
                               !l.IsDeleted)
                    .Count();

                if (expiringIn30Days > 0)
                {
                    _logger.LogInformation("{Count} lease(s) expiring in the next 30 days", expiringIn30Days);
                }

                // You can add more hourly tasks here:
                // - Check for maintenance requests
                // - Update lease statuses
                // - Send notifications
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing hourly tasks");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Task Service is stopping.");

            _dailyTimer?.Change(Timeout.Infinite, 0);
            _hourlyTimer?.Change(Timeout.Infinite, 0);

            await base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _dailyTimer?.Dispose();
            _hourlyTimer?.Dispose();
            base.Dispose();
        }
    }
}
