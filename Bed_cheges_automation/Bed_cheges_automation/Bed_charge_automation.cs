using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Activities;


namespace Hospital_management_1._0.Plugins
{
    public class Bed_charge_automation : CodeActivity
    {

        [Input("registration_number")]
        [RequiredArgument]
        public InArgument<string> Registration_number { get; set; }

        [Input("bed_type")]
        [RequiredArgument]
        [ReferenceTarget("preetham_bedmanagement")]
        public InArgument<EntityReference> bedtype { get; set; }

        [Input("doctor_fee")]
        [RequiredArgument]
        [ReferenceTarget("preetham_doctors")]
        public InArgument<EntityReference> Doctor { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            ITracingService tracingService = (ITracingService)context.GetExtension<ITracingService>();
            tracingService.Trace("Stage1");
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);

            Guid bedtype_id = this.bedtype.Get(context).Id;
            Guid doctor_id = this.Doctor.Get(context).Id;
            string _registration_number = this.Registration_number.Get<string>(context);

            Entity bed_management_data = service.Retrieve("preetham_bedmanagement", bedtype_id, new ColumnSet("preetham_priceperday"));
            Entity doctor_data = service.Retrieve("preetham_doctors", doctor_id, new ColumnSet("preetham_consultationfee"));

            decimal bedcharges_perday = ((Money)bed_management_data.Attributes["preetham_priceperday"]).Value;
            decimal consultation_fee = ((Money)doctor_data.Attributes["preetham_consultationfee"]).Value;

            decimal bill_bed_charges = 0;
            decimal total_bill = 0;
            decimal total_consultation_fee = 0;

            var fetchxml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                             <entity name='preetham_bill'>
                             <attribute name='preetham_billid' />
                             <attribute name='preetham_name' />
                             <attribute name='createdon' />
                             <attribute name='preetham_totalamount' />
                             <attribute name='preetham_consultationfee' />
                             <attribute name='preetham_register_number' />
                             <attribute name='preetham_procedurefee' />
                             <attribute name='preetham_bedcharge' />
                             <order attribute='preetham_name' descending='false' />
                             <filter type='and'>
                             <condition attribute='preetham_register_number' operator='eq' value='" + _registration_number + @"' />
                             </filter>
                             </entity>
                             </fetch>";

            EntityCollection bil_details = service.RetrieveMultiple(new FetchExpression(fetchxml));

            Entity bills = new Entity("preetham_bill");
            Entity inpatients = new Entity("preetham_inpatient");

            if (bil_details.Entities.Count == 0)
            {
                return;
            }
            foreach (Entity bill in bil_details.Entities)
            {
                if (bill.Attributes.Contains("preetham_register_number"))
                {
                    decimal curent_bed_charge = ((Money)bill.Attributes["preetham_bedcharge"]).Value;
                    decimal current_total_bill = ((Money)bill.Attributes["preetham_totalamount"]).Value;
                    decimal _consultation_fee = ((Money)bill.Attributes["preetham_consultationfee"]).Value;   
                    
                    bill_bed_charges = curent_bed_charge + bedcharges_perday;
                    total_bill = bedcharges_perday+consultation_fee;
                    total_consultation_fee = _consultation_fee + consultation_fee;
                    bills.Attributes["preetham_bedcharge"] = new Money(bill_bed_charges);
                    bills.Attributes["preetham_totalamount"] = new Money(total_bill);
                    bill.Attributes["preetham_consultationfee"] = new Money(total_consultation_fee);

                    QueryExpression inpatient_data = new QueryExpression()
                    {
                        EntityName = "preetham_inpatient",
                        ColumnSet = new ColumnSet("preetham_registrationnumber", "preetham_bed_charge_update")
                    };
                    inpatient_data.Criteria.AddCondition("preetham_registrationnumber", ConditionOperator.Equal, _registration_number);
                    EntityCollection inpatient_list = service.RetrieveMultiple(inpatient_data);
                    foreach (Entity il in inpatient_list.Entities)
                    {
                        inpatients.Attributes["preetham_bed_charge_update"] = new Money(bill_bed_charges);
                        inpatients.Id = il.Id;
                        service.Update(inpatients);
                    }
                    bills.Id = bill.Id;
                    service.Update(bills);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
