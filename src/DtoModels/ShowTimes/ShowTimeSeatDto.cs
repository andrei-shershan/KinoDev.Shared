namespace KinoDev.Shared.DtoModels.ShowTimes
{
    public class ShowTimeSeatDto
    {
        public int Id { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }

        public bool IsAvailable { get; set; }
    }
}