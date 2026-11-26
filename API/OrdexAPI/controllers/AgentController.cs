using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly InMemoryDatabase _db;

    public AgentController(InMemoryDatabase db)
    {
        _db = db;
    }


    [HttpPost("heartbeat")]
    public IActionResult Heartbeat([FromBody] Heartbeat hb)
    {
        // Aquí puedes manejar el heartbeat recibido
        // Por ejemplo, podrías guardar la información en una base de datos o actualizar el estado del agente
        Console.WriteLine($"[{DateTime.UtcNow}] Heartbeat de {hb.Machine} (ID: {hb.AgentId}) recibido.");
        return Ok(new { message = "Heartbeat recibido" });
    }

    [HttpGet("{agentId}/tasks/next")]
    public IActionResult GetNextTask(string agentId)
    {
        var next = _db.Tasks
            .Where(t => t.AgentId == agentId && t.Status == "Pending")
            .OrderBy(t => t.CreatedAt)
            .FirstOrDefault();

        if (next is null) return NotFound(new { message = "No hay tareas pendientes para este agente." });

        return Ok(next);
    }

    [HttpGet("{agentId}/tasks")]
    public IActionResult GetAgentTasks(string agentId)
    {
        var tasks = _db.Tasks
            .Where(t => t.AgentId == agentId)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        return Ok(tasks);
    }

    // Endpoint del agente para mandar el resultado de la tarea
    [HttpPost("{agentId}/tasks/{taskId}/results")]
    public IActionResult SubmitTaskResult(string agentId, string taskId, [FromBody] TaskResult result)
    {
        var task = _db.Tasks.FirstOrDefault(t => t.Id.ToString() == taskId && t.AgentId == agentId);

        if (task is null) return NotFound(new { message = "Tarea no encontrada para este agente." });

        task.Status = result.Success ? "Completed" : "Failed";
        task.Output = result.Output;
        task.CompletedAt = DateTime.UtcNow;

        return Ok(new { message = "Resultado de la tarea recibido", task = task });
    }
}