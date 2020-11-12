using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GmailToId
{
    public static class Connection
    {
        /// <summary>
        /// OAuth2 authorization in google services
        /// </summary>
        /// <param name="keyFilePath"> oauth credentials file from console.developers.google.com </param>
        /// <param name="googleScope"> type of scope from google api </param>
        /// <param name="secrets"> client id, client secret from console.developers.google.com </param>
        /// <returns> User credential </returns>
        public static async Task<UserCredential> GetAuthorizationUsingCredentials(string keyFilePath, string googleScope, ClientSecrets secrets = null)
        {
            if (String.IsNullOrEmpty(keyFilePath) && secrets == null)
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrEmpty(googleScope))
            {
                throw new ArgumentException($"'{nameof(googleScope)}' cannot be null or empty", nameof(googleScope));
            }

            UserCredential credential;

            if (!String.IsNullOrEmpty(keyFilePath))
            {
                using (var stream = new FileStream(keyFilePath, FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { googleScope },
                    "user", CancellationToken.None); // + dataStore = authorization
                }
            }
            else
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                new[] { googleScope },
                "user",
                CancellationToken.None);
            }

            return credential;
        }
    }
}