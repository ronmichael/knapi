using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web;


namespace Knapi
{
    public class Facebook
    {

        /// <summary>
        /// With the app id and secret, get an access token and add it to the headers of the Knapi object
        /// </summary>
        /// <param name="fbservice"></param>
        /// <param name="appid"></param>
        /// <param name="appsecret"></param>
        public static void GetAppAccessToken(Knapi.Service fbservice, string appid, string appsecret)
        {
            string results = Http("https://graph.facebook.com/oauth/access_token?client_id=" + appid + "&client_secret=" + appsecret + "&grant_type=client_credentials");

            if(results.StartsWith("access_token="))
            {
                string accesstoken = results.Substring(13);
                fbservice.headers.Add("Authorization", "Bearer " + accesstoken);
            }

        }

        private static string Http(string url)
        {


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            request.UserAgent = "Knapi 0.1";

            request.ContentType = "application/text";
            request.Accept = "application/text";

            return new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

        }

        
    }
}
