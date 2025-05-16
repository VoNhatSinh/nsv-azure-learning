var builder = WebApplication.CreateBuilder(args);

// Configure logging to output to both Console and Application Insights
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddApplicationInsights();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddApplicationInsightsTelemetry(option =>
    option.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"]);
var app = builder.Build();

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