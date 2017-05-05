using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper.Entity
{
    public class RestricEntity
    {
        public string md5 { get; set; }
        public int restrict { get; set; }

        public RestricEntity()
        {
            restrict = 4;
        }
    }
}
