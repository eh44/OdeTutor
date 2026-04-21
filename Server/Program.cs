using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers and enable CORS
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 2. Register your Gemini Service
builder.Services.AddHttpClient<LlmTutorService>();
builder.Services.AddScoped<LlmTutorService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 3. Apply CORS and Map the Controller endpoints
app.UseCors("AllowBlazorClient");
app.MapControllers();

// 3. THE KEEPALIVE COMMAND (Make sure this is exactly at the bottom!)
app.Run();