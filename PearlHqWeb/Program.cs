using InfluxDB.Client;
using Microsoft.EntityFrameworkCore;
using PearlHqWeb.Data;
using PearlHqWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<InfluxOptions>(builder.Configuration.GetSection("Influx"));

builder.Services.AddDbContext<PearlHqDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PearlHqDb")));

builder.Services.AddSingleton<InfluxDBClient>(sp =>
{
    var influxOptions = builder.Configuration.GetSection("Influx").Get<InfluxOptions>()!;
    return new InfluxDBClient(influxOptions.Url, influxOptions.Token);
});

builder.Services.AddScoped<DishStatusService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<PearlHqDb>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
