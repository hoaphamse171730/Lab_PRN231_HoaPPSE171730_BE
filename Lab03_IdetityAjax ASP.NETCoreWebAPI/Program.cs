using BusinessObjects.Entities;
using DataAccess.Interfaces;
using DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            // ignore navigational cycles
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MyDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<DbContext, MyDbContext>();
builder.Services.AddScoped<IUOW, UOW>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//Remember addScope above UOW, below  DBContext
//		builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrchidDAO, OrchidDAO>();

//// Add services to the container.


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









