using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Luis.Reactive.Structures
{
    public class Intent
    {
        /// <summary>
        /// Name of the intent.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Confidence score of the intent match.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Load the intent values from JSON returned from the LUIS service.
        /// </summary>
        /// <param name="intent">JSON containing the intent values.</param>
        public void Load(JObject intent)
        {
            Name = (string)intent["intent"];
            Score = (double)intent["score"];
            var actions = (JArray)intent["actions"] ?? new JArray();
        }

    }
}
