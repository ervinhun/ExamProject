namespace Api.Dto;

public class BoardDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Numbers { get; set; }
    public int FieldCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool? IsDeleted { get; set; }
}
