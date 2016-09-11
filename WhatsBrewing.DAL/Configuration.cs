using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace WhatsBrewing.DAL
{
    internal sealed class Configuration : DbConfiguration
    {
        public Configuration()
        {
            //Database.SetInitializer<Context>(new CreateDatabaseIfNotExists<Context>());

            //Database.SetInitializer(new DropCreateDatabaseAlways<Context>());
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            
        }
    }
}
