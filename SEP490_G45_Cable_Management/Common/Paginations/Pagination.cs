﻿using Common.Enum;

namespace Common.Paginations
{
    public class Pagination<T> where T : class
    {
        public int CurrentPage { get; set; }

        // number of all rows
        public int RowCount { get; set; }

        public List<T> Data { get; set; } = new List<T>();
        public int PageCount
        {
            get => (int)Math.Ceiling((double)RowCount / (int)PageSize.Size);
        }

        public int Sum { get; set; }

    }
}
