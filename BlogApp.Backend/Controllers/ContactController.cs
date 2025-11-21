using Microsoft.AspNetCore.Mvc;
using BlogApp.Backend.Services;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IContactService contactService, ILogger<ContactController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ContactMessage>> SubmitContact(SubmitContactRequest request)
    {
        try
        {
            var message = await _contactService.SubmitContactFormAsync(request);
            return CreatedAtAction(nameof(GetContactMessage), new { id = message.Id }, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting contact form");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactMessage>> GetContactMessage(string id)
    {
        try
        {
            var message = await _contactService.GetContactMessageAsync(id);
            if (message == null)
                return NotFound();
            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact message");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ContactMessage>>> GetContactMessages([FromQuery] ContactFilter filter)
    {
        try
        {
            var messages = await _contactService.GetContactMessagesAsync(filter);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contact messages");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ContactMessage>> UpdateContactStatus(string id, UpdateContactStatusRequest request)
    {
        try
        {
            var message = await _contactService.UpdateContactStatusAsync(id, request);
            return Ok(message);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact status");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

