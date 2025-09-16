using CityAid.Domain.Common;
using CityAid.Domain.Enums;
using CityAid.Domain.ValueObjects;

namespace CityAid.Domain.Entities;

public class ApprovalHistory : BaseEntity
{
    public Guid Id { get; private set; }
    public CaseId CaseId { get; private set; }
    public CaseState FromState { get; private set; }
    public CaseState ToState { get; private set; }
    public string ApprovedBy { get; private set; }
    public string? Comments { get; private set; }

    // Private constructor for EF Core
    private ApprovalHistory() { }

    public ApprovalHistory(CaseId caseId, CaseState fromState, CaseState toState, string approvedBy, string? comments = null)
    {
        Id = Guid.NewGuid();
        CaseId = caseId ?? throw new ArgumentNullException(nameof(caseId));
        FromState = fromState;
        ToState = toState;
        ApprovedBy = approvedBy ?? throw new ArgumentNullException(nameof(approvedBy));
        Comments = comments;
        SetCreatedBy(approvedBy);
    }
}