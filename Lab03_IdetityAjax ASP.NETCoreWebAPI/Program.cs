using BusinessObjects.Entities;
using DataAccess.Interfaces;
using DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Repositories;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddScoped<DbContext, MyDbContext>();
builder.Services.AddScoped<IUOW, UOW>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//Remember addScope above UOW, below  DBContext
//		builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrchidDAO, OrchidDAO>();
builder.Services.AddScoped<IAccountDAO, AccountDAO>();
builder.Services.AddScoped<IOrderDAO, OrderDAO>();
builder.Services.AddScoped<IOrderDetailDAO, OrderDetailDAO>();
builder.Services.AddScoped<IRoleDAO, RoleDAO>();


//// Add services to the container.


builder.Services.AddCors(opts =>
  opts.AddPolicy("AllowAll", p =>
    p.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader()));

var app = builder.Build();

app.UseCors("AllowAll");

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









