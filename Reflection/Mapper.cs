using SqlMapper.Attributes;
using System;
using System.Data;
using System.Reflection;

namespace SqlMapper.Reflection
{
    internal static class Mapper
    {
        internal static TEntity MapRecord<TEntity>(IDataRecord record) where TEntity : class
        {
            if (record is null) throw new ArgumentException();

            var entity = Activator.CreateInstance<TEntity>();

            //Get all the properties of the type which we wish to map to
            var properties = entity.GetType().GetProperties();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<DatabaseFieldAttribute>();
                var propertyName = property.Name;

                if (attribute is not null && !string.IsNullOrWhiteSpace(attribute.Name))
                {
                    propertyName = attribute.Name;
                }

                //Get the corresponding property from the column in the result set
                var columnValue = record.GetValue(record.GetOrdinal(propertyName));

                //If the column value is null, then simply continue to next property
                if (Convert.IsDBNull(columnValue)) continue;

                var propertyType = property.GetType();

                //Dynamically convert the object to a Type at run-time
                columnValue = Convert.ChangeType(columnValue, propertyType);

                //Set the property value for the entity
                property.SetValue(entity, columnValue, null);
            }

            return entity;
        }
    }
}
