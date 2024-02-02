using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TradingSystem
{
    public  class AuthenticationService
    {
        private string Username { get; set; }
        private string PasswordHash { get; set; }
        // Add other user-related properties

        protected static Dictionary<string, string> TraderList = new Dictionary<string, string>()
        {
            { "Nicolas","HABER"},
            {"","" },
            {"Georges","PasswordC#" }


        };
        public static void Add(string NewUser,string Password)
        {
            
            if (AuthenticationService.AuthenticateUser())
            {
               
                TraderList.Add(NewUser, Password);
                Console.WriteLine("{0} was added to the list of traders", NewUser);
            }
            else 
            {
                Console.WriteLine("Failed to add {0}", NewUser);     
            }
        }
        private bool CheckPassword(string enteredPassword)
        {

            return enteredPassword == PasswordHash;
        }
        // Sample method for user authentication
        public static bool AuthenticateUser()
        {
            Console.WriteLine("In order to have access to the desk you have to Login\nSuperUser(UserName:Nicolas;Password:HABER");
            Console.Write("UserName:");
            string username = Console.ReadLine();
            Console.Write("Password:");
            string password = Console.ReadLine();
            AuthenticationService service = new AuthenticationService();
            // Retrieve user from the database based on the username
            string[] Trader = service.GetUserFromDatabase(username);

            if (service.CheckPassword(password))
            {
                // Authentication successful
                Console.WriteLine("Login successful. Welcome, " + username);
                return true;
            }
            else
            {
                // Authentication failed
                Console.WriteLine("Invalid credentials. Please try again.");
                return false;
            }
        }
        private string[] GetUserFromDatabase(string username)
        {
            if (TraderList.TryGetValue(username, out string value))  // StockDic.ContainsKey(tickerSymbol))
            {
                Username = username;
                PasswordHash = value;
                return new string[] { username, value };
            }
            else
            {
                return null;
            }

        }

        
    }
}




