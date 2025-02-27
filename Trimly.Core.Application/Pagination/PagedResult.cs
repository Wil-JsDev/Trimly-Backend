﻿
namespace Trimly.Core.Application.Pagination
{
    public class PagedResult <T>
    {
        public PagedResult()
        {
            
        }

        public PagedResult(IEnumerable<T> items, int totalItems, int currentPage, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }

        public IEnumerable<T> Items { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }
    }
}
