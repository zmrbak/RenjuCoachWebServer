using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RenjuCoachWebServer
{
    public static class SqlHelper
    {
        public static Boolean WriteQueryToDB(String boardsize, String pointsnumber, String pointString, String chessrule, String uid)
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

        public static String GetUidFromDB(String points, String boardsize, String chessrule)
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

    }
}