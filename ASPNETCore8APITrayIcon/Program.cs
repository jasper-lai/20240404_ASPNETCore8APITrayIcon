using Serilog.Events;
using Serilog;
using System.Runtime.InteropServices;
using ASPNETCore8APITrayIcon;
using Microsoft.Extensions.Hosting;
using static System.Net.WebRequestMethods;

#region WIN32 API Import
[DllImport("kernel32.dll", SetLastError = true)]
static extern bool AllocConsole();

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool AttachConsole(int dwProcessId);
#endregion

#region Serilog ���]�m
// �]�m
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("D:/Temp/logs/WebApi-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
#endregion

var builder = WebApplication.CreateBuilder(args);

#region �ĥ� Serilog �@�� Log ���u��
// Configure your application
builder.Logging.ClearProviders(); // Clear default logging providers
builder.Logging.AddSerilog(); // Use Serilog for logging
#endregion

#region �]�w Kestrel listen �b�H�U�X�� URL
// �g���b�{����, �i�H�קK�ϥΪ̦ۦ��� appsettings.json ���]�w
builder.WebHost.UseUrls(
    "http://localhost:5000",
    "https://localhost:5001"
    //"http://192.168.224.44:5000",
    //"https://192.168.224.44:5001"
);
#endregion

#region �]�m CORS: �}�� "�~���������Ψt��" �s�� localhost
builder.Services.AddCors(options =>
{
    //����1: ���\�զW�� "�~���������Ψt��" 
    //(1) ���s�������o http://example.com ������. 
    //(2) �ӭ������@�� button, ���U����, �| HTTP POST �� http://localhost:5000 �� https://localhost:5001
    //(3) �n�]�m�H�U�զW��, �~��s�� http://localhost:5000 �� https://localhost:5001
    options.AddPolicy("MyAllowedOrigins", policy =>
    {
        string[] allowedCORS =
        //[   "http://localhost:",
        //    "https://example.com",
        //];
        [   "http://localhost:5216",
            "https://localhost:7112",
        ];

    policy.WithOrigins(allowedCORS)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    //����2: ���\���� "�~���������Ψt��" 
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
#endregion

// Add services to the container.
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

#region �ҥ� CORS middleware
app.UseCors("MyAllowedOrigins"); // Apply CORS policy
//app.UseCors("AllowAll"); // Apply CORS policy
#endregion

#region ���� Http ��V�� Https middleware
// app.UseHttpsRedirection();
#endregion

app.UseAuthorization();
app.MapControllers();

var isConsoleMode = args.Contains("--console");

// Check if the application should show a tray icon
if (isConsoleMode)
{
    // ���n: �]����X�� exe �O WinExe: �Y�����t console ������ windows �{��, �Ҧp: Window Form
    // �p�G�O Console Mode,
    // (1) �p�G�������O console, �N�������ӥ�. �Ҧp: �R�O�C���� ����.
    // (2) �p�G���������O console, �N�t�m�@�ӷs�� console ����. �Ҧp: VS2022 ���氻��.
    if (!AttachConsole(-1)) // Attach to a parent process console
    {
        AllocConsole(); // Alloc a new console if none available
    }
    Log.Information("=== in Console Mode ===");
    app.Run();
}
else
{
    Log.Information("=== in TrayIcon Mode ===");
    Application.Run(new TrayApplicationContext(app));
}
