using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tavis.IANA
{
    public class AboutLink : Link
    {
        public AboutLink()
        {
            Relation = "about";
        }
    }
    public class AlternateLink : Link
    {
        public AlternateLink()
        {
            Relation = "alternate";
        }
    }
    public class AppendixLink : Link
    {
        public AppendixLink()
        {
            Relation = "appendix";
        }
    }
    public class ArchivesLink : Link
    {
        public ArchivesLink()
        {
            Relation = "archives";
        }
    }
    public class AuthorLink : Link
    {
        public AuthorLink()
        {
            Relation = "author";
        }
    }
    public class BookmarkLink : Link
    {
        public BookmarkLink()
        {
            Relation = "Bookmark";
        }
    }
    public class CanonicalLink : Link
    {
        public CanonicalLink()
        {
            Relation = "Canonical";
        }
    }
    public class ChapterLink : Link
    {
        public ChapterLink()
        {
            Relation = "Chapter";
        }
    }
    public class CollectionLink : Link
    {
        public CollectionLink()
        {
            Relation = "collection";
        }
    }
    public class ContentsLink : Link
    {
        public ContentsLink()
        {
            Relation = "contents";
        }
    }
    public class CopyrightLink : Link
    {
        public CopyrightLink()
        {
            Relation = "copyright";
        }
    }
    public class CreateFormLink : Link
    {
        public CreateFormLink()
        {
            Relation = "create-form";
        }
    }
    public class CurrentLink : Link
    {
        public CurrentLink()
        {
            Relation = "current";
        }
    }

    public class DescribedByLink : Link
    {
        public DescribedByLink()
        {
            Relation = "describedby";
        }
    }

    public class DescribesLink : Link
    {
        public DescribesLink()
        {
            Relation = "describes";
        }
    }

    public class DisclosureLink : Link
    {
        public DisclosureLink()
        {
            Relation = "disclosure";
        }
    }

    public class DuplicateLink : Link
    {
        public DuplicateLink()
        {
            Relation = "duplicate";
        }
    }

    public class EditLink : Link
    {
        public EditLink()
        {
            Relation = "edit";
        }
    }

    public class EditFormLink : Link
    {
        public EditFormLink()
        {
            Relation = "edit-form";
        }
    }

    public class EditMediaLink : Link
    {
        public EditMediaLink()
        {
            Relation = "edit-media";
        }
    }

    public class EnclosureLink : Link
    {
        public EnclosureLink()
        {
            Relation = "enclosure";
        }
    }

    public class FirstLink : Link
    {
        public FirstLink()
        {
            Relation = "first";
        }
    }

    public class GlossaryLink : Link
    {
        public GlossaryLink()
        {
            Relation = "glossary";
        }
    }

    public class HelpLink : Link
    {
        public HelpLink()
        {
            Relation = "help";
        }
    }

    public class HostsLink : Link
    {
        public HostsLink()
        {
            Relation = "hosts";
        }
    }

    public class HubLink : Link
    {
        public HubLink()
        {
            Relation = "hub";
        }
    }

    public class IconLink : Link
    {
        public IconLink()
        {
            Relation = "icon";
        }
    }

    public class IndexLink : Link
    {
        public IndexLink()
        {
            Relation = "index";
        }
    }

    public class ItemLink : Link
    {
        public ItemLink()
        {
            Relation = "item";
        }
    }

    public class LastLink : Link
    {
        public LastLink()
        {
            Relation = "last";
        }
    }

    public class LatestVersionLink : Link
    {
        public LatestVersionLink()
        {
            Relation = "latest-version";
        }
    }

    public class LicenseLink : Link
    {
        public LicenseLink()
        {
            Relation = "license";
        }
    }

    public class LrddLink : Link
    {
        public LrddLink()
        {
            Relation = "lrdd";
        }
    }

    public class MonitorLink : Link
    {
        public MonitorLink()
        {
            Relation = "monitor";
        }
    }

    public class MonitorGroupLink : Link
    {
        public MonitorGroupLink()
        {
            Relation = "monitor-group";
        }
    }

    public class NextLink : Link
    {
        public NextLink()
        {
            Relation = "next";
        }
    }

    public class NextArchiveLink : Link
    {
        public NextArchiveLink()
        {
            Relation = "next-archive";
        }
    }

    public class NoFollowLink : Link
    {
        public NoFollowLink()
        {
            Relation = "nofollow";
        }
    }

    public class NoReferrerLink : Link
    {
        public NoReferrerLink()
        {
            Relation = "noreferrer";
        }
    }

    public class PaymentLink : Link
    {
        public PaymentLink()
        {
            Relation = "payment";
        }
    }

    public class PredecessorVersionLink : Link
    {
        public PredecessorVersionLink()
        {
            Relation = "predecessor-version";
        }
    }

    public class PrefetchLink : Link
    {
        public PrefetchLink()
        {
            Relation = "prefetch";
        }
    }

    public class PrevLink : Link
    {
        public PrevLink()
        {
            Relation = "prev";
        }
    }

    public class PreviewLink : Link
    {
        public PreviewLink()
        {
            Relation = "preview";
        }
    }

    public class PreviousLink : Link
    {
        public PreviousLink()
        {
            Relation = "Previous";
        }
    }

    public class PrevArchiveLink : Link
    {
        public PrevArchiveLink()
        {
            Relation = "prev-archive";
        }
    }

    public class PrivacyPolicyLink : Link
    {
        public PrivacyPolicyLink()
        {
            Relation = "privacy-policy";
        }
    }

    public class ProfileLink : Link
    {
        public ProfileLink()
        {
            Relation = "profile";
        }
    }

    public class RelatedLink : Link
    {
        public RelatedLink()
        {
            Relation = "related";
        }
    }

    /// <summary>
    /// Identifies a resource that is a reply to the context of the link. 
    /// </summary>
    public class RepliesLink : Link
    {
        public RepliesLink()
        {
            Relation = "replies";
        }
    }

    /// <summary>
    /// Refers to a resource that can be used to search through the link's context and related resources.
    /// </summary>
    public class SearchLink : Link
    {
        public SearchLink()
        {
            Relation = "search";
        }
    }

    /// <summary>
    /// Refers to a section in a collection of resources.
    /// </summary>
    public class SectionLink : Link
    {
        public SectionLink()
        {
            Relation = "section";
        }
    }

    /// <summary>
    /// Conveys an identifier for the link's context. 
    /// </summary>
    public class SelfLink : Link
    {
        public SelfLink()
        {
            Relation = "self";
        }
    }

    /// <summary>
    /// Indicates a URI that can be used to retrieve a service document.
    /// </summary>
    public class ServiceLink : Link
    {
        public ServiceLink()
        {
            Relation = "service";
        }
    }

    /// <summary>
    /// Refers to the first resource in a collection of resources.
    /// </summary>
    public class StartLink : Link
    {
        public StartLink()
        {
            Relation = "start";
        }
    }

    /// <summary>
    /// Refers to a stylesheet.
    /// </summary>
    public class StylesheetLink : Link
    {
        public StylesheetLink()
        {
            Relation = "stylesheet";
        }
    }

    /// <summary>
    /// Refers to a resource serving as a subsection in a collection of resources.
    /// </summary>
    public class SubSectionLink : Link
    {
        public SubSectionLink()
        {
            Relation = "subsection";
        }
    }


    /// <summary>
    /// Points to a resource containing the successor version in the version history. 
    /// </summary>
    public class SuccessorVersionLink : Link
    {
        public SuccessorVersionLink()
        {
            Relation = "successor-version";
        }
    }


    /// <summary>
    /// Gives a tag (identified by the given address) that applies to the current document. 
    /// </summary>
    public class TagLink : Link
    {
        public TagLink()
        {
            Relation = "tag";
        }
    }

    /// <summary>
    /// Refers to the terms of service associated with the link's context.
    /// </summary>
    public class TermsOfServiceLink : Link
    {
        public TermsOfServiceLink()
        {
            Relation = "terms-of-service";
        }
    }

    /// <summary>
    /// Refers to a resource identifying the abstract semantic type of which the link's context is considered to be an instance.
    /// </summary>
    public class TypeLink : Link
    {
        public TypeLink()
        {
            Relation = "type";
        }
    }

    /// <summary>
    /// Refers to a parent document in a hierarchy of documents. 
    /// </summary>
    public class UpLink : Link
    {
        public UpLink()
        {
            Relation = "up";
        }
    }

    /// <summary>
    /// Points to a resource containing the version history for the context. 
    /// </summary>
    public class VersionHistoryLink : Link
    {
        public VersionHistoryLink()
        {
            Relation = "version-history";
        }
    }

    /// <summary>
    /// Identifies a resource that is the source of the information in the link's context. 
    /// </summary>
    public class ViaLink : Link
    {
        public ViaLink()
        {
            Relation = "via";
        }
    }

    /// <summary>
    /// Points to a working copy for this resource.
    /// </summary>
    public class WorkingCopyLink : Link
    {
        public WorkingCopyLink()
        {
            Relation = "working-copy";
        }
    }

    /// <summary>
    /// Points to the versioned resource from which this working copy was obtained. 
    /// </summary>
    public class WorkingCopyOfLink : Link
    {
        public WorkingCopyOfLink()
        {
            Relation = "working-copy-of";
        }
    }
}


