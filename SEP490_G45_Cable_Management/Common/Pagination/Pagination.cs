﻿namespace Common.Pagination
{
    public class Pagination<T> where T : class
    {
        public int CurrentPage { get; set; }
        // max row in a page
        public int PageSize { get; set; }
        // number of all rows
        public int RowCount { get; set; }

        public List<T> Results { get; set; }
        public int PageCount { get; set; }

        public int Sum { get; set; }


        public Pagination(int currentPage, int rowCount, int pageSize, List<T> results, int sum)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            RowCount = rowCount;
            Results = results;
            PageCount = (int)Math.Ceiling((double)rowCount / pageSize);
            Sum = sum;
        }

        public Pagination(int currentPage, int rowCount, int pageSize, List<T> results)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            RowCount = rowCount;
            Results = results;
            PageCount = (int)Math.Ceiling((double)rowCount / pageSize);
        }

    }
}
