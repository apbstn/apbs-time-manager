using Shared.DTOs.PlannerDtos.Create;
using Shared.Models;
using Shared.Models.Planners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services.Planner;

public interface IFixedPlannerService
{
    public Task<FixedPlanner> CreatePlanner(CreateFixedPlannerDto planner);
    public Task<List<PlannerBase>> GetAll();
}
