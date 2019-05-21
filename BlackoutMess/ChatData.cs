using System.Collections.Generic;

namespace BlackoutMess
{
    public class ChatData
    {
        public int IdUser { get; }
        public string UserName { get; }
        public int IdLastMes { get; set; }

        public int CountNewMes { get; set; }
        public bool IsInitialized { get; set; }
        public List<Message> Messages { get; set; }

        public ChatData(int idUser, string userName, int idLastMes)
        {
            IdUser = idUser;
            UserName = userName;
            IdLastMes = idLastMes;
        }

        public void initMessages(List<Message> primordialMessages)
        {
            Messages = primordialMessages;
            IsInitialized = true;
        }

        public void appendMessages(List<Message> newMessages, int id_lastMes)
        {
            foreach (Message newMessage in newMessages) Messages.Add(newMessage);
            CountNewMes += newMessages.Count;
            IdLastMes = id_lastMes;
        }
    }
}