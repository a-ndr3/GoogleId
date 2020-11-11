using System;
using System.Threading.Tasks;

namespace GmailToId
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {

            try
            {
                new Program().Run1().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    
                }
            }

            
        }

        //https://console.developers.google.com/apis/api/people.googleapis.com/overview?project=
        //https://console.developers.google.com/apis/credentials/
        // allow access to people api / contacts api
        // allow unsafe apps

        private async Task Run1()
        {

            Connections connections = new Connections();
            var cred = await connections.GetUserCredential("C:\\Users\\Me\\Downloads\\client_secret_10.json");

            Worker worker = new Worker("somemail@gmail.com");

            worker.GetId(connections.GetService(cred));

        }

    }
}
