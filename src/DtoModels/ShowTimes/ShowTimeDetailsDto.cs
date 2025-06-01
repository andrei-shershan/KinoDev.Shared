using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;

namespace KinoDev.Shared.DtoModels.ShowTimes
{
    public class ShowTimeDetailsDto
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        public decimal Price { get; set; }

        public MovieDto Movie { get; set; }

        public HallDto Hall { get; set; }
    }
}