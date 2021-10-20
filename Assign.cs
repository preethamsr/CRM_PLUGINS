using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Natuzzi_PushNotification
{
    public class Assign : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = organizationServiceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            if ((context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference) || (context.InputParameters["Target"] is Entity))
            {
                if (context.MessageName.ToLower() == "assign" )
                {
                  
                    EntityReference assign = context.InputParameters["Target"] as EntityReference;
                    ColumnSet Activity_col = new ColumnSet("regardingobjectid","ownerid");
                    Entity Activity_obj = service.Retrieve(assign.LogicalName, assign.Id, Activity_col);
                    EntityReference regardingobj = Activity_obj.GetAttributeValue<EntityReference>("regardingobjectid");
                    if(regardingobj.LogicalName.ToLower()== "incident")
                    {
                        EntityReference assignee = context.InputParameters["Assignee"] as EntityReference;
                        Entity Appnotification = new Entity("appnotification");
                        EntityReference systemuser = Activity_obj.GetAttributeValue<EntityReference>("ownerid");
                        Appnotification["ownerid"] = new EntityReference(assignee.LogicalName, assignee.Id);
                        Appnotification["title"] = "" + regardingobj.Name + "";
                        Appnotification["icontype"] = new OptionSetValue(100000001);                    
                        ColumnSet assigneename=new ColumnSet("firstname", "lastname");
                        Entity Systemuserinfo = service.Retrieve("systemuser", context.UserId, assigneename);
                        string firstname = Systemuserinfo.GetAttributeValue<string>("firstname");
                        string lastname = Systemuserinfo.GetAttributeValue<string>("lastname");
                        Appnotification["body"] = "The new "+ assign.LogicalName + " activity has assigned from "+ firstname +" "+ lastname;
                        ActionModel List = new ActionModel();
                        List.actions = new List<actions>();
                        actions actions1 = new actions();
                        actions1.title = "view record";
                        actions1.data = new data();
                        actions1.data.url = "?pagetype=entityrecord&etn=incident&id=" + regardingobj.Id + "";
                        List.actions.Add(actions1);
                        JsonSerlizer Serializer = new JsonSerlizer();
                        string data = Serializer.Jsonserilizer(List, List.GetType());
                        Appnotification["data"] = data;
                        tracingService.Trace("31");
                        Guid newnotification = service.Create(Appnotification);
                    }
                    

                }
                if( context.MessageName.ToLower() == "update")
                {
                    Entity assign = context.InputParameters["Target"] as Entity;
                    ColumnSet columnSet = new ColumnSet("regardingobjectid");
                    Entity activity = service.Retrieve(assign.LogicalName, assign.Id, columnSet);
                    EntityReference regardingobj = activity.GetAttributeValue<EntityReference>("regardingobjectid");
                    if(regardingobj.LogicalName.ToLower()== "incident")
                    {
                        EntityReference owner_id = assign.GetAttributeValue<EntityReference>("ownerid");
                        Entity Appnotification = new Entity("appnotification");
                        Appnotification["ownerid"] = new EntityReference(owner_id.LogicalName, owner_id.Id);
                        Appnotification["title"] = "" + regardingobj.Name + "";
                        Appnotification["icontype"] = new OptionSetValue(100000001);
                        ColumnSet assigneename = new ColumnSet("firstname", "lastname");
                        Entity Systemuserinfo = service.Retrieve("systemuser", context.UserId, assigneename);
                        string firstname = Systemuserinfo.GetAttributeValue<string>("firstname");
                        string lastname = Systemuserinfo.GetAttributeValue<string>("lastname");
                        Appnotification["body"] = "The new " + assign.LogicalName + " activity has assigned from " + firstname + " " + lastname;
                        ActionModel List = new ActionModel();
                        List.actions = new List<actions>();
                        actions actions1 = new actions();
                        actions1.title = "view record";
                        actions1.data = new data();
                        actions1.data.url = "?pagetype=entityrecord&etn=incident&id=" + regardingobj.Id + "";
                        List.actions.Add(actions1);
                        JsonSerlizer Serializer = new JsonSerlizer();
                        string data = Serializer.Jsonserilizer(List, List.GetType());
                        Appnotification["data"] = data;
                        tracingService.Trace("31");
                        Guid newnotification = service.Create(Appnotification);

                    }
                }

            }
        }
    }
}
