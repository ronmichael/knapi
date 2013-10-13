Knapi
=====

The Knapi API Valet is designed to be a universal REST API toolkit for .NET. 
It will get you connected to any REST API in a matter of seconds.

Features:

- Trottling
- Dynamic JSON deserialization (thanks to Json.Net)


Quick example using AngelList:

	Knapi.Service angel = new Knapi.Service("https://api.angel.co/1/");

	// AngelList limits their API to 1000 requests per hour, so...
	angel.throttle.callCount = 1000;
	angel.throttle.seconds = 60 * 60;

	// Be considerate and let the server know who you are
	angel.headers.Add("From", "me@myserver.com");

	dynamic results = angel.Get("jobs");

	foreach (dynamic job in results.jobs)
	{
		Console.WriteLine(job.title);
	}


To do:

- Allow async calls
- Error handling
- Handle Retry-After responses from server
- Allow throttle bursting; custom throttle handler?
- Built in support for Authorization encoding?
- Built in support for cookies
- Handle XML
- Allow for POST

