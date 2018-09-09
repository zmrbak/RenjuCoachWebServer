using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RenjuCoachWebServer
{
    //棋盘
    public class BoardMatrix
    {
        //棋盘大小
        int boardSiz = 0;

        //棋盘二维数组
        int[,] metrix;

        //规则,默认无禁手
        RenJunRule chessRule = RenJunRule.PROHIBITED_NO;

        //用户
        String user = "RenjunTester";

        /// <summary>
        /// 初始化棋盘,必要参数，棋盘大小
        /// </summary>
        /// <param name="boardSiz"></param>
        public BoardMatrix(int boardSiz)
        {
            this.boardSiz = boardSiz;
            metrix = new int[boardSiz, boardSiz];
        }

        //放置棋子，行列开始数字为1，数组中开始为0
        public void SetMatrixPices(int row, int col, int player)
        {
            setPices(row - 1, col - 1, player);
        }

        //放置棋子
        private void setPices(int row, int col, int player)
        {
            metrix[row, col] = player;
        }

        //提取某位置的棋子
        private int getPices(int row, int col)
        {
            return metrix[row, col];
        }

        //提取棋子，Console，字符显示
        private String getPicesMatrix(int row, int col)
        {
            if (metrix[row, col] == 1)
            {
                return "O";
            }

            if (metrix[row, col] == 2)
            {
                return "X";
            }

            return "┼";
        }

        //棋盘上的某棋子，字符串化
        private String getPicesString(int row, int col)
        {
            if (metrix[row, col] == 0) return "";

            return (row + 1) + "," + (col + 1) + "," + metrix[row, col];
        }

        //把棋盘上状态转换成一个字符串
        public override string ToString()
        {
            String boardString = "";

            //行
            for (int i = 0; i < boardSiz; i++)
            {
                //列
                for (int j = 0; j < boardSiz; j++)
                {
                    String myPicesString = getPicesString(i, j);
                    if (myPicesString == "") continue;

                    boardString += myPicesString + "|";
                }
            }
            return boardString.Substring(0, boardString.Length - 1);
        }

        //把棋盘上状态转换成一个矩阵字符串，直接在Console上输出
        public String ToTextMatrix()
        {
            String boardString = "  1-2-3-4-5-6-7-8-9-101112131415" + Environment.NewLine;

            //行
            for (int i = 0; i < boardSiz; i++)
            {
                //列
                boardString += String.Format("{0:D2}", i + 1);
                for (int j = 0; j < boardSiz; j++)
                {
                    boardString += getPicesMatrix(i, j) + " ";
                }
                boardString += Environment.NewLine;
            }
            return boardString;
        }

        /// <summary>
        /// 棋盘旋转,按照指定的角度旋转
        /// </summary>
        /// <param name="Angles"></param>
        /// <returns></returns>
        public BoardMatrix MatrixTranspose(MatrixTransposeAngle Angles)
        {
            BoardMatrix boardMatrix = new BoardMatrix(boardSiz);
            switch (Angles)
            {
                //顺时针旋转90度，逆时针旋转270度，都是一样的
                case MatrixTransposeAngle.ANGLE_90:
                case MatrixTransposeAngle.ANGLE_R270:
                    //行
                    for (int i = 0; i < boardSiz; i++)
                    {
                        //列
                        for (int j = 0; j < boardSiz; j++)
                        {
                            //行列交换
                            //a[i][j] = b[h-1-j][i];
                            boardMatrix.setPices(i, j, metrix[boardSiz - 1 - j, i]);
                        }
                    }
                    break;

                //顺时针旋转180度，逆时针旋转180度，都是一样的
                case MatrixTransposeAngle.ANGLE_180:
                case MatrixTransposeAngle.ANGLE_R180:
                    //OK
                    //行
                    for (int i = 0; i < boardSiz; i++)
                    {
                        //列
                        for (int j = 0; j < boardSiz; j++)
                        {
                            //行列交换
                            //当前行=boardSiz-i
                            //当前列=boardSiz-j
                            //a[i][j] = b[h-1-i][w-1-j]; 
                            boardMatrix.setPices(i, j, metrix[boardSiz - 1 - i, boardSiz - 1 - j]);
                        }
                    }
                    break;

                //顺时针旋转270度，逆时针旋转90度，都是一样的
                case MatrixTransposeAngle.ANGLE_270:
                case MatrixTransposeAngle.ANGLE_R90:
                    //行
                    for (int i = 0; i < boardSiz; i++)
                    {
                        //列
                        for (int j = 0; j < boardSiz; j++)
                        {
                            //行列交换
                            //当前行=boardSiz-i
                            //a[i][j] = b[j][w-1-i]
                            boardMatrix.setPices(i, j, metrix[j, boardSiz - 1 - i]);
                        }
                    }
                    break;
                default:
                    break;
            }
            return boardMatrix;
        }

        /// <summary>
        /// 将棋盘状态输出成一个Json字符串
        /// </summary>
        /// <returns></returns>
        public String ToPostJson()
        {
            JObject jObject = new JObject();
            
            //棋盘大小
            jObject.Add("boardsize", boardSiz);

            int pointsnumbers = 0;
            for (int i = 0; i < boardSiz; i++)
            {
                for (int j = 0; j < boardSiz; j++)
                {
                    if (metrix[i, j] != 0) pointsnumbers++;
                }
            }

            //棋子数量
            jObject.Add("pointsnumbers", pointsnumbers);

            //规则
            jObject.Add("chessrule", ((int)chessRule).ToString());

            //输出棋子
            JArray points = new JArray();
            for (int i = 0; i < boardSiz; i++)
            {
                for (int j = 0; j < boardSiz; j++)
                {
                    if (metrix[i, j] != 0)
                    {
                        JObject item = new JObject();
                        item.Add("location", (i + 1) + "," + (j + 1));
                        item.Add("player", metrix[i, j]);

                        points.Add(item);
                    }
                }
            }
            //添加棋子
            jObject.Add("points", points);

            //用户名
            jObject.Add("user", user);

            //时间标记
            jObject.Add("timestamp", DateTime.Now.ToString());

            //签名
            jObject.Add("sign", Md5Sign(jObject.ToString(Newtonsoft.Json.Formatting.None, null)));

            return jObject.ToString(Newtonsoft.Json.Formatting.None, null);
        }

        /// <summary>
        /// 棋盘左右反转
        /// </summary>
        /// <returns></returns>
        public BoardMatrix MatrixReverseLeftRight()
        {
            BoardMatrix boardMatrix = new BoardMatrix(boardSiz);
            //左右反转
            //行
            for (int i = 0; i < boardSiz; i++)
            {
                //列
                for (int j = 0; j < boardSiz; j++)
                {
                    //行列交换
                    //当前行=boardSiz-i
                    boardMatrix.setPices(i, j, metrix[i, boardSiz - j - 1]);
                }
            }
            return boardMatrix;
        }

        public String SortedBoardString(String JsonString)
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
        public String Md5Sign(String JsonString)
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
        public Boolean CheckMd5Sign(String BoardJson,String md5Sign)
        {
            return true ? false : md5Sign == Md5Sign(BoardJson);
        }
    }
    /// <summary>
    ///棋盘旋转角度 
    /// </summary>
    public enum MatrixTransposeAngle
    {
        ANGLE_90 = 1,
        ANGLE_180 = 2,
        ANGLE_270 = 3,
        ANGLE_R90 = 4,
        ANGLE_R180 = 5,
        ANGLE_R270 = 6
    }

    public enum RenJunRule
    {
        PROHIBITED_NO = 1,
        PROHIBITED_YES = 4
    }
}
