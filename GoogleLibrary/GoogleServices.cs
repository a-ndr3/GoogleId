using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MyGoogleConnectionLib
{
    public class GooglePeopleService
    {
        private PeopleServiceService peopleService;

        /// <summary>
        /// OAuth2 service
        /// </summary>
        public GooglePeopleService(UserCredential credentials)
        {
            peopleService = new PeopleServiceService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = credentials
            });
        }

        /// <summary>
        /// Service account that uses .p12 key and service mail
        /// </summary>
        /// <param name="keyFilePath"> .p12 key obtained from dev.google.com service accounts </param>
        /// <param name="googleScope"> type of scope from google api </param>
        public GooglePeopleService(string keyFilePath, string googleScope, string serviceAccEmail)
        {
            ServiceCredential credentials;
            var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);
            credentials = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccEmail)
            {
                Scopes = new[] { googleScope },
            }.FromCertificate(certificate));
            peopleService = new PeopleServiceService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = credentials
            });
        }

        private async Task<string> RegisterUser(string mail)
        {
            var contactToCreate = new Person();

            List<Name> names = new List<Name>();
            List<EmailAddress> email = new List<EmailAddress>();
            List<Biography> biographies = new List<Biography>();
            List<Gender> genders = new List<Gender>();

            names.Add(new Name() { GivenName = "John", FamilyName = "Doe" });
            email.Add(new EmailAddress() { DisplayName = "test", Value = mail });
            biographies.Add(new Biography() { Value = "10.10.2005" });
            genders.Add(new Gender() { Value = "male" });

            contactToCreate.Names = names;
            contactToCreate.Biographies = biographies;
            contactToCreate.Genders = genders;
            contactToCreate.EmailAddresses = email;

            var request = new PeopleResource.CreateContactRequest(peopleService, contactToCreate);

            Person createdContact = request.Execute();

            return createdContact.ResourceName;

        }

        private async void DeleteUser(string id)
        {
            var request = new PeopleResource.DeleteContactRequest(peopleService, id);
            await request.ExecuteAsync();
        }

        private async Task<ListConnectionsResponse> GetAddInfo()
        {
            var peopleRequest = peopleService.People.Connections.List("people/me");
            peopleRequest.PersonFields = "names,emailAddresses,userDefined,clientData,coverPhotos,locations,phoneNumbers,photos";
            peopleRequest.SortOrder = (PeopleResource.ConnectionsResource.ListRequest.SortOrderEnum)1;

            var people = peopleRequest.Execute();

            return people;
        }

        public string GetId(string gmail)
        {
            var decId = RegisterUser(gmail);

            var contactInfoResponse = GetAddInfo();

            if (contactInfoResponse.Exception == null)
            {
                var person = contactInfoResponse.Result.Connections.Where(x => x.ResourceName.Contains(decId.Result)).ToList();

                if (person.Any())
                {
                    return person.First().CoverPhotos[0].Metadata.Source.Id;

                    //var result = typeof(Person).GetProperties()
                    //.Select(x => new { property = x.Name, value = x.GetValue(person[0]) })
                    //.Where(x => x.value != null)
                    //.ToList();

                    // photo: property photos [0]

                    //System.IO.File.WriteAllText(@"C:\\Users\\me\\Desktop\\result.json", JsonConvert.SerializeObject(result));
                }
            }
            else
            {
                var request = new PeopleResource.GetRequest(peopleService, decId.Result);
                request.PersonFields = "metadata";

                var requestResult = request.Execute();

                if (requestResult.Metadata.Sources.Count > 1)
                {
                    return requestResult.Metadata.Sources[1].Id;
                }
            }

            DeleteUser(decId.Result);


            return String.Empty;
        }
    }
}