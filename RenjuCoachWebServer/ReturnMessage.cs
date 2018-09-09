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
            String json = "";
            json = "{\"status\":\"" + (int)Status + "\",\"msg\":\"" + Msg + "\",\"uid\":\"" + uid + "\",\"boardtype\":\"" + (int)boardType + "\"}";
            return Base64Encode(json);
        }

        static String Base64Encode(String source)
        {
            byte[] inArray = System.Text.Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(inArray);
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