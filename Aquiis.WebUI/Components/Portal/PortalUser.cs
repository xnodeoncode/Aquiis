namespace Aquiis.WebUI.Components.Portal
{
    public class PortalUser
    {
        public string Id { get; set; } = default!;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Role { get; set; } = "Tenant";

        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; } = default!;
    }
}