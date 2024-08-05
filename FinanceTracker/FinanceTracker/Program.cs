using FinanceTracker.DbClass;
using FinanceTracker.Models.User;
using FinanceTracker.RoleInitiator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database connection
var connectionString = builder.Configuration.GetConnectionString("Localcon");
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Identity configuration
builder.Services.AddIdentity<FinanceUser, IdentityRole>()
    .AddEntityFrameworkStores<FinanceDbContext>()
    .AddDefaultTokenProviders();

// Register RoleInitializer
builder.Services.AddScoped<IRoleInitializer, RoleInitializer>();
builder.Services.AddTransient<EmailService>();
// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
//section
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Call the Role Initializer important when adding role okpar
using (var scope = app.Services.CreateScope())
{
    var roleInitializer = scope.ServiceProvider.GetRequiredService<IRoleInitializer>();
    await roleInitializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Operation}/{action=StartingPage}/{id?}");

app.Run();
