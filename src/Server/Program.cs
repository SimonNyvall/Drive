using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Extensions;
using Services.RestoreService;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

builder.UseFileDbContext(connectionString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.UseFileService();
builder.Services.AddScoped<IRestoreService, RestoreService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapFileService();

app.UseHateoas();

app.Run();
