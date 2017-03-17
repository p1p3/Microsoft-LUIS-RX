using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Luis.Reactive.Exceptions;
using Luis.Reactive.Structures;

namespace Luis.Reactive
{
    public class HandlersContainer
    {
        public delegate string IntentHandlerFunc(LuisResult result);

        public class Handler
        {
            public Handler( double threshold, IntentHandlerFunc exec)
            {
                Threshold = threshold;
                Exec = exec;
            }

            public double Threshold { get; }
            public IntentHandlerFunc Exec { get; }
        };

        private static IDictionary<string, IList<Handler>> handlersContainer= new Dictionary<string, IList<Handler>>();


        public static void Config()
        {
            var intents = GetTypesWith<IntentAttribute>();
            RegisterIntentHandlers(intents);
        }


        private static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit = false)
             where TAttribute : System.Attribute
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var intents = from assembly in assemblies
                          from types in assembly.GetTypes()
                          where types.IsDefined(typeof(TAttribute), inherit)
                          select types;

            return intents;
        }

        public static void RegisterHandler(Type intentType, IntentHandlerFunc intentHandler, double threshold)
        {
            var intentAttribute = intentType.GetCustomAttribute<IntentAttribute>();
            var intentName = intentAttribute.Name;
            IList<Handler> handlersList;

            if (!handlersContainer.TryGetValue(intentName, out handlersList))
            {
                handlersContainer[intentName] = new List<Handler>() {new Handler(threshold,intentHandler) };
            }
            else
            {
                handlersList.Add(new Handler(threshold, intentHandler));
            }
        }

        public static void RegisterIntentHandlers(IEnumerable<Type> intentClasses)
        {
            foreach (var intentClass in intentClasses)
            {
                foreach (var method in intentClass.GetRuntimeMethods())
                {
                    var intenthandler = method.GetCustomAttribute<IntentHandlerAttribute>();
                    if (intenthandler == null) continue;

                    var handlerFunc = (IntentHandlerFunc)method.CreateDelegate(typeof(IntentHandlerFunc));
                    RegisterHandler(intentClass, handlerFunc, intenthandler.ConfidenceThreshold);
                }
            }

        }

        public static Handler ResolveHandler(LuisResult result)
        {
            var intentName = result.TopScoringIntent.Name;

            if(intentName.ToLower().Equals("none")) throw new NotIntentFoundException(result.OriginalQuery);

            IList<Handler> possibleIntentHandlers;
            handlersContainer.TryGetValue(intentName, out possibleIntentHandlers);

            IEnumerable<string> missingEntities = new List<string>();
            var entitiesInResult = result.Entities.Select(entity => entity.Key).ToArray();
            foreach (var possibleIntentHandler in possibleIntentHandlers)
            {
                var attribute = possibleIntentHandler.Exec.GetMethodInfo().GetCustomAttribute<IntentHandlerAttribute>();
                var entitiesRequieredInHandler = attribute.RequieredEntities.Select(entity => entity.GetCustomAttribute<EntityAttribute>().Name).ToArray();


                 missingEntities = from requieredEntity in entitiesRequieredInHandler
                    where !(from entityInResult in entitiesInResult
                        select entityInResult).Contains(requieredEntity)
                    select requieredEntity;

                if (!missingEntities.Any() && result.TopScoringIntent.Score >= possibleIntentHandler.Threshold ) return possibleIntentHandler;

            }

            throw new NotHandlerFoundException($"We could not find any handler for the intent {intentName}", missingEntities.ToArray());
        }

    }
}
