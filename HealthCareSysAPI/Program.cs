using HealthCareSysAPI.CosmosModel;
using HealthCareSysAPI.Custom_Classes;
using HealthCareSysAPI.DBContext;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<HealthCareSysDBContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddSingleton<CosmosClient>(provider =>
{
    string connectionString =builder.Configuration.GetConnectionString("ChatDbContextConnection");
    return new CosmosClient(connectionString);
});

// Add services to the container.
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});
builder.Services.Configure<EmailService>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.AddScoped<messageservice>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CosmosContext>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseWebSockets();
    app.UseCors("AllowAllOrigins");
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseAuthorization();

    app.MapControllers();


    app.Run();
}