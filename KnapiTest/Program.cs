using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnapiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestPublicFacebook();
            TestPrivateFacebook();
            TestAngelList();

            Console.WriteLine("Done. Press a key.");
            Console.ReadKey();

        }


        static void TestAngelList()
        {

            Console.WriteLine("AngelList job postings\r\n");

            Knapi.Service angel = new Knapi.Service("https://api.angel.co/1/");

            angel.throttle.callCount = 1000;
            angel.throttle.seconds = 60 * 60;

            dynamic results = angel.Get("jobs");

            foreach (dynamic job in results.jobs)
            {
                Console.WriteLine(job.title);
            }

            Console.WriteLine();

        }

        static void TestPublicFacebook()
        {
            Console.WriteLine("Public Facebook test\r\n");

            Knapi.Service fb = new Knapi.Service("https://graph.facebook.com/");

            dynamic results = fb.Get("688222019", new { fields = "id,first_name,last_name" });

            Console.WriteLine("user " + results.id + ": " + results.first_name + " " + results.last_name);

            Console.WriteLine();

        }

        static void TestPrivateFacebook()
        {
            Console.WriteLine("Private Facebook test\r\n");

            Knapi.Service fb = new Knapi.Service("https://graph.facebook.com/");

            string appkey = "1431066537106620"; // this is from a useless, empty test app in FB
            string appsecret = "4337a4f8fc822e9eff68cb082a8c5ca8";

            Knapi.Facebook.GetAppAccessToken(fb, appkey, appsecret);

            dynamic results = fb.Get(appkey + "/insights");  

            Console.WriteLine(results.data.Count + " insights\r\n");

            dynamic results2 = fb.Get("688222019/permissions");

            Console.WriteLine("Permissions check: " + results2);

            Console.WriteLine();

        }

    }
}
