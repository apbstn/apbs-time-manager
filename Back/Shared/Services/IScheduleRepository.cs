using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public interface IScheduleRepository
{
    Task<Schedule> CreateAsync(Schedule schedule);
    Task<Schedule?> GetByIdAsync(int id);
    Task<List<Schedule>> GetAllAsync();
    Task<Schedule> UpdateAsync(Schedule schedule);
    Task<bool> DeleteAsync(int id);
}
