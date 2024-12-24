namespace DOCA.API.Payload.Response.Animal;

public class AnimalImageResponse
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; }
    public bool IsMain { get; set; }
}