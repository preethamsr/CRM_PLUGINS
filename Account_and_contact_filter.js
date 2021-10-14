var CSMT = CSMT || {}
//Account filter on richeste approvigionamento
CSMT.Account_filter = function (formExecutionContext) {
    var formcontext = formExecutionContext.getFormContext();
    formcontext.getControl("bd_account").addPreSearch(function () {

        addLookupFilter(formcontext);
    });
}
function addLookupFilter(formcontext) {
    var fetchxml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
        "  <entity name='account'>" +
        "    <filter type='and'>" +
        "      <condition attribute='customertypecode' operator='in'>" +
        "        <value>13</value>" +
        "        <value>11</value>" +
        "      </condition>" +
        "    </filter>" +
        "  </entity>" +
        "</fetch>"
    formcontext.getControl("bd_account").addCustomFilter(fetchxml);
}

//contact filter on richeste approvigionamento
CSMT.Contact_filter = function (formExecutionContext) {
    var formcontext = formExecutionContext.getFormContext();
    //all fields readonly if motivo stato is bozza 
    if (formcontext.getAttribute("bd_account") != null) {
        formcontext.getControl("bd_contact").addPreSearch(function () {
            addcustomLookupFilter(formcontext);
        })
    }
}
function addcustomLookupFilter(formcontext) {
    if (formcontext.getAttribute("bd_account").getValue() != null) {
        var id = formcontext.getAttribute("bd_account").getValue()[0].id;
        var fetchxml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
            "  <entity name='contact'>" +
            "    <filter type='and'>" +
            "      <condition attribute='parentcustomerid' operator='eq' uitype='account' value='" + id + "' />" +
            "    </filter>" +
            "  </entity>" +
            "</fetch>"
        formcontext.getControl("bd_contact").addCustomFilter(fetchxml);
    }
}

//all fileds readonly if motivo stato not bozza  on richeste approvigionamento
CSMT.Allreadonly_Rechiesta = function (formExecutionContext) {

    var formcontext = formExecutionContext.getFormContext();
    if (formcontext.getAttribute("statuscode").getValue() != 899240000) {
        var attributes = formcontext.data.entity.attributes.get();
        for (var i in attributes) {
            var attribute_name = attributes[i]["_attributeName"];
            if (attribute_name != "statuscode") {
                if (attribute_name != "statecode") {
                    formcontext.getControl(attribute_name).setDisabled(true);
                }
            }

        }
    }
}


//all fileds readonly if motivo stato not bozza  on Dettagli richeste approvigionamento

CSMT.ALLreadonly_dettagli_rechiesta = function (formExecutionContext) {
    var formcontext = formExecutionContext.getFormContext();
    if (formcontext.getAttribute("statuscode").getValue() != 899240005) {
        var attributes = formcontext.data.entity.attributes.get();
        for (var i in attributes) {
            var attribute_name = attributes[i]["_attributeName"];
            if (attribute_name != "statecode") {
                if (attribute_name == "statuscode") {
                    formcontext.getControl("header_statuscode").setDisabled(true)
                } else {
                    formcontext.getControl(attribute_name).setDisabled(true);
                }
                if (formcontext.getAttribute("statuscode").getValue() == 899240007) {
                    formcontext.getControl("bd_ordermanager").setDisabled(false);

                }
            }
        }
    }
}






