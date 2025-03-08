using Microsoft.EntityFrameworkCore;
using SpaceSoftSolutions.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the database context
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")));

// Register Session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // تحديد مدة انتهاء الجلسة
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddTransient<EmailService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession(); // تأكد من تسجيل الجلسة بعد static files ولكن قبل UseRouting

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
        //pattern: "{controller=Login}/{action=Login}/{id?}");
        pattern: "{controller=BaseHome}/{action=Index}/{id?}");


app.Run();
