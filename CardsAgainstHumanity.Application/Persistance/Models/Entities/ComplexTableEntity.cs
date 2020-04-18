using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CardsAgainstHumanity.Application.Persistance.Attributes;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace CardsAgainstHumanity.Application.Persistance.Models.Entities
{
    public abstract class ComplexTableEntity : TableEntity
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> cache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        protected ComplexTableEntity()
        {
            
        }

        protected ComplexTableEntity(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
            
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var x = base.WriteEntity(operationContext);

            var properties = GetComplexProperties();

            foreach (var property in properties)
            {
                x[property.Name] = new EntityProperty(JsonConvert.SerializeObject(property.GetValue(this)));
            }
            
            return x;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            var derivedProperties = GetComplexProperties();

            foreach (var property in derivedProperties)
            {
                if (properties.ContainsKey(property.Name) && property.CanWrite)
                {
                    property.SetValue(this, JsonConvert.DeserializeObject(properties[property.Name].StringValue, property.PropertyType));
                }
            }
        }

        private PropertyInfo[] GetComplexProperties()
        {
            var type = this.GetType();

            cache.TryGetValue(type, out PropertyInfo[] properties);

            if (properties == null)
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(i => i.GetCustomAttribute<ComplexPropertyAttribute>() != null)
                    .ToArray();

                cache.TryAdd(type, properties);
            }

            return properties;
        }
    }
}
