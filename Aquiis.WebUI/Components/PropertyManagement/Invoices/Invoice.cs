using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.WebUI.Components.PropertyManagement.Leases;
using Aquiis.WebUI.Components.PropertyManagement.Payments;
using Aquiis.WebUI.Models;

namespace Aquiis.WebUI.Components.PropertyManagement.Invoices {

    public class Invoice : BaseModel
    {

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int LeaseId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue, Cancelled

        public DateTime? PaidOn { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("LeaseId")]
        public virtual Lease Lease { get; set; } = null!;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        // Computed properties
        public decimal BalanceDue => Amount - AmountPaid;
        public bool IsOverdue => Status != "Paid" && DueDate < DateTime.Now;
        public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;
    }
}