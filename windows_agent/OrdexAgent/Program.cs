using OrdexAgent;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<AgentWorker>();
builder.Services.AddHttpClient("api", c =>
{
    c.BaseAddress = new Uri("http://localhost:5217/api/");
    c.DefaultRequestHeaders.Add("X-Api-Key", "1234567890");
});

var host = builder.Build();

host.Run();
