using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luis.Reactive
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CompositeEntityAttribute : EntityAttribute
    {
        public CompositeEntityAttribute(string name) : base(name)
        {

        }

    }
}
