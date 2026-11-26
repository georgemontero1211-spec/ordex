//Heartbeat model
public record Heartbeat(string AgentId, string Machine);


//Crear un Taks
public record CreateTask(string AgentId, string Type, string? Payload);

//resultado del task
public record TaskResult(bool Success, string? Output);
