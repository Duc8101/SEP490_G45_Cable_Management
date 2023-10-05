using DataAccess.Const;
using DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.DAO
{
    public class DAOIssue : BaseDAO
    {
        private IQueryable<Issue> getQuery(string? filter)
        {
            IQueryable<Issue> query = context.Issues.Where(i => i.IsDeleted == false);
            if(filter != null && filter.Trim().Length != 0)
            {
                query = query.Where(i => (i.IssueName != null && i.IssueName.ToLower().Contains(filter.ToLower().Trim()))
                || (i.IssueCode != null && i.IssueCode.ToLower().Contains(filter.ToLower().Trim())));
            }
            return query;
        }
        public async Task<List<Issue>> getList(string? filter, int page)
        {
            IQueryable<Issue> query = getQuery(filter);
            return await query.Skip(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE * (page - 1)).Take(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE)
                .OrderByDescending(i => i.UpdateAt).ThenByDescending(i => i.Status).ToListAsync();
        }

        public async Task<int> getRowCount(string? filter)
        {
            IQueryable<Issue> query = getQuery(filter);
            return await query.CountAsync();
        }

        public async Task<int> CreateIssue(Issue issue)
        {
            await context.Issues.AddAsync(issue);
            return await context.SaveChangesAsync();
        }

        public async Task<Issue?> getIssue(Guid IssueID)
        {
            return await context.Issues.SingleOrDefaultAsync(i => i.IssueId == IssueID && i.IsDeleted == false);
        }
        public async Task<int> UpdateIssue(Issue issue)
        {
            context.Issues.Update(issue);
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeleteIssue(Issue issue)
        {
            issue.IsDeleted = true;
            return await UpdateIssue(issue);
        }
    }
}
