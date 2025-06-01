using KinoDev.Shared.DtoModels.Hall;

namespace KinoDev.Shared.DtoModels.ShowTimes
{
    public class ShowTimeForDateDto
    {
        public DateTime Date { get; set; }

        public IEnumerable<HallWithMoviesDto> HallWithMovies { get; set; }
    }
}