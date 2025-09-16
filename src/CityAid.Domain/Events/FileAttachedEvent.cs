using CityAid.Domain.Common;
using CityAid.Domain.ValueObjects;

namespace CityAid.Domain.Events;

public class FileAttachedEvent : DomainEventBase
{
    public CaseId CaseId { get; }
    public Guid FileId { get; }
    public string FileName { get; }
    public string AttachedBy { get; }

    public FileAttachedEvent(CaseId caseId, Guid fileId, string fileName, string attachedBy)
    {
        CaseId = caseId;
        FileId = fileId;
        FileName = fileName;
        AttachedBy = attachedBy;
    }
}