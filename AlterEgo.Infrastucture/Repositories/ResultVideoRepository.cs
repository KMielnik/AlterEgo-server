using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Infrastructure.Repositories
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
