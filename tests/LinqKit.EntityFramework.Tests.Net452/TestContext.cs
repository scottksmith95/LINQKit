#if EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
#else
using System;
using System.Data.Entity;
#endif

namespace LinqKit.EntityFramework.Tests
{
#if EFCORE
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Entity> Entities { get; set; }
    }
#else
    [DbConfigurationType(typeof(CodeConfig))]
    public class TestContext : DbContext
    {
        // http://stackoverflow.com/questions/20460357/problems-using-entity-framework-6-and-sqlite
        // http://stackoverflow.com/questions/18882560/entity-framework-code-first-update-database-fails-on-create-database
        public TestContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
        }

        public DbSet<Entity> Entities { get; set; }
    }

    public class CodeConfig : DbConfiguration
    {
        public CodeConfig()
        {
            SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
        }
    }
#endif
}