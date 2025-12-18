using DataAccess.Abstract;

namespace DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private MasterContext masterContext;

        public UnitOfWork(MasterContext masterContext)
        {
            this.masterContext = masterContext;
        }

        public void Save()
        {
            masterContext.SaveChanges();
        }
    }
}
