using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class DAORequest : BaseDAO
    {
        private IQueryable<Request> getQuery(string? name, Guid? IssueID, string? status)
        {
            IQueryable<Request> query = context.Requests.Include(r => r.RequestCategory).Where(r => r.IsDeleted == false);
            if (name != null && name.Trim().Length != 0)
            {
                query = query.Where(r => r.RequestName != null && r.RequestName.ToLower().Contains(name.ToLower().Trim()));
            }
            if (status != null)
            {
                query = query.Where(r => r.Status == status.Trim());
            }
            if(IssueID != null)
            {
                query = query.Where(r => r.IssueId == IssueID);
            }
            return query;
        }
        public async Task<List<Request>> getList(string? name, string? status, Guid? IssueID, int page)
        {
            IQueryable<Request> query = getQuery(name, IssueID, status);
            return await query.Skip(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE)
                .OrderBy(r => r.Status == RequestConst.STATUS_PENDING).ThenByDescending(r => r.UpdateAt).ToListAsync();
        }

        public async Task<int> getRowCount(string? name, Guid? IssueID, string? status)
        {
            IQueryable<Request> query = getQuery(name, IssueID, status);
            return await query.CountAsync();
        }

        public async Task<int> CreateRequest(Request request)
        {
            await context.Requests.AddAsync(request);
            return await context.SaveChangesAsync();
        }

    }
}
