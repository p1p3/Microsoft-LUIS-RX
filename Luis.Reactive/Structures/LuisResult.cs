using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Luis.Reactive.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace Luis.Reactive.Structures
{
    public class LuisResult
    {

        public string OriginalQuery { get; set; }

        public Intent TopScoringIntent { get; set; }

        public Intent[] Intents { get; set; }

        public Utterance Utterance { get; set; }

        public IDictionary<string, IList<IEntity>> Entities { get; }

        public IDictionary<string, IList<CompositeEntity>> CompositeEntities { get; }

        public IList<IEntity> GetEntity<TEntityType>()
        {
            IList<IEntity> entities;
            var attributes = typeof(TEntityType).GetCustomAttribute<EntityAttribute>();
            Entities.TryGetValue(attributes.Name, out entities);

            return entities;
        }

        public IList<TEntityType> GetCompositeEntity<TEntityType>()
        {
            var compositeEntityType = typeof(TEntityType);
            IList<CompositeEntity> storedCompositeEntities;

            var attributes = compositeEntityType.GetCustomAttribute<CompositeEntityAttribute>();
            CompositeEntities.TryGetValue(attributes.Name, out storedCompositeEntities);

            var properties = compositeEntityType.PropertiesOfType<IEntity>();

            IList<TEntityType> entitiesToReturn = new List<TEntityType>();
            if (storedCompositeEntities == null) return entitiesToReturn;

            foreach (var entity in storedCompositeEntities)
            {
                var compositeEntityIstance = Activator.CreateInstance<TEntityType>();
                foreach (var property in properties)
                {
                    var propertyInfo = compositeEntityIstance.GetType().GetProperty(property.Name);
                    var propertyType = propertyInfo.PropertyType;
                    var attribute = propertyType.GetCustomAttribute<EntityAttribute>();

                    if (attribute == null) continue;

                    //TODO: Multiple Child of the same type
                    var childEntity =
                        entity.CompositeEntityChildren.FirstOrDefault(
                            child => attribute.Name.Equals(child.Name));

                    var childInstance = Activator.CreateInstance(propertyType);
                    childEntity.CopyEntityChildToEntityTo(ref childInstance);
                    
                    property.SetValue(compositeEntityIstance, childInstance);
                }
                entitiesToReturn.Add(compositeEntityIstance);
            }

            return entitiesToReturn;
        }

        public LuisResult(JToken result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            OriginalQuery = (string)result["query"] ?? string.Empty;
            var intents = (JArray)result["intents"] ?? new JArray();
            Intents = ParseIntentArray(intents);
            if (Intents.Length == 0)
            {
                var t = new Intent();
                t.Load((JObject)result["topScoringIntent"]);
                TopScoringIntent = t;
                Intents = new Intent[1];
                Intents[0] = TopScoringIntent;
            }
            else
            {
                TopScoringIntent = Intents[0];
            }

            var entities = (JArray)result["entities"] ?? new JArray();
            Entities = ParseEntityArrayToDictionary(entities);
            var compositeEntities = (JArray)result["compositeEntities"] ?? new JArray();
            CompositeEntities = ParseCompositeEntityArrayToDictionary(compositeEntities);
        }

        /// <summary>
        /// Parses a json array of intents into an intent array
        /// </summary>
        /// <param name="array">Json array containing intents</param>
        /// <returns>Intent array</returns>
        private Intent[] ParseIntentArray(JArray array)
        {
            var count = array.Count;
            var a = new Intent[count];
            for (var i = 0; i < count; i++)
            {
                var t = new Intent();
                t.Load((JObject)array[i]);
                a[i] = t;
            }

            return a;
        }

        /// <summary>
        /// Parses a json array of entities into an entity dictionary.
        /// </summary>
        /// <param name="array"></param>
        /// <returns>The object containing the dictionary of entities</returns>
        private IDictionary<string, IList<IEntity>> ParseEntityArrayToDictionary(JArray array)
        {
            var dict = new Dictionary<string, IList<IEntity>>();

            foreach (var item in array)
            {
                var e = new Entity();
                e.Load((JObject)item);

                IList<IEntity> entityList;
                if (!dict.TryGetValue(e.Name, out entityList))
                {
                    dict[e.Name] = new List<IEntity>() { e };
                }
                else
                {
                    entityList.Add(e);
                }
            }

            return dict;
        }

        /// <summary>
        /// Parses a json array of composite entities into a composite entity dictionary.
        /// </summary>
        /// <param name="array"></param>
        /// <returns>The object containing the dictionary of composite entities</returns>
        private IDictionary<string, IList<CompositeEntity>> ParseCompositeEntityArrayToDictionary(JArray array)
        {
            var count = array.Count;
            var dict = new Dictionary<string, IList<CompositeEntity>>();

            foreach (var item in array)
            {
                var e = new CompositeEntity();
                e.Load((JObject)item);

                IList<CompositeEntity> compositeEntityList;
                if (!dict.TryGetValue(e.Name, out compositeEntityList))
                {
                    dict[e.Name] = new List<CompositeEntity>() { e };
                }
                else
                {
                    compositeEntityList.Add(e);
                }
            }

            return dict;
        }

    }
}
