using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MyDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DbContext, MyDbContext>();

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








////Other addScope
//builder.Services.AddScoped<IUOW, UOW>();
//builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//Remember addScope above UOW, below  DBContext
//		builder.Services.AddScoped<IAuthService, AuthService>();

//// Add services to the container.
//builder.Services.AddRazorPages(options =>
//{
//    options.Conventions.AddPageRoute("/Login", "");
//});
//// update index page into @page "/Home"

//// Sau add staticFile
//app.UseSession();

