using System;
using System.Linq;

namespace PE.Framework.Helpers
{
    /// <summary>
    /// This kind of junk is only required when the data objects between the front end and the back end (and the database) are not 100% alligned!
    /// Also when your database cannot serialize the objects coming from your backend!
    /// *** THIS IS AN ELLABORATE HACK!!! ***
    /// Note the nested try/catch makes these methods rather non-optimal
    /// </summary>
    public static class ObjectMapper
    {
        public static void MapData(object from, object to, bool matchCase = true)
        {
            var fromProperties = from.GetType().GetProperties();
            var toProperties = to.GetType().GetProperties();
            //  copy data
            foreach (var toProperty in toProperties)
            {
                try
                {
                    //  get the corresponding property
                    var fromProperty = (matchCase) ? fromProperties.FirstOrDefault(p => p.Name.Equals(toProperty.Name)) : fromProperties.FirstOrDefault(p => p.Name.Equals(toProperty.Name, System.StringComparison.CurrentCultureIgnoreCase));
                    if (fromProperty == null) continue;
                    var value = fromProperty.GetValue(from);
                    if (value == null) continue;
                    toProperty.SetValue(to, value);
                }
                catch
                {
                    continue;
                }
            }
        }

        public static TTarget MapData<TSource, TTarget>(TSource source, bool matchCase = true) where TTarget : new()
        {
            try
            {
                var fromProperties = source.GetType().GetProperties();
                //  we need an instance of the target first
                var target = Activator.CreateInstance<TTarget>();
                var toProperties = target.GetType().GetProperties();
                //  copy data
                foreach (var toProperty in toProperties)
                {
                    try
                    {
                        //  get the corresponding property
                        var fromProperty = (matchCase) ? fromProperties.FirstOrDefault(p => p.Name.Equals(toProperty.Name)) : fromProperties.FirstOrDefault(p => p.Name.Equals(toProperty.Name, System.StringComparison.CurrentCultureIgnoreCase));
                        if (fromProperty == null) continue;
                        var value = fromProperty.GetValue(source);
                        if (value == null) continue;
                        toProperty.SetValue(target, value);
                    }
                    catch
                    {
                        continue;
                    }
                }
                return target;
            }
            catch
            {
                return default(TTarget);
            }
        }
    }
}
