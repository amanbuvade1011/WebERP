namespace NicheWebErpAPI.Dtos
{
    // Shared paged-list wrapper. Every high-volume list endpoint should return this instead of
    // a raw array - see docs/ai-plan/02-api-plan.md's note on server-side paging.
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
