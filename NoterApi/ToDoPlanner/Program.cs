using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Repository.CQRS.Commands;
using Repository.CQRS.Queries;
using Repository.Infrastructure;
using Repository.Repositories;
using Service.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Add services to the container.
builder.Services.AddScoped<IFileTypeRepository, FileTypeRepository>();
builder.Services.AddScoped<IFileTypeQuery, FileTypeQuery>();
builder.Services.AddScoped<IFileTypeCommand, FileTypeCommand>();

builder.Services.AddScoped<IFileTypeService, FileTypeService>();



builder.Services.AddScoped<IRecordFileRepository, RecordFileRepository>();
builder.Services.AddScoped<IRecordFileQuery, RecordFileQuery>();
builder.Services.AddScoped<IRecordFileCommand, RecordFileCommand>();

builder.Services.AddScoped<IRecordFileService, RecordFileService>();

builder.Services.AddScoped<Database>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//app.MigrateDatabase();
app.Run();
