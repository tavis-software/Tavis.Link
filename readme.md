# Tavis.Link #

Ths library attempts to extend Microsoft's [System.Net.Http](http://nuget.org/packages/System.Net.Http) functionality provide an implementation of the concepts described in [RFC 5988 Web Linking](http://tools.ietf.org/html/rfc5988) and [RFC 6570 URI Templates](http://tools.ietf.org/html/rfc6570).

The most basic concept introduced is the **Link** class.  This class provides the ability to encapsulate the knowledge required to use a URL.  

In the spirit of showing the simplest thing that works, here is how it can be used:

    var httpClient = new HttpClient();
    var link = new Link() { Target= new Uri("http://example.org/foo" };
    var response = httpClient.Send(link.CreateRequest()).Result;


This does not add much value over the existing infrastructure.  However, imagine having access to a variety of ***typed*** links.


    var httpClient = new HttpClient();
    var link = new UPSTrackingLink() { 
                         TrackingNumber = "Z12323D2323S"
                    };
    var response = httpClient.Send(link.CreateRequest()).Result;

HttpClient uses HttpRequestMessage for sending HTTP requests but you can only use a request instance once.  A link class allows you to make requests multiple times. It uses the URI template spec to set parameters on the URL.   

This allows you to create new Link classes to encapsulate the behaviour of any kind of link.

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

This approach to using links to create requests gives can give the rich, domain specific programming model without obscuring the interaction model of HTTP.  

This model of usage is extremely compatible with the use of hypermedia.  Links embedded in documents can have their ***type*** identified by the link relation and the link objects can be deserialized from the returned representations.  Following a link in a document becomes as simple as getting a reference to the link object, calling CreateRequest and passing that to the send method of HttpClient.