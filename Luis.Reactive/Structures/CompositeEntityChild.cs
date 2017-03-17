using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Luis.Reactive.Structures
{
    public class CompositeEntityChild
    {
        /// <summary>
        /// The name of the type of parent entity.
        /// </summary>
        public string ParentType { get; set; }
        /// <summary>
        /// The composite entity value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Loads the json object into the properties of the object.
        /// </summary>
        /// <param name="compositeEntityChild">Json object containing the composite entity child</param>
        public void Load(JObject compositeEntityChild)
        {
            ParentType = (string)compositeEntityChild["type"];
            Value = (string)compositeEntityChild["value"];
        }
    }
}
