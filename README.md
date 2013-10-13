Knapi
=====

The Knapi API Valet is designed to be a lightweight universal REST API toolkit for .NET. 
It will get you connected to any (JSON) REST API in a matter of seconds.

Features:

- Throttling
- Dynamic JSON deserialization


Quick example using AngelList, which has a 1000 requests her hour limit:

	Knapi.Service angel = new Knapi.Service("https://api.angel.co/1/");

	angel.throttle.callCount = 1000;
	angel.throttle.seconds = 60 * 60;

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

