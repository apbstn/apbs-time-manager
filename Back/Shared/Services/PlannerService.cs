using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.DTOs.PlannerDtos.Create;
using Shared.DTOs.PlannerDtos.Create.Mapper;
using Shared.DTOs.PlannerDtos.Response;
using Shared.DTOs.PlannerDtos.Response.Mapper;
using Shared.Models;
using Shared.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public class PlannerService : IPlannerService
{
    private readonly ApplicationDbContext _appContext;

    public PlannerService(ApplicationDbContext appContext)
    {
        _appContext = appContext;
    }

    public async Task<Planner> CreatePlanner(CreatePlannerDto planner)
    {
        var _mapper = new CreatePlannerMapper();
        var plannerEntity = _mapper.ToEntity(planner);
        if (plannerEntity.PlanType != PlannerType.Weekly)
        {
            CalculateTotalWeeklyHours(plannerEntity);
        }
        else if(plannerEntity.TotalWeeklyHours == null)
        {
            throw new MissingFieldException("For a Weekly Planner you need to include the TotalWeeklyHours");
        }


        await _appContext.Planners.AddAsync(plannerEntity);
        _appContext.SaveChanges();
        return plannerEntity;
    }

    public async Task<List<Planner>> GetAll()
    {
        var result = await _appContext.Planners
            .ToListAsync();
        return result;
    }


    public static void CalculateTotalWeeklyHours(Planner planner)
    {
        TimeSpan total = TimeSpan.Zero;

        foreach (var day in planner.DayPlans)
        {
            if (day.NumberOfHours.HasValue)
            {
                total += day.NumberOfHours.Value.ToTimeSpan();
            }
            else if (day.StartTime.HasValue && day.EndTime.HasValue)
            {
                var hours = day.EndTime.Value.ToTimeSpan() - day.StartTime.Value.ToTimeSpan();
                total += hours;
            }
        }

        planner.TotalWeeklyHours = TimeOnly.FromTimeSpan(total);
    }

    public async Task<ResponsePlannerDto> GetById(Guid id)
    {
        var _mapper = new ResponsePlannerMapper();
        var result = await _appContext.Planners
            .FirstAsync(p => p.Id == id);

        return _mapper.ToDto(result);
    }

    public async Task Delete(Guid id)
    {
        var planner = await _appContext.Planners.FindAsync(id);
        if (planner != null)
        {
            _appContext.Planners.Remove(planner);
            await _appContext.SaveChangesAsync();
        }
    }
}
