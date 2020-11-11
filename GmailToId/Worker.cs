using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailToId
{
    class Worker
    {
        private string mailToId = "";
        public Worker(string gmail)
        {
            mailToId = gmail;
        }
        private async Task<string> RegisterUser(PeopleServiceService service)
        {
            Person contactToCreate = new Person();
            List<Name> names = new List<Name>();
            List<EmailAddress> email = new List<EmailAddress>();
            List<Biography> biographies = new List<Biography>();
            List<Gender> genders = new List<Gender>();

            names.Add(new Name() { GivenName = "John", FamilyName = "Doe" });
            email.Add(new EmailAddress() { DisplayName = "test", Value = mailToId });
            biographies.Add(new Biography() { Value = "10.10.2005" });
            genders.Add(new Gender() { Value = "male" });

            contactToCreate.Names = names;
            contactToCreate.Biographies = biographies;
            contactToCreate.Genders = genders;
            contactToCreate.EmailAddresses = email;

            Google.Apis.PeopleService.v1.PeopleResource.CreateContactRequest request =
             new Google.Apis.PeopleService.v1.PeopleResource.CreateContactRequest(service, contactToCreate);

            Person createdContact = request.Execute();

            return createdContact.ResourceName;

        }

        private async void DeleteUser(PeopleServiceService service, string id)
        {
            Google.Apis.PeopleService.v1.PeopleResource.DeleteContactRequest request =
            new Google.Apis.PeopleService.v1.PeopleResource.DeleteContactRequest(service, id);
            await request.ExecuteAsync();
        }

        private async Task<ListConnectionsResponse> GetAddInfo(PeopleServiceService service)
        {
            Google.Apis.PeopleService.v1.PeopleResource.ConnectionsResource.ListRequest peopleRequest =
            service.People.Connections.List("people/me");
            peopleRequest.PersonFields = "names,emailAddresses,userDefined,clientData,coverPhotos,locations,phoneNumbers,photos";
            peopleRequest.SortOrder = (Google.Apis.PeopleService.v1.PeopleResource.ConnectionsResource.ListRequest.SortOrderEnum)1;
            ListConnectionsResponse people = peopleRequest.Execute();

            return people;
        }
        public string GetId(PeopleServiceService service)
        {
            var decId = RegisterUser(service);

            var contactInfoResponse = GetAddInfo(service);

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
                Google.Apis.PeopleService.v1.PeopleResource.GetRequest request = new Google.Apis.PeopleService.v1.PeopleResource.GetRequest(service, decId.Result);
                request.PersonFields = "metadata";

                var requestResult = request.Execute();

                if (requestResult.Metadata.Sources.Count > 1)
                {
                    return requestResult.Metadata.Sources[1].Id;
                }
            }

            DeleteUser(service, decId.Result);


            return String.Empty;
        }

    }
}
