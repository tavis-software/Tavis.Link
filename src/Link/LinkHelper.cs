using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tavis
{
    public static class LinkHelper
    {

        public static string GetLinkRelationTypeName<T>()
        {
            return GetLinkRelationTypeName(typeof (T));
        }

        public static string GetLinkRelationTypeName(Type t)
        {
            var relation = "related";
            System.Reflection.MemberInfo info = t;
            object[] attributes = info.GetCustomAttributes(typeof (LinkRelationTypeAttribute), false);
            if (attributes.Length > 0)
            {
                var rel = (LinkRelationTypeAttribute) attributes[0];
                relation = rel.Name;
            }
            return relation;
        }
    }
}
