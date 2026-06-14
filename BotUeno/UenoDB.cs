using Discord;
using Discord.WebSocket;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace BotUeno
{
    public class UenoDB
    {
        private string table { get; set; }
        private const string server = "YOUR_SERVER";
        private const string database = "YOUR_DATABASE";
        private const string username = "YOUR_USERNAME";
        private const string password = "YOUR_PASSWORD";
        private MySqlConnection dbConnection;

        public UenoDB(string table)
        {
            this.table = table;
            MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Server = server;
            stringBuilder.Database = database;
            stringBuilder.UserID = username;
            stringBuilder.Password = password;
            stringBuilder.SslMode = MySqlSslMode.None;

            var connectionString = stringBuilder.ToString();

            dbConnection = new MySqlConnection(connectionString);

            dbConnection.Open();
        }
        public MySqlDataReader FireCommand(string query)
        {
            if (dbConnection == null)
            {
                return null;
            }

            MySqlCommand command = new MySqlCommand(query, dbConnection);

            var mySqlReader = command.ExecuteReader();

            return mySqlReader;
        }
        public void CloseConnection()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
            }
        }

        public static List<String> CheckExistingUser(IUser user)
        {

            var result = new List<String>();
            var database = new UenoDB("UenoDB");

            var str = $"SELECT * FROM userinfo WHERE userid = '{user.Id}'";
            var tableName = database.FireCommand(str);

            while (tableName.Read())
            {
                var userId = (string)tableName["userid"];

                result.Add(userId);
            }

            database.CloseConnection();

            return result;
           }

        public static string EnterUser(IUser user)
        {

            var database = new UenoDB("UenoDB");

            var str = $"INSERT INTO userinfo (userid, username, lvl, xp, points, guildrole) VALUES ('{user.Id}', '{user.Username}', '1', '0', '0', 'PEASANT')";
            var table = database.FireCommand(str);


            database.CloseConnection();
            return null;
        }

        public static List<UserInfo> GetUserStatus(IUser user)
        {
            var result = new List<UserInfo>();

            var database = new UenoDB("UenoDB");

            var str = $"SELECT * FROM userinfo WHERE userid = '{user.Id}'";
            var userinfo = database.FireCommand(str);

            while (userinfo.Read())
            {//////////
                var userId = (string)userinfo["userid"];
                var userName = (string)userinfo["username"];
                var currentPoints = (int)userinfo["points"];
                var level = (int)userinfo["lvl"]; /*This is what we add*/
                var xp = (int)userinfo["xp"]; /*This is also another thing we add*/
                var guildrole = (string)userinfo["guildrole"];

                result.Add(new UserInfo
                {
                    UserId = userId,
                    Username = userName,
                    Points = currentPoints,
                    Lvl = level, /*We add this*/
                    XP = xp, /*and this*/
                    GuildRole = guildrole
                });
            }
            userinfo.Close();
            database.CloseConnection();

            return result;

        }

        public static void ChangePoints(IUser user, int points)
        {
            var database = new UenoDB("UenoDB");

            try
            {
                var strings = $"UPDATE userinfo SET points = points + '{points}' WHERE userid = '{user.Id}'";
                var reader = database.FireCommand(strings);
                reader.Close();
                database.CloseConnection();
                return;
            }
            catch (Exception)
            {
                database.CloseConnection();
                return;
            }
        }

        public static void checkPoints(IUser user, int points)
        {
            var database = new UenoDB("UenoDB");

            try
            {
                var strings = $"SELECT (points) FROM userinfo WHERE userid = '{user.Id}'";
                var reader = database.FireCommand(strings);
                reader.Close();
                database.CloseConnection();
                return;
            }
            catch (Exception)
            {
                database.CloseConnection();
                return;
            }
        }

        public static void RockPaperScissor(IUser user, int points)
        {
            var database = new UenoDB("UenoDB");

            try
            {
                var strings = $"UPDATE userinfo SET points = points - '{points}' WHERE userid = '{user.Id}'";
                var reader = database.FireCommand(strings);
                reader.Close();
                database.CloseConnection();
                return;
            }
            catch (Exception)
            {
                database.CloseConnection();
                return;
            }
        }

        public static void RockPaperScissorWin(IUser user, int points)
        {
            var database = new UenoDB("UenoDB");

            try
            {
                int won = 2 * points;

                var strings = $"UPDATE userinfo SET points = points + '{won}' WHERE userid = '{user.Id}'";
                var reader = database.FireCommand(strings);
                reader.Close();
                database.CloseConnection();
                return;
            }
            catch (Exception)
            {
                database.CloseConnection();
                return;
            }
        }

        public static void FlipCoin(IUser user, int points)
        {
            var database = new UenoDB("UenoDB");

            try
            {
                var strings = $"UPDATE userinfo SET points = points - '{points}' WHERE userid = '{user.Id}'";
                var reader = database.FireCommand(strings);
                reader.Close();
                database.CloseConnection();
                return;
            }
            catch (Exception)
            {
                database.CloseConnection();
                return;
            }
        }

        public static void FlipCoinWon(IUser user, int points)
        {
            var database = new UenoDB("UenoDB");

            try
            {
                int won = 2 * points;

                var strings = $"UPDATE userinfo SET points = points + '{won}' WHERE userid = '{user.Id}'";
                var reader = database.FireCommand(strings);
                reader.Close();
                database.CloseConnection();
                return;
            }
            catch (Exception)
            {
                database.CloseConnection();
                return;
            }
        }
    }
}
