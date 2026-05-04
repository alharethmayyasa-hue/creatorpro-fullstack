using System.Collections.Generic;
using System.Threading.Tasks;
using GraduationProject.Application.DTOs.Plan;

namespace GraduationProject.Application.Contracts.Services
{
    public interface IPlanService
    {
        Task<PlanResponseDto> CreatePlanAsync(PlanCreateDto dto);

        Task<IEnumerable<PlanResponseDto>> GetAllPlansAsync();

        Task<PlanResponseDto?> GetPlanByIdAsync(int id);

        Task<PlanResponseDto?> UpdatePlanAsync(int id, PlanUpdateDto dto);

        Task DeletePlanAsync(int id);

        Task<PlanResponseDto?> TogglePlanStatusAsync(int id);
    }
}
