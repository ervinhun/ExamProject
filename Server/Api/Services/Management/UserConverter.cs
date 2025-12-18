using Api.Dto.test;
using DataAccess.Entities.Auth;

namespace Api.Services.Management;

public static class UserConverter
{
    public static Player GetPlayerFromUser(User user)
    {
        return new Player
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Id = user.Id,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Activated = true
        };
    }

    public static PlayerDto GetPlayerDtoFromPlayer(Player player)
    {
        return new PlayerDto
        {
            Id = player.Id,
            FirstName = player.FirstName,
            LastName = player.LastName,
            Email = player.Email,
            PhoneNumber = player.PhoneNumber,
            Roles = player.Roles.Select(r => r.Name).ToList(),
            CreatedAt = player.CreatedAt,
            UpdatedAt = player.UpdatedAt,
            IsDeleted = player.IsDeleted
        };
    }
}