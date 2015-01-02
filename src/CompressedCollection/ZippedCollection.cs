using System;
using System.Collections.Generic;
using System.Linq;

namespace CompressedCollection
{
    public class ZippedCollection<T> where T : class
    {
        public IList<T> data { get; set; }
        public string[] MappedData { get; set; }

        public ZippedCollection()
        {
            MappedData = new string[] { };
        }

        public ZippedCollection<T> CreateateInstance(IList<T> sourceData)
        {
            if (sourceData == null || !sourceData.Any())
            {
                return this;
            }

            var dictionary = new Dictionary<string, long>();

            var type = typeof(T);

            var propertyInfos = type.GetProperties().
                Where(p => p.CanWrite && p.PropertyType == typeof(string))
                .Select(x => x.Name);

            IDictionary<string, DynamicGetterSetter.GenericGetter> getters = propertyInfos
                .Select(pInfo => new { Getter = DynamicGetterSetter.CreateGetMethod(type.GetProperty(pInfo)), PropName = pInfo })
                .ToDictionary(p => p.PropName, p => p.Getter);

            IDictionary<string, DynamicGetterSetter.GenericSetter> setters = propertyInfos
                .Select(pInfo => new { Setter = DynamicGetterSetter.CreateSetMethod(type.GetProperty(pInfo)), PropName = pInfo })
                .ToDictionary(p => p.PropName, p => p.Setter);

            var dictionaryValuecount = -1;
            foreach (var dataItem in sourceData)
            {
                foreach (var item in getters.Keys)
                {
                    var getter = getters[item];
                    var setter = setters[item];

                    if (getter == null || setter == null) continue;

                    var dataValue = getter(dataItem).ToString();

                    if (string.IsNullOrWhiteSpace(dataValue)) continue;

                    long dictionaryIndex = -1;

                    if (dictionary.ContainsKey(dataValue))
                    {
                        dictionaryIndex = dictionary[dataValue];
                    }
                    else
                    {
                        dictionary.Add(dataValue, ++dictionaryValuecount);
                        dictionaryIndex = dictionaryValuecount;
                    }

                    setter(dataItem, dictionaryIndex.ToString());
                }
            }

            MappedData = new string[dictionary.Count];

            foreach (var item in dictionary)
            {
                MappedData[item.Value] = item.Key;
            }

            data = sourceData;

            return this;
        }

        public IList<T> ToDecompressedList()
        {
            var newList = new List<T>();

            if (this.data == null || this.data.Count == 0 || MappedData.Length == 0) return newList;

            var type = typeof(T);

            var propertyInfos = type.GetProperties().Where(p => p.CanWrite).ToList();

            IDictionary<string, DynamicGetterSetter.GenericGetter> getters = propertyInfos
                .Select(pInfo => new { Getter = DynamicGetterSetter.CreateGetMethod(type.GetProperty(pInfo.Name)), PropName = pInfo.Name })
                .ToDictionary(p => p.PropName, p => p.Getter);

            IDictionary<string, DynamicGetterSetter.GenericSetter> setters = propertyInfos
                .Select(pInfo => new { Setter = DynamicGetterSetter.CreateSetMethod(type.GetProperty(pInfo.Name)), PropName = pInfo.Name })
                .ToDictionary(p => p.PropName, p => p.Setter);

            if (getters == null || getters.Count == 0 || setters == null || setters.Count == 0)
            {
                return newList;
            }

            foreach (var item in this.data)
            {
                var newItem = Activator.CreateInstance<T>();

                foreach (var propertyInfo in propertyInfos)
                {
                    Type propType = propertyInfo.PropertyType;

                    var getter = getters[propertyInfo.Name];
                    var setter = setters[propertyInfo.Name];

                    if (getter == null || setter == null) continue;

                    var propValue = getter(item);

                    if (propertyInfo.PropertyType == typeof(string) && !string.IsNullOrWhiteSpace(propValue.ToString()))
                    {
                        var propMappedValue = MappedData[Convert.ToInt64(propValue)];
                        setter(newItem, propMappedValue);
                    }
                    else
                    {
                        setter(newItem, Convert.ChangeType(propValue, propType));
                    }
                }

                newList.Add(newItem);
            }

            return newList;
        }
    }
}