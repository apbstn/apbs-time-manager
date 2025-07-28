using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public class ScheduleRepository : IScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public ScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Schedule> CreateAsync(Schedule schedule)
    {
        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<Schedule?> GetByIdAsync(int id)
    {
        return await _context.Schedules.FindAsync(id);
    }

    public async Task<List<Schedule>> GetAllAsync()
    {
        return await _context.Schedules.ToListAsync();
    }

    public async Task<Schedule> UpdateAsync(Schedule schedule)
    {
        _context.Schedules.Update(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null) return false;
        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();
        return true;
    }
}
