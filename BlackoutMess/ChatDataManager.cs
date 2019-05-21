using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using NewMesMonitor;

namespace BlackoutMess
{
    public class ChatDataManager
    {
        static int id_currentUser;
        private static List<ChatData> chats;

        private DBHelper dbHelper;
        private Settings Settings;

        private Thread updateChats;
        private Thread updateMonitor;

        public ChatDataManager(DBHelper dbHelper, Settings settings)
        {
            this.dbHelper = dbHelper;
            Settings = settings;

            updateChats = new Thread(UpdateChatsBackground) {IsBackground = true};

            updateMonitor = new Thread(UpdateMonitor) {IsBackground = true};
        }

        public int AuthUser(string userName, string password)
        {
            id_currentUser = dbHelper.AuthUser(userName, password);
            switch (id_currentUser)
            {
                case -1:
                    Console.WriteLine("Wrong name!");
                    return -1;
                case -2:
                    Console.WriteLine("Wrong PWD!");
                    return -2;
                default:
                    return 0;
            }
        }

        private void ReadChatData()
        {
            // Tuple(id_targetUser, targetUserName, id_lastMes)
            List<Tuple<int, string, int>> chatInfoList = dbHelper.ReadChatsInfo(id_currentUser);

            chats = new List<ChatData>();

            foreach (Tuple<int, string, int> item in chatInfoList)
            {
                chats.Add(new ChatData(item.Item1, item.Item2, item.Item3));
            }

            if (!updateChats.IsAlive)
            {
                updateChats.Start(dbHelper);
            }

            if (!updateMonitor.IsAlive)
            {
                updateMonitor.Start();
            }
        }

        public void PrintAvailableChats()
        {
            ReadChatData();
            Printer.PrintAvailableChats(chats);
        }

        public void PrintChat(int id_targetUser)
        {
            ChatData chosenChat = chats.Find(x => x.IdUser == id_targetUser);
            if (chosenChat.IsInitialized == false)
            {
                int messagesNum = Convert.ToInt32(Settings.GetSetting("messages to display").State);

                chosenChat.initMessages(dbHelper.GetLastMessages(messagesNum, id_currentUser, id_targetUser));
            }

            chosenChat.CountNewMes = 0;

            Printer.PrintChat(chosenChat);
        }

        public static void UpdateMonitor()
        {
            Process.Start("NewMesMonitor.exe");

            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("monitorpipe", PipeDirection.InOut, 1);

            // Wait for a client to connect
            pipeServer.WaitForConnection();

            // Read the request from the client. Once the client has
            // written to the pipe its security token will be available.

            StreamString ss = new StreamString(pipeServer);

            // Verify our identity to the connected client using a
            // string that the client anticipates.

            ss.WriteString("Validating server");

            while (Thread.CurrentThread.IsAlive)
            {
                string currentMonitorMessage = "";
                foreach (ChatData chatData in chats)
                {
                    if (chatData.CountNewMes == 0)
                    {
                        currentMonitorMessage += "There is no new messages with " + chatData.UserName + "\n";
                    }
                    else
                    {
                        currentMonitorMessage += "There is " + chatData.CountNewMes + " new messages with " +
                                                 chatData.UserName + "\n";
                    }
                }

                try
                {
                    ss.WriteString(currentMonitorMessage);
                }
                catch (Exception e)
                {
                    pipeServer.Close();
                    new Thread(UpdateMonitor) {IsBackground = true}.Start();
                    Thread.CurrentThread.Abort();
                }


                Thread.Sleep(1000);
            }

            ss.WriteString("Close");
            pipeServer.Close();
        }

        public static void UpdateChatsBackground(object o)
        {
            DBHelper dbHelper = o as DBHelper;
            while (true)
            {
                foreach (ChatData chatData in chats)
                {
                    if (!chatData.IsInitialized)
                    {
                        break;
                    }

                    int idLastMes = dbHelper.GetIdLastMes(id_currentUser, chatData.IdUser);
                    if (idLastMes > chatData.IdLastMes)
                    {
                        List<Message> newMessages = dbHelper.GetLastMessages(
                            dbHelper.CountNewMesForChat(id_currentUser, chatData.IdUser, chatData.IdLastMes, idLastMes),
                            id_currentUser,
                            chatData.IdUser);
                        chatData.appendMessages(newMessages, idLastMes);
                    }
                }

                Thread.Sleep(333);
            }
        }
    }
}