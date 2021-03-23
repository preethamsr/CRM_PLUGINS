using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Bill_Automation
{
    public class Automatic_bill_generation : IPlugin
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
                        IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                        IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
                        Entity targetentity = context.InputParameters["Target"] as Entity;
                        if (targetentity.LogicalName == "preetham_inpatient")
                        {
                            Guid id = Guid.Parse(context.OutputParameters["id"].ToString());
                            EntityReference accountid = ((EntityReference)targetentity.Attributes["preetham_patientnameid"]);
                            Guid patient_id = accountid.Id;
                            EntityReference bed_type = ((EntityReference)targetentity.Attributes["preetham_bedtypeid"]);
                            Guid bed_type_id = bed_type.Id;
                            EntityReference department = ((EntityReference)targetentity.Attributes["preetham_departmentid"]);
                            Guid department_id = department.Id;
                            EntityReference doctor = ((EntityReference)targetentity.Attributes["preetham_doctorid"]);
                            Guid doctor_id = doctor.Id;
                            ColumnSet col = new ColumnSet("preetham_priceperday", "preetham_availablebed");
                            Entity bed_per_price = service.Retrieve("preetham_bedmanagement", bed_type_id, col);
                            Entity bill = new Entity("preetham_bill");
                            Entity Bedmanagement = new Entity("preetham_bedmanagement");


                            Entity updatenumber = new Entity("preetham_patient_auto_number_registration");
                            QueryExpression query = new QueryExpression()
                            {
                                EntityName = "preetham_patient_auto_number_registration",
                                ColumnSet = new ColumnSet("preetham_name", "preetham_prefix", "preetham_separator", "preetham_sufix", "preetham_currentnumber")
                            };
                            EntityCollection applicationdata = service.RetrieveMultiple(query);
                            if (applicationdata.Entities.Count == 0)
                            {
                                return;
                            }
                            foreach (Entity e in applicationdata.Entities)
                            {
                                var name = e.GetAttributeValue<string>("preetham_name");
                                if (name == "BillNumberGenerator")
                                {
                                    string prefix = e.GetAttributeValue<string>("preetham_prefix");
                                    string suffix = e.GetAttributeValue<string>("preetham_sufix");
                                    string separator = e.GetAttributeValue<string>("preetham_separator");
                                    string current = e.GetAttributeValue<string>("preetham_currentnumber");
                                    int temp = int.Parse(current);
                                    temp++;
                                    current = temp.ToString("0000");
                                    updatenumber.Id = e.Id;
                                    updatenumber.Attributes["preetham_currentnumber"] = current;
                                    service.Update(updatenumber);
                                    DateTime date = DateTime.Now;
                                    string day = date.Day.ToString("00");
                                    string month = date.Month.ToString("00");
                                    string year = date.Year.ToString();

                                    Bedmanagement.Attributes["preetham_bedmanagementid"] = bed_type_id;
                                    bill.Attributes["preetham_name"] = prefix + separator + day + month + year + separator + suffix + separator + current;
                                    bill.Attributes["preetham_caseid"] = new EntityReference("preetham_inpatient", id);
                                    bill.Attributes["preetham_patientnameid"] = new EntityReference("account", patient_id);
                                    bill.Attributes["preetham_bedtypeid"] = new EntityReference("preetham_bedmanagement", bed_type_id);
                                    bill.Attributes["preetham_bedno"] = targetentity.GetAttributeValue<string>("preetham_bedno");
                                    bill.Attributes["preetham_departmentid"] = new EntityReference("preetham_departments", department_id);
                                    bill.Attributes["preetham_doctorid"] = new EntityReference("preetham_doctors", doctor_id);
                                    bill.Attributes["preetham_bedcharge"] = bed_per_price.GetAttributeValue<Money>("preetham_priceperday");
                                    bill.Attributes["createdon"] = DateTime.Now;
                                    bill.Attributes["preetham_totalamount"] = bed_per_price.GetAttributeValue<Money>("preetham_priceperday");
                                    bill.Attributes["preetham_register_number"] = targetentity.GetAttributeValue<string>("preetham_registrationnumber");
                                    int available_bed = bed_per_price.GetAttributeValue<int>("preetham_availablebed");
                                    int _availablebed = available_bed - 1;             
                                    Bedmanagement.Attributes["preetham_availablebed"] = _availablebed;
                                    service.Update(Bedmanagement);                                    
                                    service.Create(bill);

                                }
                            }


                        }

                    }
                }
            }
            catch(Exception ex)
            {
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                tracingService.Trace("Exception", ex);
            }
          
        }
    }
}
