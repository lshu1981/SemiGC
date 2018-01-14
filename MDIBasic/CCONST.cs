using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSSCADA
{
    public class CCONST
    {
        //通信
        public const int MaxBuffLen = 4096;     //串口接收缓冲区大小
        public const int MAX_SEND_TIMES = 3;    //启动时所有报文循环发送次数
        
        //报警消息存储条数
        public const int ListMax = 1000;

        //变量历史值保存个数
        public const int VarNumMax = 600;
    }
}
