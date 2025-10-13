using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.WebUI.Components.PropertyManagement.Documents;
using Aquiis.WebUI.Components.PropertyManagement.Invoices;
using Aquiis.WebUI.Components.PropertyManagement.Leases;
using Aquiis.WebUI.Components.PropertyManagement.Payments;
using Aquiis.WebUI.Components.PropertyManagement.Properties;

namespace Aquiis.WebUI.Components.PropertyManagement.Tenants {
    
    public class Tenant{
         public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(200)]
        public string EmergencyContactName { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string EmergencyContactPhone { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModified { get; set; }

        public string LastModifiedBy { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        // Navigation properties
        public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

        // Computed property
        public string FullName => $"{FirstName} {LastName}";
    }
}