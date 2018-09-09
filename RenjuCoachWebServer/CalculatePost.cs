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
        public static String RenJuPostString(String postedString)
        {
            //解析数据，可能数据不符合要求
            ReturnMessage returnMsg = new ReturnMessage();

            string requestString = Base64Decode(postedString);
            try
            {
                JObject jObject = JObject.Parse(requestString);
                String boardsize = jObject.Property("boardsize").Value.ToString();
                String pointsnumber = jObject.Property("pointsnumber").Value.ToString();
                String points = jObject.Property("points").Value.ToString();
                String chessrule = jObject.Property("chessrule").Value.ToString();
                JArray jArray = JArray.Parse(points);


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
                    returnMsg.Status = MsgStatus.Faild;
                    returnMsg.Msg = "棋子数量有误";
                    returnMsg.Uid = "";
                    return returnMsg.ToString();
                }

                //棋子数量没问题，棋盘上面摆上棋子
                BoardMatrix boardMatrix = new BoardMatrix(int.Parse(boardsize));
                foreach (JObject item in jArray)
                {
                    String xy = item.Property("location").Value.ToString();
                    String player = item.Property("player").Value.ToString();
                    xy = xy.Substring(1, xy.Length - 2);
                    String[] myXy = xy.Split(',');

                    boardMatrix.SetMatrixPices(int.Parse(myXy[0]), int.Parse(myXy[1]), int.Parse(player));
                }


                //同一盘棋，各类变种
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

                //排除相同的
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
                        if (boardsize == sqlDataReader["boardsize"].ToString().Trim())
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

                //判断是那个变种
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
                    SqlHelper.WriteQueryToDB(boardsize, pointsnumber, boardMatrix.ToString(), chessrule, Uid);

                    //向消息队列中提交信息
                    MessageQueue myMessageQueue = null;
                    string queuepath = @".\private$\" + ConfigurationManager.AppSettings["MessageQueueName"];

                    if (!MessageQueue.Exists(queuepath))
                    {
                        myMessageQueue = MessageQueue.Create(queuepath);
                    }
                    myMessageQueue = new MessageQueue(queuepath);

                    Message msg = new Message();
                    msg.Body = Uid;
                    msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                    myMessageQueue.Send(msg);
                }
                //为客户端返回信息
                //ReturnMsg returnMsg = new ReturnMsg();
                returnMsg.Status = MsgStatus.SubmitOK;
                returnMsg.Msg = Uid;
                returnMsg.Uid = Uid;
                returnMsg.BoardType = BoardType;
                //Response.Write(returnMsg.ToString());
                return returnMsg.ToString();
            }
            catch (Exception ex)
            {
                //为客户端返回出错信息
                //ReturnMsg returnMsg = new ReturnMsg();
                returnMsg.Status = MsgStatus.Faild;
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
}