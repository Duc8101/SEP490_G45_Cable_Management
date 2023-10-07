﻿using DataAccess.Entity;
using DataAccess.Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class Program
    {
        private readonly DAOTransactionHistory daoHistory = new DAOTransactionHistory();
        private readonly DAOIssue daoIssue = new DAOIssue();
        private readonly DAOWarehouse daoWare = new DAOWarehouse();
        private readonly DAOTransactionCable daoTransactionCable = new DAOTransactionCable();
        private readonly DAOCable daoCable = new DAOCable();
        private readonly DAOCableCategory daoCableCategory = new DAOCableCategory();
        private readonly DAOOtherMaterial daoMaterial = new DAOOtherMaterial();
        private readonly DAOOtherMaterialsCategory daoMaterialCategory = new DAOOtherMaterialsCategory();
        private readonly DAOTransactionOtherMaterial daoTransactionMaterial = new DAOTransactionOtherMaterial();
        private async Task<string?> getCableCategoryName(Guid CableID)
        {
            Cable? cable = await daoCable.getCable(CableID);
            if (cable == null)
            {
                return null;
            }
            int CategoryID = cable.CableCategoryId;
            CableCategory? category = await daoCableCategory.getCableCategory(CategoryID);
            return category == null ? string.Empty : category.CableCategoryName;
        }
        public static async Task Main(string [] args)
        {
            Program program = new Program();
            List<TransactionHistory> list = await program.daoHistory.getList(null, null, 1);
            Console.WriteLine(list.Count);
        }
    }
}
