using Tavis;

namespace GitHubLib
{
    [LinkRelationType("http://api.github.com/rels/user")]
    public class UserLink : Link
    {
        public string User { get; set; }

      
        public static UserResult InterpretResponse(GithubDocument document)
        {
            var result = new UserResult();
            foreach (var property in document.Properties)
            {
                switch (property.Key)
                {
                    case "login":
                        result.Login = (string) property.Value;
                        break;
                    case "following":
                        result.Following = (int)property.Value;
                        break;
                    case "followers":
                        result.Followers = (int)property.Value;
                        break;
                    case "hireable":
                        result.Hireable = (bool)property.Value;
                        break;
                }
            }

            foreach (var link in document.Links.Values)
            {
                if (link is AvatarLink)
                {
                    result.AvatarLink = (AvatarLink)link;
                }
            }
            return result;
        }
        public class UserResult
        {
            public string Login { get; set; }
            public int Following { get; set; }
            public int Followers { get; set; }
            public bool Hireable { get; set; }
            public AvatarLink AvatarLink { get; set; }
        }
    }
}