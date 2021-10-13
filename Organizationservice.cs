using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel.Description;
using System.Net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk.Query;

namespace CSMTCONNECT_webservice
{
    class Program
    {
        static void Main(string[] args)
        {
       
       
            try
            {

                string authType = "OAuth";
                string URL = "https://csmttest.crm4.dynamics.com";
                string userName = "demo-PSA@csmt.it";
                string password = "viaBranze45";
                string appId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
                string reDirectURI = "app://58145B91-0C36-4500-8554-080854F2AC97";
                string loginPrompt = "Auto";
                string ConnectionString = string.Format("AuthType = {0};Username = {1};Password = {2}; Url = {3}; AppId={4}; RedirectUri={5};LoginPrompt={6}",
                                                        authType, userName, password, URL, appId, reDirectURI, loginPrompt);

                CrmServiceClient svc = new CrmServiceClient(ConnectionString);

                if (svc.IsReady)
                {
                    PerformCRUD(svc);
                }
            }

             catch(Exception ex)
            {
                ex.Message.ToString();
            }

        }
        private static void PerformCRUD(CrmServiceClient svc)
        {

            WhoAmIRequest req = new WhoAmIRequest();
            WhoAmIResponse res = (WhoAmIResponse)svc.Execute(req);
            Console.WriteLine("Username:" + res.ResponseName.ToString());

            //CREATE
            var myContact = new Entity("contact");
            myContact.Attributes["lastname"] = "Test Webservice";
            myContact.Attributes["firstname"] = "Preetham";
            Guid RecordID = svc.Create(myContact);
            Console.WriteLine("Contact created...");


            //RETRIEVE
            Console.WriteLine("Retrieving Contact...");
            Entity contact = svc.Retrieve("contact", RecordID, new ColumnSet("firstname", "lastname"));
            Console.WriteLine("Contact First Name:" + contact["firstname"]);

            //UPDATE
            Console.WriteLine("Updating Contact...");
            Entity entContact = new Entity("contact");
            entContact.Id = RecordID;
            entContact.Attributes["lastname"] = "Sample New";
            svc.Update(entContact);
            Console.WriteLine("Contact updated");

            //DELETE
            Console.WriteLine("Contact Deleting");
            svc.Delete("contact", RecordID);
            Console.WriteLine("Contact Deleted");
            Console.ReadLine();
        }
    }
}
