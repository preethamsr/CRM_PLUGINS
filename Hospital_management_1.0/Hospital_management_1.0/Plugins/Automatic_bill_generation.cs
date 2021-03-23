using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Hospital_management_1._0.Plugins
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
                        decimal outpatient_procdeurebill = 0;
                        decimal bed_charge = 0;
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
                            bed_charge = ((Money)bed_per_price["preetham_priceperday"]).Value;

                            ColumnSet col1 = new ColumnSet("preetham_consultationfee");
                            Entity consultation_fee = service.Retrieve("preetham_doctors", doctor_id, col1);
                            decimal doctors_fee = ((Money)consultation_fee.Attributes["preetham_consultationfee"]).Value;

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

                                    var registration_number= targetentity.GetAttributeValue<string>("preetham_registrationnumber");
                                    string fetchxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                        <entity name='preetham_procedure_bill'>
                                                        <attribute name='preetham_procedure_billid' />
                                                        <attribute name='preetham_name' />
                                                        <attribute name='createdon' />
                                                        <attribute name='preetham_registrationnumber' />
                                                        <attribute name='preetham_cost' />
                                                        <order attribute='preetham_name' descending='false' />
                                                        <filter type='and'>
                               <condition attribute='preetham_registrationnumber' operator='eq' value='"+registration_number+@"' />
                               </filter>
                               </entity>
                               </fetch>";

                                   EntityCollection outpatient_procedure_list= service.RetrieveMultiple(new FetchExpression(fetchxml));
                                    if(outpatient_procedure_list.Entities.Count!=0)
                                    {
                                        foreach (Entity opl in outpatient_procedure_list.Entities)
                                        {
                                            if (opl.Attributes.Contains("preetham_cost"))
                                            {
                                                decimal outpatient_pl_money = ((Money)opl.Attributes["preetham_cost"]).Value;
                                                outpatient_procdeurebill += outpatient_pl_money;
                                            }
                                        }
                                    }
                                   

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
                                    bill.Attributes["preetham_patienttype"] = true;
                                    bill.Attributes["preetham_register_number"] = targetentity.GetAttributeValue<string>("preetham_registrationnumber");
                                    bill.Attributes["preetham_consultationfee"] = new Money(doctors_fee);
                                    if(outpatient_procdeurebill!=0)
                                    {
                                        bill.Attributes["preetham_procedurefee"] = new Money(outpatient_procdeurebill);
                                    }
                                    else
                                    {
                                        bill.Attributes["preetham_procedurefee"] = new Money(0);
                                    }
                                    decimal total_bill = outpatient_procdeurebill + doctors_fee + bed_charge;
                                    bill.Attributes["preetham_totalamount"] = new Money(total_bill);
                                    int available_bed = bed_per_price.GetAttributeValue<int>("preetham_availablebed");
                                    int _availablebed = available_bed - 1;
                                    Bedmanagement.Attributes["preetham_availablebed"] = _availablebed;
                                    service.Update(Bedmanagement);
                                    Entity Inpatient = new Entity("preetham_inpatient");
                                    Inpatient.Attributes["preetham_bed_charge_update"] = bed_per_price.GetAttributeValue<Money>("preetham_priceperday");
                                    Inpatient.Id = targetentity.Id;
                                    service.Update(Inpatient);
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
