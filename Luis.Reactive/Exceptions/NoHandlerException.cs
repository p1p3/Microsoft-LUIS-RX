using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luis.Reactive.Exceptions
{
    public class NoHandlerException:Exception
    {
        public NoHandlerException(string error,params string[] requiredEntities) : base(error) { }
    }
}
