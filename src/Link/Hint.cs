using Newtonsoft.Json.Linq;

namespace Tavis
{
    public class Hint
    {
        public string Name { get; set; }
        public JToken Content { get; set; }  // Json document
    }
}