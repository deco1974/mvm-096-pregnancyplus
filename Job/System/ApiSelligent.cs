using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Job.System
{
    public class ApiSelligent : IDisposable
    {
		private readonly IConfigurationSection _settings;

		public ApiSelligent(IConfigurationSection settings)
        {
			_settings = settings;

		}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public string Get(string action)
        {
            return DoRequest(action, "GET", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Post(string action, string content)
        {
            return DoRequest(action, "POST", content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Put(string action, string content)
        {
            return DoRequest(action, "PUT", content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string DoRequest(string action, string method, string content)
        {
            var     url = _settings["SelligentRestApiBaseUrl"] + action;
            string? signature;
            var     responseString = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (WebRequest.Create(url) is HttpWebRequest request)
            {
                request.Method = method;
                request.ContentType = "application/json";

                var timestamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                var input = string.Format("{0}-{1}-{2}", timestamp, request.Method, request.RequestUri.PathAndQuery);

                var hmacsha256Algorithm = new HMACSHA256(Encoding.UTF8.GetBytes(_settings["SelligentRestApiPassword"] ?? string.Empty));
                using (hmacsha256Algorithm)
                {
                    hmacsha256Algorithm.Initialize();
                    signature = hmacsha256Algorithm.ComputeHash(Encoding.UTF8.GetBytes(input)).ToHex()?.ToUpperInvariant();
                }

                request.Headers.Add(HttpRequestHeader.Authorization, string.Format("hmac {0}:{1}:{2}", _settings["SelligentRestApiUser"], signature, timestamp));

                if (!string.IsNullOrEmpty(content))
                {
                    using (var stream = request.GetRequestStream())
                    {
                        using (var sw = new StreamWriter(stream))
                        {
                            sw.Write(content);
                        }
                    }
                }

                try
                {

                    var response = (HttpWebResponse)request.GetResponse();
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                        {
                            responseString += reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException e)
                {
                    using (var response = e.Response)
                    {
                        var httpResponse = (HttpWebResponse)response;
                        using (var data = response.GetResponseStream())
                        {
                            if (data != null)
                            {
                                responseString = new StreamReader(data).ReadToEnd();
                            }
                        }
                    }
                }


            }

            return responseString;
        }


        public void Dispose()
        {
	        // empty
        }

	}
}