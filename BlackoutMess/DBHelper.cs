using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace BlackoutMess
{
    public class DBHelper
    {
        MySqlConnection connection;

        public DBHelper(string connectionString =
            "server = remotemysql.com; uid = qmVoW1091B; pwd = 10b8qGCc1C; database = qmVoW1091B ")
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }

        public int CountNewMesForChat(int id_currentUser, int id_targetUser, int id_lastMes, int id_newLastMes)
        {
            string countNewMesForChat = "SELECT COUNT(*) " +
                                        "FROM messages " +
                                        "WHERE ((id_from = " + id_currentUser + " and id_to = " + id_targetUser + ") " +
                                        "OR (id_from = " + id_targetUser + " and id_to = " + id_currentUser + ")) " +
                                        "AND id_message <= " + id_newLastMes + " AND id_message > " + id_lastMes;

            var cmd = new MySqlCommand(countNewMesForChat, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return -1;
            }

            int countNewMes = Int32.Parse(reader[0].ToString());
            reader.Close();
            return countNewMes;
        }


//        public int RegisterUser(string userName, string password)
//        {
//            
//        }

        /// <summary>
        /// Authenticates user.
        /// </summary>
        /// <param name="currentUserName">User ID which is authenticating.</param>
        /// <param name="password">His password</param>
        /// <returns>
        ///     <para>user id | on success</para>
        ///     <para>-1 | no users with that name</para>
        ///     <para>-2 | password is incorrect</para>
        /// </returns>
        public int AuthUser(string currentUserName, string password)
        {
            // Checks is there user with such name
            string checkUserQuery = "SELECT id_user " +
                                    "FROM users " +
                                    "WHERE userName = '" + MySqlHelper.EscapeString(currentUserName) + "'";

            var cmd = new MySqlCommand(checkUserQuery, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                reader.Close();
                return -1;
            }

            reader.Read();

            var userId = (int) reader[0];
            reader.Close();

            // Checks password of that user
            string checkPasswordQuery = "SELECT password " +
                                        "FROM users " +
                                        "WHERE id_user = '" + userId + "'";

            cmd = new MySqlCommand(checkPasswordQuery, connection);
            reader = cmd.ExecuteReader();
            reader.Read();

            var receivedPassword = reader[0] as string;
            reader.Close();
            if (receivedPassword == password)
            {
                reader.Close();
                return userId;
            }

            return -2;
        }

        /// <summary>
        /// Gets list of data of all active chats for user.
        /// </summary>
        /// <param name="id_currentUser">User ID for which you are looking for data.</param>
        /// <returns>
        ///     <para><c>null</c> | no such user or no active chats</para>
        ///     <para>List of Tuples(int, string, int) | according to Tuple(id_targetUser, targetUserName, id_lastMes)</para>
        /// </returns>
        public List<Tuple<int, string, int>> ReadChatsInfo(int id_currentUser)
        {
            string readChatInfoQuery = "SELECT id_user, userName, max(id_message) " +
                                       "FROM users, messages as a " +
                                       "WHERE (a.id_from = " + id_currentUser + " and a.id_to = id_user) " +
                                       "or (a.id_from = id_user and a.id_to = " + id_currentUser + ") " +
                                       "GROUP BY id_user order by id_user;";

            var cmd = new MySqlCommand(readChatInfoQuery, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }

            var result = new List<Tuple<int, string, int>>();

            while (reader.Read())
            {
                result.Add(new Tuple<int, string, int>((int) reader[0], (string) reader[1], (int) reader[2]));
            }

            reader.Close();

            return result;
        }

        public int GetIdLastMes(int id_currentUser, int id_targetUser)
        {
            string readChatInfoQuery = "SELECT max(id_message) " +
                                       "FROM users, messages as a " +
                                       "WHERE (a.id_from= " + id_currentUser + " and a.id_to= " + id_targetUser + ") " +
                                       "or (a.id_from = " + id_targetUser + " and a.id_to = " + id_currentUser + ") ";

            var cmd = new MySqlCommand(readChatInfoQuery, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            int result = -1;
            while (reader.Read())
            {
                result = (int) reader[0];
            }

            reader.Close();

            return result;
        }

        /// <summary>
        /// Finds user in DB.
        /// </summary>
        /// <param name="targetUserName">The user name that you are looking for.</param>
        /// <returns>
        ///     <para>-1 | no such users</para>
        ///     <para>user id | on success</para>
        /// </returns>
        public int FindUser(string targetUserName)
        {
            string findUserQuery = "SELECT id_user FROM `users` WHERE userName = '" +
                                   MySqlHelper.EscapeString(targetUserName) + "'";

            var cmd = new MySqlCommand(findUserQuery, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                reader.Close();
                return -1;
            }

            reader.Read();

            var userId = (int) reader[0];
            reader.Close();

            return userId;
        }

        /// <summary>
        /// Returns the specified number of last messages for chat.
        /// </summary>
        /// <param name="quantityOfMessages">Quantity of messages to return.</param>
        /// <param name="id_currentUser">First user id.</param>
        /// <param name="id_TargetUser">Second user id.</param>
        /// <returns>
        ///     <para>null | no such users or no messages for them</para>
        ///     <para>List(Message) | List with Message class entities</para>
        /// </returns>
        public List<Message> GetLastMessages(int quantityOfMessages, int id_currentUser, int id_TargetUser)
        {
            string getLastMessagesQuery = "select userName, content from " +
                                          "(SELECT userName, content, id_message " +
                                          "FROM users, messages, contents " +
                                          "WHERE messages.id_content = contents.id_content " +
                                          "and id_from = id_user " +
                                          "and((id_to = " + id_currentUser + " and id_from = " + id_TargetUser + ") " +
                                          "or (id_from = " + id_currentUser + " and id_to = " + id_TargetUser + ")) " +
                                          "order by id_message desc limit " + quantityOfMessages + ") newTable " +
                                          "order by id_message;";

            var cmd = new MySqlCommand(getLastMessagesQuery, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }

            var result = new List<Message>();
            while (reader.Read())
            {
                result.Add(new Message(reader[0] as string, reader[1] as string));
            }

            reader.Close();

            return result;
        }

        /// <summary>
        /// Returns the specified number of messages up to the lower limit.
        /// <para>For example from array {1, 2, 3, 4, 5} with quantity = 2 and lower limit = 3 returns {4, 5}</para>
        /// </summary>
        /// <param name="quantityOfMessages">Quantity of messages to return.</param>
        /// <param name="lowerLimitNum">Lower limit number.</param>
        /// <param name="id_currentUser">First user id.</param>
        /// <param name="id_TargetUser">Second user id.</param>
        /// <returns>
        ///     <para>null | no such users or no such messages for them</para>
        ///     <para>List(Message) | List with Message class entities</para>
        /// </returns>
        public List<Message> GetMessagesBefore(int quantityOfMessages, int lowerLimitNum, int id_currentUser,
            int id_TargetUser)
        {
            string getMessagesQuery = "select userName, content " +
                                      "from " +
                                      "(SELECT userName, content, id_message " +
                                      "FROM users, messages, contents " +
                                      "WHERE messages.id_content = contents.id_content and id_from = id_user " +
                                      "and id_message < (SELECT * FROM  " +
                                      "(select id_message from messages i " +
                                      "where ((i.id_to= " + id_currentUser + " and i.id_from= " + id_TargetUser + ") " +
                                      "or (i.id_from = " + id_currentUser + " and i.id_to = " + id_TargetUser + ")) " +
                                      "order by i.id_message desc limit " + lowerLimitNum + ") t " +
                                      "ORDER BY t.id_message LIMIT 1) " +
                                      "and((id_to = " + id_currentUser + " and id_from = " + id_TargetUser + ") " +
                                      "or (id_from = " + id_currentUser + " and id_to = " + id_TargetUser + ")) " +
                                      "order by id_message desc limit " + quantityOfMessages + ") newTable " +
                                      "order by id_message;";

            var cmd = new MySqlCommand(getMessagesQuery, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                reader.Close();
                return null;
            }

            var result = new List<Message>();

            while (reader.Read())
            {
                result.Add(new Message(reader[0] as string, reader[1] as string));
            }

            reader.Close();

            return result;
        }
    }
}