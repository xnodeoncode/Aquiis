using Aquiis.WebUI.Components.PropertyManagement.Properties;
using Aquiis.WebUI.Components.PropertyManagement.Leases;
using Aquiis.WebUI.Components.PropertyManagement.Tenants;
using Aquiis.WebUI.Components.PropertyManagement.Invoices;
using Aquiis.WebUI.Components.PropertyManagement.Payments;
using Aquiis.WebUI.Components.PropertyManagement.Documents;
using Aquiis.WebUI.Components.Account;
using Aquiis.WebUI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Aquiis.WebUI.Components.Administration.Application;
using System.Security.Claims;

namespace Aquiis.WebUI.Components.PropertyManagement
{
    public class PropertyManagementService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationService _applicationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PropertyManagementService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, ApplicationService service, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _applicationService = service;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Properties
        public async Task<List<Property>> GetPropertiesAsync()
        {
            // In a real application, this would call a database or API
            var properties = await _dbContext.Properties.ToListAsync();
            return properties;
        }

        public async Task<Property?> GetPropertyByIdAsync(int propertyId)
        {
            return await _dbContext.Properties.FirstOrDefaultAsync(p => p.Id == propertyId);
        }

        public async Task AddPropertyAsync(Property property)
        {
            await _dbContext.Properties.AddAsync(property);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePropertyAsync(Property property)
        {
            _dbContext.Properties.Update(property);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Property>> GetPropertiesByUserIdAsync(string userId)
        {
            // In a real application, this would call a database or API
            List<Property> properties = await _dbContext.Properties
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Address)
                .ToListAsync();
            return properties;
        }

        public async Task DeletePropertyAsync(int propertyId, string userId)
        {

            var cuserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (cuserId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            if (_applicationService.SoftDeleteEnabled)
            {
                await SoftDeletePropertyAsync(propertyId, cuserId);
                return;
            }
            else
            {
                var property = await _dbContext.Properties.FirstOrDefaultAsync(p => p.Id == propertyId && p.UserId == cuserId);
                if (property != null)
                {
                    _dbContext.Properties.Remove(property);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SoftDeletePropertyAsync(int propertyId, string userId)
        {
            var property = await _dbContext.Properties.FirstOrDefaultAsync(p => p.Id == propertyId && p.UserId == userId);
            if (property != null && !property.IsDeleted && !string.IsNullOrEmpty(userId))
            {
                property.IsDeleted = true;
                property.LastModified = DateTime.UtcNow;
                property.LastModifiedBy = userId;
                _dbContext.Properties.Update(property);
                await _dbContext.SaveChangesAsync();
            }
        }
        #endregion

        #region Leases

        public async Task<List<Lease>> GetLeasesAsync()
        {
            return await _dbContext.Leases
                .Include(l => l.Property)
                .Where(l => !l.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Lease>> GetActiveLeasesByPropertyIdAsync(int propertyId)
        {
            var leases = await _dbContext.Leases.Where(l => l.PropertyId == propertyId).ToListAsync();
            return leases
                .Where(l => l.IsActive)
                .ToList();
        }

        public async Task<List<Lease>> GetLeasesByTenantIdAsync(int tenantId)
        {
            return await _dbContext.Leases
                .Include(l => l.Property)
                .Where(l => l.TenantId == tenantId && !l.IsDeleted)
                .ToListAsync();
        }

    #endregion

        #region Tenants

   public async Task<List<Tenant>> GetTenantsByPropertyIdAsync(int propertyId)
        {

            var leases = await _dbContext.Leases.Where(l => l.PropertyId == propertyId).ToListAsync();
            var tenantIds = leases.Select(l => l.TenantId).Distinct().ToList();
       return await _dbContext.Tenants
           .Where(t => tenantIds.Contains(t.Id) && !t.IsDeleted)
           .ToListAsync();
   }

    public async Task<Tenant?> GetTenantByIdAsync(int tenantId)
    {
        return await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);
    }

    public async Task<List<Tenant>> GetTenantsByUserIdAsync(string userId)
    {
        return await _dbContext.Tenants
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .ToListAsync();
    }

   public async Task AddTenantAsync(Tenant tenant)
   {
       await _dbContext.Tenants.AddAsync(tenant);
       await _dbContext.SaveChangesAsync();
   }

   public async Task UpdateTenantAsync(Tenant tenant)
   {
       _dbContext.Tenants.Update(tenant);
       await _dbContext.SaveChangesAsync();
   }

   public async Task DeleteTenantAsync(Tenant tenant)
   {
       var cuserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
       if (cuserId == null)
       {
           // Handle the case when the user is not authenticated
           throw new UnauthorizedAccessException("User is not authenticated.");
       }
       if (_applicationService.SoftDeleteEnabled)
       {
           await SoftDeleteTenantAsync(tenant, cuserId);
           return;
       }
       else
       {
           if (tenant != null)
           {
               _dbContext.Tenants.Remove(tenant);
               await _dbContext.SaveChangesAsync();
           }
       }
   }

        private async Task SoftDeleteTenantAsync(Tenant tenant, string userId)
        {
            if (tenant != null && !tenant.IsDeleted && !string.IsNullOrEmpty(userId))
            {
                tenant.IsDeleted = true;
                tenant.LastModified = DateTime.UtcNow;
                tenant.LastModifiedBy = userId;
                _dbContext.Tenants.Update(tenant);
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion
   
   }
}