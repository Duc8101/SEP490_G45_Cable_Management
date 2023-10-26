using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace API.Model.DAO
{
    public class DAOIssue : BaseDAO
    {
        private IQueryable<Issue> getQuery(string? filter)
        {
            IQueryable<Issue> query = context.Issues.Where(i => i.IsDeleted == false);
            if (filter != null && filter.Trim().Length != 0)
            {
                query = query.Where(i => i.IssueName.ToLower().Contains(filter.ToLower().Trim())
                || i.IssueCode.ToLower().Contains(filter.ToLower().Trim()));
            }
            return query;
        }
        public async Task<List<Issue>> getListPagedAll(string? filter, int page)
        {
            IQueryable<Issue> query = getQuery(filter);
            return await query.OrderByDescending(i => i.Status).ThenByDescending(i => i.UpdateAt).Skip(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE * (page - 1))
                .Take(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<List<Issue>> getListPagedDoing(int page)
        {
            IQueryable<Issue> query = getQuery(null);
            return await query.OrderByDescending(i => i.Status).ThenByDescending(i => i.UpdateAt).Where(i => i.Status == IssueConst.STATUS_DOING)
                .Skip(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE).ToListAsync();
        }

        public async Task<int> getRowCount(string? filter)
        {
            IQueryable<Issue> query = getQuery(filter);
            return await query.CountAsync();
        }

        public async Task<int> getRowCount()
        {
            IQueryable<Issue> query = getQuery(null);
            return await query.Where(i => i.Status == IssueConst.STATUS_DOING).CountAsync();
        }

        public async Task<List<Issue>> getListDoing()
        {
            IQueryable<Issue> query = getQuery(null);
            return await query.OrderByDescending(i => i.Status).ThenByDescending(i => i.UpdateAt).Where(i => i.Status == IssueConst.STATUS_DOING)
                .ToListAsync();
        }
        public void CreateIssue(Issue issue)
        {
            context.Issues.Add(issue);
            context.SaveChanges();
        }

        public async Task<Issue?> getIssue(Guid IssueID)
        {
            return await context.Issues.SingleOrDefaultAsync(i => i.IssueId == IssueID && i.IsDeleted == false);
        }
        public void UpdateIssue(Issue issue)
        {
            context.Issues.Update(issue);
            context.SaveChanges();
        }
        public void DeleteIssue(Issue issue)
        {
            issue.IsDeleted = true;
            UpdateIssue(issue);
        }
    }
}
