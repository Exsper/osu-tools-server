using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.IO;
using System.Web;
using System.Collections.Specialized;

namespace osu_tools_server
{
    class Program
    {

        readonly static string LISTENER_URL = "http://localhost:1611/";



        static void Main(string[] args)
        {
            try
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(LISTENER_URL);
                listener.Start();
                Console.WriteLine("开始监听：" + LISTENER_URL);
                while (true)
                {
                    try
                    {
                        HttpListenerContext context = listener.GetContext(); //阻塞
                        HttpListenerRequest request = context.Request;
                        Console.WriteLine("收到请求：" + request.Url);
                        string input = request.Url.Query;
                        string urlpath = request.Url.LocalPath;
                        NameValueCollection query = HttpUtility.ParseQueryString(input);
                        HttpListenerResponse response = context.Response; //响应
                        response.AppendHeader("Access-Control-Allow-Origin", "*");
                        string responseBody = "";
                        try
                        {
                            responseBody = Api.GetResponse(urlpath, query);
                        }
                        catch(Exception ex)
                        {
                            responseBody = ex.Message;
                        }
                        response.ContentLength64 = Encoding.UTF8.GetByteCount(responseBody);
                        response.ContentType = "text/html; Charset=UTF-8";
                        // 输出响应内容
                        Stream output = response.OutputStream;
                        using (StreamWriter sw = new StreamWriter(output))
                        {
                            sw.Write(responseBody);
                        }
                        Console.WriteLine("响应结束");
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("程序异常，请重新打开程序：" + err.Message);
            }

            Console.Read();
        }
    }
}
