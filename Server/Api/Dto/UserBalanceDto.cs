namespace Api.Dto;

public class UserBalanceDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Balance { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool? IsDeleted { get; set; }
}
