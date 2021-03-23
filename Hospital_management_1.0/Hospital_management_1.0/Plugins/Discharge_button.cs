using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Hospital_management_1._0.Plugins
{
    public class Discharge_button : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                string registration_number = ((string)context.InputParameters["_registration_number"]);
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                tracingService.Trace("Code stage1");

                QueryExpression inpatientlist = new QueryExpression()
                {
                    EntityName = "preetham_inpatient",
                    ColumnSet = new ColumnSet("preetham_registrationnumber", "preetham_bedtypeid")
                };
                EntityCollection inpatients = service.RetrieveMultiple(inpatientlist);
                if (inpatients.Entities.Count == 0)
                {
                    return;
                }
                Entity bed_management = new Entity("preetham_bedmanagement");
                foreach (Entity ip in inpatients.Entities)
                {
                    if (ip.Attributes.Contains("preetham_registrationnumber"))
                    {
                        var _registration_number = ip.GetAttributeValue<string>("preetham_registrationnumber");
                        if (_registration_number.ToString().ToLower() == registration_number.ToString().ToLower())
                        {
                            tracingService.Trace("stage 2");
                            EntityReference bedtype = ((EntityReference)ip.Attributes["preetham_bedtypeid"]);
                            Guid bedtype_id = bedtype.Id;
                            ColumnSet col = new ColumnSet("preetham_availablebed");
                            Entity bedtype_details = service.Retrieve("preetham_bedmanagement", bedtype_id, col);
                            int available_bed = bedtype_details.GetAttributeValue<int>("preetham_availablebed");
                            int new_available_bed = available_bed + 1;
                            bed_management.Attributes["preetham_availablebed"] = new_available_bed;
                            bed_management.Id = bedtype_id;
                            service.Update(bed_management);
                            QueryExpression bill_data = new QueryExpression()
                            {
                                EntityName = "preetham_bill",
                                ColumnSet = new ColumnSet("preetham_register_number")
                            };
                            bill_data.Criteria.AddCondition("preetham_register_number", ConditionOperator.Equal, registration_number);
                            EntityCollection retivedata= service.RetrieveMultiple(bill_data);
                            if(retivedata.Entities.Count!=0)
                            {
                                foreach(Entity rtdata in retivedata.Entities)
                                {
                                    if(rtdata.Attributes.Contains("preetham_register_number"))
                                    {
                                        var request = new SetStateRequest()
                                        {
                                            EntityMoniker = new EntityReference
                                            {
                                                Id = rtdata.Id,
                                                LogicalName = rtdata.LogicalName
                                             },
                                            State = new OptionSetValue(1),
                                            Status = new OptionSetValue(2)
                                        };
                                        service.Execute(request);
                                    }
                                }
                            }
                            var setStateRequest = new SetStateRequest()
                            {
                                EntityMoniker = new EntityReference
                                {
                                    Id = ip.Id,
                                    LogicalName = ip.LogicalName
                                },
                                State = new OptionSetValue(1),
                                Status = new OptionSetValue(2)
                            };
                            service.Execute(setStateRequest);
                            QueryExpression outpatient_list = new QueryExpression()
                            {
                                EntityName = "preetham_case",
                                ColumnSet = new ColumnSet("preetham_registrationumber")
                            };
                            outpatient_list.Criteria.AddCondition("preetham_registrationumber", ConditionOperator.Equal, registration_number);
                            EntityCollection _outpatient_list = service.RetrieveMultiple(outpatient_list);
                            if(_outpatient_list.Entities.Count!=0)
                            {
                                foreach(Entity opl in _outpatient_list.Entities)
                                {
                                    var inactive_inpatient_record = new SetStateRequest()
                                    {
                                        EntityMoniker = new EntityReference
                                        {
                                            LogicalName = opl.LogicalName,
                                            Id = opl.Id

                                        },
                                         State = new OptionSetValue(1),
                                        Status = new OptionSetValue(2)
                                    };
                                    service.Execute(inactive_inpatient_record);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }           
        }
    }
}
