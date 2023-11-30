using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace CLDC.CLWS.CLWCS.Framework
{
    public static class Json
    {
        public static object ToObject(this string json)
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject(json);
        }
        public static string ToJson(this object obj)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        public static string ToJson(this object obj, string datetimeformats)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeformats };
            return JsonConvert.SerializeObject(obj, timeConverter);
        }
        public static T ToObject<T>(this string json)
        {
            return string.IsNullOrWhiteSpace(json) ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }
        public static List<T> ToList<T>(this string json)
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<List<T>>(json);
        }
        public static JObject ToJObject(this string json)
        {
            return string.IsNullOrWhiteSpace(json) ? JObject.Parse("{}") : JObject.Parse(json.Replace("&nbsp;", ""));
        }
    }
}
