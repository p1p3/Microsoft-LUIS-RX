using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luis.Reactive.Structures;
using System.Reflection;

namespace Luis.Reactive
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IntentHandlerAttribute : Attribute
    {
        /// <summary>
        /// Confidence threshold that must be reached for the handler to be activated.
        /// </summary>
        public readonly double ConfidenceThreshold;

        public readonly IEnumerable<Type> RequieredEntities;

        /// <summary>
        /// Attribute to mark methods that can be used by an IntentRouter to handle intents.
        /// </summary>
        /// <param name="confidenceThreshold">Confidence score needed to activate handler.</param>
        /// <param name="requieredEntities"></param>
        public IntentHandlerAttribute(double confidenceThreshold, params Type[] requieredEntities)
        {
            ConfidenceThreshold = confidenceThreshold;
            RequieredEntities = requieredEntities;
        }

    }


}
