using BobcatProjectDatabase;
using BossMasterDatabase;
using EdisonProjectDatabase;
using IxomProjectDatabase;
using Microsoft.EntityFrameworkCore;
using SampleMonoRepo.DotNetApi.ClientConfigurations;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ClientConfigurationResolver>();
builder.Services.AddSingleton<ClientConfiguration>(serviceProvider =>
    serviceProvider.GetRequiredService<ClientConfigurationResolver>().Current);

EdisonClientConfiguration edisonClientConfiguration = new EdisonClientConfiguration();
BobcatClientConfiguration bobcatClientConfiguration = new BobcatClientConfiguration();
IxomClientConfiguration ixomClientConfiguration = new IxomClientConfiguration();

builder.Services.AddDbContext<BossMasterDbContext>((serviceProvider, options) =>
{
    ClientConfiguration clientConfiguration = serviceProvider.GetRequiredService<ClientConfiguration>();
    options.UseSqlServer(clientConfiguration.MasterDbConnectionString);
});
builder.Services.AddDbContext<EdisonProjectDbContext>(options =>
{
    options.UseSqlServer(edisonClientConfiguration.ProjectDbConnectionString);
});
builder.Services.AddDbContext<BobcatProjectDbContext>(options =>
{
    options.UseSqlServer(bobcatClientConfiguration.ProjectDbConnectionString);
});
builder.Services.AddDbContext<IxomProjectDbContext>(options =>
{
    options.UseSqlServer(ixomClientConfiguration.ProjectDbConnectionString);
});

builder.Services.AddControllers();
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

app.MapControllers();

app.Run();
