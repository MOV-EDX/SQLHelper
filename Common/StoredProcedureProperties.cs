using System.Collections.Generic;

namespace SqlMapper.Common
{
    public class StoredProcedureProperties
    {
        public string StoredProcedureName { get; set; }
        public string ConnectionString { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public OutputParameterTypes OutputParameter { get; set; }
    }
}
