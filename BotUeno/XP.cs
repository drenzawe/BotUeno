using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotUeno
{
    class XP
    {
        public static int returnXP(SocketMessage msg) /*We create a method that returns a integer and has SocketMessage as a parameter*/
        {
            Random rand = new Random(); /*Creates a new instance of random*/
            var msgCount = msg.Content.Length; /*Counts the total amount of characters in the message*/
            var xp = rand.Next(msgCount / 3);/*Calculates the xp by getting the total length of the message dividing it by 3 and then choosing a random integer from the amount this is flexible and you can change this to your own equation if you want*/
            return xp; /*Returns the xp*/
        }

        public static void addXP(IUser user, int xp)/*Creates a new method with IUser and int xp as its params*/
        {
            var database = new UenoDB("UenoDB"); /*Sets up a connection to the database*/
            try /*Tries this*/
            {
                Random rnd = new Random();
                xp = rnd.Next(1, 5);
                var strings = $"UPDATE userinfo SET xp = xp + '{xp}' WHERE userid = '{user.Id}'"; /*This is your SQL string*/
                var reader = database.FireCommand(strings);/*fires the command*/
                reader.Close(); /*Closes the reader*/
                database.CloseConnection(); /*Closes the connection*/
                return;
            }
            catch (Exception)/*Catches any errors*/
            {
                database.CloseConnection(); /*Closes the connection if there is any errors*/
                return;
            }
        }
        public static void levelUp(IUser user)
        {
            var database = new UenoDB("UenoDB");
            try/*Tries the following code*/
            {
                var strings = $"UPDATE userinfo SET lvl = lvl + '1', xp = '0' WHERE userid = '{user.Id}'";/*this is your sql string*/
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
