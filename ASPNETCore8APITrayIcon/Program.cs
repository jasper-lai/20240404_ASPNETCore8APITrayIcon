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

#region Serilog 的設置
// 設置
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("D:/Temp/logs/WebApi-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
#endregion

var builder = WebApplication.CreateBuilder(args);

#region 採用 Serilog 作為 Log 的工具
// Configure your application
builder.Logging.ClearProviders(); // Clear default logging providers
builder.Logging.AddSerilog(); // Use Serilog for logging
#endregion

#region 設定 Kestrel listen 在以下幾個 URL
// 寫死在程式裡, 可以避免使用者自行更改 appsettings.json 的設定
builder.WebHost.UseUrls(
    "http://localhost:5000",
    "https://localhost:5001"
    //"http://192.168.224.44:5000",
    //"https://192.168.224.44:5001"
);
#endregion

#region 設置 CORS: 開放 "外部網頁應用系統" 存取 localhost
builder.Services.AddCors(options =>
{
    //釋例1: 允許白名單 "外部網頁應用系統" 
    //(1) 由瀏覽器取得 http://example.com 的頁面. 
    //(2) 該頁面有一個 button, 按下之後, 會 HTTP POST 到 http://localhost:5000 或 https://localhost:5001
    //(3) 要設置以下白名單, 才能存取 http://localhost:5000 或 https://localhost:5001
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

    //釋例2: 允許全部 "外部網頁應用系統" 
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

#region 啟用 CORS middleware
app.UseCors("MyAllowedOrigins"); // Apply CORS policy
//app.UseCors("AllowAll"); // Apply CORS policy
#endregion

#region 停用 Http 轉向至 Https middleware
// app.UseHttpsRedirection();
#endregion

app.UseAuthorization();
app.MapControllers();

var isConsoleMode = args.Contains("--console");

// Check if the application should show a tray icon
if (isConsoleMode)
{
    // 重要: 因為輸出的 exe 是 WinExe: 係指不含 console 視窗的 windows 程式, 例如: Window Form
    // 如果是 Console Mode,
    // (1) 如果母視窗是 console, 就直接拿來用. 例如: 命令列提示 視窗.
    // (2) 如果母視窗不是 console, 就配置一個新的 console 視窗. 例如: VS2022 執行偵錯.
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
