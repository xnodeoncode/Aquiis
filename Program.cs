using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Components;
using Aquiis.SimpleStart.Components.Account;
using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Components.PropertyManagement;
using Aquiis.SimpleStart.Components.Administration.Application;
using Aquiis.SimpleStart.Services;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

    //Added for session state
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
    

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString), ServiceLifetime.Scoped);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => {

    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();



// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Events.OnSignedIn = async context =>
    {
        // Track user login
        if (context.Principal != null)
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(context.Principal);
            if (user != null)
            {
                user.PreviousLoginDate = user.LastLoginDate;
                user.LastLoginDate = DateTime.UtcNow;
                user.LoginCount++;
                user.LastLoginIP = context.HttpContext.Connection.RemoteIpAddress?.ToString();
                await userManager.UpdateAsync(user);
            }
        }
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        // Check if user is locked out and redirect to lockout page
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = userManager.GetUserAsync(context.HttpContext.User).Result;
            if (user != null && userManager.IsLockedOutAsync(user).Result)
            {
                context.Response.Redirect("/Account/Lockout");
                return Task.CompletedTask;
            }
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.AddScoped<PropertyManagementService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<UserContextService>();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddScoped<LeaseRenewalPdfGenerator>();
builder.Services.AddScoped<FinancialReportService>();
builder.Services.AddScoped<FinancialReportPdfGenerator>();

// Register background service for scheduled tasks
builder.Services.AddHostedService<ScheduledTaskService>();

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    // var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // try
    // {
    //     context.Database.Migrate();
    // }
    // catch (InvalidOperationException ex) when (ex.Message.Contains("PendingModelChangesWarning"))
    // {
    //     // If there are pending model changes, create the database with current model
    //     context.Database.EnsureDeleted();
    //     context.Database.EnsureCreated();
    // }

    // Seed roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = ApplicationConstants.DefaultRoles;
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Add Admin user
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var superAdminEmail = ApplicationConstants.SuperAdminEmail;
    var adminUser = await userManager.FindByEmailAsync(superAdminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, ApplicationConstants.DefaultSuperAdminPassword);
        await userManager.AddToRoleAsync(adminUser, ApplicationConstants.DefaultSuperAdminRole);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
