using ChatWebApp.DTOLayer;
using ChatWebApp.Models;

namespace ChatWebApp.ServicesInterface
{
    public interface MessageInterface
    {
        Task<User> GetUsers(string phonenumber);
        Task<Boolean> CheckSession(string tooken);
        Task<Contacts> AddContact(string tooken, int contactId,string contactname);
        Task<List<CombinedContactViewModel>> GetContactListDTOs(string tooken);

        Task<User> GetUserById(int id);

        Task<Messages> SendMessage(SendMessage message);
        Task<List<Messages>> RecievedMessage(RecieveMessageDTO messageDTO);
    }
}
