using Microsoft.EntityFrameworkCore;
using TodoAppv2.Models;      // przestrzeñ nazw z klas¹ TodoItem (jeœli potrzebne)

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Konfiguracja po³¹czenia do SQLite
string connectionString = $"Data Source={System.IO.Path.Combine(builder.Environment.ContentRootPath, "Todo.db")}";
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
    dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
