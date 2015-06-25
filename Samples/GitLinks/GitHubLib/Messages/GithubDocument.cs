using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tavis;
using Tavis.IANA;

namespace GitHubLib
{
    public class GithubDocument
    {
     
        private JObject _doc;
        private JArray _List;
        private Dictionary<string, JToken> _Properties;
        private Dictionary<string, ILink> _Links;
        public List<GithubDocument> Items { get; set; } 
        public GithubDocument(Stream document, ILinkFactory linkFactory)
        {
            var sr = new StreamReader(document);
            var root = JToken.Load(new JsonTextReader(sr));

            _doc = root as JObject;
            if (_doc != null)
            {
                Load(linkFactory);
            }
            else
            {
                _List = root as JArray;
                Items = new List<GithubDocument>();
                foreach (JObject doc in _List)
                {
                    var childDoc = new GithubDocument(doc,linkFactory);
                    if (childDoc.Properties.ContainsKey("url"))
                    {
                        var itemUrl = new Uri((string)childDoc.Properties["url"]);
                        var itemLink = linkFactory.CreateLink<ItemLink>();
                        itemLink.Target = itemUrl;
                        childDoc.Links.Add(itemLink.Relation, itemLink);
                    }
                    Items.Add(childDoc);
                }
            }
        }

        public GithubDocument(JObject document, ILinkFactory linkFactory)
        {
            _doc = document;
            if (_doc != null)
            {
                Load(linkFactory);
            }
        }




        public void Load(ILinkFactory linkFactory)
        {
            
                // get all properties that end in _url
                _Links = _doc.Properties()
                    .Where(p => p.Name.EndsWith("_url"))
                    .Select<JProperty, ILink>(p =>
                    {
                        var link = linkFactory.CreateLink("http://api.github.com/rels/" + p.Name.Replace("_url", ""));
                        link.Target = new Uri((string)p.Value);
                        return link;
                    })
                    .ToDictionary(k => k.Relation, v => v);

                _Properties = _doc.Properties()
                    .Where(p => !p.Name.EndsWith("_url"))
                    .ToDictionary(k => k.Name, v => v.Value);
            
        }


        public Dictionary<string, ILink> Links
        {
            get { return _Links; }
        }

        public T GetLink<T>() where T : class, ILink 
        {
            var relation = LinkHelper.GetLinkRelationTypeName<T>();
            return _Links[relation] as T;
        }

        public Dictionary<string, JToken> Properties
        {
            get { return _Properties; }
        }
    }
}
