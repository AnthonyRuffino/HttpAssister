using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace HttpAssister
{
    public class RestHelper
    {

        public static string GET = "GET";
        public static string POST = "POST";
        public static string PUT = "PUT";
        public static string DELETE = "DELETE";

        

        //GET Methods
        public static string Get(string url, string path, Dictionary<string, string> parameters, bool debugMode = false)
        {
            return SendRequest(url, path, parameters, GET, debugMode);
        }

        public static string Get(string url, string path, Dictionary<string, string> parameters, out string outApiCall, bool debugMode = false)
        {
            return SendRequest(url, path, parameters, GET, out outApiCall, debugMode);
        }

        //GET Methods - Dynamic JSON return type
        public static T Get<T>(string url, string path, Dictionary<string, string> parameters, T defaultReturn, bool debugMode = false)
        {
            return SendRequest<T>(url, path, parameters, GET, defaultReturn, debugMode);
        }

        public static T Get<T>(string url, string path, Dictionary<string, string> parameters, T defaultReturn, out string outApiCall, bool debugMode = false)
        {
            return SendRequest<T>(url, path, parameters, GET, defaultReturn, out outApiCall, debugMode);
        }


        //POST Methods
        public static string Post(string url, string path, Dictionary<string, string> parameters, bool debugMode = false)
        {
            return SendRequest(url, path, parameters, POST, debugMode);
        }

        public static string Post(string url, string path, Dictionary<string, string> parameters, out string outApiCall, bool debugMode = false)
        {
            return SendRequest(url, path, parameters, POST, out outApiCall, debugMode);
        }

        //POST Methods - Dynamic JSON return type
        public static T Post<T>(string url, string path, Dictionary<string, string> parameters, T defaultReturn, bool debugMode = false)
        {
            return SendRequest<T>(url, path, parameters, POST, defaultReturn, debugMode);
        }

        public static T Post<T>(string url, string path, Dictionary<string, string> parameters, T defaultReturn, out string outApiCall, bool debugMode = false)
        {
            return SendRequest<T>(url, path, parameters, POST, defaultReturn, out outApiCall, debugMode);
        }


        //PUT Methods
        public static string Put(string url, string path, Dictionary<string, string> parameters, bool debugMode = false)
        {
            return SendRequest(url, path, parameters, PUT, debugMode);
        }

        public static string Put(string url, string path, Dictionary<string, string> parameters, out string outApiCall, bool debugMode = false)
        {
            return SendRequest(url, path, parameters, PUT, out outApiCall, debugMode);
        }

        //PUT Methods - Dynamic JSON Response
        public static T Put<T>(string url, string path, Dictionary<string, string> parameters, T defaultReturn, bool debugMode = false)
        {
            return SendRequest<T>(url, path, parameters, PUT, defaultReturn, debugMode);
        }

        public static T Put<T>(string url, string path, Dictionary<string, string> parameters, T defaultReturn, out string outApiCall, bool debugMode = false)
        {
            return SendRequest<T>(url, path, parameters, PUT, defaultReturn, out outApiCall, debugMode);
        }


        //DELETE Methods
        public static string Delete(string url, string path, Dictionary<string, string> parameters, bool debugMode = false)
        {
            return SendRequest(url, path, parameters, DELETE, debugMode);
        }

        public static string Delete(string url, string path, Dictionary<string, string> parameters, out string outApiCall, bool debugMode = false)
        {
            return SendRequest(url, path, parameters, DELETE, out outApiCall, debugMode);
        }

        //DELETE Methods - Dynamic JSON Response
        public static T Delete<T>(string url, string path, Dictionary<string, string> parameters, T defaultReturn, bool debugMode = false)
        {

            return SendRequest<T>(url, path, parameters, DELETE, defaultReturn, debugMode);
        }

        public static T Delete<T>(string url, string path, Dictionary<string, string> parameters, T defaultReturn, out string outApiCall, bool debugMode = false)
        {

            return SendRequest<T>(url, path, parameters, DELETE, defaultReturn, out outApiCall, debugMode);
        }





        //Dynamic JSON POST Method
        public static string PostJson<T>(string url, string path, T data, bool debugMode = false)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(getUri(url, path));
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
            string response = null;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(data);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                try
                {
                    response = GetResponseFromRequest(httpWebRequest);
                }
                catch (Exception e)
                {
                    if (debugMode)
                    {
                        if (debugMode)
                        {
                            Console.WriteLine("Exception: " + e.Message);
                        }
                    }
                    response = null;
                }
            }

            return response;
        }

        //Dynamic JSON POST Methods - Dynamic JSON return type
        public static U PostJson<T, U>(string url, string path, T data, U defaultReturn, bool debugMode = false)
        {

            U returnObj = defaultReturn;

            try
            {
                string jsonString = PostJson<T>(url, path, data, debugMode);

                if (jsonString != null)
                {
                    returnObj = JsonConvert.DeserializeObject<U>(jsonString);
                }
                else
                {
                    returnObj = defaultReturn;
                }
            }
            catch (Exception e)
            {
                if (debugMode)
                {
                    if (debugMode)
                    {
                        Console.WriteLine("Exception: " + e.Message);
                    }
                }
                returnObj = defaultReturn;
            }

            return returnObj;
        }

        



        ///////////
        //HELPERS//
        ///////////

        private static string getQuery(Dictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();

            if (parameters != null && parameters.Count > 0)
            {
                sb.Append("?");

                bool firstParam = true;

                foreach (string key in parameters.Keys)
                {
                    string value = parameters[key] != null && parameters[key].Length > 0 ? HttpUtility.UrlEncode(parameters[key]) : "";

                    if (firstParam)
                    {
                        sb.Append(key + "=" + value);
                        firstParam = false;
                    }
                    else
                    {
                        sb.Append("&" + key + "=" + value);
                    }
                }

                return sb.ToString();

            }
            else
            {
                return "";
            }
        }


        private static string ReadResponseFromStream(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string body = reader.ReadToEnd();
                return body;
            }
        }

        private static string GetResponseFromRequest(WebRequest httpWebRequest)
        {
            using (WebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream responseStream = httpResponse.GetResponseStream())
                {
                    return ReadResponseFromStream(responseStream);
                }
            }
        }


        public static string getUrlParam(string paramName, string queryString)
        {

            if (queryString != null && paramName != null)
            {
                Uri myUri = new Uri("http://www.makebelivewebsite.com" + queryString);
                string param = HttpUtility.ParseQueryString(myUri.Query).Get(paramName);
                return param;
            }

            return null;
        }


        public static Dictionary<string, string> getUrlParams(string queryString)
        {
            Dictionary<string, string> returnMap = new Dictionary<string, string>();

            if (queryString != null)
            {
                Uri myUri = new Uri("http://www.makebelivewebsite.com" + queryString);

                foreach (string paramName in System.Web.HttpUtility.ParseQueryString(myUri.Query).Keys)
                {
                    if (!returnMap.ContainsKey(paramName))
                    {
                        string paramValue = System.Web.HttpUtility.ParseQueryString(myUri.Query).Get(paramName);
                        returnMap.Add(paramName, paramValue);
                    }
                }
            }

            return returnMap;
        }


        public static bool validateFields(Dictionary<string, string> fields, out string missingParams)
        {
            bool allValid = true;
            missingParams = "";
            bool firstMissingParam = true;

            if (fields != null)
            {
                foreach (KeyValuePair<string, string> field in fields)
                {
                    if (field.Key != null)
                    {
                        if (field.Value == null || field.Value.Length <= 0)
                        {
                            allValid = false;
                            if (firstMissingParam)
                            {
                                missingParams = field.Key;
                            }
                            else
                            {
                                missingParams = missingParams + "," + field.Key;
                            }
                        }
                    }
                }

            }
            else
            {
                missingParams = "no params supplied";
                allValid = false;
            }

            return allValid;
        }

        public static WebRequest getWebRequest(string url, string path, Dictionary<string, string> parameters, string method, out string outApiCall, bool debugMode = false)
        {
            path = fixPathSlashes(url, path);

            string uri = url + path + getQuery(parameters);
            outApiCall = uri;

            if (debugMode)
            {
                System.Diagnostics.Debug.WriteLine("URI REQUESTED: " + uri);
            }

            WebRequest request = HttpWebRequest.CreateDefault(new Uri(uri));
            request.Method = method;
            return request;
        }

        public static string getUri(string url, string path, Dictionary<string,string> parameters)
        {
            path = fixPathSlashes(url, path);
            return url + path + getQuery(parameters);
        }

        public static string getUri(string url, string path)
        {
            path = fixPathSlashes(url, path);
            return url + path;
        }

        public static string fixPathSlashes(string url, string path)
        {
            if (url != null && path != null && path.Length > 0 && !path.StartsWith("/") && !url.EndsWith("/"))
            {
                path = "/" + path;
            }
            else if (path == null || path.Length == 0)
            {
                path = "";
            }

            return path;
        }





        //Private Methods - Heavy Lifting




        private static string SendRequest(string url, string path, Dictionary<string, string> parameters, string method, bool debugMode = false)
        {
            string outApiCall = "";
            return SendRequest(url, path, parameters, method, out outApiCall, debugMode);
        }
        private static string SendRequest(string url, string path, Dictionary<string, string> parameters, string method, out string outApiCall, bool debugMode = false)
        {
            outApiCall = "";
            try
            {
                return GetResponseFromRequest(getWebRequest(url, path, parameters, method, out outApiCall, debugMode));
            }
            catch (Exception e)
            {
                if (debugMode)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                return null;
            }
        }

        private static T SendRequest<T>(string url, string path, Dictionary<string, string> parameters, string method, T defaultReturn, bool debugMode = false)
        {
            string outApiCall = "";
            return (SendRequest<T>(url, path, parameters, method, defaultReturn, out outApiCall, debugMode));
        }

        private static T SendRequest<T>(string url, string path, Dictionary<string, string> parameters, string method, T defaultReturn, out string outApiCall, bool debugMode = false)
        {
            outApiCall = "";

            try
            {
                string jsonString = GetResponseFromRequest(getWebRequest(url, path, parameters, method, out outApiCall, debugMode));
                T returnObj = JsonConvert.DeserializeObject<T>(jsonString);
                return returnObj;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return defaultReturn;
            }
        }
    }
}

