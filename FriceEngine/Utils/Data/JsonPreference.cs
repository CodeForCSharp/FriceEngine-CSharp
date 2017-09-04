using System.Collections.Generic;
using Newtonsoft.Json;

namespace FriceEngine.Utils.Data
{
    public class JsonPreference:IDatabase
    {
        private readonly string _path;
        private readonly Dictionary<string ,object> _dict = new Dictionary<string, object>();
        public JsonPreference(string path)
        {
            _path = path;
        }

        public void Insert(string key, object value)
        {
            _dict.Add(key,value);
        }

        public object Query(string key)
        {
            return _dict[key];
        }

        public T Query<T>(string key)
        {
            return (T)_dict[key];
        }

        public void Store()
        {
            System.IO.File.WriteAllText(_path,JsonConvert.SerializeObject(_dict));
        }
    }
}
