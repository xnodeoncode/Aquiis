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

namespace Aquiis.WebUI.Components.PropertyManagement
{
    public class PropertyManagementService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public PropertyManagementService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        private List<Property> properties = new()
        {
            new Property
            {
                Id = 1,
                Address = "123 Main St",
                City = "Anytown",
                State = "CA",
                ZipCode = "12345",
                PropertyType = "House",
                MonthlyRent = 1500.00m,
                Bedrooms = 3,
                Bathrooms = 2.5m,
                SquareFeet = 1800,
                Description = "Beautiful single-family home",
                IsAvailable = true
            },
            new Property
            {
                Id = 2,
                Address = "456 Oak Ave",
                City = "Othertown",
                State = "TX",
                ZipCode = "67890",
                PropertyType = "Apartment",
                MonthlyRent = 900.00m,
                Bedrooms = 2,
                Bathrooms = 1.0m,
                SquareFeet = 900,
                Description = "Cozy apartment unit",
                IsAvailable = false
            }
        };

        public async Task<List<Property>> GetPropertiesAsync()
        {
            // In a real application, this would call a database or API
            var properties = await _dbContext.Properties.ToListAsync();
            return properties;
        }

        public async Task AddPropertyAsync(Property property)
        {
            await _dbContext.Properties.AddAsync(property);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Property>> GetPropertiesByUserIdAsync(string userId)
        {
            // In a real application, this would call a database or API
            var properties = await _dbContext.Properties
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Address)
                .ToListAsync();
            return properties;
        }

        public async Task DeletePropertyAsync(int propertyId, string userId)
        {
            var property = await _dbContext.Properties.FirstOrDefaultAsync(p => p.Id == propertyId && p.UserId == userId);
            if (property != null)
            {
                _dbContext.Properties.Remove(property);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}