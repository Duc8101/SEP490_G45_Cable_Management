﻿using System;
using System.Collections.Generic;

namespace API.Entity
{
    public partial class RequestCategory
    {
        public RequestCategory()
        {
            Requests = new HashSet<Request>();
        }

        public int RequestCategoryId { get; set; }
        public string? RequestCategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Request> Requests { get; set; }
    }
}
