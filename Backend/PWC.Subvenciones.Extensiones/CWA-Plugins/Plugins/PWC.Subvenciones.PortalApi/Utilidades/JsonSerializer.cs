using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PWC.Subvenciones.PortalApi.Utilidades
{
    public class JsonSerializer
    {
        public static string Serialize<T>(T aObject) where T : new()
        {
            T serializedObj = new T();
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            ser.WriteObject(ms, aObject);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public static T Deserialize<T>(string aJSON) where T : new()
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            T deserializedObj = ser.Deserialize<T>(aJSON);
            return deserializedObj;
        }
    }
}
