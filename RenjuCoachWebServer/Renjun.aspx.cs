﻿using System;
using System.IO;

namespace RenjuCoachWebServer
{
    public partial class RenjunCalculate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
            {
                //读出客户端POST来的的数据(BASE64)
                Stream postData = Request.InputStream;
                StreamReader sr = new StreamReader(postData);
                string postedString = sr.ReadToEnd();
                sr.Close();

                Response.Write(CalculatePost.RenJuPostString(postedString));
            }
            else
            {
                String uid = Request.QueryString["uid"];
                String boardtype = Request.QueryString["boardtype"];

                Response.Write(CalculateGet.RenJuGetString(uid, boardtype));
            }
        }
    }
}