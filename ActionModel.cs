using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Natuzzi_PushNotification
{
    [DataContract]
    public class ActionModel
    {
        [DataMember]
             public List<actions> actions { get; set; }

    }
    [DataContract]
    public class actions
    {
        [DataMember]
        public string title { get; set; } = "View record";
        [DataMember]
        public data data { get; set; }
    }
    [DataContract]
    public class data
    {
        [DataMember]
        public string url { get; set; }
    } 

 
    
}

