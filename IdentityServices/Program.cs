using IdentityServer4.Services;
using IdentityServer4.Validation;
using IdentityServices;
using IdentityServices.Data;
using Microsoft.EntityFrameworkCore;
using static IdentityServices.DataAccess;


var builder = WebApplication.CreateBuilder(args);
var migrationsAssembly = typeof(Program).GetType().Assembly.GetName().Name;
var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>().AddTransient<IProfileService, ProfileService>().AddTransient<IAuthRepository, AuthRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryClients(IdentityConfiguration.GetClients())
    .AddInMemoryApiResources(IdentityConfiguration.GetApiResources())
    .AddInMemoryClients(IdentityConfiguration.GetClients())
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
        b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(migrationsAssembly));

    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
        b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(migrationsAssembly));
    });


var app = builder.Build();
app.UseIdentityServer();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
