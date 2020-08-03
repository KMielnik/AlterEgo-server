using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastucture.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Infrastucture.Repositories
{
    public class ResultVideoRepository : GenericMediaRepository<ResultVideo>, IResultVideoRepository
    {
        private readonly DbSet<ResultVideo> _resultVideos;

        public ResultVideoRepository(AlterEgoContext context) : base(context)
        {
            _resultVideos = context.Set<ResultVideo>();
        }
    }
}
