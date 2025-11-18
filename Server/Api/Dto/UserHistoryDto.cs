namespace Api.Dto;

public class UserHistoryDto
{
    public long Id { get; set; }
    public Guid PublicId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
