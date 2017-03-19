using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luis.Reactive.Structures;

namespace Luis.Reactive.Utils.Extensions
{
   public static class CompositeEntityChildExtensions
    {
        public static TEntityType CopyEntityChildToEntityTo<TEntityType>(this CompositeEntityChild compositeEntityChild, ref TEntityType otherInstance)
        {
            // TODO: Dont cast this way
            foreach (var propertyChild in compositeEntityChild.GetType().GetProperties())
            {
                var value = propertyChild.GetValue(compositeEntityChild);
                var instancePropertyInfo = otherInstance.GetType().GetProperty(propertyChild.Name);
                instancePropertyInfo.SetValue(instancePropertyInfo, value, null);
            }

            return otherInstance;
        }
    }
}
