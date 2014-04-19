Knapi
=====

The Knapi API Valet is designed to be a lightweight universal REST API toolkit for .NET. 
It will get you connected to any (JSON) REST API in a matter of seconds.

Features:
--------------------------

- Throttling
- Dynamic JSON deserialization

Installation
-------------------------------------------------------------
Knapi is available via NuGet (https://www.nuget.org/packages/Knapi) and can be installed from the Package Manager:

	PM> Install-Package Knapi

Examples
--------------------------

Pull some recent job postings from AngelList:

	Knapi.Service angel = new Knapi.Service("https://api.angel.co/1/");

	angel.throttle.callCount = 1000;
	angel.throttle.seconds = 60 * 60;

	dynamic results = angel.Get("jobs");

	foreach (dynamic job in results.jobs)
	{
		Console.WriteLine(job.title);
	}

Query Facebook's Open Graph for a user's name:

    Knapi.Service fb = new Knapi.Service("https://graph.facebook.com/");

    dynamic results = fb.Get(userid, new { fields = "id,first_name,last_name" });

    Console.WriteLine("Hello " + results.first_name + " " + results.last_name);

Make a secure query to Facebook's open graph using the Knapi Facebook helper:

    Knapi.Service fb = new Knapi.Service("https://graph.facebook.com/");

    Knapi.Facebook.GetAppAccessToken(fb, yourappkey, yourappsecret);

    dynamic results = fb.Get(auserid + "/permissions");

    Console.WriteLine("Permissions check: " + results);

Get some data about threads from Disqus:

	Knapi.Service dq = new Knapi.Service("https://disqus.com/api/3.0/");

    dq.parameters.Add("access_token", my_access_token);
    dq.parameters.Add("api_key", my_api_key);
    dq.parameters.Add("api_secret", my_api_secret);

    dq.headers.Add("headername", "can set HTTP headers too");

    var output = dq.Get("threads/set.json", new
    {
        thread = new string[] { "link:http://myhome.com", "link:http://yourhome.com" }
    });

Verify and retrieve user login at Stormpath:

    Knapi.Service stormpath = new Knapi.Service("https://api.stormpath.com/v1/applications/$YOUR_APPLICATION_ID/", Knapi.ApiType.JSON);

    stormpath.Credentials = new NetworkCredential("$YOUR_API_KEY_ID", "$YOUR_API_KEY_SECRET");

    stormpath.parameters.Add("type", "basic");
    stormpath.parameters.Add("value", Knapi.Tools.Base64Encode("%USER_NAME", "%PASSWORD"));

    dynamic results = stormpath.Post("loginAttempts");

    if (results.account != null) 
    {
        stormpath.parameters.Clear();
        dynamic account = stormpath.Get(results.account.href);
        string email = account.email;
        string name = account.fullName;
    }
    else
    {
        // authentication failed
    }



Change history
--------------------------

1.0.3 - April 19th, 2014
- Fixed compeltely broken Post function
- Added Tools class with new Base64Encode function for typical username & password encoding
- Added Credentials property (allowing basic HTTPS authentication, etc)
- Added SvcApiType property to set whether data is posted as HTTP form (default) or JSON; used for Post, not Get
- WebException during HTTP request will now try to return results as parsed JSON

1.0.2 - January 12th, 2014
- Added parameters option to main class to set parameters that are passed with all API calls
- Updated Get to accept string arrays in parameters

1.0.1 - November 4th, 2013
- Added tests
- Added Facebook helper to get app access_token 

1.0.0 - October 1st, 2013
- Initial release


To do:
--------------------------

- Allow async calls
- Better error handling
- Handle Retry-After responses from server
- Allow throttle bursting; custom throttle handler?
- Built in support for cookies
- Handle XML

