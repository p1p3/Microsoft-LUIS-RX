using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Luis.Reactive.Structures
{
    public interface ICompositeEntity
    {
        string Name { get; set; }
        string Value { get; set; }
    }


    public class CompositeEntity : ICompositeEntity
    {
        /// <summary>
        /// The name of the type of parent entity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The composite entity value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// A list of child entities of the composite entity.
        /// </summary>
        public CompositeEntityChild[] CompositeEntityChildren { get; set; }

        /// <summary>
        /// Loads the json object into the properties of the object.
        /// </summary>
        /// <param name="compositeEntity">Json object containing the composite entity</param>
        public void Load(JObject compositeEntity)
        {
            Name = (string)compositeEntity["parentType"];
            Value = (string)compositeEntity["value"];
            try
            {
                var values = (JArray)compositeEntity["children"] ?? new JArray();
                CompositeEntityChildren = ParseValuesArray(values);
            }
            catch (Exception)
            {
                CompositeEntityChildren = null;
            }

        }

        /// <summary>
        /// Parses Json array of composite entity children into composite entity child array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns>entities array</returns>
        private CompositeEntityChild[] ParseValuesArray(JArray array)
        {
            var count = array.Count;
            var a = new CompositeEntityChild[count];
            for (var i = 0; i < count; i++)
            {
                var t = new CompositeEntityChild();
                t.Load((JObject)array[i]);
                a[i] = t;
            }
            return a;
        }
    }
}
