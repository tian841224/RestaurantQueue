using BackStage.Service;
using Microsoft.Data.Sqlite;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DbService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<CustomerService>();

//µù¥Usqlite
builder.Services.AddScoped(x =>
{
    string SavePath = $" ..\\{builder.Configuration["Sqlite:DbName"]}.db";
    return new SqliteConnection($"Data Source={SavePath}");
});

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
