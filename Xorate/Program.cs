using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Xorate.Data;
using Xorate.Helpers;
using Xorate.Interfaces;
using Xorate.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();

builder.Services.AddDbContextFactory<ApplicationContext>(opts =>
{
    opts.UseSqlServer(
    builder.Configuration["ConnectionStrings:DefaultConnection"]);
});
builder.Services.AddDbContext<ApplicationContext>(opts =>
{
    opts.UseSqlServer(
    builder.Configuration["ConnectionStrings:DefaultConnection"]);
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
});

//Установка глобального фильтра отлова ошибок
builder.Services.AddControllersWithViews(options => options.Filters.Add(new CustomExceptionFilter()));
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IPost, PostRepository>();
builder.Services.AddScoped<IComment, CommentRepository>();
builder.Services.AddScoped<IShortPost, ShortPostRepository>();
builder.Services.AddScoped<IService, ServiceRepository>();
builder.Services.AddScoped<IReview, ReviewRepository>();
builder.Services.AddScoped<CookieHelper>();
builder.Services.AddScoped<MapGenerator>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapBlazorHub();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Определен для формирования ссылок в MapGenerator
app.MapControllerRoute(
    name: "post",
    pattern: "/{path}");

//Определен для формирования ссылок в MapGenerator
app.MapControllerRoute(
    name: "shortPost",
    pattern: "/ratings/{path}");

//Определен для формирования ссылок в MapGenerator
app.MapControllerRoute(
    name: "shortPostsSiteMap",
    pattern: "/shortposts-sitemap.xml");


app.Use(async (context, next) =>
{
    await next.Invoke();

    if (context.Response.StatusCode == 404)
        context.Response.Redirect("/404");
});

app.Run();