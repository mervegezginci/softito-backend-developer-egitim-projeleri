using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using seyahat_projesi.Data;
using seyahat_projesi.Data.Repository;
using seyahat_projesi.Data.Repository.IRepository;
using seyahat_projesi.Model;
using seyahat_projesi.Services;
using System.Text.Json.Serialization;

// Register PDFsharp Font Resolver
PdfSharp.Fonts.GlobalFontSettings.FontResolver = new seyahat_projesi.Services.MyFontResolver();

var builder = WebApplication.CreateBuilder(args);

// Add Controllers and MVC Views
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Prevent infinite loops on serialization of relational data
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Add Memory Caching
builder.Services.AddMemoryCache();

// Add HttpContext Accessor (required for extracting client IP in LogService)
builder.Services.AddHttpContextAccessor();

// Register LogService
builder.Services.AddScoped<LogService>();

// Register DapperRepository
builder.Services.AddScoped<DapperRepository>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configure SQL Server Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Simple testing constraints
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true; // E-postaların benzersiz olmasını zorunlu kıl
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Authentication Cookies Paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
});

var app = builder.Build();

// Auto-run Migrations and Seeding on Server Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // This will apply migrations automatically
        Console.Write("Running database migrations...");
        await context.Database.MigrateAsync();
        Console.WriteLine(" Done.");

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        Console.Write("Seeding database initial records...");
        await DbInitializer.SeedAsync(context, userManager, roleManager);
        Console.WriteLine(" Done.");

        Console.Write("Seeding 20 records for each table...");
        await DbSeeder.Seed20Async(context, userManager);
        Console.WriteLine(" Done.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}

// HTTP pipeline configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
