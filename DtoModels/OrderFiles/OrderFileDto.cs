namespace KinoDev.Shared.DtoModels.OrderFiles
{
    public class OrderFileDto
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsActive { get; set; }
        
        public Guid OrderId { get; set; }
    }
}