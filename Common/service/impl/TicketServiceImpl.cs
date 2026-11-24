using Common.model.entity;
using Common.repository;
using Common.service.interf;
namespace Common.service.impl;
// Intentional bug fix: original Java Create did not save and GetRepository returned null.
public sealed class TicketServiceImpl(TicketRepository repository) : TicketService
{
    private readonly TicketRepository repository=repository ?? throw new ArgumentNullException(nameof(repository));
    public TicketRepository GetRepository()=>repository;
    public void Create(Ticket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        repository.Save(new Ticket {Subject=ticket.Subject,Body=ticket.Body});
    }
    public Ticket? FindById(long id)=>repository.FindById(id);
    public Ticket GetById(long id)=>repository.GetById(id);
    public Ticket Save(Ticket entity)=>repository.Save(entity);
    public void DeleteById(long id)=>repository.DeleteById(id);
    public void Delete(Ticket entity)=>repository.Delete(entity);
    public List<Ticket> FindAll()=>repository.FindAll();
}
