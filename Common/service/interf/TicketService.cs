using Common.model.entity;
using Common.repository;
namespace Common.service.interf;
public interface TicketService
{
    TicketRepository GetRepository();
    void Create(Ticket ticket);
    Ticket? FindById(long id);
    Ticket GetById(long id);
    Ticket Save(Ticket entity);
    void DeleteById(long id);
    void Delete(Ticket entity);
    List<Ticket> FindAll();
}
