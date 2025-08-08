using Microsoft.AspNetCore.Mvc;

namespace Educar.Backend.Web.Infrastructure;

public class PaginatedQuery
{
    [FromQuery(Name = "page_number")]
    public int PageNumber { get; init; } = 1;
    
    [FromQuery(Name = "page_size")]
    public int PageSize { get; init; } = 10;
}