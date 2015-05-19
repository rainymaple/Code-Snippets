using System;
using STORI.Data.Common.Interfaces;
using STORI.Data.Interfaces;

namespace STORI.Data.Services
{
    public class StoriUow: IStoriUow, IDisposable
    {
        public StoriUow(IRepositoryProvider repositoryProvider)
        {
            CreateDbContext();

            repositoryProvider.DbContext = DbContext;
            RepositoryProvider = repositoryProvider;
        }

        // Repositories
        public StoriEntities DbContext { get; set; }


        public IRepository<T> GetRepository<T>() where T:class
        {
            if (typeof (T).Namespace != "STORI.Data")
            {
                throw new Exception(string.Format("Cannot get repository from type of {0}",typeof(T).Name));
            }
            var repo = GetStandardRepo<T>();
            return repo;
        }

        public IRepository<ApiUser> ApiUsers { get { return GetStandardRepo<ApiUser>(); } }
        public IRepository<ApiAppRegistration> ApiAppRegistrations { get { return GetStandardRepo<ApiAppRegistration>(); } }

        public IRepository<Student> Students { get { return GetStandardRepo<Student>(); } }
        public IRepository<Objective> Objectivies { get { return GetStandardRepo<Objective>(); } }
        public IRepository<School> Schools { get { return GetStandardRepo<School>(); } }
        public IRepository<Grade> Grades { get { return GetStandardRepo<Grade>(); } }
        public IRepository<UserAccount> UserAccounts { get { return GetStandardRepo<UserAccount>(); } }

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public void Commit()
        {
            //System.Diagnostics.Debug.WriteLine("Committed");
            DbContext.SaveChanges();
        }

        protected void CreateDbContext()
        {
            DbContext = new StoriEntities();

            // Do NOT enable proxied entities, else serialization fails
            DbContext.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = false;

            // Because Web API will perform validation, we don't need/want EF to do so
            //DbContext.Configuration.ValidateOnSaveEnabled = false;

            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            // We won't use this performance tweak because we don't need
            // the extra performance and, when autodetect is false,
            // we'd have to be careful. We're not being that careful.
        }

        protected IRepositoryProvider RepositoryProvider { get; set; }

        private IRepository<T> GetStandardRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepositoryForEntityType<T>();
        }
        private T GetRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepository<T>();
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }

        #endregion
    }
}
