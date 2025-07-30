using AuthenticationAuthorization.Context;
using ChatWebApp.DTOLayer;
using ChatWebApp.Models;

namespace ChatWebApp.ServicesInterface
{
    public interface RegistrationInterface
    {
        Task<User> RegisterUser(User user);
        Task<Boolean> CheckExist(User user);
        Task<User?> VerifyUserAsync(LoginDTO loginData);
        string GenerateJwtToken(User user, JwtTokenGenerator tokenGenerator);
        Task<string> SessionUpdate(Session session);

        Task<User> GetByTooken(string tooken);

        Task<Boolean> CheckTookenValidity(string tooken);
    }
}
