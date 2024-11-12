using CryptoCalculator.Components;
using CryptoCalculator.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Blazored.LocalStorage;
using ApexCharts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<DCAService>();
builder.Services.AddScoped<DCAService>();
builder.Services.AddBlazoredLocalStorage();

// Register HttpClient with BaseAddress from configuration
var baseAddress = builder.Configuration.GetSection("HttpClient:BaseAddress").Value;
builder.Services.AddHttpClient("CryptoCalculator", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["HttpClient:BaseAddress"]);
});

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.MapRazorPages();
app.MapControllers();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
