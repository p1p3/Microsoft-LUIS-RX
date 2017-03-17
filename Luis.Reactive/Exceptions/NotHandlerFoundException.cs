using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luis.Reactive.Exceptions
{
    public class NotHandlerFoundException:Exception
    {
        public NotHandlerFoundException(string error,params string[] requiredEntities) : base(error) { }
    }
}
