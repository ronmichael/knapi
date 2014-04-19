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
using System.Diagnostics;
using System.Dynamic;

namespace Knapi
{

    public enum ApiType { HTTP, JSON };

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
        public Dictionary<string, string> parameters { get; set; }
        public ApiType SvcApiType { get; set; }
        public ICredentials Credentials { get; set; }
        public DateTime? lastCall { get; set; }
        public int callCount { get; set; }


        public Service()
        {
            Startup();
            SvcApiType = ApiType.HTTP;
        }

        public Service(string baseUrl)
        {
            this.baseUrl = baseUrl;
            SvcApiType = ApiType.HTTP;
            Startup();

        }

        public Service(string baseUrl, ApiType aptype )
        {
            this.baseUrl = baseUrl;
            this.SvcApiType = aptype;
            Startup();

        }

        private void Startup()
        {
            callCount = 0;
            headers = new Dictionary<string, string>();
            parameters = new Dictionary<string, string>();
            throttle = new Throttle();

        }




        public dynamic Get(string url, dynamic requestParameters = null)
        {
            string data = "";

            if(parameters.Count>0)
            {

                foreach (KeyValuePair<string, string> pr in parameters)
                {
                    if (data.Length > 0) data += "&";
                    data += HttpUtility.UrlEncode(pr.Key) + "=" + HttpUtility.UrlEncode(pr.Value);
                }

            }

            if (requestParameters != null)
            {
                Type px = requestParameters.GetType();
                foreach (var x in px.GetProperties())
                {
                    if (x.GetValue(requestParameters, null) is string[])
                    {
                        string[] xx = x.GetValue(requestParameters, null);
                        foreach (string y in xx)
                        {
                            if (data.Length > 0) data += "&";
                            data += HttpUtility.UrlEncode(x.Name + "[]") + "=" + HttpUtility.UrlEncode(y);
                        }
                    }
                    else
                    {
                        if (data.Length > 0) data += "&";
                        data += HttpUtility.UrlEncode(x.Name) + "=" + HttpUtility.UrlEncode(x.GetValue(requestParameters, null));
                    }

                }
            }

            if (url.Contains("?")) url += "&";
            else url += "?";
            url += data;

            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = baseUrl + url;
            string response = Http(url);

            return JsonConvert.DeserializeObject(response);

        }


        public dynamic Post(string url, dynamic requestParameters = null)
        {
            string data = "";


            if (SvcApiType == ApiType.JSON)
            {
                Dictionary<string, string> dx = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> pr in parameters) dx.Add(pr.Key, pr.Value);
                if (requestParameters != null)
                {
                    Type px = requestParameters.GetType();
                    foreach (var x in px.GetProperties())
                    {
                        if (x.GetValue(requestParameters, null) is string[])
                        {
                            string[] xx = x.GetValue(requestParameters, null);
                            foreach (string y in xx) dx.Add(x.Name, y);
                        }
                        else
                        {
                            dx.Add(x.Name, x.GetValue(requestParameters, null));
                        }

                    }

                }

                data =  JsonConvert.SerializeObject(dx);

            }
            else
            {

                if (parameters.Count > 0)
                {

                    foreach (KeyValuePair<string, string> pr in parameters)
                    {
                        if (data.Length > 0) data += "&";
                        data += HttpUtility.UrlEncode(pr.Key) + "=" + HttpUtility.UrlEncode(pr.Value);
                    }

                }

                if (requestParameters != null)
                {
                    Type px = requestParameters.GetType();
                    foreach (var x in px.GetProperties())
                    {
                        if (x.GetValue(requestParameters, null) is string[])
                        {
                            string[] xx = x.GetValue(requestParameters, null);
                            foreach (string y in xx)
                            {
                                if (data.Length > 0) data += "&";
                                data += HttpUtility.UrlEncode(x.Name + "[]") + "=" + HttpUtility.UrlEncode(y);
                            }
                        }
                        else
                        {
                            data += HttpUtility.UrlEncode(x.Name) + "=" + HttpUtility.UrlEncode(x.GetValue(requestParameters, null));
                        }

                    }

                }
            }


            if (!url.StartsWith("http://") && !url.StartsWith("https://")) url = baseUrl + url;
            string response = Http(url, "POST", data);
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
            if (Credentials != null) request.Credentials = Credentials;


            
            request.UserAgent = "Knapi 0.1";

            foreach (KeyValuePair<string,string> header in headers)
            {
                string checkKey = header.Key.ToLower();

                if (checkKey=="user-agent")
                    request.UserAgent = header.Value;
                else
                    request.Headers.Add(header.Key, header.Value);
            }


            request.ContentType = "application/json"; // but it's not really always json?
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
            try
            {
                output = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
            }
            catch(WebException err)
            {
                if(err.Response!=null)
                {
                    // assume the response is in json ... works for Stormpath, perhaps not others
                    output = new StreamReader(err.Response.GetResponseStream()).ReadToEnd();
                }
            }
            return output;


        }

        
    }


}
