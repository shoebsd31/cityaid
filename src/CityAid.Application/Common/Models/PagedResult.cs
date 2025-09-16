namespace CityAid.Application.Common.Models;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; }
    public int Total { get; }
    public int Page { get; }
    public int PageSize { get; }

    public PagedResult(IEnumerable<T> items, int total, int page, int pageSize)
    {
        Items = items;
        Total = total;
        Page = page;
        PageSize = pageSize;
    }
}