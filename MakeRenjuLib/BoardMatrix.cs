﻿using Newtonsoft.Json.Linq;
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

        public RenJunRule ChessRule { get => chessRule; set => chessRule = value; }
        public string User { get => user; set => user = value; }

        /// <summary>
        /// 初始化棋盘,必要参数，棋盘大小
        /// </summary>
        /// <param name="boardSiz"></param>
        public BoardMatrix(int boardSiz, RenJunRule chessRule = RenJunRule.PROHIBITED_NO, String user = "RenjunTester")
        {
            this.boardSiz = boardSiz;
            metrix = new int[boardSiz, boardSiz];

            this.ChessRule = chessRule;
            this.User = user;
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

        public string ToRenJunLib()
        {
            String RejunString = 
@"          X=Black   O=White          
                                     
    A B C D E F G H I J K L M N O    
";
            for (int i = 0; i < 15; i++)
            {
                RejunString += String.Format(" {0,2}",15-i);
                for (int j = 0; j < 15; j++)
                {
                    if (metrix[i, j] == 0)
                    {
                        RejunString += " .";
                        continue;
                    }

                    if (metrix[i, j] == 1)
                    {
                        RejunString += " O";
                        continue;
                    }

                    if (metrix[i, j] == 2)
                    {
                        RejunString += " X";
                        continue;
                    }
                }
                RejunString += String.Format(" {0,-2}", 15 - i)+Environment.NewLine;
            }
            RejunString +=
@"    A B C D E F G H I J K L M N O    

    ----- Text Board -----

";
            return RejunString;
        }

        /// <summary>
        /// 将棋盘状态输出成一个Json字符串
        /// </summary>
        /// <returns></returns>
        public String ToPostJson()
        {
            JObject jObject = new JObject();

            //棋盘大小
            jObject.Add("boardsize", boardSiz.ToString());

            int pointsnumbers = 0;
            for (int i = 0; i < boardSiz; i++)
            {
                for (int j = 0; j < boardSiz; j++)
                {
                    if (metrix[i, j] != 0) pointsnumbers++;
                }
            }

            //棋子数量
            jObject.Add("pointsnumbers", pointsnumbers.ToString());

            //规则
            jObject.Add("chessrule", ((int)ChessRule).ToString());

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
                        item.Add("player", metrix[i, j].ToString());

                        points.Add(item);
                    }
                }
            }
            //添加棋子
            jObject.Add("points", points);

            //用户名
            jObject.Add("user", User);

            //时间标记
            jObject.Add("timestamp", DateTime.Now.ToString());

            //签名
            jObject.Add("sign", BoardCheck.Md5Sign(jObject.ToString(Newtonsoft.Json.Formatting.None, null)));

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

        public BoardMatrix MatrixReverseUpDown()
        {
            BoardMatrix boardMatrix = new BoardMatrix(boardSiz);
            //行
            //上下反转
            for (int i = 0; i < boardSiz; i++)
            {
                //列
                for (int j = 0; j < boardSiz; j++)
                {
                    //行列交换
                    //当前行=boardSiz-i
                    boardMatrix.setPices(i, j, metrix[boardSiz - i - 1, j]);
                }
            }
            return boardMatrix;
        }

        public BoardMatrix ReMatrixReverseUpDown()
        {
            return MatrixReverseUpDown();
        }

        /// <summary>
        /// 判断棋盘上的棋是否已经结束
        /// </summary>
        /// <returns></returns>
        public Boolean IsChessExited()
        {
            int count_w = 0;
            int count_b = 0;
            //横
            for (int i = 0; i < boardSiz; i++)
            {
                count_w = 0;
                count_b = 0;

                for (int j = 0; j < boardSiz; j++)
                {
                    if (metrix[i, j] == 1)
                    {
                        count_b++;
                        count_w = 0;
                        if (count_b >= 5) return true;
                        continue;
                    }

                    if (metrix[i, j] == 2)
                    {
                        count_b = 0;
                        count_w++;
                        if (count_w >= 5) return true;
                        continue;
                    }

                    count_b = 0;
                    count_w = 0;
                }
            }

            //竖
            for (int i = 0; i < boardSiz; i++)
            {
                count_w = 0;
                count_b = 0;

                for (int j = 0; j < boardSiz; j++)
                {
                    if (metrix[j, i] == 1)
                    {
                        count_b++;
                        count_w = 0;
                        if (count_b >= 5) return true;
                        continue;
                    }

                    if (metrix[j, i] == 2)
                    {
                        count_b = 0;
                        count_w++;
                        if (count_w >= 5) return true;
                        continue;
                    }

                    count_b = 0;
                    count_w = 0;
                }
            }

            //左上，右下
            for (int i = 0; i < boardSiz - 5; i++)
            {
                count_w = 0;
                count_b = 0;
                for (int j = 0; j < boardSiz - 5; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (metrix[j + k, i + k] == 1)
                        {
                            count_b++;
                            count_w = 0;
                            if (count_b >= 5) return true;
                            continue;
                        }

                        if (metrix[j + k, i + k] == 2)
                        {
                            count_b = 0;
                            count_w++;
                            if (count_w >= 5) return true;
                            continue;
                        }
                    }
                    count_b = 0;
                    count_w = 0;
                }
            }


            //右上，左下
            //行
            for (int i = 0; i < boardSiz - 5; i++)
            {
                count_w = 0;
                count_b = 0;
                //列
                for (int j = 5; j < boardSiz; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (metrix[i + k, j - k] == 1)
                        {
                            count_b++;
                            count_w = 0;
                            if (count_b >= 5) return true;
                            continue;
                        }

                        if (metrix[i + k, j - k] == 2)
                        {
                            count_b = 0;
                            count_w++;
                            if (count_w >= 5) return true;
                            continue;
                        }
                    }
                    count_b = 0;
                    count_w = 0;
                }
            }

            //没有超过五子，未分胜负
            return false;
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
