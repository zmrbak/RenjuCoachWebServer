using Newtonsoft.Json.Linq;
using RenjuCoachWebServer;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public static Boolean RunFinished = true;

        static void Main(string[] args)
        {
            List<ArrayList> renJunStartList = new List<ArrayList>();
            ArrayList array = new ArrayList();

            //寒星开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼●┼┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("6, 8, 1");
            renJunStartList.Add(array);

            //溪月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼●┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("6, 9, 1");
            renJunStartList.Add(array);

            // 疏星开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼●┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("6, 10, 1");
            renJunStartList.Add(array);

            // 花月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×●┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("7, 9, 1");
            renJunStartList.Add(array);


            // 残月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×┼●┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("7, 10, 1");
            renJunStartList.Add(array);

            // 雨月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●●┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("8, 9, 1");
            renJunStartList.Add(array);


            // 金星开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●┼●┼┼8
            //┼┼┼┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("8, 10, 1");
            renJunStartList.Add(array);


            // 松月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼●┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("9, 8, 1");
            renJunStartList.Add(array);

            // 丘月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼●┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("9, 9, 1");
            renJunStartList.Add(array);

            // 新月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼●┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("9, 10, 1");
            renJunStartList.Add(array);

            // 瑞星开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            //┼┼┼┼●┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("10, 8, 1");
            renJunStartList.Add(array);

            // 山月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            //┼┼┼┼┼●┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("10, 9, 1");
            renJunStartList.Add(array);

            // 长星开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼┼┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼×┼┼┼┼┼9
            //┼┼●┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("9, 7, 2");
            array.Add("10, 6, 1");
            renJunStartList.Add(array);


            // 峡月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼┼┼┼┼7
            //┼┼┼×●┼┼┼┼8
            //┼┼●┼┼┼┼┼┼9
            //┼┼┼┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("8, 7, 2");
            array.Add("9, 6, 1");
            renJunStartList.Add(array);

            // 恒星开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼×┼┼┼┼┼7
            //┼┼●┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            //┼┼┼┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 7, 2");
            array.Add("8, 6, 1");
            renJunStartList.Add(array);

            // 水月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼×┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼●┼┼9
            //┼┼┼┼┼┼┼┼┼9
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 9, 2");
            array.Add("9, 10, 1");
            renJunStartList.Add(array);

            // 流星开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼×┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            //┼┼┼┼┼┼●┼┼10
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 9, 2");
            array.Add("10, 10, 1");
            renJunStartList.Add(array);

            // 云月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼×┼┼┼┼7
            //┼┼┼●●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            //┼┼┼┼┼┼┼┼┼10
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 8, 2");
            array.Add("8, 7, 1");
            renJunStartList.Add(array);

            // 浦月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼×┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼●┼┼┼9
            //┼┼┼┼┼┼┼┼┼10
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 9, 2");
            array.Add("9, 9, 1");
            renJunStartList.Add(array);

            // 岚月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼×┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            //┼┼┼┼┼●┼┼┼10
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 9, 2");
            array.Add("10, 9, 1");
            renJunStartList.Add(array);


            // 银月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼×┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼●┼┼┼┼9
            //┼┼┼┼┼┼┼┼┼10
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 9, 2");
            array.Add("9, 8, 1");
            renJunStartList.Add(array);


            // 明星开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼×┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            //┼┼┼┼●┼┼┼┼10
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 9, 2");
            array.Add("10, 8, 1");
            renJunStartList.Add(array);

            // 斜月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼×┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼●┼┼┼┼┼9
            //┼┼┼┼┼┼┼┼┼10
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 9, 2");
            array.Add("9, 7, 1");
            renJunStartList.Add(array);

            // 名月开局
            //4 5 6 7 8 9 101112
            //┼┼┼┼┼┼┼┼┼5
            //┼┼┼┼┼┼┼┼┼6
            //┼┼┼┼┼×┼┼┼7
            //┼┼┼┼●┼┼┼┼8
            //┼┼┼┼┼┼┼┼┼9
            //┼┼●┼┼┼┼┼┼10
            array = new ArrayList();
            array.Add("8, 8, 1");
            array.Add("7, 9, 2");
            array.Add("10, 6, 1");
            renJunStartList.Add(array);


            foreach (var items in renJunStartList)
            {
                BoardMatrix boardMatrix = new BoardMatrix(15);
                boardMatrix.ChessRule = RenJunRule.PROHIBITED_YES;
                boardMatrix.User = "TestUser";

                foreach (var item in items)
                {
                    //删除空格
                    String points = item.ToString().Replace(" ", "");
                    String[] myPoints = points.Split(',');
                    boardMatrix.SetMatrixPices(int.Parse(myPoints[0]), int.Parse(myPoints[1]), int.Parse(myPoints[2]));
                }

                while (true)
                {
                    Console.WriteLine(boardMatrix.ToTextMatrix());
                    String json = boardMatrix.ToPostJson();
                    HttpClientDoPost(json);

                    if(RunFinished==true)
                    {
                        break;
                    }

                    while (GotResult == false)
                    {
                        Thread.Sleep(100);
                    }

                    String[] point = LastResult.Split(',');
                    if (point.Length < 3) continue;

                    int x = int.Parse(point[0]);
                    int y = int.Parse(point[1]);
                    int p = int.Parse(point[2]);
                    boardMatrix.SetMatrixPices(x, y, p);
                }

            }
           

            
        }

        /// <summary>
        /// 发送棋盘数据
        /// </summary>
        /// <param name="boardString"></param>
        public static  void HttpClientDoPost(String boardString)
        {
            GotResult = false;
            RunFinished = false;
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
                    RunFinished = true;
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
