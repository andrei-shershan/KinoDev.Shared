using KinoDev.Shared.DtoModels.Hall;

namespace KinoDev.Shared.DtoModels.ShowTimes
{
    public class ShowTimeSummary
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        public HallDto Hall { get; set; }

        public MovieDto Movie { get; set; }
    }
}
