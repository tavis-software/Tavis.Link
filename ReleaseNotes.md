# Release Notes


### 2.0.4  (Warning: There are breaking changes in this version)
- Updated LinkFactory to work with ILink
- Removed EmbedTarget support
- Added support for default values of Link Parameters
- Added reflection based discovery of Link parameters for filling templates
- Broke the Follow and Apply steps down into separate extension methods.
- Added LinkRelation as parameter to IResponseHandler.  
- Removed IRequestBuilder interface.  If you don't want to use the DelegatingRequestBuilder pipeline then you can implement IRequestFactory and take complete control of the process.
- Made Follow method work with links that only implement the interfaces.
- Introduced IResponseHandler, IRequestFactory and ILink interface to remove need for deriving from Link.
- Renamed RfcLink to LinkAttributes
- Created a seperate property for Template. This makes it easier to re-use links for different sets of parameters as the template is not lost when Target is resolved.
- Removed dependency on Nuget based HttpClient
- Removed IResponseHandler from the Link class.  Its only role is now as a request factory
- Introduced HttpResponseMachine and HttpResponseMachine<T>  (profile matching not working yet)

### 2.0.0 - Never released
- Implemented a ResponseHandler infrastructure to allow Links to process responses
- Attempted to make Link objects immutable to enable them to be used on multiple threads.

### 0.4.1 
- Added support for link hints
- Added support for URI identifier on uri template parameters


### 0.4.0 
- Converted library to be a portable class library

