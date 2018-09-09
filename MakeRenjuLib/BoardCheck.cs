using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RenjuCoachWebServer
{
    public static class BoardCheck
    {
        /// <summary>
        /// 棋盘字符串排序,剔除sign
        /// </summary>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        private static String SortedBoardString(String JsonString)
        {
            JObject jObject = JObject.Parse(JsonString);

            //棋盘基本信息
            List<Tuple<String, String>> JsonKeys = new List<Tuple<string, string>>();
            foreach (KeyValuePair<String, JToken> jsonKeys in jObject)
            {
                if (jsonKeys.Key == "sign") continue;
                JsonKeys.Add(new Tuple<string, string>(jsonKeys.Key, jsonKeys.Value.ToString()));
            }
            JsonKeys.Sort();


            //提取棋子，排序
            List<Tuple<int, int, int>> PointsList = new List<Tuple<int, int, int>>();
            JArray jPoints = JArray.Parse(jObject["points"].ToString());
            for (var i = 0; i < jPoints.Count; i++)
            {
                JObject js = JObject.Parse(jPoints[i].ToString());
                String location = js["location"].ToString();
                string[] locations = location.Split(',');
                int row = int.Parse(locations[0]);
                int col = int.Parse(locations[1]);
                int player = int.Parse(js["player"].ToString());
                PointsList.Add(new Tuple<int, int, int>(row, col, player));
            }
            PointsList.Sort();

            //排序后的棋子
            JArray jNewPoints = new JArray();
            foreach (var item in PointsList)
            {
                JObject Jpoint = new JObject();
                Jpoint.Add("location", item.Item1 + "," + item.Item2);
                Jpoint.Add("player", item.Item3);
                jNewPoints.Add(Jpoint);
            }

            //排序后的棋子
            JObject jNewBoard = new JObject();
            foreach (var item in JsonKeys)
            {
                if (item.Item1 == "points")
                {
                    jNewBoard.Add("points", jNewPoints);
                    continue;
                }

                jNewBoard.Add(item.Item1, item.Item2);
            }

            //棋盘字符串
            return jNewBoard.ToString(Newtonsoft.Json.Formatting.None, null);
        }

        /// <summary>
        /// Md5签名
        /// </summary>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        public static String Md5Sign(String JsonString)
        {
            String SortedJsonString = SortedBoardString(JsonString);

            //MD5签名
            byte[] sor = Encoding.UTF8.GetBytes(SortedJsonString);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                //加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
                strbul.Append(result[i].ToString("x2"));
            }
            return strbul.ToString();
        }


        /// <summary>
        /// 检查MD5签名
        /// </summary>
        /// <param name="BoardJson"></param>
        /// <param name="md5Sign"></param>
        /// <returns></returns>
        public static Boolean CheckMd5Sign(String BoardJson, String md5Sign)
        {
            return md5Sign == Md5Sign(BoardJson) ? true : false;
        }

        /// <summary>
        /// 包含有签名的棋盘数据
        /// </summary>
        /// <param name="BoardJsonWithSign"></param>
        /// <returns></returns>
        public static Boolean CheckMd5Sign(String BoardJsonWithSign)
        {
            String md5String = Md5Sign(BoardJsonWithSign);
            JObject jObject = JObject.Parse(BoardJsonWithSign);
            return md5String == jObject["sign"].ToString() ? true : false;
        }
    }
}