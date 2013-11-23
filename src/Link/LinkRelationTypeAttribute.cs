using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tavis
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LinkRelationTypeAttribute : Attribute
    {
        private readonly string _name;

        public LinkRelationTypeAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
