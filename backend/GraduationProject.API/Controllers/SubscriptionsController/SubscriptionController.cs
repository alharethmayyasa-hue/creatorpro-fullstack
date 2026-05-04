using GraduationProject.API.Controllers.Base;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.DTOs.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class SubscriptionController : BaseController
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(SubscribeDto dto)
    {
        if (CurrentUserId == null)
            return UnauthorizedResponse("User ID not found in token.");

        var result = await _subscriptionService.SubscribeUserAsync(CurrentUserId.Value, dto.PlanId);

        return Success(result, "Subscribed successfully");
    }

    [HttpGet("my-status")]
    public async Task<IActionResult> GetMyStatus()
    {
        if (CurrentUserId == null)
            return UnauthorizedResponse();

        var status = await _subscriptionService.GetSubscriptionStatusAsync(CurrentUserId.Value);

        return Success(status, status == null
            ? "No active subscription found."
            : "Subscription status retrieved successfully");
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel()
    {
        if (CurrentUserId == null)
            return UnauthorizedResponse();

        var result = await _subscriptionService.CancelSubscriptionAsync(CurrentUserId.Value);

        return Success(result, "Subscription cancelled successfully");
    }

    [HttpGet("validate")]
    public async Task<IActionResult> Validate()
    {
        if (CurrentUserId == null)
            return UnauthorizedResponse();

        var isValid = await _subscriptionService.IsSubscriptionValidAsync(CurrentUserId.Value);

        return Success(new { IsActive = isValid });
    }

    [HttpGet("active-details")]
    public async Task<IActionResult> GetActive()
    {
        if (CurrentUserId == null)
            return UnauthorizedResponse();

        var result = await _subscriptionService.GetUserActiveSubscriptionAsync(CurrentUserId.Value);

        return Success(result, result == null
            ? "No active subscription found."
            : "Active subscription retrieved successfully");
    }
}
