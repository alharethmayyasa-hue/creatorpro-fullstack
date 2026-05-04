using GraduationProject.API.Controllers.Base;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.API.Controllers;

[Route("api/[controller]")]
public class PlanController : BaseController
{
    private readonly IPlanService _planService;

    public PlanController(IPlanService planService)
    {
        _planService = planService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(PlanCreateDto dto)
    {
        var result = await _planService.CreatePlanAsync(dto);
        return CreatedResponse(result, "Plan created successfully");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _planService.GetAllPlansAsync();
        return Success(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _planService.GetPlanByIdAsync(id);
        if (result == null)
            return NotFoundResponse("Plan not found");
        return Success(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, PlanUpdateDto dto)
    {
        var result = await _planService.UpdatePlanAsync(id, dto);
        if (result == null)
            return NotFoundResponse("Plan not found");
        return Success(result, "Plan updated successfully");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _planService.DeletePlanAsync(id);
        return Success<object>(null, "Plan deleted successfully");
    }

    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Toggle(int id)
    {
        var result = await _planService.TogglePlanStatusAsync(id);
        if (result == null)
            return NotFoundResponse("Plan not found");
        return Success(result, "Plan status toggled successfully");
    }
}
