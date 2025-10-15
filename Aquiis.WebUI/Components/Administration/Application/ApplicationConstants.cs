namespace Aquiis.WebUI.Components.Administration.Application
{
    public static class ApplicationConstants
    {
        public static string DefaultSuperAdminRole { get; } = "SuperAdministrator";
        public static string DefaultAdminRole { get; } = "Administrator";
        public static string DefaultPropertyManagerRole { get; } = "PropertyManager";
        public static string DefaultTenantRole { get; } = "Tenant";
        public static string DefaultUserRole { get; } = "User";
        public static string DefaultGuestRole { get; } = "Guest";

        public static string DefaultSuperAdminPassword { get; } = "SuperAdmin@123!";
        public static string DefaultAdminPassword { get; } = "Admin@123!";
        public static string DefaultPropertyManagerPassword { get; } = "PropertyManager@123!";
        public static string DefaultTenantPassword { get; } = "Tenant@123!";
        public static string DefaultUserPassword { get; } = "User@123!";
        public static string DefaultGuestPassword { get; } = "Guest@123!";

        public static string AdministrationPath { get; } = "/Administration";
        public static string PropertyManagementPath { get; } = "/PropertyManagement";
        public static string TenantPortalPath { get; } = "/TenantPortal";


        public static string SuperAdminUserName { get; } = "superadmin";
        public static string SuperAdminEmail { get; } = "superadmin@example.local";

        public static IReadOnlyList<string> DefaultRoles { get; } = new List<string>
        {
            DefaultSuperAdminRole,
            DefaultAdminRole,
            DefaultPropertyManagerRole,
            DefaultTenantRole,
            DefaultUserRole,
            DefaultGuestRole
        };

        public static IReadOnlyList<string> DefaultPasswords { get; } = new List<string>
        {
            DefaultSuperAdminPassword,
            DefaultAdminPassword,
            DefaultPropertyManagerPassword,
            DefaultTenantPassword,
            DefaultUserPassword,
            DefaultGuestPassword
        };
    }
}