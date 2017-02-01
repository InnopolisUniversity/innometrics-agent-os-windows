using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Transmission
{
    public static class Sender
    {
        public static string Send(string uri, string json, string contentType, out string statusCode)
        {
            // source of the solution
            // http://www.terminally-incoherent.com/blog/2008/05/05/send-a-https-post-request-with-c/

            // create a request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            // turn our request string into a byte stream
            byte[] postBytes = Encoding.ASCII.GetBytes(json);

            // this is important - make sure you specify type this way
            request.ContentType = contentType;
            request.ContentLength = postBytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(postBytes, 0, postBytes.Length);
            }

            // grab te response and print it out to the console along with the status code
            string responseString;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                responseString = streamReader.ReadToEnd();
            }
            statusCode = response.StatusCode.ToString();
            return responseString;
        }
    }
}
