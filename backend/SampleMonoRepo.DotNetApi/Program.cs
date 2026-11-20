using SampleMonoRepo.DotNetApi.ClientConfigurations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ClientConfigurationResolver>();
builder.Services.AddSingleton<ClientConfiguration>(serviceProvider =>
    serviceProvider.GetRequiredService<ClientConfigurationResolver>().Current);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:20000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

ClientConfiguration clientConfiguration = app.Services.GetRequiredService<ClientConfiguration>();
app.Logger.LogInformation("Loaded client configuration {ClientCode}.", clientConfiguration.ClientCode);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.MapGet("/api/hello", () => Results.Text("Hello World"))
    .WithName("GetHello");

app.Run();
