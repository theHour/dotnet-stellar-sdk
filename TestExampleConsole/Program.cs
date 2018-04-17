using Newtonsoft.Json.Linq;
using stellar_dotnetcore_sdk;
using stellar_dotnetcore_sdk.responses;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestExampleConsole
{
    public class Program
    {
        public static string URL = "https://horizon-testnet.stellar.org";
        /// <summary>
        /// Simple playground for Stellar network
        /// Testing c# sdk
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void Main(string[] args)
        {
            Console.WriteLine("----Create a new account----");

            var account = CreateRandomAccount();

            Console.WriteLine($"Address: {account.KeyPair.Address} | Secret: {account.KeyPair.SecretSeed}");

            Console.WriteLine($"-----Get test XML for first account----");
            var result = GetTestAsset(account.KeyPair.Address).GetAwaiter().GetResult();
            Console.WriteLine($"Getting the test was successful: {result}");

            Console.WriteLine("----Connect to server----");
            var server = ConnectToServer(URL);

            Console.WriteLine("----Get balance of first account-----");
            AccountResponse accountResponse = GetBalance(account.KeyPair, server).GetAwaiter().GetResult();
            foreach(var balance in accountResponse.Balances)
            {
                Console.WriteLine($"Balance: {balance.BalanceString} | Asset type: {balance.AssetType}");
            }

            var account2 = CreateRandomAccount();

            Console.WriteLine($"Address: {account2.KeyPair.Address} | Secret: {account2.KeyPair.SecretSeed}");
            Console.WriteLine($"-----Get test XML for second account----");
            var result2 = GetTestAsset(account2.KeyPair.Address).GetAwaiter().GetResult();
            Console.WriteLine($"Getting the test was successful: {result2}");

            Console.WriteLine("----Get balance of second account-----");
            AccountResponse accountResponse2 = GetBalance(account2.KeyPair, server).GetAwaiter().GetResult();
            foreach (var balance in accountResponse2.Balances)
            {
                Console.WriteLine($"Balance: {balance.BalanceString} | Asset type: {balance.AssetType}");
            }
            ///Do payment from one account to another
            

            /// Prevent from closing
            Console.Read();
        }

        /// <summary>
        /// Creates a random account 
        /// Account can be easily retrieved from secretSeed
        /// </summary>
        /// <returns></returns>
        public static Account CreateRandomAccount()
        {
            var keypair = KeyPair.Random();
            var account = new Account(keypair, 0);
            return account;
        }

        /// <summary>
        /// connect to server either your own or to stellars horizon test
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Server ConnectToServer(string url)
        {
             return  new Server(url);
        }

        /// <summary>
        /// Get sequence number of the account. Each time a transaction (operation) is executed the sequence number will increment
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static long GetSequence(string address)
        {
            using (var client = new HttpClient())
            {
                string response = client.GetStringAsync($"{URL}/accounts/{address}").Result;
                var json = JObject.Parse(response);
                return (long)json["sequence"];
            }
        }

        /// <summary>
        /// Get test XML from friendbot
        /// </summary>
        /// <param name="addres"></param>
        /// <returns></returns>
        public static async Task<bool> GetTestAsset(string addres)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"https://friendbot.stellar.org?addr={addres}");
            return response.IsSuccessStatusCode;
        } 

        /// <summary>
        /// Read the balances for the given account.
        /// Balances are in the array because an account can hold a lot of assets
        /// </summary>
        /// <param name="keyPair"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public static async Task<AccountResponse> GetBalance(KeyPair keyPair, Server server)
        {
            return await server.Accounts.Account(keyPair);
        }
    }
}
