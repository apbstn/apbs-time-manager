using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.DTOs.PlannerDtos.Create;
using Shared.DTOs.PlannerDtos.Create.Mapper;
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
}
