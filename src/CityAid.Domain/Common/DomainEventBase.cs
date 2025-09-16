namespace CityAid.Domain.Common;

public abstract class DomainEventBase : IDomainEvent
{
    public DateTime OccurredOn { get; }
    public Guid EventId { get; }

    protected DomainEventBase()
    {
        OccurredOn = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}