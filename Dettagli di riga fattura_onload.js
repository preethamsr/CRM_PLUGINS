var CSMT=CSMT || {}
CSMT.Readonlyprices=function(formExecutionContext)
{

    var formcontext=formExecutionContext.getFormContext();
    formcontext.getControl("msdyn_price").setVisible(false);
    var fattura_details_id=formcontext.getAttribute("msdyn_invoicelineid").getValue()[0].id;
    var fetchxml="<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>"+
    "  <entity name='invoicedetail'>"+
    "    <attribute name='productid' />"+
    "    <attribute name='invoicedetailid' />"+
    " <filter type='and'>"+
    "    <condition attribute='invoicedetailid' operator='eq' uitype='invoicedetail' value='"+fattura_details_id+"' />"+
    "      </filter>"+
    "    <link-entity name='invoice' from='invoiceid' to='invoiceid' link-type='inner' alias='ab'>"+
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
                console.log(response_obj)
                var response_id=response_obj["ab.msdyn_projectinvoicestatus"];
                if(response_id==192350002)
                {
                  
                    formcontext.getControl("msdyn_amount").setDisabled(true);
                    // formcontext.getControl("msdyn_price").setDisabled(true);
                    formcontext.getControl("msdyn_tax").setDisabled(true);
                    formcontext.getControl("msdyn_extendedamount").setDisabled(true);
            
                } else if(response_id !==192350002)
                {
        
                    setInterval(function(){
                    formcontext.getControl("msdyn_amount").setDisabled(false);
                    // formcontext.getControl("msdyn_price").setDisabled(false);
                    formcontext.getControl("msdyn_tax").setDisabled(false);
                    formcontext.getControl("msdyn_extendedamount").setDisabled(false);
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
