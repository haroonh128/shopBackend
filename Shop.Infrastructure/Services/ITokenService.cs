using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}