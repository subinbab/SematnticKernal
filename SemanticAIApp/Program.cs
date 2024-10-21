using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var builder1 = Kernel.CreateBuilder();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Add Semantic Kernel
builder.Services.AddSingleton<Kernel>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            deploymentName: configuration["DEPLOYMENT_MODEL"],
            endpoint: configuration["AZURE_OPEN_AI_ENDPOINT"],
            apiKey: configuration["AZURE_OPEN_AI_KEY"])
        .Build();
});
var kernel = builder1.Build();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

