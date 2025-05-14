using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ST10435077___CLDV6211_POE;
using Azure.Storage.Blobs;
using ST10435077___CLDV6211_POE.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with retry policy
builder.Services.AddDbContext<EventEaseContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventEaseConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
});

// Add BlobServiceClient and BlobService
builder.Services.AddSingleton(x => 
    new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorageConnection")));
builder.Services.AddSingleton<BlobService>();

// Add HealthChecks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EventEaseContext>();

var app = builder.Build();

// Configure error handling first
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Add both conventional and attribute routing
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map HealthChecks
app.MapHealthChecks("/health");

app.Run();
