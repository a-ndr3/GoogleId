using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GmailToId
{
    interface IAuth
    {
        public Task<UserCredential> GetUserCredential(string keyFile, ClientSecrets secrets);

    }

    public class Connections : IAuth
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyFile"> oauth credentials file from console.developers.google.com </param>
        /// <param name="secrets"> client id, client secret from console.developers.google.com </param>
        /// <returns></returns>
        public async Task<UserCredential> GetUserCredential(string keyFile, ClientSecrets secrets = null)
        {
            UserCredential credential;

            if (keyFile.Any())
            {
                using (var stream = new FileStream(keyFile, FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { PeopleServiceService.Scope.Contacts },
                        "user", CancellationToken.None); // + dataStore = authorization
                }
            }
            else
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                new[] { PeopleServiceService.Scope.Contacts },
                "user",
                CancellationToken.None);
            }

            return credential;
        }

        public PeopleServiceService GetService(UserCredential credentials)
        {
            return new Google.Apis.PeopleService.v1.PeopleServiceService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
            });
        }

    }
}
