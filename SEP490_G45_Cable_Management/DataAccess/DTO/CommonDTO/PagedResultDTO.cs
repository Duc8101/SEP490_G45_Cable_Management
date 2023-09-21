using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO.CommonDTO
{
    public class PagedResultDTO<T> : PagedBaseDTO where T : class
    {
        public List<T> Results { get; set; }
        public int Sum { get; set; }

        public PagedResultDTO(int currentPage, int pageSize, List<T> results, int sum)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            RowCount = results.Count;
            Results = results;
            PageCount = (int) Math.Ceiling((double) Results.Count / pageSize);     
            Sum = sum;
        }

        public PagedResultDTO(int currentPage, int pageSize, List<T> results)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            RowCount = results.Count;
            Results = results;
            PageCount = (int) Math.Ceiling((double) Results.Count / pageSize);
        }

    }
}
