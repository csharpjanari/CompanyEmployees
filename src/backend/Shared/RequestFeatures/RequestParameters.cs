namespace Shared.RequestFeatures;

public abstract class RequestParameters
{
    // Paging
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }

    // Sorting
    public string OrderBy { get; set; }

    // Data Shaping
    public string Fields { get; set; }
}