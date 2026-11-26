using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// servicios (deben registrarse ANTES de builder.Build())
builder.Services.AddSingleton<InMemoryDatabase>();

//agregar controladores
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordex API", Version = "v0.0.1" });

    // Boton "Authorize" para enviar el header X-Api-Key desde Swagger UI
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Description = "API key requerida en el header X-Api-Key"
    });
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", doc)] = []
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//API KEY TEMPORAL
var ApiKey = "1234567890";

// Autenticacion simple por header (Swagger queda excluido)
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/swagger"))
    {
        await next();
        return;
    }

    if (ctx.Request.Headers["X-Api-Key"] != ApiKey)
    {
        ctx.Response.StatusCode = 401;
        return;
    }

    await next();
});

// Mapear controladores
app.MapControllers();

app.Run();
