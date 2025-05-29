using System;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Reflection;
using EFSecondLevelCache;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.Filters;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using Utility;
using System.Data.Objects;
using DomainClasses.Configurations.Cms;
using DomainClasses.Entities.Users;
using RefactorThis.GraphDiff;
using DomainClasses.Entities.AM;
using DomainClasses.Entities.Localization;
using DomainClasses.Entities.Directory;
using DomainClasses.Helper;
using DomainClasses.Entities.Theme;
using DomainClasses.Entities.Logg;

namespace DataLayer.Context
{
    public class ApplicationDbContext : IdentityDbContext
        <User, Role, long, UserLogin,UserRole,UserClaim>,
        IUnitOfWork
    {

        #region Constructor


        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            //this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            //for bulk insert
            //this.Configuration.AutoDetectChangesEnabled = false;
        }

        public DbSet<RoleAccess> RoleAccesses { get; set; }
        public DbSet<ActivityLogType> ActivityLogTypes { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }

      

        public DbSet<LocalizedProperty> LocalizedProperties { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<CentralAdmin> CentralAdmins { get; set; }


        public DbSet<Country> Countries { get; set; }
        public DbSet<StateProvince> StateProvinces { get; set; }
        public DbSet<GenericAttribute> GenericAttributes { get; set; }
        public DbSet<ThemeVariable> ThemeVariables { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Log> Logs { get; set; }
        
        
        #endregion

        #region IUnitOfWork

        public T Update<T>(T entity, System.Linq.Expressions.Expression<Func<IUpdateConfiguration<T>, object>> mapping)
            where T : class, new()
        {

            return this.UpdateGraph(entity, mapping);

        }

        private string[] GetChangedEntityNames()
        {
            return ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added ||
                            x.State == EntityState.Modified ||
                            x.State == EntityState.Deleted)
                .Select(x => ObjectContext.GetObjectType(x.Entity.GetType()).FullName)
                .Distinct()
                .ToArray();
        }

        public new System.Data.Entity.IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Modified;
        }
        public void MarkAsDetached<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Detached;
        }
        public void MarkAsAdded<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Added;
        }

        public void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public IList<T> GetRows<T>(string sql, params object[] parameters) where T : class
        {
            return Database.SqlQuery<T>(sql, parameters).ToList();
        }

        public override int SaveChanges()
        {
            return SaveAllChanges();
        }

        public int SaveAllChanges(bool invalidateCacheDependencies = true)
        {
            try
            {
                var result = base.SaveChanges();
                if (!invalidateCacheDependencies) return result;
                var changedEntityNames = GetChangedEntityNames();
                new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
                return result;
            }
            catch (Exception ex)
            {
                var GVEs = GetValidationErrors();
                throw new ArgumentNullException("SaveAllChanges");
            }
        }


        public override Task<int> SaveChangesAsync()
        {
            return SaveAllChangesAsync();
        }

        public Task<int> SaveAllChangesAsync(bool invalidateCacheDependencies = true)
        {

            var result = base.SaveChangesAsync();
            if (!invalidateCacheDependencies) return result;

            var changedEntityNames = GetChangedEntityNames();
            new EFCacheServiceProvider().InvalidateCacheDependencies(changedEntityNames);
            return result;
        }



        public IEnumerable<TEntity> AddThisRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            return ((DbSet<TEntity>)Set<TEntity>()).AddRange(entities);
        }


        public void ForceDatabaseInitialize()
        {
            base.Database.Initialize(force: true);
        }

        public void EnableFiltering(string name)
        {
            this.EnableFilter(name);
        }

        public void EnableFiltering(string name, string parameter, object value)
        {
            this.EnableFilter(name).SetParameter(parameter, value);
        }

        public void DisableFiltering(string name)
        {
            this.DisableFilter(name);
        }

        public void BulkInsertData<T>(IList<T> data)
        {
            this.BulkInsert(data);
        }

        public bool ValidateOnSaveEnabled
        {
            get { return Configuration.ValidateOnSaveEnabled; }
            set { Configuration.ValidateOnSaveEnabled = value; }
        }

        public bool ProxyCreationEnabled
        {
            get { return Configuration.ProxyCreationEnabled; }
            set { Configuration.ProxyCreationEnabled = value; }
        }

        bool IUnitOfWork.AutoDetectChangesEnabled
        {
            get { return Configuration.AutoDetectChangesEnabled; }
            set { Configuration.AutoDetectChangesEnabled = value; }
        }

        public bool ForceNoTracking { get; set; }
        #endregion

        #region Override OnModelCreating
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if(modelBuilder==null)
                throw new ArgumentNullException("modelBuilder");

            DbInterception.Add(new FilterInterceptor());
            DbInterception.Add(new YeKeInterceptor());

            modelBuilder.Configurations.AddFromAssembly(typeof(SettingConfig).GetTypeInfo().Assembly);
            LoadEntities(typeof(User).GetTypeInfo().Assembly, modelBuilder, "DomainClasses.Entities");
        }

        #endregion

        #region AutoRegisterEntityType

        public void LoadEntities(Assembly asm, DbModelBuilder modelBuilder, string nameSpace)
        {
            var entityTypes = asm.GetTypes()
                .Where(type => type.BaseType != null &&
                               type.Namespace == nameSpace &&
                               type.BaseType == null)
                .ToList();

            var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");
            entityTypes.ForEach(type => entityMethod.MakeGenericMethod(type).Invoke(modelBuilder, new object[] { }));
        }
        #endregion

        #region StoredProcedures



        [DbFunction("DataLayer.Context", "GetUserPermissions")]

        public IList<string> GetUserPermissions(int[] roleIds)
        {

            //            var query = new StringBuilder();
            //            query.Append(
            //                @"
            //    select i.Name as ItemName, f.Name as FirmName, c.Name as CategoryName 
            //    from Item i
            //      inner join Firm f on i.FirmId = f.FirmId
            //      inner join Category c on i.CategoryId = c.CategoryId
            //    where c.CategoryId in (");

            //            if (roleIds != null && roleIds.Length > 0)
            //            {
            //                for (var i = 0; i < roleIds.Length; i++)
            //                {
            //                    if (i != 0)
            //                        query.Append(",");
            //                    query.Append(roleIds[i]);
            //                }
            //            }
            //            else
            //            {
            //                query.Append("-1"); // It is for empty result when no one category selected
            //            }
            //            query.Append(")");

            //            var sqlQuery = query.ToString();
            return null;
        }
        #endregion


        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                previousTimeout = this.Database.CommandTimeout;
                this.Database.CommandTimeout = timeout;
            }

            var transactionalBehavior = doNotEnsureTransaction
                ? TransactionalBehavior.DoNotEnsureTransaction
                : TransactionalBehavior.EnsureTransaction;
            var result = this.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                this.Database.CommandTimeout = previousTimeout;
            }

            return result;
        }


        public IDictionary<string, object> GetModifiedProperties<T>(T entity)
        {
            return new Dictionary<string, object>();
        }
    }
}
