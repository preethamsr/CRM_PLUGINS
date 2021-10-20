using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Natuzzi_PushNotification
{
    public class JsonSerlizer
    {
        public  string Jsonserilizer(ActionModel model, Type objtype)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(ActionModel));
            MemoryStream msobj = new MemoryStream();
            js.WriteObject(msobj, model);
            msobj.Position = 0;
            StreamReader streamReader = new StreamReader(msobj);
            string data = streamReader.ReadToEnd();
            return data;
        }
    }
}
