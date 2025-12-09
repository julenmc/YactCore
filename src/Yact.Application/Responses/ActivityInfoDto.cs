namespace Yact.Application.Responses;

public class ActivityInfoDto
{
    public int Id { get; set; }
    public int CyclistId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Path { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Distance { get; set; }
    public double Elevation { get; set; }
    public string? Type { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
