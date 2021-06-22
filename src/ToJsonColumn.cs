using System;

namespace EfCore.JsonColumn
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ToJsonColumn : Attribute
    {
        public string Name { get; set; }
        public ToJsonColumn()
        {

        }

        public ToJsonColumn(string name)
        {
            Name = name;
        }

    }
}