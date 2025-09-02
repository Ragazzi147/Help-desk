using api.DTO;
using api.Entities;
using api.Entities.Enuns;
using api.Mappers;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;


[ApiController]
[Route("api/v1/ticket")]
public class TicketController : ControllerBase
{
    
    private readonly ITicketService _ticketService;
    private readonly IUserService _userService;
    
    public TicketController(ITicketService ticketService, IUserService userService)
    {
        _ticketService = ticketService;
        _userService = userService;
    }
    
    [HttpPost("create")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Create([FromBody] TicketCreateDto dto)
    {
        if (dto == null)
            return BadRequest("Dados inválidos");
        
        var ticket = new Ticket(
            title: dto.Title,
            description: dto.Description,
            severity: (Severity)dto.Severity,
            status: (StatusTicket)dto.Status,
            ownerId: dto.OwnerId
        );

        await _ticketService.Insert(ticket);
        return CreatedAtAction(nameof(Create), new { id = ticket.Id }, ticket);
    }
    
    [HttpGet]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1)
    {
        var tickets = await _ticketService.GetAllWithOwner(pageNumber, 20);
        var ticketsDto = tickets.Select(TicketMapper.ToWithOwnerDto).ToList();
        return Ok(ticketsDto);
    }
    
    [HttpGet("search/title")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> FindByTitle([FromQuery] string title)
    {
        var tickets = await _ticketService.FindByTitle(title);
        return Ok(tickets.Select(TicketMapper.ToDto));
    }

    [HttpGet("search/severity")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> FindBySeverity([FromQuery] Severity severity)
    {
        var tickets = await _ticketService.FindBySeverity(severity);
        return Ok(tickets.Select(TicketMapper.ToDto));
    }

    [HttpGet("search/status")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> FindByStatus([FromQuery] StatusTicket status)
    {
        var tickets = await _ticketService.FindByStatus(status);
        return Ok(tickets.Select(TicketMapper.ToDto));
    }
    
    [HttpGet("{id:int}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var ticket = await _ticketService.GetById(id);
            return Ok(TicketMapper.ToDto(ticket));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Update([FromBody] TicketUpdateDto dto, int id)
    {
        try
        {
            if (dto == null)
            {
                throw new ApplicationException("Não foi possível atualizar o ticket");
            }

            var ticketEntity = new Ticket(
                title: dto.Title,
                description: dto.Description,
                severity: dto.Severity,  
                status: dto.Status,
                ownerId: dto.OwnerId
            );

            await _ticketService.Update(ticketEntity, id);

            var response = new
            {
                Id = id,
                Title = dto.Title,
                Description = dto.Description,
                Severity = dto.Severity.ToString(),
                Status = dto.Status.ToString(),
                OwnerId = dto.OwnerId
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            return NotFound(new { message = e.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _ticketService.Delete(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}