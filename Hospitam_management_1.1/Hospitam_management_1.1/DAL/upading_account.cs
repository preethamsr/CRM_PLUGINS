using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Hospitam_management_1._1.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Hospitam_management_1._1.DAL
{
    public class upading_account
    {
        public IEnumerable<account_entites> update_account(Guid id)
        {

            List<account_entites> new_data = new List<account_entites>();
        string resource = "https://orgf65ea8ad.crm4.dynamics.com/";
        UserCredential clientredentials = new UserCredential("hospitalmanagement@pradstrial.onmicrosoft.com", "ranjitha@716");
        string client_id = "2b572be0-722d-4581-8481-ae89890bdc0a";
        AuthenticationContext authenticationContext = new AuthenticationContext("https://login.microsoftonline.com/2ce71a2c-f78d-45c8-9d23-275b60dde51f/oauth2/v2.0/authorize");
        AuthenticationResult authenticationResult = authenticationContext.AcquireToken(resource, client_id, clientredentials);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            httpClient.BaseAddress = new Uri("https://orgf65ea8ad.api.crm4.dynamics.com/api/data/v9.2/");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://orgf65ea8ad.api.crm4.dynamics.com/api/data/v9.1/accounts("+id+ ")?$select=preetham_registrationnumber,name,preetham_reasonforregistration&$expand=preetham_DepartmentId($select=preetham_name),preetham_doctorId($select=preetham_name)"));
            var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
            if(response.IsSuccessStatusCode)
            {
                dynamic accountdata = JValue.Parse(response.Content.ReadAsStringAsync().Result);
                dynamic values = JValue.Parse(accountdata.value.ToString());
                foreach(var val in values)
                {
                    account_entites ae = new account_entites();
                    ae.preetham_registrationnumber = val.preetham_registrationnumber;
                    ae.reason_for_registration = val.reason_for_registration;
                    ae.name = val.name;
                    ae.Department=val.preetham_DepartmentId.preetham_name;
                    ae.Doctor=val.preetham_doctorId.preetham_name;
                    new_data.Add(ae);
                }
               
            }
            return new_data;
        }
    }
}