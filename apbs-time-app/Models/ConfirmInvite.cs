namespace apbs_time_app.Models;

public class ConfirmInvite
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string TenantId { get; set; }
}
