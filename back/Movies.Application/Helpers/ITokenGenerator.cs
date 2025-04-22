using Movies.Domain.Entities;

namespace Movies.Application.Helpers;

public interface ITokenGenerator
{
    string GenerateJwtToken(User user);
}