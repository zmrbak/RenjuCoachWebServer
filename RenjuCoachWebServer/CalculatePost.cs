using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Messaging;
using System.Web;
using static RenjuCoachWebServer.RenjunCalculate;

namespace RenjuCoachWebServer
{
    public static class CalculatePost
    {
        //POST来的数据，用来提交计算
        public static String RenJuPostString(String postedString)
        {
            ReturnMessage returnMsg = new ReturnMessage();

            //检查签名是否正确
            if(BoardCheck.CheckMd5Sign(postedString)==false)
            {
                returnMsg.Status = MsgStatus.FAILD;
                returnMsg.Msg = "签名错误！";
                returnMsg.Uid = "";
                return returnMsg.ToString();
            }

            try
            {
                //重新排列棋子
                JObject jObject = JObject.Parse(postedString);
                int boardsize = int.Parse(jObject.Property("boardsize").Value.ToString());
                String pointsnumber = jObject.Property("pointsnumber").Value.ToString();
                String points = jObject.Property("points").Value.ToString();
                String chessrule = jObject.Property("chessrule").Value.ToString();
                JArray jArray = JArray.Parse(points);

                //判断棋盘大小
                if(boardsize<=5)
                {
                    //为客户端返回出错信息
                    returnMsg.Status = MsgStatus.FAILD;
                    returnMsg.Msg = "棋盘太小，标准棋盘大小为：15*15";
                    returnMsg.Uid = "";
                    return returnMsg.ToString();
                }

                //判断棋子数量是否正确
                int white = 0;
                int black = 0;
                foreach (JObject item in jArray)
                {
                    String Str2 = item.Property("player").Value.ToString();
                    if (item.Property("player").Value.ToString() == "1")
                    {
                        black++;
                    }
                    else
                    {
                        white++;
                    }
                }

                //棋子数量不对，直接返回
                if (((white + black) != int.Parse(pointsnumber)) || (!((black == white) || (black == white + 1))))
                {
                    //为客户端返回出错信息
                    returnMsg.Status = MsgStatus.FAILD;
                    returnMsg.Msg = "黑白棋子数量有误，无法继续";
                    returnMsg.Uid = "";
                    return returnMsg.ToString();
                }

                //棋子数量没问题，棋盘上面摆上棋子
                BoardMatrix boardMatrix = new BoardMatrix(boardsize);
                foreach (JObject item in jArray)
                {
                    String xy = item.Property("location").Value.ToString();
                    String player = item.Property("player").Value.ToString();
                    xy = xy.Substring(1, xy.Length - 2);
                    String[] myXy = xy.Split(',');

                    int x = int.Parse(myXy[0]);
                    int y = int.Parse(myXy[1]);
                    int p = int.Parse(player);

                    if(x> boardsize || x<1 || y> boardsize || y<1 ||p>2 ||p<1)
                    {
                        returnMsg.Status = MsgStatus.FAILD;
                        returnMsg.Msg = "棋子越界，（棋子范围1-"+ boardsize + "）";
                        returnMsg.Uid = "";
                        return returnMsg.ToString();
                    }

                    if ( p > 2 || p < 1)
                    {
                        returnMsg.Status = MsgStatus.FAILD;
                        returnMsg.Msg = "棋子颜色错误！（棋子颜色，1：黑旗，2：白棋）";
                        returnMsg.Uid = "";
                        return returnMsg.ToString();
                    }
                    boardMatrix.SetMatrixPices(x, y,p);
                }

                //同一盘棋，八个变种（棋盘旋转，反转）
                List<KeyValuePair<BOARD_TYPE, BoardMatrix>> wzqStringArryList = new List<KeyValuePair<BOARD_TYPE, BoardMatrix>>();
                BoardMatrix ANGLE_0 = boardMatrix;
                BoardMatrix ANGLE_90 = boardMatrix.MatrixTranspose(MatrixTransposeAngle.ANGLE_90);
                BoardMatrix ANGLE_180 = boardMatrix.MatrixTranspose(MatrixTransposeAngle.ANGLE_180);
                BoardMatrix ANGLE_270 = boardMatrix.MatrixTranspose(MatrixTransposeAngle.ANGLE_270);

                BoardMatrix ANGLE_0_REVERSE_UP_DOWN_ANGLE_0 = ANGLE_0.MatrixReverseUpDown();
                BoardMatrix ANGLE_0_REVERSE_UP_DOWN_ANGLE_90 = ANGLE_0_REVERSE_UP_DOWN_ANGLE_0.MatrixTranspose(MatrixTransposeAngle.ANGLE_90);
                BoardMatrix ANGLE_0_REVERSE_UP_DOWN_ANGLE_180 = ANGLE_0_REVERSE_UP_DOWN_ANGLE_0.MatrixTranspose(MatrixTransposeAngle.ANGLE_180);
                BoardMatrix ANGLE_0_REVERSE_UP_DOWN_ANGLE_270 = ANGLE_0_REVERSE_UP_DOWN_ANGLE_0.MatrixTranspose(MatrixTransposeAngle.ANGLE_270);

                wzqStringArryList.Add(new KeyValuePair<BOARD_TYPE, BoardMatrix>(BOARD_TYPE.ANGLE_0, ANGLE_0));
                wzqStringArryList.Add(new KeyValuePair<BOARD_TYPE, BoardMatrix>(BOARD_TYPE.ANGLE_90, ANGLE_90));
                wzqStringArryList.Add(new KeyValuePair<BOARD_TYPE, BoardMatrix>(BOARD_TYPE.ANGLE_180, ANGLE_180));
                wzqStringArryList.Add(new KeyValuePair<BOARD_TYPE, BoardMatrix>(BOARD_TYPE.ANGLE_270, ANGLE_270));

                wzqStringArryList.Add(new KeyValuePair<BOARD_TYPE, BoardMatrix>(BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_0, ANGLE_0_REVERSE_UP_DOWN_ANGLE_0));
                wzqStringArryList.Add(new KeyValuePair<BOARD_TYPE, BoardMatrix>(BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_90, ANGLE_0_REVERSE_UP_DOWN_ANGLE_90));
                wzqStringArryList.Add(new KeyValuePair<BOARD_TYPE, BoardMatrix>(BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_180, ANGLE_0_REVERSE_UP_DOWN_ANGLE_180));
                wzqStringArryList.Add(new KeyValuePair<BOARD_TYPE, BoardMatrix>(BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_270, ANGLE_0_REVERSE_UP_DOWN_ANGLE_270));


                String CheckSQLString = "";
                List<String> pointLists = new List<string>();
                foreach (KeyValuePair<BOARD_TYPE, BoardMatrix> item in wzqStringArryList)
                {
                    String pointString = item.Value.ToString();
                    Boolean pointExist = false;

                    foreach (String myPoint in pointLists)
                    {
                        if (myPoint == pointString)
                        {
                            pointExist = true;
                            break;
                        }
                    }

                    if (pointExist == false)
                    {
                        pointLists.Add(pointString);
                        CheckSQLString += " points LIKE '" + pointString + "' OR";
                    }
                }
                CheckSQLString = CheckSQLString.Substring(0, CheckSQLString.Length - 2);

                //创建唯一ID
                String Uid = System.Guid.NewGuid().ToString("N");
                Boolean boardExist = false;
                String pointsFromDataBase = "";

                //在数据库中检索，检查是否已经存在
                SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.AppSettings["SqlConnectionRenjun"]);
                try
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand("", sqlConnection);
                    sqlCommand.CommandText = "SELECT * FROM ChessBoard WHERE " + CheckSQLString;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.Read())
                    {
                        if (boardsize == int.Parse(sqlDataReader["boardsize"].ToString().Trim()))
                        {
                            if (chessrule == sqlDataReader["chessrule"].ToString().Trim())
                            {
                                Uid = sqlDataReader["UID"].ToString().Trim();
                                pointsFromDataBase = sqlDataReader["points"].ToString().Trim();
                                boardExist = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }

                //判断是那个变种，默认：ANGLE_0
                BOARD_TYPE BoardType = BOARD_TYPE.ANGLE_0;
                if (boardExist == true)
                {
                    foreach (KeyValuePair<BOARD_TYPE, BoardMatrix> item in wzqStringArryList)
                    {
                        String pointString = item.Value.ToString();
                        if (pointsFromDataBase == pointString)
                        {
                            BoardType = item.Key;
                            break;
                        }
                    }
                }
                else
                {
                    //没查到，写入数据库
                    SqlHelper.WriteQueryToDB(boardsize.ToString(), pointsnumber, boardMatrix.ToString(), chessrule, Uid);

                    //向消息队列中提交信息
                    MessageQueue myMessageQueue = null;
                    string queuepath = @".\private$\" + ConfigurationManager.AppSettings["MessageQueueName"];

                    if (!MessageQueue.Exists(queuepath))
                    {
                        myMessageQueue = MessageQueue.Create(queuepath);
                    }
                    myMessageQueue = new MessageQueue(queuepath);

                    //把Uid发送给消息队列等待服务器计算
                    Message msg = new Message();
                    msg.Body = Uid;
                    msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                    myMessageQueue.Send(msg);
                }

                //为客户端返回信息
                returnMsg.Status = MsgStatus.SUBMIT_OK;
                returnMsg.Msg = "提交成功，请继续查询计算结果！";
                returnMsg.Uid = Uid;
                returnMsg.BoardType = BoardType;
                return returnMsg.ToString();
            }
            catch (Exception ex)
            {
                //为客户端返回出错信息
                returnMsg.Status = MsgStatus.FAILD;
                returnMsg.Msg = ex.Message;
                returnMsg.Uid = "";
                return returnMsg.ToString();
            }
        }

        static String Base64Decode(String source)
        {
            byte[] inArray = Convert.FromBase64String(source);
            return System.Text.Encoding.UTF8.GetString(inArray);
        }
    }

    public enum BOARD_TYPE
    {
        ANGLE_0 = 1,
        ANGLE_90 = 2,
        ANGLE_180 = 3,
        ANGLE_270 = 4,

        ANGLE_0_REVERSE_UP_DOWN_ANGLE_0 = 5,
        ANGLE_0_REVERSE_UP_DOWN_ANGLE_90 = 6,
        ANGLE_0_REVERSE_UP_DOWN_ANGLE_180 = 7,
        ANGLE_0_REVERSE_UP_DOWN_ANGLE_270 = 8,
    }
}