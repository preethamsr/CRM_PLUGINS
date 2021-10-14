var CSMT=CSMT || {};
CSMT.TriggerWF=function(primaryControl){
    var formContext = primaryControl;
var record_id=formContext.data.entity.getId();
var entity = {
    "EntityId": record_id // accountId
 };
  
 var WorkflowId = "798252A3-217D-4D11-8FCD-9ABA20D45C46";
  
 var req = new XMLHttpRequest();
 req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v9.0/workflows(" + WorkflowId + ")/Microsoft.Dynamics.CRM.ExecuteWorkflow", true);
 req.setRequestHeader("OData-MaxVersion", "4.0");
 req.setRequestHeader("OData-Version", "4.0");
 req.setRequestHeader("Accept", "application/json");
 req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
 req.onreadystatechange = function() {
     if (this.readyState === 4) {
         req.onreadystatechange = null;
  
         if (this.status === 200) {
             Xrm.Utility.alertDialog("Success");
         } else {
             Xrm.Utility.alertDialog(this.statusText);
         }
     }
 };
 req.send(JSON.stringify(entity));
 
}