using LinkDev.Talabat.Core.Domain.Contract.Persistence;

namespace LinkDev.Talabat.APIs.Extensions
{
    public static class InitializerExtensions
    {
        public static async Task<WebApplication> InitializeStoreContextAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var storeContext = services.GetRequiredService<IStoreDbInitializer>();
            var identityContextInitializer = services.GetRequiredService<IStoreIdentityDbInitializer>();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                await storeContext.InitializeAsync();
                await storeContext.SeedAsync();
                await identityContextInitializer.InitializeAsync();
                await identityContextInitializer.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "an error has been occured during applying the migration");

            }
            return app;
        }
    }
}
