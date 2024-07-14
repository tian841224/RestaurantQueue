using BackStage.Service;
using CommonLibrary.Interface;
using CommonLibrary.Service;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("±Ò°Êµ{¦¡");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<IDbService, DbService>();
    builder.Services.AddScoped<ReportService>();
    builder.Services.AddScoped<CustomerService>();

    //µù¥Usqlite
    //builder.Services.AddScoped(x =>
    //{
    //    string SavePath = $" ..\\{builder.Configuration["Sqlite:DbName"]}.db";
    //    return new SqliteConnection($"Data Source={SavePath}");
    //});

    //µù¥UMysql
    builder.Services.AddScoped(x =>
    {
        return new MySqlConnection($"{builder.Configuration["ConnectionStrings:MySQL"]}");
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

}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}