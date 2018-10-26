using Newtonsoft.Json;
using System.Linq;

namespace ROYN
{
    public class RoynResult
    {
        public string Raw { get; private set; }

        public T GetResult<T>()
        {
            return JsonConvert.DeserializeObject<T>(Raw);
        }

        internal void SetResult<T>(T data, string[] columns)
        {
            var jsonResolver = new PropertyIncludeSerializerContractResolver();
            var type = typeof(T).GetGenericArguments().Single();
            var properties = PropertyPathHelper.ResolveIncludeProperties(type, columns);

            foreach (var p in properties)
            {
                jsonResolver.IncludeProperty(p.Key, p.Value.ToArray());
            }

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = jsonResolver,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.Indented
            };
            
            Raw = JsonConvert.SerializeObject(data, serializerSettings);
        }
    }
}