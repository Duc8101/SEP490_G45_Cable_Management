﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTO
{
    public class PagedResultDTO<T> where T : class
    {
        public int CurrentPage { get; set; }
        // number of row in a page
        public int PageSize { get; set; }
        // number of all rows
        public int RowCount { get; set; }

        public List<T> Results { get; set; }

        // number of page
        public int PageCount { get; set; }

        public int FirstRowOnPage
        {
            get { return Math.Min(RowCount, (CurrentPage - 1) * PageSize + 1); }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }

        public int Sum { get; set; }


        public PagedResultDTO(int currentPage, int pageSize, List<T> results, int sum)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            RowCount = results.Count;
            Results = results;
            PageCount = (int) Math.Ceiling((double)Results.Count / pageSize);
            Sum = sum;
        }

        public PagedResultDTO(int currentPage, int pageSize, List<T> results)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            RowCount = results.Count;
            Results = results;
            PageCount = (int) Math.Ceiling((double)Results.Count / pageSize);
        }

    }
}