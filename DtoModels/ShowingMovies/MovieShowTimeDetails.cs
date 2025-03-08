namespace KinoDev.Shared.DtoModels.ShowingMovies
{
  public class MovieShowTimeDetails
  {
    public int HallId { get; set; }

    public string HallName { get; set; }

    public DateTime Time { get; set; }

    public decimal Price { get; set; }

    public bool IsSellingAvailable { get; set; }
  }
}