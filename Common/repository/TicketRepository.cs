using Common.model.entity;
namespace Common.repository;
public sealed class TicketRepository(IPersistenceSession<Ticket> session) : EntityRepository<Ticket,long?>(session,x=>x.Id)
{
    public List<Ticket> GetAllTickets()=>Session.Query.Select(t=>new Ticket {Subject=t.Subject,Body=t.Body}).ToList();
}
