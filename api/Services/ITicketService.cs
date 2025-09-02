using api.Entities;
using api.Entities.Enuns;

namespace api.Services;

public interface ITicketService
{
    Task<IEnumerable<Ticket>> GetAll();
    Task<IEnumerable<Ticket>> GetAllWithOwner(int pageNumber, int pageSize);
    Task<Ticket> GetById(int id);
    Task<IEnumerable<Ticket>> FindByTitle(string title);
    Task<IEnumerable<Ticket>> FindBySeverity(Severity severity);
    Task<IEnumerable<Ticket>> FindByStatus(StatusTicket status);
    Task Insert(Ticket ticket);
    Task Update(Ticket ticket, int id);
    Task Delete(int id);
}