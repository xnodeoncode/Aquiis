
namespace Aquiis.WebUI.Models
{
    public class BaseModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? LastModifiedOn { get; set; }

        public string LastModifiedBy { get; set; } = string.Empty;
        
        public bool IsDeleted { get; set; } = false;
    }
}