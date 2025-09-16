using CityAid.Domain.ValueObjects;
using File = CityAid.Domain.Entities.File;

namespace CityAid.Domain.Repositories;

public interface IFileRepository
{
    Task<File?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<File>> GetByCaseIdAsync(CaseId caseId, CancellationToken cancellationToken = default);
    Task<File> AddAsync(File file, CancellationToken cancellationToken = default);
    Task UpdateAsync(File file, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}