using Microsoft.EntityFrameworkCore;
using NotesApp.Data; // простір імен для DbContext

var builder = WebApplication.CreateBuilder(args);

// Отримати рядок підключення до SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Використовуємо SQLite замість SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
