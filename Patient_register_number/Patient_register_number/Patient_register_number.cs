using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Bill_number_generater
{
   public class Patient_register_number : IPlugin
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
                        if (targetentity.LogicalName == "account")
                        {
                            Entity patientregisternumber = new Entity("preetham_patient_auto_number_registration");
                            QueryExpression query = new QueryExpression()
                            {
                                EntityName = "preetham_patient_auto_number_registration",
                                ColumnSet = new ColumnSet("preetham_name", "preetham_prefix", "preetham_separator", "preetham_sufix", "preetham_currentnumber")
                            };
                            EntityCollection patient_register_number_data = service.RetrieveMultiple(query);
                            if (patient_register_number_data.Entities.Count == 0)
                            {
                                return;
                            }
                            foreach (Entity data in patient_register_number_data.Entities)
                            {
                                string name = data.GetAttributeValue<string>("preetham_name");
                                if (name == "Patient_Auto_Register_Number")
                                {
                                    string prifix = data.GetAttributeValue<string>("preetham_prefix");
                                    string suffix = data.GetAttributeValue<string>("preetham_sufix");
                                    string separator = data.GetAttributeValue<string>("preetham_separator");
                                    string current_number = data.GetAttributeValue<string>("preetham_currentnumber");
                                    int temp = int.Parse(current_number);
                                    temp++;
                                    current_number = temp.ToString("0000");
                                    patientregisternumber.Id = data.Id;
                                    patientregisternumber.Attributes["preetham_currentnumber"] = current_number;
                                    service.Update(patientregisternumber);
                                    DateTime dateobj = DateTime.Now;
                                    string day = dateobj.Day.ToString("00");
                                    string month = dateobj.Month.ToString("00");
                                    string year = dateobj.Year.ToString();
                                    targetentity.Attributes["preetham_registrationnumber"] = prifix + separator + day + month + year + separator + suffix + separator + current_number;
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ITracingService tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                tracing.Trace("exception", ex);
            }



        }
    }


  
