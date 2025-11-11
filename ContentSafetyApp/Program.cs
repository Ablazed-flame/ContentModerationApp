using Azure;
using Azure.AI.ContentSafety;
using ContentModerationApp.Data;
using ContentModerationApp.Helpers;
using ContentModerationApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddSingleton<AzureContentSafetyService>();
builder.Services.AddSingleton(new ContentSafetyClient(
    new Uri(builder.Configuration["AzureContentSafety:Endpoint"]),
    new AzureKeyCredential(builder.Configuration["AzureContentSafety:ApiKey"])
));
builder.Services.AddHttpClient<CohereApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CohereApi:Endpoint"]);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["CohereApi:ApiKey"]}");
});
builder.Services.AddScoped<IContentModerationService, ContentModerationService>();
builder.Services.AddScoped<IContentSubmissionService, ContentSubmissionService>();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
   // options.AccessDeniedPath = "/Identity/Account/AccessDenied"; 
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.SeedRolesAndAdminAsync(scope.ServiceProvider);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
