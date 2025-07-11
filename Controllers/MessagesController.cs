using ChatWebApp.DTOLayer;
using ChatWebApp.Models;
using ChatWebApp.ServicesInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MessageInterface messageInterface;
        public MessagesController(MessageInterface _messageInterface)
        {
            messageInterface = _messageInterface;
        }
        [HttpPost("GetUserByNumber")]
        public async Task<ActionResult<User>> GetUser(string number)
        {
            var data = await messageInterface.GetUsers(number);
            if (data==null)
            {
                return BadRequest("User Not Found");
            }
            return Ok(data);
        }

        [HttpPost("AddContact")]
        public async Task<ActionResult<Contacts>> AddContact(AddContactDTO newcontact)
        {
            bool expire = await messageInterface.CheckSession(newcontact.Tooken);
            if (expire)
            {
                return BadRequest("Session has been expire");
            }
            else
            {
                var data = await messageInterface.AddContact(newcontact.Tooken, newcontact.ContactId, newcontact.ContactName);
                return data;
            }
        }

        [HttpPost("GetContactList")]
        public async Task<ActionResult<List<ContactListDTO>>> GetContactList(string tooken)
        {
            bool expire = await messageInterface.CheckSession(tooken);
            if (expire)
            {
                return BadRequest("Session has been expire");
            }
            else
            {
               var data = await messageInterface.GetContactListDTOs(tooken);
                return Ok(data);
            }
        }

        [HttpPost("GetById")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            return await messageInterface.GetUserById(id);
        }

        [HttpPost("SendMessage")]
        public async Task<ActionResult<Messages>> SendMessage(SendMessage newmessage)
        {
            var data = await messageInterface.SendMessage(newmessage);
            return Ok(data);
        }

        [HttpPost("RecievedMessage")]
        public async Task<ActionResult<List<Messages>>> RecievedMessage(RecieveMessageDTO recivedMeatadata)
        {
            var data = await messageInterface.RecievedMessage(recivedMeatadata);
            return Ok(data);
        }
    }
}
