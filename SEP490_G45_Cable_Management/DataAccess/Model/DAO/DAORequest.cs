using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAORequest : BaseDAO
    {
        public async Task<List<Request>> getList(string? name, string? status, int page)
        {
            if (name == null || name.Trim().Length == 0)
            {
                if(status == null)
                {
                    return await context.Requests.Where(r => r.IsDeleted == false).Skip(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE * (page - 1))
                        .Take(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE).OrderByDescending(r => r.CreatedAt).ToListAsync();
                }
                return await context.Requests.Where(r => r.IsDeleted == false && r.Status == status.Trim()).Skip(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE * (page - 1))
                       .Take(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE).OrderByDescending(r => r.CreatedAt).ToListAsync();
            }

            if (status == null)
            {
                return await context.Requests.Where(r => r.IsDeleted == false && r.RequestName.ToLower().Contains(name.ToLower().Trim()))
                    .Skip(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE * (page - 1))
                    .Take(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE).OrderByDescending(r => r.CreatedAt).ToListAsync();
            }
            return await context.Requests.Where(r => r.IsDeleted == false && r.RequestName.ToLower().Contains(name.ToLower().Trim())
            && r.Status == status.Trim()).Skip(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE * (page - 1))
            .Take(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE).OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<int> getRowCount(string? name, string? status)
        {
            List<Request> list;
            if (name == null || name.Trim().Length == 0)
            {
                if (status == null)
                {
                    list = await context.Requests.Where(r => r.IsDeleted == false).ToListAsync();
                }
                else
                {
                    list = await context.Requests.Where(r => r.IsDeleted == false && r.Status == status.Trim()).ToListAsync();
                }
            }
            else
            {
                if (status == null)
                {
                    list = await context.Requests.Where(r => r.IsDeleted == false && r.RequestName.ToLower().Contains(name.ToLower().Trim())).ToListAsync();
                }
                else
                {
                    list = await context.Requests.Where(r => r.IsDeleted == false && r.RequestName.ToLower().Contains(name.ToLower().Trim()) && r.Status == status.Trim()).ToListAsync();
                }
            }
            return list.Count;
        }
    }
}
