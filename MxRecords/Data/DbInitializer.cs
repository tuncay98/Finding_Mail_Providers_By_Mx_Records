using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MxRecords.Data
{
    public class DbInitializer
    {
        public static void Initialize(SystemContext context)
        {

            if (context.Logs.Any())
            {
                return;   // DB has been seeded
            }
        }
    }
}
