using Dapper;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SluTentasal.Web.Data;
using SluTentasal.Web.Health;
using SluTentasal.Web.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.Configure<TentasalOptions>(builder.Configuration.GetSection(TentasalOptions.SectionName));

var databaseOptions = builder.Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
    ?? new DatabaseOptions();
var connectionString = builder.Configuration.GetConnectionString("Tentasal");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:Tentasal saknas i konfigurationen.");
}

if (databaseOptions.Provider == DatabaseProvider.Sqlite)
{
    SqlMapper.AddTypeHandler(new SqliteDateTimeHandler());
}

builder.Services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(databaseOptions.Provider, connectionString));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<LocalClock>();
builder.Services.AddScoped<PasswordRepository>();

builder.Services.AddHealthChecks()
    .AddCheck<ConfigurationHealthCheck>("configuration")
    .AddCheck<DatabaseHealthCheck>("database", timeout: TimeSpan.FromSeconds(5));

var app = builder.Build();

if (databaseOptions.Provider == DatabaseProvider.Sqlite && app.Environment.IsDevelopment())
{
    SqliteDevSeeder.Seed(
        app.Services.GetRequiredService<IDbConnectionFactory>(),
        app.Services.GetRequiredService<LocalClock>().Now);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

// /health/live: processen är igång (inga beroenden kontrolleras).
// /health:      konfiguration och databas.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = HealthResponseWriter.WriteJson });

app.Run();
