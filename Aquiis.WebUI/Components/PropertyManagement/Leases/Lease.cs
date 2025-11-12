using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.WebUI.Components.PropertyManagement.Properties;
using Aquiis.WebUI.Components.PropertyManagement.Documents;
using Aquiis.WebUI.Components.PropertyManagement.Tenants;
using Aquiis.WebUI.Components.PropertyManagement.Invoices;
using Aquiis.WebUI.Models;

namespace Aquiis.WebUI.Components.PropertyManagement.Leases {
    
    public class Lease : BaseModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyRent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SecurityDeposit { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Pending, Expired, Terminated

        [StringLength(1000)]
        public string Terms { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; } = null!;

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        // Computed properties
        public bool IsActive => Status == "Active" && DateTime.Now >= StartDate && DateTime.Now <= EndDate;
        public int DaysRemaining => EndDate > DateTime.Now ? (EndDate - DateTime.Now).Days : 0;
    }
}