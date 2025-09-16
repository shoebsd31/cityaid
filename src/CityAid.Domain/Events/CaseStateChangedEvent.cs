using CityAid.Domain.Common;
using CityAid.Domain.Enums;
using CityAid.Domain.ValueObjects;

namespace CityAid.Domain.Events;

public class CaseStateChangedEvent : DomainEventBase
{
    public CaseId CaseId { get; }
    public CaseState PreviousState { get; }
    public CaseState NewState { get; }
    public string ChangedBy { get; }
    public string? Reason { get; }

    public CaseStateChangedEvent(CaseId caseId, CaseState previousState, CaseState newState, string changedBy, string? reason = null)
    {
        CaseId = caseId;
        PreviousState = previousState;
        NewState = newState;
        ChangedBy = changedBy;
        Reason = reason;
    }
}