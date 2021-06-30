using System;
using System.Data;
using System.Reflection;

namespace SqlBuilder.Reflector
{
    public static class Reflector
    {
        public static TEntity ReflectType<TEntity>(IDataRecord record) where TEntity : class, new()
        {
            if (record is null) throw new ArgumentException();

            TEntity entity = new TEntity();

            //Get all the properties of the type which we wish to map to
            PropertyInfo[] properties = entity.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                //Get the corresponding property from the column in the result set
                object columnValue = record.GetValue(record.GetOrdinal(property.Name));

                //If the column value is null, then simply continue to next property
                if (Convert.IsDBNull(columnValue)) continue;

                Type propertyType = property.GetType();

                //Dynamically convert the object to a Type at run-time
                columnValue = Convert.ChangeType(columnValue, propertyType);

                //Set the property value for the entity
                property.SetValue(entity, columnValue, null);
            }

            return entity;
        }
    }
}
