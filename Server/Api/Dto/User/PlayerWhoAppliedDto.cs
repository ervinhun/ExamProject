using Api.Dto.test;

namespace Api.Dto.User;

public class PlayerWhoAppliedDto
{
    public Guid Id { get; set; }
    public PlayerDto Player { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
}

public class PlayerWhoAppliedRequestDto
{
    public Guid UserId { get; set; }
    public bool IsApproved { get; set; }
    public bool IsActivated { get; set; }
    public Guid ReviewedBy { get; set; }
}
