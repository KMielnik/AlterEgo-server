using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastucture.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Infrastucture.Repositories
{
    public class DrivingVideoRepository : GenericMediaRepository<DrivingVideo>, IDrivingVideoRepository
    {
        private readonly DbSet<DrivingVideo> _drivingVideos;

        public DrivingVideoRepository(AlterEgoContext context) : base(context)
        {
            _drivingVideos = context.Set<DrivingVideo>();
        }
    }
}
