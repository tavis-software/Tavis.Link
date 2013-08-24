using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Tavis.IANA;

namespace Tavis
{
    public class LinkFactory
    {
        private readonly Dictionary<string, LinkRegistration>  _LinkRegistry = new Dictionary<string, LinkRegistration>(StringComparer.OrdinalIgnoreCase);
        private readonly List<IHttpResponseHandler> _GlobalResponseHandlers = new List<IHttpResponseHandler>();


        public HintFactory HintFactory { get; set; }

        public LinkFactory()
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

            HintFactory = new HintFactory();  // Default hintfactory

        }

        public void AddLinkType<T>() where T : Link, new()
        {
            var t = new T();
            _LinkRegistry.Add(t.Relation, new LinkRegistration() {LinkType =typeof(T) } ); 
        }

        public void AddHandler<T>(IHttpResponseHandler handler) where T : Link, new()
        {
            var t = new T();
            var reg = _LinkRegistry[t.Relation];
            reg.ResponseHandlers.Add(handler);
        }

        public void AddGlobalHandler(IHttpResponseHandler handler) 
        {
            _GlobalResponseHandlers.Add(handler);
        }

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
            SetupHandlers(reg, t);
            return t;

        }
        
        public T CreateLink<T>() where T : Link, new()
        {
            var t = new T();
            var reg = _LinkRegistry[t.Relation];
            SetupHandlers(reg, t);
            return t;

        }

        private static void SetupHandlers(LinkRegistration reg, Link t)
        {
            if (reg.ResponseHandlers.Count == 1)
            {
                t.HttpResponseHandler = reg.ResponseHandlers.First(); // Must be just a IHttpResponseHandler
            }
            else
            {
                foreach (var handler in reg.ResponseHandlers)
                {
                    t.AddHandler((DelegatingResponseHandler) handler);
                }
            }
        }

    }

    internal class LinkRegistration
    {
        public Type LinkType { get; set; }
        public List<IHttpResponseHandler> ResponseHandlers { get; set; }

        public LinkRegistration()
        {
            ResponseHandlers = new List<IHttpResponseHandler>();
        }
    }

   
}
