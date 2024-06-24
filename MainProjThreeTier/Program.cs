using UserProject_BAL.Interfaces;
using UserProject_BAL.Sevices;
using UserProject_DAL.Interfaces;
using UserProject_DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<IDataAccess, FileDataAccess>();
// 1. Access Configuration (consider using environment variables or Azure Key Vault)
IConfiguration configuration = builder.Configuration;

// 2. Register IDataAccess implementation with secure connection string retrieval
builder.Services.AddSingleton<IConfiguration>(configuration); // Provide configuration for secure access
builder.Services.AddSingleton<SqlDataAccess>(new SqlDataAccess(configuration)); // Provide configuration for secure access
builder.Services.AddScoped<IDataAccess, FileDataAccess>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();

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
