using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luis.Reactive.Exceptions
{
   public class NotIntentFound :Exception
    {
        public NotIntentFound(string inputText):base($"Not intent found for {inputText}") { }
    }
}
