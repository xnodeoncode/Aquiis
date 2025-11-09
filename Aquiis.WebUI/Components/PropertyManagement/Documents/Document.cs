using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.WebUI.Components.PropertyManagement.Invoices;
using Aquiis.WebUI.Components.PropertyManagement.Leases;
using Aquiis.WebUI.Components.PropertyManagement.Payments;
using Aquiis.WebUI.Components.PropertyManagement.Properties;
using Aquiis.WebUI.Components.PropertyManagement.Tenants;

namespace Aquiis.WebUI.Components.PropertyManagement.Documents {

    public class Document {
    
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string FileExtension { get; set; } = string.Empty; // .pdf, .jpg, .docx, etc.

        [Required]
        public byte[] FileData { get; set; } = Array.Empty<byte>();

        [StringLength(255)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FileType { get; set; } = string.Empty; // PDF, Image, etc.

        public long FileSize { get; set; }

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } = string.Empty; // Lease Agreement, Invoice, Receipt, Photo, etc.

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Foreign keys - at least one must be set
        public int? PropertyId { get; set; }
        public int? TenantId { get; set; }
        public int? LeaseId { get; set; }
        public int? InvoiceId { get; set; }
        public int? PaymentId { get; set; }

        [Required]
        [StringLength(100)]
        public string UploadedBy { get; set; } = string.Empty; // User who uploaded the document

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModified { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        [ForeignKey("PropertyId")]
        public virtual Property? Property { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }

        [ForeignKey("LeaseId")]
        public virtual Lease? Lease { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice? Invoice { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }

        // Computed property
        public string FileSizeFormatted
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = FileSize;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }
                return $"{len:0.##} {sizes[order]}";
            }
        }
    }
}