using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.DTOs.PlannerDtos.Create;
using Shared.DTOs.PlannerDtos.Create.Mapper;
using Shared.Models;
using Shared.Models.Planners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services.Planner;

public class FixedPlannerService : IFixedPlannerService
{
    private readonly ApplicationDbContext _appContext;

    public FixedPlannerService(ApplicationDbContext appContext)
    {
        _appContext = appContext;
    }

    public async Task<FixedPlanner> CreatePlanner(CreateFixedPlannerDto planner)
    {
        var _mapper = new CreateFixedPlannerMapper();
        var plannerEntity = _mapper.ToEntity(planner);

        await _appContext.FixedPlanners.AddAsync(plannerEntity);
        _appContext.SaveChanges();
        return plannerEntity;
    }

    public async Task<List<PlannerBase>> GetAll()
    {
        var result = await _appContext.Planners
            .ToListAsync();
        return result;
    }
}
