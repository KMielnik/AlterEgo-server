using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Infrastructure.Repositories
{
    public class ImageRepository : GenericMediaRepository<Image>, IImageRepository
    {
        private readonly DbSet<Image> _images;

        public ImageRepository(AlterEgoContext context) : base(context)
        {
            _images = context.Set<Image>();
        }
    }
}
