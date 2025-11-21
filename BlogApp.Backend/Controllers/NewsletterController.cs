using Microsoft.AspNetCore.Mvc;
using BlogApp.Backend.Services;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly INewsletterService _newsletterService;
    private readonly ILogger<NewsletterController> _logger;

    public NewsletterController(INewsletterService newsletterService, ILogger<NewsletterController> logger)
    {
        _newsletterService = newsletterService;
        _logger = logger;
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult<NewsletterSubscription>> Subscribe(SubscribeNewsletterRequest request)
    {
        try
        {
            var subscription = await _newsletterService.SubscribeAsync(request);
            return Ok(subscription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to newsletter");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("unsubscribe")]
    public async Task<ActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
    {
        try
        {
            var success = await _newsletterService.UnsubscribeAsync(request.Email, request.Token);
            return success ? Ok(new { message = "Successfully unsubscribed" }) : BadRequest(new { message = "Invalid email or token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unsubscribing from newsletter");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("check/{email}")]
    public async Task<ActionResult<bool>> CheckSubscription(string email)
    {
        try
        {
            var isSubscribed = await _newsletterService.IsSubscribedAsync(email);
            return Ok(isSubscribed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking subscription");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("subscriptions")]
    public async Task<ActionResult<List<NewsletterSubscription>>> GetSubscriptions([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        try
        {
            var subscriptions = await _newsletterService.GetSubscriptionsAsync(page, pageSize);
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subscriptions");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

public class UnsubscribeRequest
{
    public string Email { get; set; } = string.Empty;
    public string? Token { get; set; }
}

