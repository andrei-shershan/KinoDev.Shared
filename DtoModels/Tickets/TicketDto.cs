namespace KinoDev.Shared.DtoModels.Tickets
{
    public class TicketDto
    {
        public int Id { get; set; }
        
        public int ShowTimeId { get; set; }

        public int SeatId { get; set; }

        public int OrderId { get; set; }
    }
}