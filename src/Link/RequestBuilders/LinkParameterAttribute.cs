using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tavis.RequestBuilders
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LinkParameterAttribute : Attribute
    {
         private readonly string _name;

        /// <summary>
        /// Create a new attribute.  For IANA registered link relations name will be simple string, otherwise it should be a URI 
        /// </summary>
        /// <param name="name"></param>
         public LinkParameterAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
        public object Default
        {
            get;
            set;
        }
    }
}
