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
    public class DAOIssue : BaseDAO
    {
        public async Task<List<Issue>> getList(string? filter, int page)
        {
            if(filter == null || filter.Trim().Length == 0)
            {
                return await context.Issues.Where(i => i.IsDeleted == false).Skip(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE * (page - 1))
                    .Take(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE).OrderByDescending(i => i.UpdateAt).ThenByDescending(i => i.Status)
                    .ToListAsync();
            }
            return await context.Issues.Where(i => i.IsDeleted == false && 
            (i.IssueName.ToLower().Contains(filter.ToLower().Trim()) || i.IssueCode.ToLower().Contains(filter.ToLower().Trim())))
                .Skip(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE * (page - 1))
                .Take(PageSizeConst.MAX_ISSUE_LIST_IN_PAGE).OrderByDescending(i => i.UpdateAt).ThenByDescending(i => i.Status)
                .ToListAsync();
        }

        public async Task<int> getRowCount(string? filter)
        {
            List<Issue> list;
            if (filter == null || filter.Trim().Length == 0)
            {
                list = await context.Issues.Where(i => i.IsDeleted == false).ToListAsync();
            }
            else
            {
                list = await context.Issues.Where(i => i.IsDeleted == false && (i.IssueName.ToLower().Contains(filter.ToLower().Trim()) 
                || i.IssueCode.ToLower().Contains(filter.ToLower().Trim()))).ToListAsync();
            }
            return list.Count;
        }

        public async Task<int> CreateIssue(Issue issue)
        {
            await context.Issues.AddAsync(issue);
            return await context.SaveChangesAsync();
        }

        public async Task<Issue?> getIssue(Guid IssueID)
        {
            return await context.Issues.SingleOrDefaultAsync(i => i.IssueId == IssueID);
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
