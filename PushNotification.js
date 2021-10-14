var CSMT = CSMT || {}
CSMT.pushnotification = function (formExecutionContext) {
    var formcontext = formExecutionContext.getFormContext();
    var i = Xrm.Utility.getGlobalContext();
    var UserId = i._userSettings.userId;
    UserId=UserId.replace("{", "").replace("}", "");
    var orgURL = formcontext.context.getClientUrl();
    var recordId =formcontext.data.entity.getId().replace("{", "").replace("}", "");
    // var entityName =formcontext.data.entity.getEntityName();

    // //Get the object type code dynamically following the above url
    // var objectTypeCode = Mscrm.EntityPropUtil.EntityTypeName2CodeMap["bd_procurementrequestdetails"]
    // var recordURL = "https://" + orgURL + "/main.aspx?etc=" + objectTypeCode +
    //     "&id=%7b"+recordId +"%7d&pagetype=entityrecord";

    var notification =
    {
        "title": "Approvato ",
        "body": "Dettagli richiesta approvvigionamento sono stati approvati",
        "ownerid@odata.bind": "/systemusers(" + UserId + ")",
        "icontype": 100000001,
        "data": JSON.stringify({
            "actions": [
                {
                    "title": "Visualizza il record",
                    "data": {
                        "url":"?pagetype=entityrecord&etn=bd_procurementrequestdetails&id="+recordId+"",
                        "navigationTarget": "inline"
                    }
                }
            ]
        })
    }
    Xrm.WebApi.createRecord("appnotification", notification).
        then(
            function success(result) {
                console.log("notification created with single action : " + result.id);
            },
            function (error) {
                console.log(error.message);
            }
        );

}