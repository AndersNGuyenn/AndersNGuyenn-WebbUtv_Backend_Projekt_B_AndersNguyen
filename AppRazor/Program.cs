using AppRazor.Pages;
using Configuration.Extensions;
using DbContext.Extensions;
using DbRepos;
using Encryption.Extensions;
using Services;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Configuration.AddSecrets(builder.Environment);

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddEncryptions(builder.Configuration);
builder.Services.AddDatabaseConnections(builder.Configuration);
builder.Services.AddUserBasedDbContext();
builder.Services.AddVersionInfo();
builder.Services.AddEnvironmentInfo();

builder.Services.AddScoped<AdminDbRepos>();
builder.Services.AddScoped<FriendsDbRepos>();
builder.Services.AddScoped<PetsDbRepos>();
builder.Services.AddScoped<QuotesDbRepos>();
builder.Services.AddScoped<AddressesDbRepos>();

builder.Services.AddScoped<IAdminService, AdminServiceDb>();
builder.Services.AddScoped<IFriendsService, FriendsServiceDb>();
builder.Services.AddScoped<IPetsService, PetsServiceDb>();
builder.Services.AddScoped<IAddressesService, AddressesServiceDb>();
builder.Services.AddScoped<IQuotesService, QuotesServiceDb>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();