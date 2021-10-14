var CSMT = CSMT || {};
CSMT.Rigadioffertastatic = function (formExecutionContext) {
    var formcontext = formExecutionContext.getFormContext();
    var riga_di_offerta_id = formcontext.getAttribute("msdyn_quotelineid").getValue();
    if (riga_di_offerta_id!= null) {
    var lookupval = new Array();
    lookupval[0] = new Object();
    lookupval[0].id = riga_di_offerta_id[0].id;
    lookupval[0].name = riga_di_offerta_id[0].name;
    lookupval[0].entityType = riga_di_offerta_id[0].entityType;
    setTimeout(function () {
        formcontext.getAttribute("msdyn_quotelineid").setValue(lookupval);
        formcontext.getControl("msdyn_quotelineid").setDisabled(true);
    }, 2000)
    }
    
}
