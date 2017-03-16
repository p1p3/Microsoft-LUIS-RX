using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luis.Reactive
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IntentAttribute:Attribute
    {

        public string Name { get; set; }

        
        public IntentAttribute(string name)
        {
            Name = name;
        }
    }
}

