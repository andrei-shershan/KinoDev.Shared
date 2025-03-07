namespace KinoDev.Shared.DtoModels
{
    public class MovieDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateOnly ReleaseDate { get; set; }

        public int Duration { get; set; }

        public string Url { get; set; }
    }
}
