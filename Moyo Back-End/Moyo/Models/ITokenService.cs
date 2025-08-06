using Moyo.View_Models;

namespace Moyo.Models
{
    public interface ITokenService
    {
        string CreateToken(User user, IEnumerable<string> roles, IEnumerable<string> scopes);
    }
}
