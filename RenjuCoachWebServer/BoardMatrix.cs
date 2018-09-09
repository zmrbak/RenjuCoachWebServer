using Newtonsoft.Json.Linq;
using System;

namespace RenjuCoachWebServer
{
    //棋盘
    public class BoardMatrix
    {
        int boardSiz = 0;
        int[,] metrix;
        //初始化棋盘
        public BoardMatrix(int boardSiz)
        {
            this.boardSiz = boardSiz;
            metrix = new int[boardSiz, boardSiz];
        }
        //放置棋子
        //行列开始数字为1
        //数组中开始为0
        public void SetMatrixPices(int row, int col, int player)
        {
            setPices(row - 1, col - 1, player);
        }

        private void setPices(int row, int col, int player)
        {
            metrix[row, col] = player;
        }
        //提取某位置的棋子
        private int getPices(int row, int col)
        {
            return metrix[row, col];
        }
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
        //棋盘字符串化
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
        //矩阵旋转
        public BoardMatrix MatrixTranspose(MatrixTransposeAngle Angles)
        {
            BoardMatrix boardMatrix = new BoardMatrix(boardSiz);
            switch (Angles)
            {
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

        public String ToPostJson()
        {
            JObject jObject = new JObject();
            jObject.Add("boardsize", boardSiz);

            int pointsnumber = 0;
            for (int i = 0; i < boardSiz; i++)
            {
                for (int j = 0; j < boardSiz; j++)
                {
                    if (metrix[i, j] != 0) pointsnumber++;
                }
            }
            jObject.Add("pointsnumber", pointsnumber);
            jObject.Add("chessrule", 1);
            jObject.Add("user", "test");
            jObject.Add("timestamp", DateTime.Now.ToString());
            jObject.Add("sign", "ewoiYm9hcm");

            JArray points = new JArray();

            for (int i = 0; i < boardSiz; i++)
            {
                for (int j = 0; j < boardSiz; j++)
                {
                    if (metrix[i, j] != 0)
                    {
                        JObject item = new JObject();
                        item.Add("location", "[" + (i + 1) + "," + (j + 1) + "]");
                        item.Add("player", "" + metrix[i, j] + "");

                        points.Add(item);
                    }
                }
            }
            jObject.Add("points", points);

            return jObject.ToString(Newtonsoft.Json.Formatting.None, null);
        }

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
        public BoardMatrix ReMatrixReverseLeftRight()
        {
            return MatrixReverseLeftRight();
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

        public enum MatrixTransposeAngle
        {
            ANGLE_90 = 1,
            ANGLE_180 = 2,
            ANGLE_270 = 3,
            ANGLE_R90 = 4,
            ANGLE_R180 = 5,
            ANGLE_R270 = 6
        }
    }
}
