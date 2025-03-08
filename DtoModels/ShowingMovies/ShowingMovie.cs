namespace KinoDev.Shared.DtoModels.ShowingMovies
{
  public class ShowingMovie
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public int Duration { get; set; }

    public string Url { get; set; }

    public IEnumerable<MovieShowTimeDetails> MovieShowTimeDetails { get; set; }
  }
}