namespace OrdexAgent;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct )
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var info = new
                {
                    Machine = Environment.MachineName,
                    User = Environment.UserName,
                    OS = Environment.OSVersion,
                    Time = DateTimeOffset.UtcNow
                };
                
                logger.LogInformation("Worker running at: {time} with info: {@info}", DateTimeOffset.Now, info);

            } catch (Exception ex)
            {
                logger.LogError(ex, "Error enviando heartbeat: {message}", ex.Message);
            }

        }
    }
}
