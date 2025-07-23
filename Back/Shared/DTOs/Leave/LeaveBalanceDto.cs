using System;

namespace Shared.DTOs.Leave;

public class LeaveBalanceDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime LastAllocationMonth { get; set; }
}