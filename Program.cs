var builder = WebApplication.CreateBuilder(args);

// Registers MVC controllers and views.
builder.Services.AddControllersWithViews();

// Registers the memory cache service.
builder.Services.AddMemoryCache();

// Registers the response caching service.
builder.Services.AddResponseCaching();

// Registers session support.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Sets the session timeout to 20 minutes.
    options.IdleTimeout = TimeSpan.FromMinutes(20);

    // Prevents session cookies from being accessed by JavaScript.
    options.Cookie.HttpOnly = true;

    // Makes the session cookie essential.
    options.Cookie.IsEssential = true;
});

// Registers HttpClient for asynchronous API requests.
builder.Services.AddHttpClient();

var app = builder.Build();

// Handles production exceptions and HTTPS requests.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Enables HTTPS redirection and static files.
app.UseHttpsRedirection();
app.UseStaticFiles();

// Enables routing.
app.UseRouting();

// Enables response caching.
app.UseResponseCaching();

// Enables session management.
app.UseSession();

// Enables authorization middleware.
app.UseAuthorization();

// Defines the default MVC route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Starts the application.
app.Run();