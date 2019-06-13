using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;

namespace WebLogin
{
    class HTTPMethods
    {
        public static string GET(string url, string referer, CookieContainer cookies)
        {
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
            webReq.Method = "GET";
            webReq.CookieContainer = cookies;
            webReq.UserAgent = "";
            webReq.Referer = referer;

            HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();

            string pageSrc;
            StreamReader sr = new StreamReader(webResp.GetResponseStream());
            pageSrc = sr.ReadToEnd();

            return pageSrc;


        }

        public static bool POST(string url, string postData, string referer, ref CookieContainer cookies)
        {
            string words = "Authentication failed!";
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
            webReq.Method = "POST";
            webReq.CookieContainer = cookies;
            webReq.UserAgent = "";
            webReq.Referer = referer;
           
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            Stream postStream = webReq.GetRequestStream();
            byte[] postByte = Encoding.ASCII.GetBytes(postData);
            postStream.Write(postByte, 0, postByte.Length);
            postStream.Dispose();

            HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
            cookies.Add(webResp.Cookies);

            string pageSrc;
            StreamReader sr = new StreamReader(webResp.GetResponseStream());
            pageSrc = sr.ReadToEnd();

            sr.Dispose();

            return (!pageSrc.Contains(words));
        }



    }

}