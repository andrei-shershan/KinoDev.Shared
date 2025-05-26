using KinoDev.Shared.DtoModels.Seats;

namespace KinoDev.Shared.DtoModels.Hall
{
    public class HallSummary : HallDto
    {
        public IEnumerable<SeatDto> Seats { get; set; } = new List<SeatDto>();
    }
}