namespace KinoDev.Shared.DtoModels.Tickets
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        
        public int ShowTimeId { get; set; }

        public int SeatId { get; set; }

        public Guid OrderId { get; set; }
    }
}