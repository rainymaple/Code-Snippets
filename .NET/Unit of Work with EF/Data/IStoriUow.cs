using STORI.Data.Common.Interfaces;

namespace STORI.Data.Interfaces
{
    /// <summary>
    /// Interface for the Code Camper "Unit of Work"
    /// </summary>
    public interface IStoriUow
    {
        // Save pending changes to the data store.
        void Commit();

        StoriEntities DbContext { get; set; }

        IRepository<T> GetRepository<T>() where T : class;

        /*
        IRepository<ApiUser> ApiUsers { get; }
        IRepository<ApiAppRegistration> ApiAppRegistrations { get; }
        IRepository<Student> Students { get; }
        IRepository<Objective> Objectivies { get; }
        IRepository<School> Schools { get; }
        IRepository<Grade> Grades { get; }
        IRepository<UserAccount> UserAccounts { get; }
        */

    }
}
