using KinoDev.Shared.DtoModels.Movies;

namespace KinoDev.Shared.DtoModels.Hall
{
    public class HallWithMoviesDto
    {
        public HallDto Hall { get; set; }

        public IEnumerable<MovieWithShowTime> Movies { get; set; }
    }
}