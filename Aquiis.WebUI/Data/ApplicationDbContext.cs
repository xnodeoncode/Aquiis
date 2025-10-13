using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Aquiis.WebUI.Components.PropertyManagement.Properties;
using Aquiis.WebUI.Components.PropertyManagement.Leases;
using Aquiis.WebUI.Components.PropertyManagement.Tenants;
using Aquiis.WebUI.Components.Account;

namespace Aquiis.WebUI.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

    }
}