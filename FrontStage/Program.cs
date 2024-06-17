using FrontStage.Service;
using Microsoft.Data.Sqlite;
using Microsoft.OpenApi.Models;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<DbService>();
builder.Services.AddScoped<QueueService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // 獲取 XML 註解文件的路徑
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // 告訴 Swagger 使用 XML 註解文件
    c.IncludeXmlComments(xmlPath);
});

//註冊sqlite
builder.Services.AddScoped(x =>
{
    string SavePath = $" {Environment.CurrentDirectory}\\{builder.Configuration["Sqlite:DbName"]}.db";
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
