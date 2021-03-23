using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;
using Hospitam_management_1._1.Models;
using Microsoft.Xrm.Tooling.Connector;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using System.Net;
using System.Configuration;
using Microsoft.Crm.Sdk.Messages;
using System.IO;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using AuthenticationContext = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Hospitam_management_1._1.DAL
{
    public class DAL_account_entites
    {
        public IEnumerable<account_entites> Retrive_records()
        {
           
            List<account_entites> data_list = new List<account_entites>();
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
            request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://orgf65ea8ad.api.crm4.dynamics.com/api/data/v9.2/accounts?$select=name,preetham_reasonforregistration,preetham_reasonforregistrationnotes,preetham_registrationnumber&$expand=preetham_DepartmentId($select=preetham_name),preetham_doctorId($select=preetham_name)"));
            var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
            
            if (response.IsSuccessStatusCode)
            {                           
                dynamic userdetails = JValue.Parse( response.Content.ReadAsStringAsync().Result);
                dynamic values = JValue.Parse(userdetails.value.ToString());
                foreach(var user in values)
                {
                    account_entites ae = new account_entites();
                    if (user.preetham_registrationnumber!= null)
                    {

                        ae.preetham_registrationnumber =user.preetham_registrationnumber;
                        ae.name = user.name;
                        ae.Department = user.preetham_DepartmentId.preetham_name;
                        ae.Doctor = user.preetham_doctorId.preetham_name;
                        ae.reason_for_registration = user.preetham_reasonforregistration;
                        ae.account_id = user.accountid;
                        ae.department_id = user.preetham_DepartmentId.preetham_departmentsid;
                        ae.doctor_id = user.preetham_doctorId.preetham_doctorsid;
                       
                        data_list.Add(ae);
                    }
                    
                }
             
            }
            return data_list;
        }            
        }
    }
