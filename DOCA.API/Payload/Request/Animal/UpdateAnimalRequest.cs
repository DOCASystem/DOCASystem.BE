namespace DOCA.API.Payload.Request.Animal;

public class UpdateAnimalRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Age { get; set; }
    public string? Sex { get; set; }
}