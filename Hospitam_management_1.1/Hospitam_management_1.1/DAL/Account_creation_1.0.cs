using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Hospitam_management_1._1.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System.Net.Http.Formatting;
using System.Text;

namespace Hospitam_management_1._1.DAL
{
    public class Account_creation
    {
        public IEnumerable<account_creation> retrieve_department()
        {
            List<account_creation> department_list = new List<account_creation>();
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
            HttpRequestMessage request = null;
            request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://orgf65ea8ad.api.crm4.dynamics.com/api/data/v9.2/preetham_departmentses?$select=preetham_name"));
            var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
            if (response.IsSuccessStatusCode)
            {
                dynamic departments = JValue.Parse(response.Content.ReadAsStringAsync().Result);
                dynamic values = JValue.Parse(departments.value.ToString());
                foreach (var dep in values)
                {
                    account_creation ac = new account_creation();
                    ac.Department = dep.preetham_name;
                    ac.department_id = dep.preetham_departmentsid;
                    department_list.Add(ac);


                }
            }
            return department_list;
        }

        public IEnumerable<account_creation> Retrieve_doctors()
        {
            List<account_creation> doctors_data = new List<account_creation>();
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
            HttpRequestMessage request = null;
            request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://orgf65ea8ad.api.crm4.dynamics.com/api/data/v9.2/preetham_doctorses?$select=preetham_name"));
            var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
            if (response.IsSuccessStatusCode)
            {
                dynamic doctors = JValue.Parse(response.Content.ReadAsStringAsync().Result);
                dynamic values = JValue.Parse(doctors.value.ToString());
                foreach (var doc in values)
                {
                    account_creation ac = new account_creation();
                    ac.doctor_id = doc.preetham_doctorsid;
                    ac.Doctor = doc.preetham_name;
                    doctors_data.Add(ac);
                }
            }
            return doctors_data;
        }

        public void new_account(account_creation account_Creation)
        {
            try
            {
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
                account_creation_1 ac_1 = new account_creation_1()
                {
                    name = account_Creation.name
                };

               
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,new Uri("https://orgf65ea8ad.api.crm4.dynamics.com/api/data/v9.2/accounts"))
                {
                    Content = new StringContent(ac_1.ToString(), Encoding.UTF8, "application/json")
                };
                var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Account Created");
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex);
            }
            

        }
    }
}