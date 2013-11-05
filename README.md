Knapi
=====

The Knapi API Valet is designed to be a lightweight universal REST API toolkit for .NET. 
It will get you connected to any (JSON) REST API in a matter of seconds.

Features:
--------------------------

- Throttling
- Dynamic JSON deserialization


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



Change history
--------------------------

1.0.1 - November 4th, 2013
- Added tests
- Added Facebook helper to get app access_token 

1.0.0 - October 1st, 2013
- Initial release


To do:
--------------------------

- Allow async calls
- Error handling
- Handle Retry-After responses from server
- Allow throttle bursting; custom throttle handler?
- Built in support for Authorization encoding?
- Built in support for cookies
- Handle XML
- Allow for POST

