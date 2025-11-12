using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.WebUI.Components.PropertyManagement.Documents;
using Aquiis.WebUI.Components.PropertyManagement.Leases;
using Aquiis.WebUI.Models;

namespace Aquiis.WebUI.Components.PropertyManagement.Properties
{
    public class Property : BaseModel
    {
        public string OrganizationId { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [StringLength(50)]
        public string State { get; set; } = string.Empty;

        [StringLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PropertyType { get; set; } = string.Empty; // House, Apartment, Condo, etc.

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyRent { get; set; }

        public int Bedrooms { get; set; }
        
        [Column(TypeName = "decimal(3,1)")]
        public decimal Bathrooms { get; set; }
        public int SquareFeet { get; set; }

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        // Computed property for property status
        public string Status
        {
            get
            {
                // Check for active lease
                var activeLease = Leases?.FirstOrDefault(l => l.Status == "Active");
                if (activeLease != null) return "Occupied";
                
                // Check for pending lease
                var pendingLease = Leases?.FirstOrDefault(l => l.Status == "Pending");
                if (pendingLease != null) return "Pending";
                
                // Otherwise use IsAvailable flag
                return IsAvailable ? "Available" : "Occupied";
            }
        }
    }
}