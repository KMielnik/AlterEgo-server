using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Settings
{
    public class EntityFrameworkSettings
    {
        public bool InMemory { get; set; }
        public string ConnectionString { get; set; }
    }
}
