using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Tavis.IANA;

namespace Tavis
{
    /// <summary>
    /// 
    /// </summary>
    public class LinkFactory 
    {
        private readonly Dictionary<string, LinkRegistration>  _LinkRegistry = new Dictionary<string, LinkRegistration>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public HintFactory HintFactory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LinkFactory(bool includeIANALinks = true)
        {
            if (includeIANALinks)
            {
                RegisterIANALinks();
            }
            HintFactory = new HintFactory();  // Default hintfactory
        }

        public IEnumerable<LinkRegistration> Registrations
        {
            get
            {
                return _LinkRegistry.Values;
            }
        }

        private void RegisterIANALinks()
        {
            // Register all official IANA link types
            AddLinkType<AboutLink>();
            AddLinkType<AlternateLink>();
            AddLinkType<AppendixLink>();
            AddLinkType<ArchivesLink>();
            AddLinkType<AuthorLink>();
            AddLinkType<BookmarkLink>();
            AddLinkType<CanonicalLink>();
            AddLinkType<ChapterLink>();
            AddLinkType<CollectionLink>();
            AddLinkType<ContentsLink>();
            AddLinkType<CopyrightLink>();
            AddLinkType<CreateFormLink>();
            AddLinkType<CurrentLink>();
            AddLinkType<DescribedByLink>();
            AddLinkType<DescribesLink>();
            AddLinkType<DisclosureLink>();
            AddLinkType<DuplicateLink>();
            AddLinkType<EditLink>();
            AddLinkType<EditFormLink>();
            AddLinkType<EnclosureLink>();
            AddLinkType<FirstLink>();
            AddLinkType<GlossaryLink>();
            AddLinkType<HelpLink>();
            AddLinkType<HostsLink>();
            AddLinkType<HubLink>();
            AddLinkType<IconLink>();
            AddLinkType<IndexLink>();
            AddLinkType<ItemLink>();
            AddLinkType<LastLink>();
            AddLinkType<LatestVersionLink>();
            AddLinkType<LicenseLink>();
            AddLinkType<LrddLink>();
            AddLinkType<MonitorLink>();
            AddLinkType<MonitorGroupLink>();
            AddLinkType<NextLink>();
            AddLinkType<NextArchiveLink>();
            AddLinkType<NoFollowLink>();
            AddLinkType<NoReferrerLink>();
            AddLinkType<PaymentLink>();
            AddLinkType<PredecessorVersionLink>();
            AddLinkType<PrefetchLink>();
            AddLinkType<PrevLink>();
            AddLinkType<PreviewLink>();
            AddLinkType<PreviousLink>();
            AddLinkType<PrevArchiveLink>();
            AddLinkType<PrivacyPolicyLink>();
            AddLinkType<ProfileLink>();
            AddLinkType<RelatedLink>();
            AddLinkType<RepliesLink>();
            AddLinkType<SearchLink>();
            AddLinkType<SectionLink>();
            AddLinkType<SelfLink>();
            AddLinkType<ServiceLink>();
            AddLinkType<StartLink>();
            AddLinkType<StylesheetLink>();
            AddLinkType<SubSectionLink>();
            AddLinkType<SuccessorVersionLink>();
            AddLinkType<TagLink>();
            AddLinkType<TermsOfServiceLink>();
            AddLinkType<TypeLink>();
            AddLinkType<UpLink>();
            AddLinkType<VersionHistoryLink>();
            AddLinkType<ViaLink>();
            AddLinkType<WorkingCopyLink>();
            AddLinkType<WorkingCopyOfLink>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddLinkType<T>() where T : Link, new()
        {
            var t = new T();
            _LinkRegistry.Add(t.Relation, new LinkRegistration() {LinkType =typeof(T) } ); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void SetHandler<T>(IHttpResponseHandler handler) where T : Link, new()
        {
            var t = new T();
            var reg = _LinkRegistry[t.Relation];
            reg.ResponseHandler = handler;
        }


        public void SetRequestBuilder<T>(IEnumerable<DelegatingRequestBuilder> builders) where T : Link, new()
        {
            var t = new T();
            var reg = _LinkRegistry[t.Relation];

            IHttpRequestBuilder builderList = new DefaultRequestBuilder();
            foreach (var requestBuilder in builders.Reverse())
            {
                requestBuilder.NextBuilder = builderList;
                builderList = (IHttpRequestBuilder) requestBuilder;
            }
            reg.RequestBuilder = builderList;
        }

        public void SetRequestBuilder<T>(DelegatingRequestBuilder builder) where T : Link, new()
        {
            SetRequestBuilder<T>(new []{builder});
        }

        public void SetRequestBuilder(Type linkType, DelegatingRequestBuilder builder) 
        {
            var t = (Link)Activator.CreateInstance(linkType);
            var reg = _LinkRegistry[t.Relation];
            reg.RequestBuilder = builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public Link CreateLink(string relation)
        {
            if (!_LinkRegistry.ContainsKey(relation))
            {
                return new Link()
                    {
                        Relation = relation
                    };
            }
            var reg = _LinkRegistry[relation];
            var t = Activator.CreateInstance(reg.LinkType) as Link;
            t.HttpResponseHandler = reg.ResponseHandler;
            return t;

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateLink<T>() where T : Link, new()
        {
            var t = new T();
            var reg = _LinkRegistry[t.Relation];
            t.HttpResponseHandler = reg.ResponseHandler;
            if (reg.RequestBuilder != null) t.HttpRequestBuilder = reg.RequestBuilder;
            
            return t;
        }

        public T CreateLink<T>(Uri url) where T : Link, new()
        {
            var link = CreateLink<T>();
            link.Target = url;
            return link;
        }

    }

    public class LinkRegistration
    {
        public Type LinkType { get; set; }
        public IHttpResponseHandler ResponseHandler { get; set; }
        public IHttpRequestBuilder RequestBuilder { get; set; }
    }

   
}
