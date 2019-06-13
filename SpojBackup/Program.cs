using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace WebLogin

{
    class Program
    {
        static bool Login(ref CookieContainer cookie)
        {

            CookieContainer myCookies = cookie;

            //string mySrc = HTTPMethods.GET("https://www.spoj.com/login", "https://www.spoj.com/login", myCookies);
            Console.Write("Username: ");
            String user = Console.ReadLine();
            Console.Write("Password: ");
            String pass = Console.ReadLine();
            Console.Clear();
            String postData = "login_user=" + user + "&password=" + pass + "&next_raw=%2F";
            bool mySrc = HTTPMethods.POST("https://www.spoj.com/login", postData, "https://www.spoj.com/login", ref myCookies);

            if (mySrc)
            {

                Console.WriteLine("Successful Login!");
                return true;
            }
            else
            {

                Console.WriteLine("Unsuccessful Login!");
                return false;
                
            }

        }

        static void DownloadByURL()
        {

            Console.Write("Url: "); string remoteUri = Console.ReadLine();
            Console.Write("FileName: "); string fileName = Console.ReadLine(), myStringWebResource = null;
            WebClient myWebClient = new WebClient();
            myStringWebResource = remoteUri + fileName;
            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
            myWebClient.DownloadFile(myStringWebResource, fileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, myStringWebResource);
            Console.WriteLine("\nDownloaded file saved in the following file system folder:\n\t" + AppDomain.CurrentDomain.BaseDirectory);

        }

        static void DownloadByRequest(String NameOfProblem, CookieContainer cookie)
        {
            Console.Write("Downloading " + NameOfProblem + "...");
            String name = NameOfProblem;
            String url = "https://www.spoj.com/problems/" + name + "/edit2/";
            String referer = "https://www.spoj.com/problems/" + name + "/edit2";
            String postData2 = "export%5Ball%5D=on&form_action=export";
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
            webReq.Method = "POST";
            webReq.CookieContainer = cookie;
            webReq.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
            webReq.Referer = referer;
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            Stream postStream = webReq.GetRequestStream();
            byte[] postByte = Encoding.ASCII.GetBytes(postData2);
            postStream.Write(postByte, 0, postByte.Length);
            postStream.Dispose();
            try
            {
                HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                using (Stream output = File.OpenWrite(name + ".zip"))
                using (Stream input = webResp.GetResponseStream())
                {
                    input.CopyTo(output);
                }

                Console.Write("Successfully Downloaded");
                Console.WriteLine();
            }
            catch(WebException webex)
            {
                

                Console.Write("Fail Downloaded");
                Console.WriteLine();
            }

            finally
            {

            }


        }

        static void Main(string[] args)
        {


            //String name = NameOfProblem;
            //String url = "https://www.spoj.com/problems/" + name + "/edit2/";
            //String referer = "https://www.spoj.com/problems/" + name + "/edit2";
            //String postData2 = "export%5Ball%5D=on&form_action=export";
            //HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
            //webReq.Method = "POST";
            //webReq.CookieContainer = cookie;
            //webReq.UserAgent = "";
            //webReq.Referer = referer;
            //webReq.ContentType = "application/x-www-form-urlencoded";
            //webReq.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            //Stream postStream = webReq.GetRequestStream();
            //byte[] postByte = Encoding.ASCII.GetBytes(postData2);
            //postStream.Write(postByte, 0, postByte.Length);
            //postStream.Dispose();
            //HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
            //using (Stream output = File.OpenWrite(name + ".zip"))
            //using (Stream input = webResp.GetResponseStream())
            //{
            //    input.CopyTo(output);
            //}



            //List <String> listName= new List<String>();
            //foreach (var node in htmlDoc.DocumentNode.SelectNodes("//table[@class='problems table table-condensed']/tr[@class='problemrow_']")){
            //    if(node.SelectSingleNode(".//a[@title='Submit a solution to this problem.']") != null){
            //        String nameTemp = node.SelectSingleNode(".//a[@title='Submit a solution to this problem.']").InnerText;
            //        listName.Add(nameTemp);
            //    }
            //}

            //for (int i = 0; i < listName.Count; i++){
            //    String tempValue = listName[i];
            //    string nameOfProblem = tempValue.Remove(0, 1);
            //    DownloadByRequest(nameOfProblem, myCookies);
            //}


            //Create CookieContainer
            CookieContainer myCookies = new CookieContainer();

            //Login
            Boolean checkLogin = Login(ref myCookies);
            if(!checkLogin){
                return;
            }

            //GetHeaderOfSpojPage
            string mySrc2 = HTTPMethods.GET("https://www.spoj.com/problems/my/", "https://www.spoj.com/problems/my/", myCookies);


            //LoadHtml
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(mySrc2);

            //GetListPage
            List<String> pageList = new List<string>();
            List<int> startPage = new List<int>();

            foreach (var node in htmlDoc.DocumentNode.SelectNodes("//center/ul[@class='pagination']/li"))
            {

                if (node.SelectSingleNode(".//a[@class='pager_link']") != null)
                {
                    String page = node.SelectSingleNode(".//a[@class='pager_link']").InnerText;
                    pageList.Add(page);
                }


            }


            int count = 0;
            for (int index = 0; index < pageList.Count - 1; index++)
            {
                startPage.Add(count);
                count = count + 50;

            }




            //Download
            for (int index = 0; index < startPage.Count; index++){

                string downloadUrl = "https://www.spoj.com/problems/my/sort=0,start=" + startPage[index];
                string mySrc3 = HTTPMethods.GET(downloadUrl, downloadUrl, myCookies);
                HtmlDocument htmlDoc2 = new HtmlDocument();
                htmlDoc.LoadHtml(mySrc3);
                List <String> listName= new List<String>();
                foreach (var node in htmlDoc.DocumentNode.SelectNodes("//table[@class='problems table table-condensed']/tr[@class='problemrow_']")){
                    if(node.SelectSingleNode(".//a[@title='Submit a solution to this problem.']") != null){
                        String nameTemp = node.SelectSingleNode(".//a[@title='Submit a solution to this problem.']").InnerText;
                        listName.Add(nameTemp);
                    }
                }

                for (int i = 0; i < listName.Count; i++){
                    String tempValue = listName[i];
                    string nameOfProblem = tempValue.Remove(0, 1);
                    DownloadByRequest(nameOfProblem, myCookies);

                }
            }




        }




    }
}