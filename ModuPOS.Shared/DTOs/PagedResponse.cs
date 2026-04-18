using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.DTOs
{
    public record PagedResponse<T> (
        List<T> Items,
        int TotalItems,
        int PageIndex,
        int PageSize,
        int TotalPages,
        bool HasPreviousPage,
        bool HasNextPage
    )
    {
        public static PagedResponse<T> Montar(List<T> items, int totalItems, int pageIndex, int pageSize) 
        {
            var totalPages = (int)Math.Ceiling(totalItems/ (double)pageSize);
            return new PagedResponse<T>(
                Items: items,
                TotalItems: totalItems,
                PageIndex: pageIndex,
                PageSize: pageSize,
                TotalPages: totalPages,
                HasPreviousPage: pageIndex > 0,
                HasNextPage: pageIndex < totalPages - 1
            );
        }
    }
}
