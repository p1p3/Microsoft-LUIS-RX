using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luis.Reactive.Exceptions
{
   public class NotIntentFoundException :Exception
    {
        public NotIntentFoundException(string inputText):base($"Not intent found for {inputText}") { }
    }
}
