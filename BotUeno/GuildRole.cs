using Discord;
using Discord.WebSocket;
using System;

namespace BotUeno
{
    class GuildRole
    {
        public static void rolePeasant(IUser user)
        {
            var database = new UenoDB("UenoDB");
            try/*Tries the following code*/
            {
                var strings = $"UPDATE userinfo SET guildrole = 'PEASANT' WHERE userid = '{user.Id}'";/*this is your sql string*/
                var reader = database.FireCommand(strings); /*Fires the command*/
                reader.Close(); /*Closes the reader*/
                database.CloseConnection(); /*Closes the connections*/
                return;
            }
            catch (Exception)/*Catches any errors*/
            {
                database.CloseConnection(); /*Closes the connection*/
                return;
            }
        }
        public static void rolePlebian(IUser user)
        {
            var database = new UenoDB("UenoDB");
            try/*Tries the following code*/
            {
                var strings = $"UPDATE userinfo SET guildrole = 'PLEBIAN' WHERE userid = '{user.Id}'";/*this is your sql string*/
                var reader = database.FireCommand(strings); /*Fires the command*/
                reader.Close(); /*Closes the reader*/
                database.CloseConnection(); /*Closes the connections*/
                return;
            }
            catch (Exception)/*Catches any errors*/
            {
                database.CloseConnection(); /*Closes the connection*/
                return;
            }
        }
        public static void roleCommoner(IUser user)
        {
            var database = new UenoDB("UenoDB");
            try/*Tries the following code*/
            {
                var strings = $"UPDATE userinfo SET guildrole = 'COMMONER' WHERE userid = '{user.Id}'";/*this is your sql string*/
                var reader = database.FireCommand(strings); /*Fires the command*/
                reader.Close(); /*Closes the reader*/
                database.CloseConnection(); /*Closes the connections*/
                return;
            }
            catch (Exception)/*Catches any errors*/
            {
                database.CloseConnection(); /*Closes the connection*/
                return;
            }
        }
        public static void roleLoyalist(IUser user)
        {
            var database = new UenoDB("UenoDB");
            try/*Tries the following code*/
            {
                var strings = $"UPDATE userinfo SET guildrole = 'LOYALIST' WHERE userid = '{user.Id}'";/*this is your sql string*/
                var reader = database.FireCommand(strings); /*Fires the command*/
                reader.Close(); /*Closes the reader*/
                database.CloseConnection(); /*Closes the connections*/
                return;
            }
            catch (Exception)/*Catches any errors*/
            {
                database.CloseConnection(); /*Closes the connection*/
                return;
            }
        }
        public static void roleElite(IUser user)
        {
            var database = new UenoDB("UenoDB");
            try/*Tries the following code*/
            {
                var strings = $"UPDATE userinfo SET guildrole = 'ELITE' WHERE userid = '{user.Id}'";/*this is your sql string*/
                var reader = database.FireCommand(strings); /*Fires the command*/
                reader.Close(); /*Closes the reader*/
                database.CloseConnection(); /*Closes the connections*/
                return;
            }
            catch (Exception)/*Catches any errors*/
            {
                database.CloseConnection(); /*Closes the connection*/
                return;
            }
        }
    }
}
