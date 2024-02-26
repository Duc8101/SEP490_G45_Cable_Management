using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Model.DAO
{
    public class DAORequest : BaseDAO
    {
        private IQueryable<Request> getQuery(string? name, int? RequestCategoryID, string? status, Guid? CreatorID, Guid? IssueID)
        {
            IQueryable<Request> query = context.Requests.Include(r => r.RequestCategory).Include(r => r.Creator).Include(r => r.Approver).Include(r => r.Issue).Where(r => r.IsDeleted == false);
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(r => r.RequestName.ToLower().Contains(name.ToLower().Trim()));
            }
            if (status != null && status.Trim().Length > 0)
            {
                query = query.Where(r => r.Status == status.Trim());
            }
            if (CreatorID.HasValue)
            {
                query = query.Where(r => r.CreatorId == CreatorID);
            }
            if (IssueID.HasValue)
            {
                query = query.Where(r => r.IssueId == IssueID);
            }
            if (RequestCategoryID.HasValue)
            {
                query = query.Where(r => r.RequestCategoryId == RequestCategoryID);
            }
            return query;
        }
        public async Task<List<Request>> getListAll(string? name, int? RequestCategoryID, string? status, Guid? CreatorID, int page)
        {
            IQueryable<Request> query = getQuery(name, RequestCategoryID, status, CreatorID, null);
            return await query.OrderBy(r => (r.Status == RequestConst.STATUS_PENDING) ? 0 : 1).ThenByDescending(r => r.UpdateAt).Skip(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE * (page - 1))
                .Take(PageSizeConst.MAX_REQUEST_LIST_IN_PAGE).ToListAsync();
        }
        public async Task<int> getRowCount(string? name, int? RequestCategoryID, string? status, Guid? CreatorID)
        {
            IQueryable<Request> query = getQuery(name, RequestCategoryID, status, CreatorID, null);
            return await query.CountAsync();
        }
        public async Task CreateRequest(Request request)
        {
            await context.Requests.AddAsync(request);
            await context.SaveChangesAsync();
        }
        public async Task<Request?> getRequest(Guid RequestID)
        {
            return await context.Requests.Include(r => r.RequestCategory).Include(r => r.Creator).Include(r => r.Issue).Include(r => r.DeliverWarehouse)
                .Include(r => r.Approver).SingleOrDefaultAsync(r => r.RequestId == RequestID && r.IsDeleted == false);
        }
        public async Task UpdateRequest(Request request)
        {
            context.Requests.Update(request);
            await context.SaveChangesAsync();
        }
        public async Task DeleteRequest(Request request)
        {
            request.IsDeleted = true;
            await UpdateRequest(request);
        }
        public async Task<List<Request>> getListByIssue(Guid IssueID)
        {
            IQueryable<Request> query = getQuery(null, null, null, null, IssueID);
            return await query.ToListAsync();
        }

    }
}
