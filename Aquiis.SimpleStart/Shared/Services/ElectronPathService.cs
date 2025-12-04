using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.Extensions.Configuration;

namespace Aquiis.SimpleStart.Shared.Services;

public class ElectronPathService
{
    private readonly IConfiguration _configuration;

    public ElectronPathService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the database file path. Uses Electron's user data directory when running as desktop app,
    /// otherwise uses the local Data folder for web mode.
    /// </summary>
    public async Task<string> GetDatabasePathAsync()
    {
        var dbFileName = _configuration["ApplicationSettings:DatabaseFileName"] ?? "app.db";
        
        if (HybridSupport.IsElectronActive)
        {
            var userDataPath = await Electron.App.GetPathAsync(PathName.UserData);
            var dbPath = Path.Combine(userDataPath, dbFileName);
            
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            return dbPath;
        }
        else
        {
            // Web mode - use path from connection string or construct from settings
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                // Extract path from connection string
                var dataSourcePrefix = connectionString.IndexOf("DataSource=", StringComparison.OrdinalIgnoreCase);
                if (dataSourcePrefix >= 0)
                {
                    var start = dataSourcePrefix + "DataSource=".Length;
                    var semicolonIndex = connectionString.IndexOf(';', start);
                    var path = semicolonIndex > 0 
                        ? connectionString.Substring(start, semicolonIndex - start) 
                        : connectionString.Substring(start);
                    return path.Trim();
                }
            }
            
            // Fallback to Infrastructure/Data directory
            return Path.Combine("Infrastructure", "Data", dbFileName);
        }
    }

    /// <summary>
    /// Gets the connection string for the database.
    /// </summary>
    public async Task<string> GetConnectionStringAsync()
    {
        var dbPath = await GetDatabasePathAsync();
        return $"DataSource={dbPath};Cache=Shared";
    }

    /// <summary>
    /// Static helper for early startup before DI is available.
    /// Reads configuration directly from appsettings.json.
    /// </summary>
    public static async Task<string> GetConnectionStringAsync(IConfiguration configuration)
    {
        var service = new ElectronPathService(configuration);
        return await service.GetConnectionStringAsync();
    }
}
