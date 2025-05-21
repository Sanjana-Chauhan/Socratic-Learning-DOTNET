using Microsoft.EntityFrameworkCore;
using SOCRATIC_LEARNING_DOTNET.Data;
using SOCRATIC_LEARNING_DOTNET.Endpoints;
using SOCRATIC_LEARNING_DOTNET.Interfaces;
using SOCRATIC_LEARNING_DOTNET.Services;

var builder = WebApplication.CreateBuilder(args);

// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient for GroqService
builder.Services.AddHttpClient<GroqService>();

// PostgreSQL connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Dependency injection for services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<GroqService>();

var app = builder.Build();

app.UseCors("AllowAll");
// Dev tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints
app.MapChatEndpoints();
app.Run();
