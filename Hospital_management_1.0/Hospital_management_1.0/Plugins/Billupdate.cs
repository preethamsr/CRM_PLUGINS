using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Hospital_management_1._0.Plugins
{
    public class Billupdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (context.MessageName == "Create")
                {
                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        Entity targetentity = context.InputParameters["Target"] as Entity;
                        if (targetentity.LogicalName == "preetham_procedure_bill")
                        {
                            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                            IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
                            var registration_number = targetentity.GetAttributeValue<string>("preetham_registrationnumber");
                            decimal amount = ((Money)targetentity.Attributes["preetham_cost"]).Value;

                            Entity targetbill_entity = new Entity("preetham_bill");

                            QueryExpression query = new QueryExpression()
                            {
                                EntityName = "preetham_bill",
                                ColumnSet = new ColumnSet("preetham_register_number", "preetham_procedurefee", "preetham_totalamount")
                            };

                            EntityCollection bill_data = service.RetrieveMultiple(query);

                            if (bill_data.Entities.Count == 0)
                            {
                                return;
                            }
                            foreach (Entity data in bill_data.Entities)
                            {
                                if (data.Attributes.Contains("preetham_register_number"))
                                {
                                    var bill_registrationnumber = data.GetAttributeValue<string>("preetham_register_number");
                                    if (bill_registrationnumber.ToString().ToLower() == registration_number.ToString().ToLower())
                                    {
                                        var procedure_bill = data.GetAttributeValue<Money>("preetham_procedurefee");

                                        if (procedure_bill != null)
                                        {
                                            decimal _procedure_bill = ((Money)data.Attributes["preetham_procedurefee"]).Value;
                                            decimal total_procedure_bill = _procedure_bill + amount;
                                            targetbill_entity.Attributes["preetham_procedurefee"] = new Money(total_procedure_bill);
                                            decimal toatal_amount = ((Money)data.Attributes["preetham_totalamount"]).Value;
                                            decimal temp_total_amount = toatal_amount + amount;
                                            targetbill_entity.Attributes["preetham_totalamount"] = new Money(temp_total_amount);
                                            targetbill_entity.Id = data.Id;
                                            service.Update(targetbill_entity);
                                            return;
                                        }
                                        else
                                        {
                                            Money temp_amount = new Money(amount);
                                            targetbill_entity.Attributes["preetham_procedurefee"] = temp_amount;
                                            decimal toatal_amount = ((Money)data.Attributes["preetham_totalamount"]).Value;
                                            decimal temp_total_plus_procedure = toatal_amount + amount;
                                            Money temp_total_amount = new Money(temp_total_plus_procedure);
                                            targetbill_entity.Attributes["preetham_totalamount"] = temp_total_amount;
                                            targetbill_entity.Id = data.Id;
                                            service.Update(targetbill_entity);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                tracingService.Trace("Exception", ex);
            }
        }
    }
}
