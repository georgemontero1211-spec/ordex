using System.Net.Http.Json;

namespace OrdexAgent;

public class AgentWorker : BackgroundService
{
    private readonly IHttpClientFactory _http;
    private readonly ILogger<AgentWorker> _log;
    private readonly string _agentId = Environment.MachineName;

    public AgentWorker(IHttpClientFactory http, ILogger<AgentWorker> log)
        => (_http, _log) = (http, log);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var client = _http.CreateClient("api");

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await client.PostAsJsonAsync("heartbeat",
                    new { AgentId = _agentId, Machine = Environment.MachineName }, ct);

                var resp = await client.GetAsync($"Agent/{_agentId}/tasks/next", ct);

                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var task = await resp.Content.ReadFromJsonAsync<TaskDto>(ct);
                    await ProcessTask(client, task!, ct);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error comunicando con la API");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), ct);
        }
    }

    private async Task ProcessTask(HttpClient client, TaskDto task, CancellationToken ct)
    {
        _log.LogInformation("Ejecutando tarea {Type} ({Id})", task.Type, task.Id);

        bool success = true;
        string output;

        try
        {
            output = task.Type switch
            {
                "Ping"        => "pong",
                "GetDiskInfo" => string.Join(", ", DriveInfo.GetDrives()
                                    .Where(d => d.IsReady)
                                    .Select(d => $"{d.Name} {d.AvailableFreeSpace / 1_000_000_000}GB libres")),
                "GetHostname" => Environment.MachineName,
                _             => throw new NotSupportedException($"Tarea desconocida: {task.Type}")
            };
        }
        catch (Exception ex)
        {
            success = false;
            output = ex.Message;
        }

        await client.PostAsJsonAsync($"Agent/{_agentId}/tasks/{task.Id}/results",
            new { Success = success, Output = output }, ct);
    }
}

record TaskDto(Guid Id, string Type, string? Payload);
