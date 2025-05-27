using Shared.DTOs.PlannerDtos.Create;
using Shared.DTOs.PlannerDtos.Response;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public interface IPlannerService
{
    public Task<Planner> CreatePlanner(CreatePlannerDto planner);
    public Task<List<Planner>> GetAll();
    public Task<ResponsePlannerDto> GetById(Guid id);
    public Task Delete(Guid id);
}
