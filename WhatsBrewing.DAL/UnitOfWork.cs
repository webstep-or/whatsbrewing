using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsBrewing.DAL
{
    public class UnitOfWork : IUnitOfwork
    {
        public UnitOfWork(string connString)
        {
            DBContext = new Context(connString);
        }

        public Context DBContext { get; set; }
        //public DbTransaction Transaction { get; set; }

        public void Commit()
        {
            if (DBContext != null)
            {
                DBContext.SaveChanges();
                //Transaction.Commit();
            }
        }

        public void Dispose()
        {
            if (DBContext != null)
            {
                //Transaction.Dispose();
                //Transaction = null;
                DBContext.Dispose();
                DBContext = null;
            }
        }
    }
}
