using AlterEgo.Core.Domains;
using AlterEgo.Core.Interfaces.Repositories;
using AlterEgo.Infrastucture.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AlterEgo.Infrastucture.Repositories
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
