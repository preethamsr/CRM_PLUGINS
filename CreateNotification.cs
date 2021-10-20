using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Natuzzi_PushNotification
{
    public class CreateNotification : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = organizationServiceFactory.CreateOrganizationService(context.UserId);
            if (context.MessageName.ToLower() == "create")
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity target_obj = context.InputParameters["Target"] as Entity;
                    if (target_obj.LogicalName.ToLower() == "postcomment")
                    {
                        EntityReference createdby = target_obj.GetAttributeValue<EntityReference>("createdby");
                        EntityReference postid = target_obj.GetAttributeValue<EntityReference>("postid");
                        QueryExpression query = new QueryExpression();
                        query.EntityName = "post";
                        ColumnSet columnSet = new ColumnSet("regardingobjectid", "createdby", "largetext");
                        query.ColumnSet = columnSet;
                        Entity post = service.Retrieve(postid.LogicalName, postid.Id, columnSet);
                        EntityReference regardingobj = post.GetAttributeValue<EntityReference>("regardingobjectid");
                        if (regardingobj.LogicalName.ToLower() == "incident")
                        {
                            EntityReference postcreatedby = post.GetAttributeValue<EntityReference>("createdby");
                            string post_name = post.GetAttributeValue<string>("largetext");
                            ColumnSet incidentcolumnsset = new ColumnSet("ownerid");
                            Entity caseentity = service.Retrieve(regardingobj.LogicalName, regardingobj.Id, incidentcolumnsset);
                            EntityReference caseowner = caseentity.GetAttributeValue<EntityReference>("ownerid");
                            if (createdby.Id != postcreatedby.Id)
                            {
                                Entity Appnotification = new Entity("appnotification");
                                Appnotification["ownerid"] = new EntityReference(postcreatedby.LogicalName, postcreatedby.Id);
                                Appnotification["title"] = "" + regardingobj.Name + "";
                                Appnotification["icontype"] = new OptionSetValue(100000001);
                                Appnotification["body"] = "The new comment for " + post_name + "";
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
                                Guid newnotification = service.Create(Appnotification);
                            }
                            if (createdby.Id != caseowner.Id)
                            {
                                Entity Appnotification = new Entity("appnotification");
                                Appnotification["ownerid"] = new EntityReference(caseowner.LogicalName, caseowner.Id);
                                Appnotification["title"] = "" + regardingobj.Name + "";
                                Appnotification["icontype"] = new OptionSetValue(100000001);
                                Appnotification["body"] = "The new comment for " + post_name + "";
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
                                Guid newnotification = service.Create(Appnotification);
                            }
                        }

                    }
                    if (target_obj.Attributes.Contains("regardingobjectid"))
                    {
                        EntityReference regardingobjectid = target_obj.GetAttributeValue<EntityReference>("regardingobjectid");
                        if (regardingobjectid.LogicalName.ToLower() == "incident")
                        {
                            ColumnSet Caseowner_column = new ColumnSet("ownerid");
                            Entity Caseentity = service.Retrieve(regardingobjectid.LogicalName, regardingobjectid.Id, Caseowner_column);
                            EntityReference caseownerref = Caseentity.GetAttributeValue<EntityReference>("ownerid");
                            if (target_obj.LogicalName.ToLower() == "email")
                            {
                                OptionSetValue status = target_obj.GetAttributeValue<OptionSetValue>("statuscode");
                                if (status.Value == 4)
                                {
                                    Entity Appnotification = new Entity("appnotification");
                                    Appnotification["ownerid"] = new EntityReference(caseownerref.LogicalName, caseownerref.Id);
                                    Appnotification["title"] = "" + regardingobjectid.Name + "";
                                    Appnotification["icontype"] = new OptionSetValue(100000001);
                                    Appnotification["body"] = "The new " + target_obj.LogicalName + " activity has been created";
                                    ActionModel List = new ActionModel();
                                    List.actions = new List<actions>();
                                    actions actions1 = new actions();
                                    actions1.title = "view record";
                                    actions1.data = new data();
                                    actions1.data.url = "?pagetype=entityrecord&etn=incident&id=" + regardingobjectid.Id + "";
                                    List.actions.Add(actions1);
                                    JsonSerlizer Serializer = new JsonSerlizer();
                                    string data = Serializer.Jsonserilizer(List, List.GetType());
                                    Appnotification["data"] = data;
                                    Guid newnotification = service.Create(Appnotification);

                                }
                            }
                            if (target_obj.LogicalName.ToLower() == "appointment" || target_obj.LogicalName.ToLower() == "phonecall" || target_obj.LogicalName.ToLower() == "task")
                            {
                                EntityReference createdby = target_obj.GetAttributeValue<EntityReference>("createdby");
                                EntityReference owner = target_obj.GetAttributeValue<EntityReference>("ownerid");
                                if (createdby.Id != owner.Id)
                                {
                                    Entity Appnotification = new Entity("appnotification");
                                    Appnotification["ownerid"] = new EntityReference(owner.LogicalName, owner.Id);
                                    Appnotification["title"] = "" + regardingobjectid.Name + "";
                                    Appnotification["icontype"] = new OptionSetValue(100000001);
                                    Appnotification["body"] = "The new " + target_obj.LogicalName + " activity has been created";
                                    ActionModel List = new ActionModel();
                                    List.actions = new List<actions>();
                                    actions actions1 = new actions();
                                    actions1.title = "view record";
                                    actions1.data = new data();
                                    actions1.data.url = "?pagetype=entityrecord&etn=incident&id=" + regardingobjectid.Id + "";
                                    List.actions.Add(actions1);
                                    JsonSerlizer Serializer = new JsonSerlizer();
                                    string data = Serializer.Jsonserilizer(List, List.GetType());
                                    Appnotification["data"] = data;
                                    Guid newnotification = service.Create(Appnotification);
                                }
                                if (createdby.Id != caseownerref.Id)
                                {
                                    Entity Appnotification = new Entity("appnotification");
                                    Appnotification["ownerid"] = new EntityReference(caseownerref.LogicalName, caseownerref.Id);
                                    Appnotification["title"] = "" + regardingobjectid.Name + "";
                                    Appnotification["icontype"] = new OptionSetValue(100000001);
                                    Appnotification["body"] = "The new" + " " + target_obj.LogicalName + " activity has been created";
                                    ActionModel List = new ActionModel();
                                    List.actions = new List<actions>();
                                    actions actions1 = new actions();
                                    actions1.title = "view record";
                                    actions1.data = new data();
                                    actions1.data.url = "?pagetype=entityrecord&etn=incident&id=" + regardingobjectid.Id + "";
                                    List.actions.Add(actions1);
                                    JsonSerlizer Serializer = new JsonSerlizer();
                                    string data = Serializer.Jsonserilizer(List, List.GetType());
                                    Appnotification["data"] = data;
                                    Guid newnotification = service.Create(Appnotification);
                                }
                            }
                            if (target_obj.LogicalName.ToLower() == "post")
                            {
                                EntityReference Postowner = target_obj.GetAttributeValue<EntityReference>("createdby");
                                if (Postowner.Id != caseownerref.Id)
                                {
                                    ColumnSet column = new ColumnSet("title");
                                    Entity case_entity = service.Retrieve(regardingobjectid.LogicalName, regardingobjectid.Id, column);
                                    string casename = case_entity.GetAttributeValue<string>("title");
                                    Entity Appnotification = new Entity("appnotification");
                                    Appnotification["ownerid"] = new EntityReference(caseownerref.LogicalName, caseownerref.Id);
                                    Appnotification["title"] = "" + casename + "";
                                    Appnotification["icontype"] = new OptionSetValue(100000001);
                                    Appnotification["body"] = "The new" + " " + target_obj.LogicalName + " has added";
                                    ActionModel List = new ActionModel();
                                    List.actions = new List<actions>();
                                    actions actions1 = new actions();
                                    actions1.title = "view record";
                                    actions1.data = new data();
                                    actions1.data.url = "?pagetype=entityrecord&etn=incident&id=" + regardingobjectid.Id + "";
                                    List.actions.Add(actions1);
                                    JsonSerlizer Serializer = new JsonSerlizer();
                                    string data = Serializer.Jsonserilizer(List, List.GetType());
                                    Appnotification["data"] = data;
                                    Guid newnotification = service.Create(Appnotification);
                                }

                            }

                        }
                    }
                }
            }

        }
    }
}
