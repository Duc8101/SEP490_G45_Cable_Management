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
        private IQueryable<Request> getQuery(string? name, string? status, Guid? CreatorID)
        {
            IQueryable<Request> query = context.Requests.Include(r => r.RequestCategory).Include(r => r.Creator).Include(r => r.Approver).Include(r => r.Issue).Where(r => r.IsDeleted == false);
            if (name != null && name.Trim().Length != 0)
            {
                query = query.Where(r => r.RequestName != null && r.RequestName.ToLower().Contains(name.ToLower().Trim()));
            }
            if (status != null)
            {
                query = query.Where(r => r.Status == status.Trim());
            }
            if(CreatorID != null)
            {
                query = query.Where(r => r.CreatorId == CreatorID);
            }
            return query;
        }
        public async Task<List<Request>> getList(string? name, string? status, Guid? CreatorID, int page)
        {
            IQueryable<Request> query = getQuery(name, status, CreatorID);
            return await query.OrderByDescending(r => r.Status).ThenByDescending(r => r.UpdateAt).Skip(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE * (page - 1))
                .Take(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<int> getRowCount(string? name, string? status, Guid? CreatorID)
        {
            IQueryable<Request> query = getQuery(name, status, CreatorID);
            return await query.CountAsync();
        }

        public void CreateRequest(Request request)
        {
            context.Requests.Add(request);
            context.SaveChanges();
        }
        public async Task<Request?> getRequest(Guid RequestID)
        {
            return await context.Requests.Include(r => r.RequestCategory).Include(r => r.Creator).Include(r => r.Issue).SingleOrDefaultAsync(r => r.RequestId == RequestID && r.IsDeleted == false);
        }

        public void UpdateRequest(Request request)
        {
            context.Requests.Update(request);
            context.SaveChanges();
        }

    }
}
