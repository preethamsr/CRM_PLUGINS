using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
namespace Hospital_management_1._0.Plugins
{
    public class Makepaymentoutpatient : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            string _registration_number = ((string)context.InputParameters["registration_number"]);
            QueryExpression outpatient_list = new QueryExpression()
            {
                EntityName= "preetham_case",
                ColumnSet = new ColumnSet("preetham_registrationumber")
            };
            outpatient_list.Criteria.AddCondition("preetham_registrationumber", ConditionOperator.Equal, _registration_number);
            EntityCollection _outpatient_list = service.RetrieveMultiple(outpatient_list);
            if(_outpatient_list.Entities.Count!=0)
            {
                foreach(Entity opl in _outpatient_list.Entities)
                {
                    if(opl.Attributes.Contains("preetham_registrationumber"))
                    {
                        var req = new SetStateRequest()
                        {
                            EntityMoniker = new EntityReference()
                            {
                                LogicalName = opl.LogicalName,
                                Id = opl.Id
                            },
                            State = new OptionSetValue(1),
                            Status = new OptionSetValue(2)
                        };
                        service.Execute(req);
                    }
                }
                QueryExpression outpatient_bill_data = new QueryExpression()
                {
                    EntityName = "preetham_bill",
                    ColumnSet = new ColumnSet("preetham_register_number")
                };
                outpatient_bill_data.Criteria.AddCondition("preetham_register_number", ConditionOperator.Equal, _registration_number);
                EntityCollection outpatient = service.RetrieveMultiple(outpatient_bill_data);
                if(outpatient.Entities.Count!=0)
                {
                    Entity bill = new Entity("preetham_bill");
                    foreach(Entity obd in outpatient.Entities)
                    {
                        if(obd.Attributes.Contains("preetham_register_number"))
                        {
                            bill.Attributes["preetham_paymentstatus"] = true;
                            bill.Id = obd.Id;
                            service.Update(bill);
                            var inactive_record = new SetStateRequest()
                            {
                                EntityMoniker = new EntityReference()
                                {
                                    LogicalName = obd.LogicalName,
                                    Id = obd.Id
                                },
                                State = new OptionSetValue(1),
                                Status = new OptionSetValue(2)
                            };
                            service.Execute(inactive_record);
                           
                        }
                    }
                }
            }
        }
    }
}
