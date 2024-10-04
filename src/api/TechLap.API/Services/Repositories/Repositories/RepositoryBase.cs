using TechLap.API.Data;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class RepositoryBase
    {
        protected readonly TechLapContext _dbContext;

        public RepositoryBase(TechLapContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
