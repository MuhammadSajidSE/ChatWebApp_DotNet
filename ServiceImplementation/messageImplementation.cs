using ChatWebApp.Context;
using ChatWebApp.DTOLayer;
using ChatWebApp.Models;
using ChatWebApp.ServicesInterface;
using Microsoft.EntityFrameworkCore;

namespace ChatWebApp.ServiceImplementation
{
    public class messageImplementation : MessageInterface
    {
        private readonly MyDBConext context;
        public messageImplementation(MyDBConext _context)
        {
            context = _context;
        }
        public async Task<Contacts> AddContact(string tooken, int contactId,string contactname)
        {
            var user = await context.Sessions.FirstOrDefaultAsync(a => a.JWTtooken == tooken);
            var newcontact = new Contacts
            {
                Name= contactname,
                UserId = user.UserId,
                ContactId = contactId,
                
            };
            await context.Contacts.AddAsync(newcontact);
            await context.SaveChangesAsync();
            return newcontact;
        }
        public async Task<bool> CheckSession(string tooken)
        {
            var data = await context.Sessions.FirstOrDefaultAsync(a => a.JWTtooken == tooken);
            DateTime generatedTime = data.Generatedtime;
            DateTime currentTimes = DateTime.Now;
            bool expire = (currentTimes - generatedTime) >= TimeSpan.FromDays(365);
            return expire;
        }
        public async Task<List<object>> GetContactListDTOs(string tooken)
        {
            var userid = await context.Sessions.FirstOrDefaultAsync(a => a.JWTtooken == tooken);
            if (userid == null)
            {
                throw new InvalidOperationException("Session not found for the provided token.");
            }

            var result = await (from contact in context.Contacts
                                join user in context.Users
                                on contact.UserId equals user.UserId
                                select new
                                {
                                    Contactid = contact.ContactId,
                                    UserId = contact.UserId,
                                    ContactName = contact.Name,
                                    UserPhone = user.phoneNo
                                }).ToListAsync();

            var returndata = result.Where(c => c.UserId == userid.UserId).Cast<object>().ToList();
            return returndata;
        }

        public async Task<User> GetUserById(int id)
        {
            return await context.Users.FirstOrDefaultAsync(a=>a.UserId == id);
             
        }

        public async Task<User> GetUsers(string phonenumber)
        {
           var data = await context.Users.FirstOrDefaultAsync(a=>a.phoneNo == phonenumber);
            return data;
        }
        public async Task<List<Messages>> RecievedMessage(RecieveMessageDTO messageDTO)
        {
            var user = await context.Sessions.FirstOrDefaultAsync(a => a.JWTtooken == messageDTO.JWTtooken);
            var messages = await context.Messages.Where(a => (a.SenderId == user.UserId && a.RecieverId == messageDTO.ContactId) || (a.RecieverId == user.UserId && a.SenderId == messageDTO.ContactId)).ToListAsync();
            return messages;
        }

        public async Task<Messages> SendMessage(SendMessage message)
        {
            var id = await context.Sessions.FirstOrDefaultAsync(a=>a.JWTtooken == message.JWTtooken);
            var newmessage = new Messages
            {
                Message = message.Message,
                SenderId = id.UserId,
                RecieverId = message.Contactid,
                DateTime = DateTime.UtcNow,
            };
            await context.Messages.AddAsync(newmessage);
            await context.SaveChangesAsync();
            return newmessage;
        }
    }
}
