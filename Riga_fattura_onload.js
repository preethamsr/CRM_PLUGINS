var CSMT=CSMT || {}
CSMT.onloadpricefield=function(formExecutionContext)
{
    var formcontext=formExecutionContext.getFormContext();
    var fattura_details_id=formcontext.getAttribute("invoiceid").getValue()[0].id;
    var fetchxml="<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>"+
    "  <entity name='invoicedetail'>"+
    "    <filter type='and'>"+
    "      <condition attribute='invoiceid' operator='eq' uiname='Progetto completo - fixed' uitype='invoice' value='"+fattura_details_id+"' />"+
    "    </filter>"+
    "    <link-entity name='invoice' from='invoiceid' to='invoiceid' link-type='inner' alias='aa'>"+
    "      <attribute name='msdyn_projectinvoicestatus' />"+
    "    </link-entity>"+
    "  </entity>"+
    "</fetch>"
    fetchxml="?fetchXml="+encodeURIComponent(fetchxml);
    Xrm.WebApi.retrieveMultipleRecords("invoicedetail",fetchxml).then(
        function success(response)
        {
            for(var i=0; i<=response.entities.length; i++)
            {
                if(i==0){
                var response_obj=response.entities[i];
                var response_id=response_obj["aa.msdyn_projectinvoicestatus"];
                if(response_id==192350002)
                {
                  
                    formcontext.getControl("msdyn_contractlineamount").setDisabled(true);
                    formcontext.getControl("msdyn_invoicedtilldate").setDisabled(true);
                    formcontext.getControl("tax").setDisabled(true);
                    formcontext.getControl("msdyn_balanceretaineramount").setDisabled(true);
                    formcontext.getControl("bd_revenuecenteer").setDisabled(true);
                    formcontext.getControl("bd_costcenter").setDisabled(true);
            
                } 
                else if(response_id !==192350002)
                {
        
                    setInterval(function(){
                    formcontext.getControl("msdyn_contractlineamount").setDisabled(false);
                    formcontext.getControl("msdyn_invoicedtilldate").setDisabled(false);
                    formcontext.getControl("tax").setDisabled(false);
                    formcontext.getControl("msdyn_balanceretaineramount").setDisabled(false);
                    formcontext.getControl("bd_revenuecenteer").setDisabled(false);
                    formcontext.getControl("bd_costcenter").setDisabled(false);
                 },500)
                 
             
                }
            }  
        }
        }),
        function(error)
        {
           Xrm.Utility.alertDialog("failed");
        }
}
