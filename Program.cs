using Microsoft.EntityFrameworkCore;
using ST10435077___CLDV6211_POE;
using Azure.Storage.Blobs;
using ST10435077___CLDV6211_POE.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<EventEaseContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventEaseConnection")));

// Add Azure Blob Storage
builder.Services.AddSingleton(x => 
    new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorageConnection")));
builder.Services.AddSingleton<BlobService>();

// Add Session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Session middleware
app.UseSession();

app.UseAuthorization();

// Configure custom error handling
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
