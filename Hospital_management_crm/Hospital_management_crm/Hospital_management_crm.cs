using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Hospital_management_crm
{
    public class Hospital_management_crm : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context =
(IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService =
(ITracingService)serviceProvider.GetService(typeof(ITracingService));
            if (context.MessageName == "Create")
            {
                tracingService.Trace("successs");
                if (context.InputParameters.Contains("Target") &&
context.InputParameters["Target"] is Entity)
                {
                    IOrganizationServiceFactory serviceFactory =
(IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service =
(IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);

                    Entity target = context.InputParameters["Target"] as Entity;
                    if (target.LogicalName == "account")
                    {
                        tracingService.Trace("Problem found");
                        Entity followuptask = new Entity("task");
                        followuptask["subject"] = "Follow up the patient";
                        followuptask["description"] = "Call the patient and ask about the health";
                        followuptask["scheduledstart"] = DateTime.Now;
                        followuptask["scheduledend"] = DateTime.Now.AddDays(7);
                        followuptask["category"] = context.PrimaryEntityName;
                        if (context.OutputParameters.Contains("id"))
                        {
                            Guid regardingobjectid = new
Guid(context.OutputParameters["id"].ToString());
                            string regardingobjecttype = "account";
                            followuptask["regardingobjectid"] = new
EntityReference(regardingobjecttype, regardingobjectid);
                            service.Create(followuptask);

                        }
                    }
                }
            }
        }
    }
}
