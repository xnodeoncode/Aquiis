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
using Aquiis.WebUI.Components.Administration.Application;
using Aquiis.WebUI.Services;
using System.Security.Claims;

namespace Aquiis.WebUI.Components.PropertyManagement
{
    public class PropertyManagementService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationService _applicationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserContextService _userContext;

        public PropertyManagementService(
            ApplicationDbContext dbContext, 
            UserManager<ApplicationUser> userManager, 
            ApplicationService service, 
            IHttpContextAccessor httpContextAccessor,
            UserContextService userContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _applicationService = service;
            _httpContextAccessor = httpContextAccessor;
            _userContext = userContext;


        }

        #region Properties
        public async Task<List<Property>> GetPropertiesAsync()
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Properties
                .Include(p => p.Leases)
                .Include(p => p.Documents)
                .Where(p => !p.IsDeleted && p.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Properties
            .Include(p => p.Leases)
            .Include(p => p.Documents)
            .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == organizationId && !p.IsDeleted);
        }

        // public async Task<Property?> GetPropertyByIdNoLeaseAsync(int propertyId)
        // {
        //     if (_userId == null)
        //     {
        //         // Handle the case when the user is not authenticated
        //         throw new UnauthorizedAccessException("User is not authenticated.");
        //     }
            
        //     var organizationId = await _userContext.GetOrganizationIdAsync();
             
        //      return await _dbContext.Properties
        //     .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == organizationId && !p.IsDeleted);
        // }

        // public async Task<List<Property>> GetAvailablePropertiesAsync()
        // {
        //     if (_userId == null)
        //     {
        //         // Handle the case when the user is not authenticated
        //         throw new UnauthorizedAccessException("User is not authenticated.");
        //     }
            
        //     var organizationId = await _userContext.GetOrganizationIdAsync();

        //     return await _dbContext.Properties
        //     .Include(p => p.Documents)
        //     .Where(p => p.IsAvailable && p.OrganizationId == organizationId && !p.IsDeleted).ToListAsync();
        // }

        public async Task<List<Property>> GetPropertiesByOrganizationIdAsync(string organizationId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            // In a real application, this would call a database or API
            List<Property> properties = await _dbContext.Properties
                .Include(p => p.Leases)
                .Include(p => p.Documents)
                .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
                .ToListAsync();
            return properties;
        }

        public async Task AddPropertyAsync(Property property)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            property.OrganizationId = organizationId!;

            await _dbContext.Properties.AddAsync(property);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePropertyAsync(Property property)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            if (property.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this property.");
            }

            _dbContext.Properties.Update(property);
            await _dbContext.SaveChangesAsync();
        }

        

        public async Task DeletePropertyAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            
            if (_applicationService.SoftDeleteEnabled)
            {
                await SoftDeletePropertyAsync(propertyId);
                return;
            }
            else
            {
                var property = await _dbContext.Properties
                    .FirstOrDefaultAsync(p => p.Id == propertyId &&
                        p.OrganizationId == organizationId);

                if (property != null)
                {
                    _dbContext.Properties.Remove(property);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SoftDeletePropertyAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var property = await _dbContext.Properties
                .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == organizationId);
                
            if (property != null && !property.IsDeleted)
            {
                property.IsDeleted = true;
                property.LastModifiedOn = DateTime.UtcNow;
                property.LastModifiedBy = _userId;
                _dbContext.Properties.Update(property);
                await _dbContext.SaveChangesAsync();
            }
        }
        #endregion

        #region Tenants

        public async Task<List<Tenant>> GetTenantsAsync()
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Tenants
                .Include(t => t.Leases)
                .Where(t => !t.IsDeleted && t.OrganizationId == organizationId)
                .ToListAsync();
        }
        
        public async Task<List<Tenant>> GetTenantsByPropertyIdAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var leases = await _dbContext.Leases
                .Include(l => l.Tenant)
                .Where(l => l.PropertyId == propertyId && l.Tenant.OrganizationId == organizationId && !l.IsDeleted && !l.Tenant.IsDeleted)
                .ToListAsync();

            var tenantIds = leases.Select(l => l.TenantId).Distinct().ToList();
            
            return await _dbContext.Tenants
                .Where(t => tenantIds.Contains(t.Id) && t.OrganizationId == organizationId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<Tenant?> GetTenantByIdAsync(int tenantId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Tenants
                .Include(t => t.Leases)
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.OrganizationId == organizationId && !t.IsDeleted);
        }

        public async Task<List<Tenant>> GetTenantsByOrganizationIdAsync(string organizationId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            return await _dbContext.Tenants
                .Include(t => t.Leases)
                .Where(t => t.OrganizationId == organizationId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task AddTenantAsync(Tenant tenant)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            tenant.OrganizationId = organizationId!;
            await _dbContext.Tenants.AddAsync(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTenantAsync(Tenant tenant)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var organizationId = await _userContext.GetOrganizationIdAsync();

            if (tenant.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this tenant.");
            }

            tenant.LastModifiedOn = DateTime.UtcNow;
            tenant.LastModifiedBy = _userId;
            
            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTenantAsync(Tenant tenant)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            if (_applicationService.SoftDeleteEnabled)
            {
                await SoftDeleteTenantAsync(tenant, userId);
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
                tenant.LastModifiedOn = DateTime.UtcNow;
                tenant.LastModifiedBy = userId;
                _dbContext.Tenants.Update(tenant);
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Leases

        public async Task<List<Lease>> GetLeasesAsync()
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => !l.IsDeleted && !l.Tenant.IsDeleted && !l.Property.IsDeleted && l.Property.OrganizationId == organizationId)
                .ToListAsync();
        }
        public async Task<Lease?> GetLeaseByIdAsync(int leaseId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .FirstOrDefaultAsync(l => l.Id == leaseId && !l.IsDeleted && !l.Tenant.IsDeleted && !l.Property.IsDeleted && l.Property.OrganizationId == organizationId);
        }

        public async Task<List<Lease>> GetActiveLeasesByPropertyIdAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();

            var leases = await _dbContext.Leases
            .Include(l => l.Property)
            .Include(l => l.Tenant)
            .Where(l => l.PropertyId == propertyId && !l.IsDeleted && !l.Tenant.IsDeleted && !l.Property.IsDeleted && l.Property.OrganizationId == organizationId)
            .ToListAsync();
            
            return leases
                .Where(l => l.IsActive)
                .ToList();
        }

        public async Task<List<Lease>> GetLeasesByOrganizationIdAsync(string organizationId)
        {

            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            return await _dbContext.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => !l.IsDeleted && !l.Tenant.IsDeleted && !l.Property.IsDeleted && l.Property.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<List<Lease>> GetLeasesByTenantIdAsync(int tenantId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => l.TenantId == tenantId && !l.Tenant.IsDeleted && !l.IsDeleted && l.Property.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<Lease?> AddLeaseAsync(Lease lease)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var property = await GetPropertyByIdAsync(lease.PropertyId);
            if(property is null || property.OrganizationId != organizationId)
                return lease;

            await _dbContext.Leases.AddAsync(lease);

            property.IsAvailable = false;
            property.LastModifiedOn = DateTime.UtcNow;
            property.LastModifiedBy = _userId;

            _dbContext.Properties.Update(property);

            await _dbContext.SaveChangesAsync();

            return lease;
        }

        public async Task UpdateLeaseAsync(Lease lease)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            if (lease.Property.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("User does not have access to this lease.");
            }
            
            lease.LastModifiedOn = DateTime.UtcNow;
            lease.LastModifiedBy = _userId;

            _dbContext.Leases.Update(lease);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteLeaseAsync(int leaseId)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            if( !await _dbContext.Leases.AnyAsync(l => l.Id == leaseId && l.Property.OrganizationId == organizationId))
            {
                throw new UnauthorizedAccessException("User does not have access to this lease.");
            }

            if (_applicationService.SoftDeleteEnabled)
            {
                await SoftDeleteLeaseAsync(leaseId, userId);
                return;
            }
            else
            {
                var lease = await _dbContext.Leases.FirstOrDefaultAsync(l => l.Id == leaseId);
                if (lease != null)
                {
                    _dbContext.Leases.Remove(lease);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SoftDeleteLeaseAsync(int leaseId, string userId)
        {
            var lease = await _dbContext.Leases.FirstOrDefaultAsync(l => l.Id == leaseId);
            if (lease != null && !lease.IsDeleted && !string.IsNullOrEmpty(userId))
            {
                lease.IsDeleted = true;
                lease.LastModifiedOn = DateTime.UtcNow;
                lease.LastModifiedBy = userId;
                _dbContext.Leases.Update(lease);
                await _dbContext.SaveChangesAsync();
            }
        }

    #endregion

        #region Invoices
        
        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Invoices
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(i => i.Payments)
                .Where(i => !i.IsDeleted && i.Lease.Property.OrganizationId == organizationId)
                .OrderByDescending(i => i.DueDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetInvoicesByOrganizationIdAsync(string organizationId)
        {
            return await _dbContext.Invoices
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(i => i.Payments)
                .Where(i => !i.IsDeleted && i.Lease.Property.OrganizationId == organizationId)
                .OrderByDescending(i => i.DueDate)
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();


            return await _dbContext.Invoices
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == invoiceId
                    && !i.IsDeleted 
                    && i.Lease.Property.OrganizationId == organizationId);
        }

        public async Task<List<Invoice>> GetInvoicesByLeaseIdAsync(int leaseId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Invoices
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(i => i.Payments)
                .Where(i => i.LeaseId == leaseId
                    && !i.IsDeleted
                    && i.Lease.Property.OrganizationId == organizationId)
                .OrderByDescending(i => i.DueDate)
                .ToListAsync();
        }

        public async Task AddInvoiceAsync(Invoice invoice)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            var lease = await _dbContext.Leases
                .Include(l => l.Property)
                .FirstOrDefaultAsync(l => l.Id == invoice.LeaseId && !l.IsDeleted);

            if (lease == null || lease.Property.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("User does not have access to this lease.");
            }

            await _dbContext.Invoices.AddAsync(invoice);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            var cuserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (cuserId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();
            var lease = await _dbContext.Leases
                .Include(l => l.Property)
                .FirstOrDefaultAsync(l => l.Id == invoice.LeaseId && !l.IsDeleted);

            if (lease == null || lease.Property.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("User does not have access to this lease.");
            }

            invoice.LastModifiedOn = DateTime.UtcNow;
            invoice.LastModifiedBy = cuserId;

            _dbContext.Invoices.Update(invoice);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInvoiceAsync(Invoice invoice)
        {
            var cuserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (cuserId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            if (_applicationService.SoftDeleteEnabled)
            {
                invoice.IsDeleted = true;
                invoice.LastModifiedOn = DateTime.UtcNow;
                invoice.LastModifiedBy = cuserId;
                _dbContext.Invoices.Update(invoice);
            }
            else
            {
                _dbContext.Invoices.Remove(invoice);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var lastInvoice = await _dbContext.Invoices
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();
            
            var nextNumber = lastInvoice != null ? lastInvoice.Id + 1 : 1;
            return $"INV-{DateTime.Now:yyyyMM}-{nextNumber:D5}";
        }

        #endregion

        #region Payments
        
        public async Task<List<Payment>> GetPaymentsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Property)
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Tenant)
                .Where(p => !p.IsDeleted && p.Invoice.Lease.Property.OrganizationId == organizationId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByUserIdAsync(string userId)
        {
            return await _dbContext.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Property)
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Tenant)
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _dbContext.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Property)
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Tenant)
                .FirstOrDefaultAsync(p => p.Id == paymentId && !p.IsDeleted);
        }

        public async Task<List<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId)
        {
            return await _dbContext.Payments
                .Include(p => p.Invoice)
                .Where(p => p.InvoiceId == invoiceId && !p.IsDeleted)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            
            // Update invoice paid amount
            await UpdateInvoicePaidAmountAsync(payment.InvoiceId);
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            payment.LastModifiedOn = DateTime.UtcNow;
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();
            
            // Update invoice paid amount
            await UpdateInvoicePaidAmountAsync(payment.InvoiceId);
        }

        public async Task DeletePaymentAsync(Payment payment)
        {
            var cuserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (cuserId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var invoiceId = payment.InvoiceId;

            if (_applicationService.SoftDeleteEnabled)
            {
                payment.IsDeleted = true;
                payment.LastModifiedOn = DateTime.UtcNow;
                payment.LastModifiedBy = cuserId;
                _dbContext.Payments.Update(payment);
            }
            else
            {
                _dbContext.Payments.Remove(payment);
            }
            await _dbContext.SaveChangesAsync();
            
            // Update invoice paid amount
            await UpdateInvoicePaidAmountAsync(invoiceId);
        }

        private async Task UpdateInvoicePaidAmountAsync(int invoiceId)
        {
            var invoice = await _dbContext.Invoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                var totalPaid = await _dbContext.Payments
                    .Where(p => p.InvoiceId == invoiceId && !p.IsDeleted)
                    .SumAsync(p => p.Amount);
                
                invoice.AmountPaid = totalPaid;
                
                // Update invoice status based on payment
                if (totalPaid >= invoice.Amount)
                {
                    invoice.Status = "Paid";
                    invoice.PaidOn = DateTime.UtcNow;
                }
                else if (totalPaid > 0)
                {
                    invoice.Status = "Partial";
                }
                
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Documents
        
        public async Task<List<Document>> GetDocumentsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Documents
                .Include(d => d.Property)
                .Include(d => d.Tenant)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Property)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Tenant)
                .Include(d => d.Invoice)
                .Include(d => d.Payment)
                .Where(d => !d.IsDeleted && d.OrganizationId == organizationId)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        // public async Task<List<Document>> GetDocumentsByUserIdAsync(string userId)
        // {
        //     return await _dbContext.Documents
        //         .Include(d => d.Property)
        //         .Include(d => d.Tenant)
        //         .Include(d => d.Lease)
        //             .ThenInclude(l => l!.Property)
        //         .Include(d => d.Lease)
        //             .ThenInclude(l => l!.Tenant)
        //         .Include(d => d.Invoice)
        //         .Include(d => d.Payment)
        //         .Where(d => d.UserId == userId && !d.IsDeleted)
        //         .OrderByDescending(d => d.CreatedOn)
        //         .ToListAsync();
        // }

        public async Task<Document?> GetDocumentByIdAsync(int documentId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();


            return await _dbContext.Documents
                .Include(d => d.Property)
                .Include(d => d.Tenant)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Property)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Tenant)
                .Include(d => d.Invoice)
                .Include(d => d.Payment)
                .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted && d.OrganizationId == organizationId);
        }

        public async Task<List<Document>> GetDocumentsByLeaseIdAsync(int leaseId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Documents
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Property)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Tenant)
                .Where(d => d.LeaseId == leaseId && !d.IsDeleted && d.OrganizationId == organizationId)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }
        
        public async Task<List<Document>> GetDocumentsByPropertyIdAsync(int propertyId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Documents
                .Include(d => d.Property)
                .Include(d => d.Tenant)
                .Include(d => d.Lease)
                .Where(d => d.PropertyId == propertyId && !d.IsDeleted && d.OrganizationId == organizationId)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Document>> GetDocumentsByTenantIdAsync(int tenantId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Documents
                .Include(d => d.Property)
                .Include(d => d.Tenant)
                .Include(d => d.Lease)
                .Where(d => d.TenantId == tenantId && !d.IsDeleted && d.OrganizationId == organizationId)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        public async Task<Document> AddDocumentAsync(Document document)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            document.CreatedBy = _userId!;
            document.OrganizationId = organizationId!;
            document.CreatedOn = DateTime.UtcNow;
            _dbContext.Documents.Add(document);
            await _dbContext.SaveChangesAsync();
            return document;
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            document.LastModifiedBy = _userId!;
            document.LastModifiedOn = DateTime.UtcNow;
            _dbContext.Documents.Update(document);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(Document document, bool hardDelete = false)
        {

            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            if (hardDelete)
            {
                _dbContext.Documents.Remove(document);
            }
            else
            {
                document.IsDeleted = true;
                document.LastModifiedBy = _userId!;
                document.LastModifiedOn = DateTime.UtcNow;
                _dbContext.Documents.Update(document);
            }
            await _dbContext.SaveChangesAsync();
        }

        #endregion
   }
}