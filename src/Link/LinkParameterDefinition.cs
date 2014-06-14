using System;

namespace Tavis
{
    /// <summary>
    /// Definition of parameter used to fill URI templates
    /// </summary>
    public class LinkParameterDefinition
    {
        public string Name { get; set; }
        public Uri Identifier { get; set; }
        //todo: Should we add datatype here?
    }
}