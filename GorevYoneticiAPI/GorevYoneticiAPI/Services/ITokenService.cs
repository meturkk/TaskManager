using GorevYoneticiAPI.Models;

namespace GorevYoneticiAPI.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
