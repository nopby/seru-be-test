namespace Seru.BackendTest.Data.Dtos;

public record AddUserDto(
    string Email,
    string Password,
    string Name,
    bool IsAdmin);