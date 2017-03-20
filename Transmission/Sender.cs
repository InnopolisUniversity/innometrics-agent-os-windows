using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using CommonModels;
using CommonModels.Helpers;

namespace Transmission
{
    public class Sender
    {
        public string AuthorizationUri { get; }
        public string SendDataUri { get; }
        private string Token { get; set; }

        public Sender(string authorizationUri, string sendDataUri)
        {
            AuthorizationUri = authorizationUri;
            SendDataUri = sendDataUri;
        }

        public bool Authorized => Token != null;

        /// <summary>Get and store token</summary>
        /// <returns>Success of authorization</returns>
        /// <exception cref="WebException"></exception>
        public bool Authorize(string username, string password, out HttpStatusCode statusCode)
        {
            var loginData = new {username = username, password = password};
            string json = JsonMaker.Serialize(loginData);
            string tokenJson = Send(AuthorizationUri, json, "application/json", out statusCode);
            if (statusCode == HttpStatusCode.OK)
            {
                Token = JsonMaker.DeserializeToken(tokenJson);
            }
            return Token != null;
        }

        /// <exception cref="WebException"></exception>
        public string SendActivities(Report activities, out HttpStatusCode statusCode)
        {
            return Send(SendDataUri, JsonMaker.Serialize(activities), "application/json", out statusCode);
        }

        /// <exception cref="WebException"></exception>
        public string SendActivities(string json, out HttpStatusCode statusCode)
        {
            return Send(SendDataUri, json, "application/json", out statusCode);
        }

        /// <exception cref="WebException"></exception>
        private string Send(string uri, string json, string contentType, out HttpStatusCode statusCode)
        {
            // source of the solution
            // http://www.terminally-incoherent.com/blog/2008/05/05/send-a-https-post-request-with-c/
            
            // create a request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "POST";
            if (Token != null)
                request.Headers["Authorization"] = $"Token {Token}";

            // turn our request string into a byte stream
            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            // this is important - make sure you specify type this way
            request.ContentType = contentType;
            //request.ContentLength = postBytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(postBytes, 0, postBytes.Length);
            }

            // grab te response and print it out to the console along with the status code
            string responseString = null;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse) request.GetResponse();
            }
            catch (WebException e)
            {
                response = (HttpWebResponse) e.Response;
                statusCode = response.StatusCode;
                return response?.StatusDescription;
            }
            catch (Exception e)
            {
                statusCode = 0;
                return e.Message;
            }
            
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                responseString = streamReader.ReadToEnd();
            }
            statusCode = response.StatusCode;
            return responseString;
        }
    }
}
