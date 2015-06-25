using Tavis;
using Tavis.RequestBuilders;

namespace GitHubLib
{
    [LinkRelationType("http://api.github.com/rels/code_search")]
    public class CodeSearchLink : Link
    {
        public enum SearchSort
        {
            stars,
            forks,
            updated,
            defaultsort
        };

        public string Query { get; set; }

        [LinkParameter("page", Default = 1)]
        public int Page { get; set; }
        [LinkParameter("per_page", Default = 20)]
        public int PerPage { get; set; }
        [LinkParameter("sort", Default = SearchSort.defaultsort)]
        public SearchSort Sort { get; set; }

        public CodeSearchLink()
        {
            Page = 1;
            PerPage = 20;
            Sort = SearchSort.defaultsort;
            
        }

        public static CodeSearchResults InterpretResponse(GithubDocument document)
        {
            var results = new CodeSearchResults();

            results.Count = (int)document.Properties["total_count"]; 

            return results;
        }
        public class CodeSearchResults
        {
            public int Count { get; set; }
        }
    }
}