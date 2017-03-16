using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luis.Reactive
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        public EntityAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Name of the Intent defined in the LUIS server.
        /// Will use the method name if not set.
        /// </summary>
        public string Name { get; private set; }

    }
}
