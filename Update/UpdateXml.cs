using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Update
{
    internal class UpdateXml
    {
        internal Version Version { get; }
        internal Uri Uri { get; }
        internal string FileName { get; }
        internal string Md5 { get; }
        internal string Description { get; }
        internal string LaunchArgs { get; }

        internal UpdateXml(Version version, Uri uri, string fileName, string md5, string description,
            string launchArgs)
        {
            this.Version = version;
            this.Uri = uri;
            this.FileName = fileName;
            this.Md5 = md5;
            this.Description = description;
            this.LaunchArgs = launchArgs;
        }

        internal bool IsNewerThan(Version version)
        {
            return this.Version > version;
        }

        internal static bool ExistsOnServer(Uri location)
        {
            try
            {
                HttpWebRequest req = WebRequest.Create(location.AbsoluteUri) as HttpWebRequest;
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                resp.Close();

                return resp.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        internal static UpdateXml[] Parse(Uri location, string appID)
        {
            Version version = null;
            string url = "", fileName = "", md5 = "", description = "", launchArgs = "";

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(location.AbsoluteUri);

                XmlNodeList nodes = doc.DocumentElement.SelectNodes($"//update[@appId='{appID}']");

                if (nodes == null)
                {
                    return null;
                }

                UpdateXml[] xmls = new UpdateXml[nodes.Count];

                for (int i = 0; i < nodes.Count; i++)
                {
                    version = System.Version.Parse(nodes[i]["version"].InnerText);
                    url = nodes[i]["url"].InnerText;
                    fileName = nodes[i]["fileName"].InnerText;
                    md5 = nodes[i]["md5"].InnerText.ToLower();
                    description = nodes[i]["description"].InnerText;
                    launchArgs = nodes[i]["launchArgs"].InnerText;
                    xmls[i] = new UpdateXml(version, new Uri(url), fileName, md5, description, launchArgs);
                }
                return xmls;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
