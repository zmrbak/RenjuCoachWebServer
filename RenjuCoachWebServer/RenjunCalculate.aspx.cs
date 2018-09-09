using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RenjuCoachWebServer
{
    public partial class RenjunCalculate : System.Web.UI.Page
    {
        //接受外部请求
        protected void Page_Load(object sender, EventArgs e)
        {
            //返回结果
            ReturnMsg returnMsg = new ReturnMsg();

            if (Request.HttpMethod == "POST")
            {
                //读出客户端POST来的的数据(BASE64)
                Stream postData = Request.InputStream;
                StreamReader sr = new StreamReader(postData);
                string requestString = Base64Decode(sr.ReadToEnd());
                sr.Close();

                //解析数据，可能数据不符合要求
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
                        Response.Write(returnMsg.ToString());
                        return;
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
                    BoardMatrix ANGLE_90 = boardMatrix.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_90);
                    BoardMatrix ANGLE_180 = boardMatrix.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_180);
                    BoardMatrix ANGLE_270 = boardMatrix.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_270);

                    BoardMatrix ANGLE_0_REVERSE_UP_DOWN_ANGLE_0 = ANGLE_0.MatrixReverseUpDown();
                    BoardMatrix ANGLE_0_REVERSE_UP_DOWN_ANGLE_90 = ANGLE_0_REVERSE_UP_DOWN_ANGLE_0.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_90);
                    BoardMatrix ANGLE_0_REVERSE_UP_DOWN_ANGLE_180 = ANGLE_0_REVERSE_UP_DOWN_ANGLE_0.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_180);
                    BoardMatrix ANGLE_0_REVERSE_UP_DOWN_ANGLE_270 = ANGLE_0_REVERSE_UP_DOWN_ANGLE_0.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_270);

                    

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
                        WriteQueryToDB(boardsize, pointsnumber, boardMatrix.ToString(), chessrule, Uid);

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
                    Response.Write(returnMsg.ToString());
                }
                catch (Exception ex)
                {
                    //为客户端返回出错信息
                    //ReturnMsg returnMsg = new ReturnMsg();
                    returnMsg.Status = MsgStatus.Faild;
                    returnMsg.Msg = ex.Message;
                    returnMsg.Uid = "";
                    Response.Write(returnMsg.ToString());
                }
            }
            else
            {
                //GET数据，先判断UID,如果没有UID，则什么都不返回
                String uid = Request.QueryString["uid"];
                String boardtype = Request.QueryString["boardtype"];
                if (uid == null || uid.Trim() == "")
                {
                    return;
                }

                //根据UID查询数据库中的计算结果
                SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.AppSettings["SqlConnectionRenjun"]);
                try
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand("", sqlConnection);
                    sqlCommand.CommandText = "SELECT * FROM ChessBoard WHERE uid LIKE  '" + uid + "'";
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    if (sqlDataReader.Read())
                    {
                        String nextstep = sqlDataReader["nextstep"].ToString().Trim();
                        if (nextstep == "")
                        {
                            //如果没有结果，则返回正在运行...
                            returnMsg.Status = MsgStatus.Running;
                        }
                        else
                        {
                            int boardsize = int.Parse(sqlDataReader["boardsize"].ToString().Trim());
                            int pointsnumber = int.Parse(sqlDataReader["pointsnumber"].ToString().Trim());

                            //有结果，则返回结果
                            if (boardtype == null || boardtype.Trim() == "" || boardtype == "1")
                            {
                                //原始数据，不需要转换
                                if ((pointsnumber + 1) % 2 == 0)
                                {
                                    //偶数，白子
                                    returnMsg.Msg = nextstep + ",2";
                                }
                                else
                                {
                                    returnMsg.Msg = nextstep + ",1";
                                }
                                returnMsg.BoardType = BOARD_TYPE.ANGLE_0;
                            }
                            else
                            {
                                //需要转换
                                //returnMsg.BoardType = Enum.Parse(BOARD_TYPE, boardtype);
                                //returnMsg.BoardType = Enum.Parse(typeof(BOARD_TYPE), boardtype.ToString());

                                BoardMatrix boardMatrix = new BoardMatrix(boardsize);

                                //放上棋子
                                String[] myXy = nextstep.Split(',');
                                if ((pointsnumber + 1) % 2 == 0)
                                {
                                    //偶数，白子
                                    boardMatrix.SetMatrixPices(int.Parse(myXy[0]), int.Parse(myXy[1]), 2);
                                }
                                else
                                {
                                    //奇数，黑子
                                    boardMatrix.SetMatrixPices(int.Parse(myXy[0]), int.Parse(myXy[1]), 1);
                                }

                                //根据BOARD_TYPE进行逆向转换
                                switch (int.Parse(boardtype))
                                {
                                    case (int)BOARD_TYPE.ANGLE_0:
                                        returnMsg.Msg = boardMatrix.ToString();
                                        returnMsg.BoardType = BOARD_TYPE.ANGLE_0;
                                        break;
                                    case (int)BOARD_TYPE.ANGLE_90:
                                        returnMsg.Msg = boardMatrix.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_R90).ToString();
                                        returnMsg.BoardType = BOARD_TYPE.ANGLE_90;
                                        break;
                                    case (int)BOARD_TYPE.ANGLE_180:
                                        returnMsg.Msg = boardMatrix.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_R180).ToString();
                                        returnMsg.BoardType = BOARD_TYPE.ANGLE_180;
                                        break;
                                    case (int)BOARD_TYPE.ANGLE_270:
                                        returnMsg.Msg = boardMatrix.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_R270).ToString();
                                        returnMsg.BoardType = BOARD_TYPE.ANGLE_270;
                                        break;
                                    case (int)BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_0:
                                        returnMsg.Msg = boardMatrix.MatrixReverseUpDown().ToString();
                                        returnMsg.BoardType = BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_0;
                                        break;
                                    case (int)BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_90:
                                        returnMsg.Msg = boardMatrix.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_R90).MatrixReverseUpDown().ToString();
                                        returnMsg.BoardType = BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_90;
                                        break;
                                    case (int)BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_180:
                                        returnMsg.Msg = boardMatrix.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_R180).MatrixReverseUpDown().ToString();
                                        returnMsg.BoardType = BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_180;
                                        break;
                                    case (int)BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_270:
                                        returnMsg.Msg = boardMatrix.MatrixTranspose(BoardMatrix.MatrixTransposeAngle.ANGLE_R270).MatrixReverseUpDown().ToString();
                                        returnMsg.BoardType = BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_270;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            returnMsg.Status = MsgStatus.Finished;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    returnMsg.Msg = "";
                }
                finally
                {
                    sqlConnection.Close();
                }
                returnMsg.Uid = uid;
                Response.Write(returnMsg.ToString());
            }
        }

        Boolean WriteQueryToDB(String boardsize, String pointsnumber, String pointString, String chessrule, String uid)
        {
            //在数据库中查询
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.AppSettings["SqlConnectionRenjun"]);
            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("", sqlConnection);
                sqlCommand.CommandText = @"INSERT INTO [RenjunCoach].[dbo].[ChessBoard]
           ([boardsize]
           ,[pointsnumber]
           ,[points]
           ,[chessrule]
           ,[UID]
           ,[status]
           ,[nextstep]) VALUES (@boardsize,@pointsnumber,@points,@chessrule,@UID,@status,@nextstep)";
                SqlParameter para1 = new SqlParameter("@boardsize", SqlDbType.Int);
                SqlParameter para2 = new SqlParameter("@pointsnumber", SqlDbType.Int);
                SqlParameter para3 = new SqlParameter("@points", SqlDbType.Text);
                SqlParameter para4 = new SqlParameter("@chessrule", SqlDbType.Int);
                SqlParameter para5 = new SqlParameter("@UID", SqlDbType.Text);
                SqlParameter para6 = new SqlParameter("@status", SqlDbType.Int);
                SqlParameter para7 = new SqlParameter("@nextstep", SqlDbType.Text);

                para1.Value = boardsize;
                para2.Value = pointsnumber;
                para3.Value = pointString;
                para4.Value = chessrule;
                para5.Value = uid;
                para6.Value = "0";
                para7.Value = "";

                sqlCommand.Parameters.Add(para1);
                sqlCommand.Parameters.Add(para2);
                sqlCommand.Parameters.Add(para3);
                sqlCommand.Parameters.Add(para4);
                sqlCommand.Parameters.Add(para5);
                sqlCommand.Parameters.Add(para6);
                sqlCommand.Parameters.Add(para7);

                sqlCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        String GetUidFromDB(String points, String boardsize, String chessrule)
        {
            //在数据库中查询
            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.AppSettings["SqlConnectionRenjun"]);
            try
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("", sqlConnection);
                sqlCommand.CommandText = "SELECT * FROM ChessBoard WHERE points LIKE '" + points + "'";
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    if (boardsize == sqlDataReader["boardsize"].ToString().Trim())
                        if (chessrule == sqlDataReader["chessrule"].ToString().Trim())
                            return sqlDataReader["UID"].ToString().Trim();
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        static String Base64Encode(String source)
        {
            byte[] inArray = System.Text.Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(inArray);
        }
        static String Base64Decode(String source)
        {
            byte[] inArray = Convert.FromBase64String(source);
            return System.Text.Encoding.UTF8.GetString(inArray);
        }

        public class ReturnMsg
        {
            //执行状态
            private MsgStatus status = 0;
            //发送的消息
            private string msg = "";
            //uid
            private String uid = "";
            //
            private BOARD_TYPE boardType;

            public string Msg { get => msg; set => msg = value; }
            public MsgStatus Status { get => status; set => status = value; }
            public string Uid { get => uid; set => uid = value; }
            public BOARD_TYPE BoardType { get => boardType; set => boardType = value; }

            public override string ToString()
            {
                String json = "";
                json = "{\"status\":\"" + (int)Status + "\",\"msg\":\"" + Msg + "\",\"uid\":\"" + uid + "\",\"boardtype\":\"" + (int)boardType + "\"}";
                return Base64Encode(json);
            }
        }

        public enum MsgStatus
        {
            //失败
            Faild = 0,
            //提交成功
            SubmitOK = 1,
            //正在计算中
            Running = 2,
            //计算完成
            Finished = 3,
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
}