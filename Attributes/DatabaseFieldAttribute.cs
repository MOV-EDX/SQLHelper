using System;

namespace SqlMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DatabaseFieldAttribute : Attribute
    {
        public DatabaseFieldAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
