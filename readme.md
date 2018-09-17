# Tavis.Link #

Ths library attempts to extend Microsoft's [System.Net.Http](http://nuget.org/packages/System.Net.Http) functionality provide an implementation of the concepts described in [RFC 5988 Web Linking](http://tools.ietf.org/html/rfc5988) and [RFC 6570 URI Templates](http://tools.ietf.org/html/rfc6570).


## Feature Summary ##

- Create strongly typed links that encapsulate your URI construction rules and other interaction constraints defined by the link relation.
- Easily re-use IANA registered links like first, last, previous, next, edit, create-form, profile.
- Generate and consume link headers
- Allows you to setup a link once and call it many times, with different parameters.
- Create global response handlers for dealing with 500, 404, 3XX responses.
- Create response handlers for link relation types to enable re-use of response behaviours and decouple response handling from requests for better evolvability.


## Examples ##

Use a generic link object to make a HTTP request.

    var httpClient = new HttpClient();
    var link = new Link() { Target= new Uri("http://example.org/foo" };
    var response = httpClient.Send(link.CreateRequest()).Result;

Create a strongly typed link to encapsulate the parameterization of a URI

    var httpClient = new HttpClient();
    var link = new UPSTrackingLink() { 
                         TrackingNumber = "Z12323D2323S"
                    };
    var response = httpClient.Send(link.CreateRequest()).Result;


Use standard IANA link, a responsehandler the FollowLinkAsync extension method
    
    var httpClient = new HttpClient();
    var linkFactory = new LinkFactory();
    linkFactory.AddGlobalHandler(new NotFoundHandler());
    linkFactory.AddGlobalHandler(new ServerUnavailableHandler());
    linkFactory.AddHandler<HelpLink>(new HelpLinkHandler());

    var helpLink = linkFactory.CreateLink<HelpLink>();  // rel="help" is an IANA standard
    helpLink.Target= new Uri("http://example.org/foo";

    httpClient.FollowLinkAsync(helpLink);  // Extension method that SendRequest and then
                                           // passes response to ResponseHandlers

Add a link header to a response

    var response = new HttpResponseMessage();
    response.Headers.AddLinkHeader(new AboutLink() {Target = new Uri("http://example.org/about")});

Consume link headers on the client

     var links = response.ParseLinkHeaders(linkFactory);
     var aboutLink = links.Where(l => l is AboutLink).FirstOrDefault();
     

Example of creating a strongly typed link

    public class BingMapLink : Link {

        public BingMapLink()
        {
            Target = new Uri("http://www.bing.com/maps/?v=2&cp={lat}~{long}&lvl={level}");
            SetParameter("level", 10);
        }

        public void SetCoordinates(double latitude, double longitude)
        {
            SetParameter("lat", latitude.ToString());
            SetParameter("long", longitude.ToString());
        }
       
        public void SetLevel(int level)
        {
            SetParameter("level", level.ToString());
        }
    }

which can then be used like this,

	var httpClient = new HttpClient();
    var link = new BingMapLink();
	link.SetCoordinates(45,-73);
    var response = httpClient.Send(link.CreateRequest()).Result;

This approach to using links to create requests can give a rich, domain specific programming model without obscuring the interaction model of HTTP.  

This model of usage is extremely compatible with the use of hypermedia.  Links embedded in documents can have their ***type*** identified by the link relation and the link objects can be deserialized from the returned representations.  Following a link in a document becomes as simple as getting a reference to the link object, calling FollowLinkAsync().
