using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Data;
using ForumDigitalParadise.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ForumDigitalParadise.Data.Services;
using ForumDigitalParadise.Shared.Hubs;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IForum, ForumService>();
builder.Services.AddScoped<IPost, PostService>();
builder.Services.AddScoped<IUpload, UploadService>();
builder.Services.AddScoped<IPostReply, PostReplyService>();
builder.Services.AddScoped<IFollower, FollowerService>();
builder.Services.AddScoped<IForumFollower, ForumFollowerService>();
builder.Services.AddScoped<INotification, NotificationService>();
builder.Services.AddScoped<IPostLike, PostLikeService>();
builder.Services.AddScoped<ISwitch, SwitchService>();
builder.Services.AddScoped<IApplicationUser, ApplicationUserService>();

builder.Services.AddSignalR();

builder.Services.AddTransient<DataSeeder>();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var hostEnvironment = services.GetRequiredService<IWebHostEnvironment>();
    var dataSeeder = services.GetRequiredService<DataSeeder>();

    try
    {
        DataSeeder.Equals(services, hostEnvironment);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }

    await dataSeeder.SeedSuperUser();
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();

