using DoAn_Frontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Register API Services
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IProductApiService, ProductApiService>();
builder.Services.AddScoped<ICartApiService, CartApiService>();
builder.Services.AddScoped<IOrderApiService, OrderApiService>();
builder.Services.AddScoped<IReviewApiService, ReviewApiService>();
builder.Services.AddScoped<IAdminApiService, AdminApiService>();

// Keep ApiService for backward compatibility (will delegate to new services)
builder.Services.AddScoped<ApiService>();

builder.Services.AddDataProtection();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();