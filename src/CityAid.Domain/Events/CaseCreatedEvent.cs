using CityAid.Domain.Common;
using CityAid.Domain.ValueObjects;

namespace CityAid.Domain.Events;

public class CaseCreatedEvent : DomainEventBase
{
    public CaseId CaseId { get; }
    public string CreatedBy { get; }

    public CaseCreatedEvent(CaseId caseId, string createdBy)
    {
        CaseId = caseId;
        CreatedBy = createdBy;
    }
}