namespace Api.Dto;

public class UpdateTransactionDto
{
    public string Status { get; set; }
    public Guid? ReviewedBy { get; set; }
}
