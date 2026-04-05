using IssueGPT.Api.Data;
using IssueGPT.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Load environment variables
builder.Configuration.AddJsonFile(".env", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
// Add CORS - allow requests from frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add other CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Database Configuration
var connectionString = $"Server={builder.Configuration["DB_SERVER"]},{builder.Configuration["DB_PORT"]};Database={builder.Configuration["DB_NAME"]};User Id={builder.Configuration["DB_USER"]};Password={builder.Configuration["DB_PASSWORD"]};Encrypt=false;TrustServerCertificate=true;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register services
builder.Services.AddScoped<GitHubService>();
builder.Services.AddScoped<AnalysisService>();
builder.Services.AddScoped<CopilotPromptService>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
