namespace Shared.DTOs.UserDtos
{
    public class UserTenantDto
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? TeamId { get; set; }
    }
}