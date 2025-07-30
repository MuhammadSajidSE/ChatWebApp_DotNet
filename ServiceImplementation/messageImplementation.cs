using ChatWebApp.Context;
using ChatWebApp.DTOLayer;
using ChatWebApp.Hubs;
using ChatWebApp.Migrations;
using ChatWebApp.Models;
using ChatWebApp.ServicesInterface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatWebApp.ServiceImplementation
{
    public class messageImplementation : MessageInterface
    {
        private readonly MyDBConext context;
        private readonly IHubContext<ChatHub> hub;
        public messageImplementation(MyDBConext _context, IHubContext<ChatHub> _hub)
        {
            context = _context;
            hub = _hub;
        }
        public async Task<Contacts> AddContact(string tooken, int contactId,string contactname)
        {
            var user = await context.Sessions.FirstOrDefaultAsync(a => a.JWTtooken == tooken);
            var newcontact = new Contacts
            {
                Name= contactname,
                UserId = user.UserId,
                ContactId = contactId,
                LastMessage = DateTime.Now,
                
            };
            await context.Contacts.AddAsync(newcontact);
            await context.SaveChangesAsync();
            var unsaved = await context.usavedContacts
     .FirstOrDefaultAsync(a => a.UserId == user.UserId && a.ContactId == contactId);

            if (unsaved != null)
            {
                context.usavedContacts.Remove(unsaved);
                await context.SaveChangesAsync(); // Don't forget to save changes
            }

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
        public async Task<List<CombinedContactViewModel>> GetContactListDTOs(string tooken)
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
                                    UserPhone = user.phoneNo,
                                    lastmessage = contact.LastMessage
                                }).ToListAsync();
            var Unsavedconatct = await context.usavedContacts.Where(a => a.UserId == userid.UserId).ToListAsync();
            var returndata = result.Where(c => c.UserId == userid.UserId).ToList();

            var fromUsaved = Unsavedconatct.Select(u => new CombinedContactViewModel
            {
                UserId = u.UserId,
                ContactId = u.ContactId,
                LastMessage = u.LastMessage,
                ContactNumber = u.ContactNumber,
                Name = null
            });

            var fromContacts = returndata.Select(c => new CombinedContactViewModel
            {
                UserId = c.UserId,
                ContactId = c.Contactid,
                LastMessage = c.lastmessage,
                Name = c.ContactName,
                ContactNumber = null
            });

            var combinedList = fromUsaved.Concat(fromContacts)
                .OrderByDescending(c => c.LastMessage)
                .ToList();
            return combinedList;
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
        //public async Task<List<Messages>> RecievedMessage(RecieveMessageDTO messageDTO)
        //{
        //    var user = await context.Sessions.FirstOrDefaultAsync(a => a.JWTtooken == messageDTO.JWTtooken);
        //    var messages = await context.Messages.Where(a => (a.SenderId == user.UserId && a.RecieverId == messageDTO.ContactId) || (a.RecieverId == user.UserId && a.SenderId == messageDTO.ContactId)).ToListAsync();
        //    return messages;
        //}
        public async Task<List<Messages>> RecievedMessage(RecieveMessageDTO messageDTO)
        {
            var user = await context.Sessions.FirstOrDefaultAsync(a => a.JWTtooken == messageDTO.JWTtooken);
            var messages = await context.Messages
                .Where(a =>
                    (a.SenderId == user.UserId && a.RecieverId == messageDTO.ContactId) ||
                    (a.RecieverId == user.UserId && a.SenderId == messageDTO.ContactId))
                .ToListAsync();
            return messages;
        }


        //public async Task<Messages> SendMessage(SendMessage message)
        //{
        //    var id = await context.Sessions.FirstOrDefaultAsync(a=>a.JWTtooken == message.JWTtooken);
        //    var contacts = await context.Users.FirstOrDefaultAsync(a => a.UserId == message.Contactid);
        //    var contactSave = await context.Contacts
        //        .Where(a =>
        //            (a.UserId == id.UserId && a.ContactId == message.Contactid) ||
        //            (a.UserId == message.Contactid && a.ContactId == id.UserId))
        //        .ToListAsync();
        //    if (contactSave.Count!=2)
        //    {
        //        var checkusavedexist = await context.usavedContacts.FirstOrDefaultAsync(a => a.UserId == message.Contactid && a.ContactId == id.UserId);
        //        if (checkusavedexist == null)
        //        {
        //            var usavedvontact = new UsavedContact
        //            {
        //                UserId = message.Contactid,
        //                ContactId = id.UserId,
        //                ContactNumber = contacts.phoneNo,
        //                LastMessage = DateTime.UtcNow
        //            };
        //            await context.usavedContacts.AddAsync(usavedvontact);
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //    var newmessage = new Messages
        //    {
        //        Message = message.Message,
        //        SenderId = id.UserId,
        //        RecieverId = message.Contactid,
        //        DateTime = DateTime.UtcNow,
        //    };
        //    await context.Messages.AddAsync(newmessage);
        //    await context.SaveChangesAsync();
        //    var data = await context.Contacts.FirstOrDefaultAsync(a => a.UserId == id.UserId && a.ContactId == message.Contactid);
        //    if (data!=null)
        //    {
        //        data.LastMessage = DateTime.UtcNow;
        //        await context.SaveChangesAsync();
        //    }
        //    var data2 = await context.usavedContacts.FirstOrDefaultAsync(a => a.UserId == message.Contactid && a.ContactId == id.UserId);
        //    if (data2 != null)
        //    {
        //        data2.LastMessage = DateTime.UtcNow;
        //        await context.SaveChangesAsync();
        //    }
        //    return newmessage;
        //}
        //public async Task<Messages> SendMessage(SendMessage message)
        //{
        //    var id = await context.Sessions.FirstOrDefaultAsync(a => a.JWTtooken == message.JWTtooken);
        //    var contacts = await context.Users.FirstOrDefaultAsync(a => a.UserId == message.Contactid);

        //    var contactSave = await context.Contacts
        //        .Where(a =>
        //            (a.UserId == id.UserId && a.ContactId == message.Contactid) ||
        //            (a.UserId == message.Contactid && a.ContactId == id.UserId))
        //        .ToListAsync();

        //    if (contactSave.Count != 2)
        //    {
        //        var checkusavedexist = await context.usavedContacts.FirstOrDefaultAsync(a =>
        //            a.UserId == message.Contactid && a.ContactId == id.UserId);
        //        if (checkusavedexist == null)
        //        {
        //            var usavedcontact = new UsavedContact
        //            {
        //                UserId = message.Contactid,
        //                ContactId = id.UserId,
        //                ContactNumber = contacts.phoneNo,
        //                LastMessage = DateTime.UtcNow
        //            };
        //            await context.usavedContacts.AddAsync(usavedcontact);
        //            await context.SaveChangesAsync();
        //        }
        //    }

        //    var newmessage = new Messages
        //    {
        //        Message = message.Message,
        //        SenderId = id.UserId,
        //        RecieverId = message.Contactid,
        //        DateTime = DateTime.UtcNow
        //    };

        //    await context.Messages.AddAsync(newmessage);
        //    await context.SaveChangesAsync();

        //    var data = await context.Contacts.FirstOrDefaultAsync(a => a.UserId == id.UserId && a.ContactId == message.Contactid);
        //    if (data != null)
        //    {
        //        data.LastMessage = DateTime.UtcNow;
        //    }

        //    var data2 = await context.usavedContacts.FirstOrDefaultAsync(a =>
        //        a.UserId == message.Contactid && a.ContactId == id.UserId);
        //    if (data2 != null)
        //    {
        //        data2.LastMessage = DateTime.UtcNow;
        //    }

        //    await context.SaveChangesAsync();

        //    // 👇 Real-time broadcast using SignalR
        //    await hub.Clients.All.SendAsync("ReceiveMessage", newmessage);

        //    return newmessage;
        //}


        public async Task<Messages> SendMessage(SendMessage message)
        {
            var id = await context.Sessions.FirstOrDefaultAsync(a => a.JWTtooken == message.JWTtooken);
            var contacts = await context.Users.FirstOrDefaultAsync(a => a.UserId == message.Contactid);

            var contactSave = await context.Contacts
                .Where(a =>
                    (a.UserId == id.UserId && a.ContactId == message.Contactid) ||
                    (a.UserId == message.Contactid && a.ContactId == id.UserId))
                .ToListAsync();

            if (contactSave.Count != 2)
            {
                var checkusavedexist = await context.usavedContacts.FirstOrDefaultAsync(a =>
                    a.UserId == message.Contactid && a.ContactId == id.UserId);
                if (checkusavedexist == null)
                {
                    var usavedcontact = new UsavedContact
                    {
                        UserId = message.Contactid,
                        ContactId = id.UserId,
                        ContactNumber = contacts.phoneNo,
                        LastMessage = DateTime.UtcNow
                    };
                    await context.usavedContacts.AddAsync(usavedcontact);
                    await context.SaveChangesAsync();
                }
            }

            var newmessage = new Messages
            {
                Message = message.Message,
                SenderId = id.UserId,
                RecieverId = message.Contactid,
                DateTime = DateTime.UtcNow
            };

            await context.Messages.AddAsync(newmessage);
            await context.SaveChangesAsync();

            var data = await context.Contacts.FirstOrDefaultAsync(a => a.UserId == id.UserId && a.ContactId == message.Contactid);
            if (data != null)
            {
                data.LastMessage = DateTime.UtcNow;
            }

            var data2 = await context.usavedContacts.FirstOrDefaultAsync(a =>
                a.UserId == message.Contactid && a.ContactId == id.UserId);
            if (data2 != null)
            {
                data2.LastMessage = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            // ✅ Only send to receiver if online
            var receiverConnectionId = ChatHub.GetConnectionId(message.Contactid);
            if (!string.IsNullOrEmpty(receiverConnectionId))
            {
                await hub.Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", newmessage);
            }

            return newmessage;
        }

    }
}
