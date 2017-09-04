using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriceEngine.Utils.Data
{
    interface IDatabase
    {
        void Insert(string key, object value);
        object Query(string key);
        T Query<T>(string key);
    }
}
