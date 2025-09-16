using CityAid.Domain.Repositories;
using CityAid.Domain.ValueObjects;
using CityAid.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using File = CityAid.Domain.Entities.File;

namespace CityAid.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly CityAidDbContext _context;

    public FileRepository(CityAidDbContext context)
    {
        _context = context;
    }

    public async Task<File?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Files
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<File>> GetByCaseIdAsync(CaseId caseId, CancellationToken cancellationToken = default)
    {
        return await _context.Files
            .Where(f => f.CaseId == caseId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<File> AddAsync(File file, CancellationToken cancellationToken = default)
    {
        _context.Files.Add(file);
        return file;
    }

    public Task UpdateAsync(File file, CancellationToken cancellationToken = default)
    {
        _context.Files.Update(file);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var file = await GetByIdAsync(id, cancellationToken);
        if (file != null)
        {
            _context.Files.Remove(file);
        }
    }
}