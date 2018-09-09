using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RenjuCoachWebServer
{
    public class ReturnMessage
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
            JObject jObject = new JObject();
            jObject.Add("status", ((int)Status).ToString());
            jObject.Add("msg", Msg);
            jObject.Add("uid", uid);
            jObject.Add("boardtype", ((int)boardType).ToString());

            //return Base64Encode(jObject.ToString(Newtonsoft.Json.Formatting.None,null));
            return jObject.ToString(Newtonsoft.Json.Formatting.None, null);
        }

        static String Base64Encode(String source)
        {
            byte[] inArray = System.Text.Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(inArray);
        }
    }

    public enum MsgStatus
    {
        //内部失败
        FAILD = 0,
        //提交成功
        SUBMIT_OK = 1,
        //正在计算中
        RUNNING = 2,
        //计算完成
        FINISHED = 3,
        //无效棋盘
        INVALID=4,
    }    
}