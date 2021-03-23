using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Hospital_management_1._0.Plugins
{
   public class Out_patient_bill : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));


                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                tracingService.Trace("stage1");
                string register_number = ((string)context.InputParameters["registration_number"]);
                decimal total_procedure_bill = 0;
                decimal _Consultaion_fee = 0;
                Decimal _total_fee = 0;

                QueryExpression query = new QueryExpression()
                {
                    EntityName = "preetham_case",
                    ColumnSet = new ColumnSet("preetham_name", "preetham_patientnameid", "preetham_departmentid", "preetham_doctorsid", "preetham_registrationumber")
                };
                query.Criteria.AddCondition("preetham_registrationumber", ConditionOperator.Equal, register_number);
                EntityCollection newcase = service.RetrieveMultiple(query);
                if (newcase.Entities.Count == 0)
                {
                    return;
                }
                foreach (Entity caselist in newcase.Entities)
                {
                    if (caselist.Attributes.Contains("preetham_registrationumber"))
                    {
                        tracingService.Trace("stage2");
                    string _register_number = caselist.GetAttributeValue<string>("preetham_registrationumber");
                    if (register_number.ToString().ToLower() == _register_number.ToString().ToLower())
                    {
                        Guid _case_id = caselist.Id;
                        EntityReference accountid = ((EntityReference)caselist.Attributes["preetham_patientnameid"]);
                        Guid patient_id = accountid.Id;
                        EntityReference department_id = ((EntityReference)caselist.Attributes["preetham_departmentid"]);
                        Guid _department_id = department_id.Id;
                        EntityReference doctors_id = ((EntityReference)caselist.Attributes["preetham_doctorsid"]);
                        Guid _doctors_id = doctors_id.Id;

                        ColumnSet col = new ColumnSet("preetham_consultationfee");
                        Entity consultation_fee = service.Retrieve("preetham_doctors", _doctors_id,col);
                            _Consultaion_fee = ((Money)consultation_fee.Attributes["preetham_consultationfee"]).Value;

                        Entity updatenumber = new Entity("preetham_patient_auto_number_registration");
                            

                        QueryExpression auto_number = new QueryExpression()
                        {
                            EntityName = "preetham_patient_auto_number_registration",
                            ColumnSet = new ColumnSet("preetham_name", "preetham_prefix", "preetham_separator", "preetham_sufix", "preetham_currentnumber")
                        };
                        EntityCollection applicationdata = service.RetrieveMultiple(auto_number);
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

                                QueryExpression procedure_list = new QueryExpression()
                                {
                                    EntityName = "preetham_procedure_bill",
                                    ColumnSet = new ColumnSet("preetham_registrationnumber", "preetham_cost")
                                };
                                 procedure_list.Criteria.AddCondition("preetham_registrationnumber",ConditionOperator.Equal,register_number);
                                EntityCollection _procedurelist = service.RetrieveMultiple(procedure_list);

                                Entity bill = new Entity("preetham_bill");

                                 if (_procedurelist.Entities.Count == 0)
                                {
                                        
                                        bill.Attributes["preetham_name"] = prefix + separator + day + month + year + separator + suffix + current;
                                        bill.Attributes["preetham_patientnameid"] = new EntityReference("account", patient_id);
                                        bill.Attributes["preetham_register_number"] = register_number;
                                        bill.Attributes["preetham_outpatientid"] = new EntityReference("preetham_case", _case_id);
                                        bill.Attributes["preetham_departmentid"] = new EntityReference("preetham_departments", _department_id);                                       
                                        bill.Attributes["preetham_procedurefee"] = new Money(0);
                                        bill.Attributes["preetham_patienttype"] = false;
                                        bill.Attributes["preetham_consultationfee"] = new Money(_Consultaion_fee);
                                        bill.Attributes["preetham_doctorid"] = new EntityReference("preetham_doctors", _doctors_id);
                                        _total_fee = 0 + _Consultaion_fee;
                                        bill.Attributes["preetham_totalamount"] = new Money(_total_fee);
                                        service.Create(bill);
                                        return;
                                    }
                                
                                foreach (Entity pl in _procedurelist.Entities)
                                {
                                        if(pl.Attributes.Contains("preetham_registrationnumber"))
                                        { 
                                    string _procedure_list_registration_number = pl.GetAttributeValue<string>("preetham_registrationnumber");
                                    if (_procedure_list_registration_number.ToString().ToLower() == register_number.ToString().ToLower())
                                    {
                                        decimal _procedure_bill = ((Money)pl.Attributes["preetham_cost"]).Value;
                                        bill.Attributes["preetham_name"] = prefix + separator + day + month + year + separator + suffix + current;
                                        bill.Attributes["preetham_patientnameid"] = new EntityReference("account", patient_id);
                                        bill.Attributes["preetham_register_number"] = register_number;
                                        bill.Attributes["preetham_outpatientid"] = new EntityReference("preetham_case", _case_id);
                                        bill.Attributes["preetham_departmentid"] = new EntityReference("preetham_departments", _department_id);
                                        total_procedure_bill += _procedure_bill;
                                        bill.Attributes["preetham_procedurefee"] = new Money(total_procedure_bill);
                                        bill.Attributes["preetham_patienttype"] =false;
                                        bill.Attributes["preetham_consultationfee"] = new Money(_Consultaion_fee);
                                        bill.Attributes["preetham_doctorid"] = new EntityReference("preetham_doctors", _doctors_id);
                                    }
                                }
                                    }
                                    tracingService.Trace("stage 3");                                 
                                        _total_fee = total_procedure_bill + _Consultaion_fee;
                                        bill.Attributes["preetham_totalamount"] = new Money(_total_fee);
                                        service.Create(bill);
                                        return;                                 
                            }
                        }
                    }
                }
            }
                
            }
            catch(Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
