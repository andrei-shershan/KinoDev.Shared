namespace KinoDev.Shared.DtoModels.ShowTimes
{
    public class ShowTimeSeatsDto
    {
        public int Id { get; set; }

        public int HallId { get; set; }

        public DateTime Time { get; set; }

        public decimal Price { get; set; }

        public IEnumerable<ShowTimeSeatDto> Seats { get; set; }
    }
}