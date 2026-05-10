using NextHorizon.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextHorizon.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container 
builder.Services.AddControllersWithViews();

// Register your custom services
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAgentDashboardService, AgentDashboardService>();
builder.Services.AddScoped<IAgentRankingService, AgentRankingService>();
builder.Services.AddScoped<IQaAgentTicketsService, QaAgentTicketsService>();
builder.Services.AddScoped<IQaAgentsService, QaAgentsService>();
builder.Services.AddScoped<IQaDashboardService, QaDashboardService>();
builder.Services.AddScoped<IQaEvaluationService, QaEvaluationService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IQaRatingQueueService, QaRatingQueueService>();
builder.Services.AddScoped<IQaRatedHistoryService, QaRatedHistoryService>();
builder.Services.AddScoped<IQaResolvedTicketsService, QaResolvedTicketsService>();
builder.Services.AddScoped<IQaReviewService, QaReviewService>();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".NextHorizon.Session";
});

var app = builder.Build(); 

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseSession(); 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
