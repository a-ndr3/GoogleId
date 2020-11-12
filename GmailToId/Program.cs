using Google.Apis.PeopleService.v1;
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
                new Program().Run().Wait();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private async Task Run()
        {

            var conn = await Connection.GetAuthorizationUsingCredentials(@"C:\Users\Me\\Downloads\client_secret_10.json", PeopleServiceService.Scope.Contacts);

            var googlePeopleService = new GooglePeopleService(conn);

            var id = googlePeopleService.GetId("somemail@gmail.com");

        }
    }

}
