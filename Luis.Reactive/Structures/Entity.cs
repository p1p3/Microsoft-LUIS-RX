using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Luis.Reactive.Structures
{

    public interface IEntity
    {
        string Name { get; set; }
        string Value { get; set; }
    }

    public class Entity :IEntity
    {
        /// <summary>
        /// The name of the type of Entity, e.g. "Topic", "Person", "Location".
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The entity value, e.g. "Latest scores", "Alex", "Cambridge".
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Confidence score that LUIS matched the entity, the higher the better.
        /// </summary>
        public double Score { get; set; }
        /// <summary>
        /// The index of the first character of the entity within the given text
        /// </summary>
        public int StartIndex { get; set; }
        /// <summary>
        /// The index of the last character of the entity within the given text
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        /// The resolution dictionary containing specific parameters for built-in entities
        /// </summary>
        public Dictionary<string, Object> Resolution;

        /// <summary>
        /// Loads the Entity values from a JSON object returned from the LUIS service.
        /// </summary>
        /// <param name="entity">The JObject containing the entity values</param>
        public void Load(JObject entity)
        {
            Name = (string)entity["type"];
            Value = (string)entity["entity"];
            try
            {
                Score = (double)entity["score"];
            }
            catch (Exception)
            {
                Score = -1;
            }
            try
            {
                StartIndex = (int)entity["startIndex"];
            }
            catch (Exception)
            {
                StartIndex = -1;
            }
            try
            {
                EndIndex = (int)entity["endIndex"];
            }
            catch (Exception)
            {
                EndIndex = -1;
            }
            try
            {
                Resolution = entity["resolution"].ToObject<Dictionary<string, Object>>();
            }
            catch (Exception)
            {
                Resolution = new Dictionary<string, Object>();
            }
        }


    }
}
