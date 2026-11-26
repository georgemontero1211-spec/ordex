using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class AgentTaskController : ControllerBase
{
    private readonly InMemoryDatabase _db;

    public AgentTaskController(InMemoryDatabase db)
    {
        _db = db;
    }

    [HttpGet("tasks")]
    public IActionResult GetTasks()
    {
        return Ok(_db.Tasks);
    }

    

    //Crear un Taks
    [HttpPost("create")]
    public IActionResult CreateTask([FromBody] CreateTask req)
    {
        var task = new AgentTask
        {
            Id = Guid.NewGuid(),
            AgentId = req.AgentId,
            Type = req.Type,
            Payload = req.Payload,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        
        //Agregar la tarea a la base de datos en memoria
        _db.Tasks.Add(task);
        
        return Ok(new { message = "Tarea creada", task = task });
    }
}