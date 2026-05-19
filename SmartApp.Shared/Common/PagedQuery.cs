using System;
using System.Collections.Generic;
using System.Text;

namespace SmartApp.Shared.Common;

public abstract class PagedQuery
{
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string Filter { get; init; }
    public string SortBy { get; init; }
    public bool SortDesc { get; init; } = false;
}
