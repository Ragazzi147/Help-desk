
using api.Data;
using api.Entities;
using api.Entities.Enuns;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class TicketService: ITicketService
{
    
    private readonly AppDbContext _context;

    public TicketService(AppDbContext dbContext)
    {
        _context = dbContext;
    }
    
    public async Task<IEnumerable<Ticket>> GetAll()
    {
        return await _context.Tickets.AsNoTracking().ToListAsync();
    }
    
    public async Task<IEnumerable<Ticket>> GetAllWithOwner(int pageNumber = 1, int pageSize = 20)
    {
        return await _context.Tickets
            .Include(t => t.Owner)
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Ticket> GetById(int id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == id);
        if (ticket != null)
        {
            return ticket;
        }
        throw new Exception("Ticket not found");
    }

    public async Task<IEnumerable<Ticket>> FindByTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return await GetAll();

        return await _context.Tickets
            .Where(x => x.Title.ToLower().Contains(title.ToLower()))
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> FindBySeverity(Severity severity)
    {
        return await _context.Tickets
            .Where(x => x.Severity == severity)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Ticket>> FindByStatus(StatusTicket status)
    {
        return await _context.Tickets
            .Where(x => x.Status == status)
            .ToListAsync();
    }

    public async Task Insert(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Ticket ticket, int id)
    {
        var existingTicket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == id);
        if (existingTicket == null)
        {
            throw new Exception("Ticket not found");
        }
        existingTicket.EditTicket(ticket);
        _context.Tickets.Update(existingTicket);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.Id == id);
        if (ticket == null)
        {
            throw new Exception("Ticket not found");
        }
        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
    }
}