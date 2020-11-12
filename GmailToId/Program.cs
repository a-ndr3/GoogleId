using Google.Apis.PeopleService.v1;
using System;
using System.Threading.Tasks;
using MyGoogleConnectionLib;

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
            var conn = await Connection.GetAuthorizationUsingCredentials(@"C:\client_secret_10.json", PeopleServiceService.Scope.Contacts);

            var googlePeopleService = new GooglePeopleService(conn);

            //var googlePeopleService = new GooglePeopleService(@"C:\secr10.p12", PeopleServiceService.Scope.Contacts, "mine.gserviceaccount.com");

            var id = googlePeopleService.GetId("somemail@gmail.com");

        }
    }

}
