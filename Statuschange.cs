using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Text.Json;


namespace Natuzzi_PushNotification
{
    public class Statuschange : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (context.MessageName.ToLower() == "update")
                {
                    IOrganizationServiceFactory organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = (IOrganizationService)organizationServiceFactory.CreateOrganizationService(context.UserId);
                    ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                    if ((context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity))
                    {
                        Entity email;
                        Entity target;
                        Entity Appnotification = new Entity("appnotification");
                        EntityReference regardingObj;
                        target = context.InputParameters["Target"] as Entity;
                        ColumnSet columnSet = new ColumnSet("regardingobjectid", "ownerid", "modifiedby");
                        email = service.Retrieve("" + target.LogicalName + "", target.Id, columnSet);
                        regardingObj = email.GetAttributeValue<EntityReference>("regardingobjectid");
                        if (regardingObj.LogicalName.ToLower() == "incident")
                        {
                            EntityReference modifiedBy = email.GetAttributeValue<EntityReference>("modifiedby");
                            EntityReference OwnerID = email.GetAttributeValue<EntityReference>("ownerid");
                            ColumnSet casecolumnset = new ColumnSet("ownerid");
                            Entity case_owner = service.Retrieve(regardingObj.LogicalName, regardingObj.Id, casecolumnset);
                            EntityReference caseonwer = case_owner.GetAttributeValue<EntityReference>("ownerid");
                            if (modifiedBy.Id != OwnerID.Id)
                            {
                                Appnotification["ownerid"] = new EntityReference(OwnerID.LogicalName, OwnerID.Id);
                                Appnotification["title"] = "" + regardingObj.Name + "";
                                Appnotification["icontype"] = new OptionSetValue(100000001);
                                Appnotification["body"] = "The " + target.LogicalName + " activity  status  has been changed";
                                ActionModel List = new ActionModel();
                                List.actions = new List<actions>();
                                actions actions1 = new actions();
                                actions1.title = "view record";
                                actions1.data = new data();
                                actions1.data.url = "?pagetype=entityrecord&etn=incident&id=" + regardingObj.Id + "";
                                List.actions.Add(actions1);
                                JsonSerlizer Serializer = new JsonSerlizer();
                                string data = Serializer.Jsonserilizer(List, List.GetType());
                                Appnotification["data"] = data;
                                tracingService.Trace("31");
                                Guid newnotification = service.Create(Appnotification);
                            }
                            if (caseonwer.Id != modifiedBy.Id)
                            {
                                Appnotification["ownerid"] = new EntityReference(caseonwer.LogicalName, caseonwer.Id);
                                Appnotification["title"] = "" + regardingObj.Name + "";
                                Appnotification["icontype"] = new OptionSetValue(100000001);
                                Appnotification["body"] = "The " + target.LogicalName + " activity  status  has been changed";
                                ActionModel List = new ActionModel();
                                List.actions = new List<actions>();
                                actions actions1 = new actions();
                                actions1.title = "view record";
                                actions1.data = new data();
                                actions1.data.url = "?pagetype=entityrecord&etn=incident&id=" + regardingObj.Id + "";
                                List.actions.Add(actions1);
                                JsonSerlizer Serializer = new JsonSerlizer();
                                string data = Serializer.Jsonserilizer(List, List.GetType());
                                Appnotification["data"] = data;
                                tracingService.Trace("31");
                                service.Create(Appnotification);
                            }

                        }
                    }

                }

            }
            catch (InvalidPluginExecutionException ex)
            {
                throw ex;
            }
        }


    }
}
