using Aquiis.Core.Entities;

namespace Aquiis.Professional.Shared.Services;

/// <summary>
/// Provides centralized mapping between entity types and their navigation routes.
/// This ensures consistent URL generation across the application when navigating to entity details.
/// </summary>
public static class EntityRouteHelper
{
    private static readonly Dictionary<string, string> RouteMap = new()
    {
        { "Lease", "/propertymanagement/leases/view" },
        { "Payment", "/propertymanagement/payments/view" },
        { "Invoice", "/propertymanagement/invoices/view" },
        { "Maintenance", "/propertymanagement/maintenance/view" },
        { "Application", "/propertymanagement/applications" },
        { "Property", "/propertymanagement/properties/edit" },
        { "Tenant", "/propertymanagement/tenants/view" },
        { "Prospect", "/PropertyManagement/ProspectiveTenants" }
    };

    /// <summary>
    /// Gets the full navigation route for a given entity type and ID.
    /// </summary>
    /// <param name="entityType">The type of entity (e.g., "Lease", "Payment", "Maintenance")</param>
    /// <param name="entityId">The unique identifier of the entity</param>
    /// <returns>The full route path including the entity ID, or "/" if the entity type is not mapped</returns>
    public static string GetEntityRoute(string? entityType, Guid entityId)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            return "/";
        }

        if (RouteMap.TryGetValue(entityType, out var route))
        {
            return $"{route}/{entityId}";
        }
        
        // Fallback to home if entity type not found
        return "/"; 
    }

    /// <summary>
    /// Checks if a route mapping exists for the given entity type.
    /// </summary>
    /// <param name="entityType">The type of entity to check</param>
    /// <returns>True if a route mapping exists, false otherwise</returns>
    public static bool HasRoute(string? entityType)
    {
        return !string.IsNullOrWhiteSpace(entityType) && RouteMap.ContainsKey(entityType);
    }

    /// <summary>
    /// Gets all supported entity types that have route mappings.
    /// </summary>
    /// <returns>A collection of supported entity type names</returns>
    public static IEnumerable<string> GetSupportedEntityTypes()
    {
        return RouteMap.Keys;
    }
}
