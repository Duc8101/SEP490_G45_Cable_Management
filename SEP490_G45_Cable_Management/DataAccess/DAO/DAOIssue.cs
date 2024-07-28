﻿using Common.Entity;
using Common.Enums;
using DataAccess.DBContext;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO
{
    public class DAOIssue : BaseDAO
    {
        public DAOIssue(CableManagementContext context) : base(context)
        {
        }

        private IQueryable<Issue> getQuery(string? filter)
        {
            IQueryable<Issue> query = _context.Issues.Where(i => i.IsDeleted == false);
            if (filter != null && filter.Trim().Length > 0)
            {
                query = query.Where(i => i.IssueName.ToLower().Contains(filter.ToLower().Trim())
                || i.IssueCode.ToLower().Contains(filter.ToLower().Trim()));
            }
            return query;
        }

        public List<Issue> getListIssueAllStatus(string? filter, int page)
        {
            IQueryable<Issue> query = getQuery(filter);
            return query.OrderByDescending(i => i.Status).ThenByDescending(i => i.UpdateAt).Skip((int)PageSize.Size * (page - 1))
                .Take((int)PageSize.Size).ToList();
        }

        public List<Issue> getListIssueStatusDoing(int page)
        {
            IQueryable<Issue> query = getQuery(null);
            return query.OrderByDescending(i => i.Status).ThenByDescending(i => i.UpdateAt).Where(i => i.Status == IssueStatus.Doing.getDescription())
                .Skip((int)PageSize.Size * (page - 1) * (page - 1)).Take((int)PageSize.Size * (page - 1)).ToList();
        }

        public int getRowCount(string? filter)
        {
            IQueryable<Issue> query = getQuery(filter);
            return query.Count();
        }

        public int getRowCount()
        {
            IQueryable<Issue> query = getQuery(null);
            return query.Where(i => i.Status == IssueStatus.Doing.getDescription()).Count();
        }

        public List<Issue> getListIssueStatusDoing()
        {
            IQueryable<Issue> query = getQuery(null);
            return query.OrderByDescending(i => i.Status).ThenByDescending(i => i.UpdateAt).Where(i => i.Status == IssueStatus.Doing.getDescription())
                .ToList();
        }

        public void CreateIssue(Issue issue)
        {
            _context.Issues.Add(issue);
            Save();
        }

        public Issue? getIssue(Guid issueId)
        {
            return _context.Issues.Include(i => i.Requests).SingleOrDefault(i => i.IssueId == issueId && i.IsDeleted == false);
        }

        public void UpdateIssue(Issue issue)
        {
            _context.Issues.Update(issue);
            Save();
        }

        public void DeleteIssue(Issue issue)
        {
            issue.IsDeleted = true;
            UpdateIssue(issue);
        }
    }
}
