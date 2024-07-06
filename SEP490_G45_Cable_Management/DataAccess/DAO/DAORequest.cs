using Common.Const;
using Common.Entity;
using Common.Enum;
using DataAccess.DBContext;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAORequest : BaseDAO
    {
        public DAORequest(CableManagementContext context) : base(context)
        {
        }

        private IQueryable<Request> getQuery(string? name, int? requestCategoryId, string? status, Guid? creatorId)
        {
            IQueryable<Request> query = _context.Requests.Include(r => r.RequestCategory).Include(r => r.Creator).Include(r => r.Approver).Include(r => r.Issue).Where(r => r.IsDeleted == false);
            if (name != null && name.Trim().Length > 0)
            {
                query = query.Where(r => r.RequestName.ToLower().Contains(name.ToLower().Trim()));
            }
            if (status != null && status.Trim().Length > 0)
            {
                query = query.Where(r => r.Status == status.Trim());
            }
            if (creatorId.HasValue)
            {
                query = query.Where(r => r.CreatorId == creatorId);
            }
            if (requestCategoryId.HasValue)
            {
                query = query.Where(r => r.RequestCategoryId == requestCategoryId);
            }
            return query;
        }
        public List<Request> getListRequest(string? name, int? requestCategoryId, string? status, Guid? creatorId, int page)
        {
            IQueryable<Request> query = getQuery(name, requestCategoryId, status, creatorId);
            return query.OrderBy(r => r.Status == RequestConst.STATUS_PENDING ? 0 : 1).ThenByDescending(r => r.UpdateAt)
                .Skip((int)PageSize.Size * (page - 1)).Take((int)PageSize.Size).ToList();
        }

        public int getRowCount(string? name, int? requestCategoryId, string? status, Guid? creatorId)
        {
            IQueryable<Request> query = getQuery(name, requestCategoryId, status, creatorId);
            return query.Count();
        }

        public void CreateRequest(Request request)
        {
            _context.Requests.Add(request);
            Save();
        }

        public Request? getRequest(Guid requestId)
        {
            return _context.Requests.Include(r => r.RequestCategory).Include(r => r.Creator).Include(r => r.Issue)
                .Include(r => r.DeliverWarehouse).Include(r => r.Approver).SingleOrDefault(r => r.RequestId == requestId && r.IsDeleted == false);
        }

        public void UpdateRequest(Request request)
        {
            _context.Requests.Update(request);
            Save();
        }

        public void DeleteRequest(Request request)
        {
            request.IsDeleted = true;
            UpdateRequest(request);
        }

    }
}
