namespace Tavis
{
    public static class ILinkFactoryExtensions
    {
        public static ILink CreateLink<T>(this ILinkFactory linkFactory)
        {
            var rel = LinkHelper.GetLinkRelationTypeName<T>();
            return linkFactory.CreateLink(rel);
        }
   
    }
}