using apbs_time_app.Models;
using Shared.DTOs.UserDtos;
using Shared.Models;

namespace apbs_time_app.Services;

public interface IInvitationService
{
    public Task<Invitation> CreateInviteExist(User user, string tenantId);
    public Task<Invitation> CreateInviteNotExist(UserNoPassDto user, string tenantId);
    public InviteData ExtractInvitationData(string data);
    public Task<bool> CheckInvite(ConfirmInvite confirm);
    public Task<List<Invitation>> GetInvitationsByTenantIdAsync(string tenantId);
}
