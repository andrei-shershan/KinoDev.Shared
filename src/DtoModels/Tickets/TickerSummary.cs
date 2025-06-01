namespace KinoDev.Shared.DtoModels.Tickets
{
    public class TickerSummary
    {
        public Guid TicketId { get; set; }

        public int SeatId { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }

        public decimal Price { get; set; }
    }
}