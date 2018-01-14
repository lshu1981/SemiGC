using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSSCADA
{
    public class CRC
    {
        public static byte[] CRC16Chk(byte[] data,int iLen)
        {
            byte CRC_L = 0xFF;
            byte CRC_H = 0xFF;   //CRC寄存器 
            byte SH;
            byte SL;
            byte[] temp = data;
            int j;

            for (int i = 0; i < iLen; i++)
            {
                CRC_L = (byte)(CRC_L ^ temp[i]); //每一个数据与CRC寄存器进行异或 
                for (j = 0; j < 8; j++)
                {
                    SH = (byte)(CRC_H & 0x01);
                    SL = (byte)(CRC_L & 0x01);

                    CRC_H = (byte)(CRC_H >> 1);      //高位右移一位
                    CRC_H = (byte)(CRC_H & 0x7F);
                    CRC_L = (byte)(CRC_L >> 1);      //低位右移一位 
                    CRC_L = (byte)(CRC_L & 0x7F);

                    if (SH == 0x01) //如果高位字节最后一位为1 
                    {
                        CRC_L = (byte)(CRC_L | 0x80);   //则低位字节右移后前面补1 
                    }             //否则自动补0 
                    if (SL == 0x01) //如果LSB为1，则与多项式码进行异或 
                    {
                        CRC_H = (byte)(CRC_H ^ 0xA0);
                        CRC_L = (byte)(CRC_L ^ 0x01);
                    }
                }
            }
            byte[] result = new byte[2];
            result[0] = CRC_H;       //CRC高位 
            result[1] = CRC_L;       //CRC低位 
            return result;
        }
        public static bool bCheckCRC(byte[] data, int iLen)
        {
            byte[] result = new byte[2];
            result = CRC16Chk(data,iLen -2);
            if (Math.Equals(result[0], data[iLen - 1]) && Math.Equals(result[1], data[iLen - 2]))
                return true;
            else
                return false;
        }
    }
}
