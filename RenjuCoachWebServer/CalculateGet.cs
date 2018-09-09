using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static RenjuCoachWebServer.RenjunCalculate;

namespace RenjuCoachWebServer
{
    public static class CalculateGet
    {
        public static String RenJuGetString(String uid, String boardtype)
        {
            ReturnMessage returnMsg = new ReturnMessage();

            if (uid == null || uid.Trim() == "")
            {
                returnMsg.Msg = "参数有误！";
                returnMsg.Status = MsgStatus.FAILD;
                return returnMsg.ToString();
            }

            if (boardtype == null || boardtype.Trim() == "")
            {
                returnMsg.Msg = "参数有误！";
                returnMsg.Uid = uid;
                returnMsg.Status = MsgStatus.FAILD;
                return returnMsg.ToString();
            }
            returnMsg.Uid = uid;
            returnMsg.BoardType = (BOARD_TYPE)(int.Parse(boardtype));

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
                    //String status= sqlDataReader["status"].ToString().Trim();
                    String nextstep = sqlDataReader["nextstep"].ToString().Trim();
                    if (nextstep == "")
                    {
                        //如果没有结果，则返回正在运行...
                        returnMsg.Status = MsgStatus.RUNNING;
                        returnMsg.Msg = "正在计算中...";
                        return returnMsg.ToString();
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
                            BoardMatrix boardMatrix = new BoardMatrix(boardsize);

                            //把这一颗棋子放在棋盘上
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
                                    returnMsg.Msg = boardMatrix.MatrixTranspose(MatrixTransposeAngle.ANGLE_R90).ToString();
                                    returnMsg.BoardType = BOARD_TYPE.ANGLE_90;
                                    break;
                                case (int)BOARD_TYPE.ANGLE_180:
                                    returnMsg.Msg = boardMatrix.MatrixTranspose(MatrixTransposeAngle.ANGLE_R180).ToString();
                                    returnMsg.BoardType = BOARD_TYPE.ANGLE_180;
                                    break;
                                case (int)BOARD_TYPE.ANGLE_270:
                                    returnMsg.Msg = boardMatrix.MatrixTranspose(MatrixTransposeAngle.ANGLE_R270).ToString();
                                    returnMsg.BoardType = BOARD_TYPE.ANGLE_270;
                                    break;
                                case (int)BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_0:
                                    returnMsg.Msg = boardMatrix.MatrixReverseUpDown().ToString();
                                    returnMsg.BoardType = BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_0;
                                    break;
                                case (int)BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_90:
                                    returnMsg.Msg = boardMatrix.MatrixTranspose(MatrixTransposeAngle.ANGLE_R90).MatrixReverseUpDown().ToString();
                                    returnMsg.BoardType = BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_90;
                                    break;
                                case (int)BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_180:
                                    returnMsg.Msg = boardMatrix.MatrixTranspose(MatrixTransposeAngle.ANGLE_R180).MatrixReverseUpDown().ToString();
                                    returnMsg.BoardType = BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_180;
                                    break;
                                case (int)BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_270:
                                    returnMsg.Msg = boardMatrix.MatrixTranspose(MatrixTransposeAngle.ANGLE_R270).MatrixReverseUpDown().ToString();
                                    returnMsg.BoardType = BOARD_TYPE.ANGLE_0_REVERSE_UP_DOWN_ANGLE_270;
                                    break;
                                default:
                                    break;
                            }
                        }
                        returnMsg.Status = MsgStatus.FINISHED;
                    }
                    return returnMsg.ToString();
                }
                else
                {
                    returnMsg.Status = MsgStatus.FAILD;
                    returnMsg.Msg = "无效UID...";
                    return returnMsg.ToString();
                }
            }
            catch (Exception ex)
            {
                returnMsg.Msg = ex.Message;
                returnMsg.Status = MsgStatus.FAILD;
                return returnMsg.ToString();
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}