using Newtonsoft.Json.Linq;
using RenjuCoachWebServer;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace RenjuCoachRemoteTest
{
    class Program
    {
        public static String result = "";
        public static String uri = "http://202.115.129.217:9799/Renju/RenJun.aspx";
        //public static String uri = "http://localhost:62088/Renjun.aspx";
        public static String LastResult = "";
        public static Boolean GotResult = true;

        static void Main(string[] args)
        {
            BoardMatrix boardMatrix = new BoardMatrix(15);
            boardMatrix.ChessRule = RenJunRule.PROHIBITED_NO;
            boardMatrix.User = "TestUser";
            boardMatrix.SetMatrixPices(8, 8, 1);
            boardMatrix.SetMatrixPices(8, 9, 2);
            boardMatrix.SetMatrixPices(8, 10, 1);

            while(true)
            {
                Console.WriteLine(boardMatrix.ToTextMatrix());
                String json = boardMatrix.ToPostJson();
                HttpClientDoPost(json);
                while (GotResult == false)
                {
                    Thread.Sleep(1000);
                }

                String[] point = LastResult.Split(',');
                int x = int.Parse(point[0]);
                int y = int.Parse(point[1]);
                int p = int.Parse(point[2]);
                boardMatrix.SetMatrixPices(x, y, p);
            }
        }

        /// <summary>
        /// 发送棋盘数据
        /// </summary>
        /// <param name="boardString"></param>
        public static  void HttpClientDoPost(String boardString)
        {
            GotResult = false;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };

            using (var httpclient = new HttpClient(handler))
            {
                httpclient.BaseAddress = new Uri(uri);

                var buffer = System.Text.Encoding.UTF8.GetBytes(boardString);
                var byteContent = new ByteArrayContent(buffer);

                var task =  httpclient.PostAsync(uri, byteContent);
                //在这里会等待task返回。
                var rep = task.Result;

                var task2 = rep.Content.ReadAsStringAsync();
                //在这里会等待task返回。
                var responseString = task2.Result;

                //Console.WriteLine("POST RESULT:\t" + responseString);

                JObject jObject = JObject.Parse(responseString);
                if (jObject.Property("status").Value.ToString() == "4")
                {
                    Console.WriteLine("棋局结束！");
                    return;
                }

                while (GotResult == false)
                {
                    HttpClientDoGet(responseString);
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// 提取服务器计算结果
        /// </summary>
        /// <param name="Json"></param>
        public static void HttpClientDoGet(String Json)
        {
            JObject jObject = JObject.Parse(Json);

            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };
            using (var httpclient = new HttpClient(handler))
            {
                httpclient.BaseAddress = new Uri(uri);
                httpclient.DefaultRequestHeaders.Accept.Clear();
                httpclient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //远程获取数据
                var task = httpclient.GetAsync("?uid=" + jObject.Property("uid").Value.ToString() + "&boardtype=" + jObject.Property("boardtype").Value.ToString());
                
                //在这里会等待task返回。
                var rep = task.Result;

                //读取响应内容
                var task2 = rep.Content.ReadAsStringAsync();
                //在这里会等待task返回。
                var retString = task2.Result;

                //Console.Write(retString);
                JObject json = JObject.Parse(retString);
                if (json.Property("status").Value.ToString() == "3")
                {
                    LastResult = json.Property("msg").Value.ToString();
                    Console.WriteLine(Environment.NewLine + "落子位置：" + LastResult + Environment.NewLine);
                    GotResult = true;
                }
                else
                {
                    Console.Write(".");
                }
            }
        }
    }
}
