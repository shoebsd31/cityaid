using CityAid.Domain.Common;
using CityAid.Domain.Enums;
using CityAid.Domain.Events;
using CityAid.Domain.ValueObjects;

namespace CityAid.Domain.Entities;

public class Case : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    private readonly List<File> _files = [];
    private readonly List<ApprovalHistory> _approvalHistory = [];

    public CaseId Id { get; private set; }
    public CityCode CityCode { get; private set; }
    public TeamType TeamType { get; private set; }
    public CaseState State { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public decimal? Budget { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? WorkNotes { get; private set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public IReadOnlyList<File> Files => _files.AsReadOnly();
    public IReadOnlyList<ApprovalHistory> ApprovalHistory => _approvalHistory.AsReadOnly();

    // Private constructor for EF Core
    private Case() { }

    public Case(CaseId id, CityCode cityCode, TeamType teamType, string title, string? description, string createdBy)
    {
        if (teamType != TeamType.Alpha && teamType != TeamType.Beta)
            throw new ArgumentException("Only Alpha and Beta teams can create cases", nameof(teamType));

        Id = id;
        CityCode = cityCode;
        TeamType = teamType;
        State = CaseState.Initiated;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description;
        SetCreatedBy(createdBy);

        _domainEvents.Add(new CaseCreatedEvent(id, createdBy));
        _approvalHistory.Add(new ApprovalHistory(Id, CaseState.Initiated, CaseState.Initiated, createdBy, "Case created"));
    }

    public void UpdateMetadata(string? title, string? description, decimal? budget, DateTime? startDate, DateTime? endDate, string? workNotes, string updatedBy)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title = title;

        Description = description;
        Budget = budget;
        StartDate = startDate;
        EndDate = endDate;
        WorkNotes = workNotes;

        UpdateTimestamp(updatedBy);
    }

    public void SubmitForAnalysis(string submittedBy)
    {
        if (State != CaseState.Initiated)
            throw new InvalidOperationException($"Case must be in Initiated state to submit for analysis. Current state: {State}");

        ChangeState(CaseState.Pending_Analysis, submittedBy, "Submitted for analysis");
    }

    public void SubmitToFinance(string submittedBy)
    {
        if (State != CaseState.Pending_Analysis)
            throw new InvalidOperationException($"Case must be in Pending_Analysis state to submit to finance. Current state: {State}");

        ChangeState(CaseState.Pending_Finance, submittedBy, "Submitted to finance for approval");
    }

    public void ApproveByFinance(string approvedBy)
    {
        if (State != CaseState.Pending_Finance)
            throw new InvalidOperationException($"Case must be in Pending_Finance state for finance approval. Current state: {State}");

        ChangeState(CaseState.Pending_PMO, approvedBy, "Approved by finance");
    }

    public void RejectByFinance(string rejectedBy, string? reason = null)
    {
        if (State != CaseState.Pending_Finance)
            throw new InvalidOperationException($"Case must be in Pending_Finance state for finance rejection. Current state: {State}");

        ChangeState(CaseState.Rejected, rejectedBy, reason ?? "Rejected by finance");
    }

    public void ApproveByPMO(string approvedBy)
    {
        if (State != CaseState.Pending_PMO)
            throw new InvalidOperationException($"Case must be in Pending_PMO state for PMO approval. Current state: {State}");

        ChangeState(CaseState.Approved, approvedBy, "Final approval by PMO");
    }

    public void RejectByPMO(string rejectedBy, string? reason = null)
    {
        if (State != CaseState.Pending_PMO)
            throw new InvalidOperationException($"Case must be in Pending_PMO state for PMO rejection. Current state: {State}");

        ChangeState(CaseState.Rejected, rejectedBy, reason ?? "Rejected by PMO");
    }

    public void RetriggerByPMO(string retriggeredBy)
    {
        if (State != CaseState.Rejected)
            throw new InvalidOperationException($"Can only retrigger rejected cases. Current state: {State}");

        ChangeState(CaseState.Pending_Finance, retriggeredBy, "Retriggered by PMO");
    }

    public void AttachFile(File file, string attachedBy)
    {
        if (file.CaseId != Id)
            throw new ArgumentException("File must belong to this case", nameof(file));

        _files.Add(file);
        _domainEvents.Add(new FileAttachedEvent(Id, file.Id, file.Name, attachedBy));
        UpdateTimestamp(attachedBy);
    }

    private void ChangeState(CaseState newState, string changedBy, string? reason = null)
    {
        var previousState = State;
        State = newState;
        UpdateTimestamp(changedBy);

        _domainEvents.Add(new CaseStateChangedEvent(Id, previousState, newState, changedBy, reason));
        _approvalHistory.Add(new ApprovalHistory(Id, previousState, newState, changedBy, reason));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public bool CanBeViewedByUser(CityCode userCity, TeamType userTeam)
    {
        // Country-level teams can see all cases in their domain
        if (userTeam == TeamType.PMO)
            return true;

        if (userTeam == TeamType.Finance)
            return CityCode == userCity || userTeam == TeamType.PMO;

        // City-level teams can only see their own cases
        return CityCode == userCity && TeamType == userTeam;
    }

    public bool CanBeModifiedByUser(CityCode userCity, TeamType userTeam)
    {
        // Only the owning team or PMO can modify a case
        return (CityCode == userCity && TeamType == userTeam) || userTeam == TeamType.PMO;
    }
}