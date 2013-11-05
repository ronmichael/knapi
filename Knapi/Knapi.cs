using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using System.Collections;
using System.Threading;
using System.Web;


namespace Knapi
{
    public class Service
    {
        
        public class Throttle
        {
            public int callCount = 0;
            public int seconds = 0;
            public bool allowBursting = false; // not implemented; no bursting yet
            public int wait {
                get {
                    if (callCount == 0) return 0;
                    return (seconds*1000)/callCount;
                }
            }
        }


        public Throttle throttle { get; set; }
        public string baseUrl { get; set; }
        public Dictionary<string, string> headers { get; set; }


        public DateTime? lastCall { get; set; }
        public int callCount { get; set; }
        
        public Service()
        {
            Startup();
        }

        public Service(string baseUrl)
        {
            this.baseUrl = baseUrl;
            Startup();

        }
        
        private void Startup()
        {
            callCount = 0;
            headers = new Dictionary<string, string>();
            throttle = new Throttle();

        }




        public dynamic Get(string url, dynamic parameters = null)
        {
            string data = "";

            if(parameters!=null)
            {
                Type px = parameters.GetType();
                foreach(var x in px.GetProperties())
                {
                    data += HttpUtility.UrlEncode(x.Name) + "=" + HttpUtility.UrlEncode(x.GetValue(parameters, null));
                }
                if (url.Contains("?")) url += "&";
                else url += "?";
                url += data;
            }
            
            string response = Http(baseUrl + url);

            return JsonConvert.DeserializeObject(response);

        }




        private string Http(string url, string method = "GET", string data = "")
        {
            
            if (lastCall != null && throttle.callCount > 0)
            {
                int wait = throttle.wait;
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks-((DateTime)lastCall).Ticks);
                if (ts.TotalMilliseconds < wait)
                {
                    Thread.Sleep(wait - Convert.ToInt32(ts.TotalMilliseconds));
                }
            }

            callCount++;
            lastCall = DateTime.Now;

            string output = "";

            //string key = "";
            //var encKey = Convert.ToBase64String(Encoding.Default.GetBytes(key + ":"));
            //request.Headers.Add("Authorization", "Basic " + encKey);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            request.UserAgent = "Knapi 0.1";

            foreach (KeyValuePair<string,string> header in headers)
            {
                string checkKey = header.Key.ToLower();

                if (checkKey=="user-agent")
                    request.UserAgent = header.Value;
                else
                    request.Headers.Add(header.Key, header.Value);
            }


            request.ContentType = "application/json";
            request.Accept = "application/json";

            if (data.Length > 0)
            {
                byte[] ba = Encoding.UTF8.GetBytes(data);
                request.ContentLength = ba.Length;

                Stream ds = request.GetRequestStream();
                ds.Write(ba, 0, ba.Length);
                ds.Close();
            }


            // check for Retry-After response header (# of seconds to wait)

            output = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

            return output;


        }

        
    }


}
