using AuthenticationAuthorization.Context;
using ChatWebApp.Context;
using ChatWebApp.DTOLayer;
using ChatWebApp.Models;
using ChatWebApp.ServicesInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatWebApp.ServiceImplementation
{
    public class RegistrationImplementation : RegistrationInterface
    {
        private readonly MyDBConext context;
        public RegistrationImplementation(MyDBConext _context)
        {
            context = _context;
        }

        public async Task<bool> CheckExist(User user)
        {
            var data = await context.Users.FirstOrDefaultAsync(a=>a.phoneNo==user.phoneNo);
            if (data==null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<User> RegisterUser(User user)
        {
            var passwordHasher = new PasswordHasher<User>();
            user.password = passwordHasher.HashPassword(user, user.password);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> VerifyUserAsync(LoginDTO loginData)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.email == loginData.email);

            if (user == null)
                return null;

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.password, loginData.password);

            return result == PasswordVerificationResult.Success ? user : null;
        }
        public string GenerateJwtToken(User user, JwtTokenGenerator tokenGenerator)
        {
            return tokenGenerator.GenerateToken(user);
        }
        public async Task<string> SessionUpdate(Session session)
        {
            var existingSession = await context.Sessions.FirstOrDefaultAsync(a => a.UserId == session.UserId);
            if (existingSession == null)
            {
                await context.Sessions.AddAsync(session);
                await context.SaveChangesAsync();
                return session.JWTtooken;
            }
            else
            {
                DateTime generatedTime = existingSession.Generatedtime;
                DateTime currentTimes = DateTime.Now;
                bool expire = (currentTimes - generatedTime) >= TimeSpan.FromDays(365);
                if (expire)
                {
                    context.Sessions.Remove(existingSession);
                    context.Sessions.Add(session);
                    await context.SaveChangesAsync();
                }
                // Add logic to handle expiration or update session if needed  
                return existingSession.JWTtooken;
            }
        }
    }
}
