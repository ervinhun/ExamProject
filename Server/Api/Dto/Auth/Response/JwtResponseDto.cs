using DataAccess.Entities.Auth;

namespace Api.Dto.Auth.Response;

public record JwtResponseDto
{
    public required string AccessToken { get; set; } 
    public string RefreshToken { get; set; } = String.Empty;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public UserResponseDto User { get; set; } =  new();
=======
    public UserResponseDto User { get; set; }
>>>>>>> Stashed changes
=======
    public UserResponseDto User { get; set; }
>>>>>>> Stashed changes
}