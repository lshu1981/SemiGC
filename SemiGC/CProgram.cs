using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.OleDb;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using PublicDll;

namespace SemiGC
{
    public class CFlow
    {
        public string LayName = "";
        public int LayIndex = -1;
        public int iCyc = 1;
        public bool bCyc = false;
        public int iIndex = -1;
        public int PresCyc = -1;
        public Double TimeTol = 0;//总时间秒数
        public Double TimeRun = 0;//运行时间秒数
        public Double TimeHold = 0;//保持时间秒数
        public string sTimeTol      //总时间字符串
        {
            get
            {
               return (new TimeSpan(0, 0, (int)TimeTol)).ToString("c");
            }
        }
        public string sTimeRun      //运行时间字符串
        {
            get
            {
                return (new TimeSpan(0, 0, (int)TimeRun)).ToString("c");
            }
        }
        public string sTimeRemain      //剩余时间字符串
        {
            get
            {
                return (new TimeSpan(0, 0, (int)(TimeTol - TimeRun))).ToString("c");
            }
        }
        public string sTimeHold      //保持时间字符串
        {
            get
            {
                return (new TimeSpan(0, 0, (int)TimeHold)).ToString("c");
            }
        }
        public Double TimeRunTol = 0;//已运行总时间，就是前面所有层的时间总加
        public CFlow Clone()
        {
            CFlow obj = (CFlow)this.MemberwiseClone();
            return obj;
        }
    }

    public class CRunRules
    {
        public string Key = "";
        public string Name = "";
        public string Linker = "";
        public string RecipeType = "";
        public int Value = 0;
        public int Type = 1;
        public int index = 0;
        public int indexDT = 0;
        public int indexJP = 0;
        public int indexMD = 0;
    }

    /// <summary>
    /// 配方程序
    /// </summary>
    public class CProgram
    {
        /// <summary>
        /// 配方层列表
        /// </summary>
        public List<CLayer> ListLayer = new List<CLayer>();
        /// <summary>
        /// 运行展开后的层列表
        /// </summary>
        public List<CFlow> ListFlow = new List<CFlow>();
        /// <summary>
        /// 配方的路径
        /// </summary>
        public FileInfo fileInfo = null;
        /// <summary>
        /// 配方总时间
        /// </summary>
        public double TimeTol = 0;
        /// <summary>
        /// 配方运行时间
        /// </summary>
        public double TimeRun
        {
            get
            {
                try
                {
                    return ListFlow[LayRunIndex].TimeRun + ListFlow[LayRunIndex].TimeRunTol;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 配方启动时间
        /// </summary>
        public DateTime DT_Start;
        public DateTime DT_End;
        public string UserName ="";

        public bool bSave = false;

        /// <summary>
        /// 配方内容
        /// </summary>
        public string RecipeText
        {
            get
            {
                string stmp = "";
                foreach (CLayer nLay in ListLayer)
                {
                    foreach (CLayerCell nCell in nLay.ListCell)
                    {
                        stmp += nCell.StrValue + ",";
                    }
                    stmp += "]";
                }
                return stmp;
            }
        }
        /// <summary>
        /// 配方标示
        /// </summary>
        public Guid rowguid;
        public string sTimeTol      //总时间字符串
        {
            get
            {
                return (new TimeSpan(0, 0, (int)TimeTol)).ToString("c");
            }
        }
        public string sTimeRun      //运行时间字符串
        {
            get
            {
                try
                {
                    return (new TimeSpan(0, 0, (int)TimeRun)).ToString("c");
                }
                catch (Exception ex)
                {
                    return (new TimeSpan(0, 0, 0)).ToString("c");
                }
            }
        }
        public string sTimeRemain      //剩余时间字符串
        {
            get
            {
                return (new TimeSpan(0, 0, (int)(TimeTol - TimeRun))).ToString("c");
            }
        }

        /// <summary>
        /// 当前运行层序号，从0开始计算
        /// </summary>
        public int LayRunIndex = -1;

        public List<CRunRules> RunRules = new List<CRunRules>();
        public List<CRunRules> VentRule = new List<CRunRules>();

        /// <summary>
        /// 配方名称
        /// </summary>
        public String Name
        {
            get
            { return Path.GetFileNameWithoutExtension(fileInfo.Name); }
        }

        private List<FileInfo> GetAllFile(string targetDirName, string fileSearchPattern)
        {
            List<FileInfo> fileList = new List<FileInfo>();
            DirectoryInfo dirTarget = new DirectoryInfo(targetDirName);
            foreach (DirectoryInfo subDir in dirTarget.GetDirectories())
                fileList.AddRange(GetAllFile(subDir.Name, fileSearchPattern));  // <-这句
            fileList.AddRange(dirTarget.GetFiles(fileSearchPattern));
            return fileList;
        }

        /// <summary>
        /// 配方复制
        /// </summary>
        /// <returns></returns>
        public CProgram Clone()
        {
            CProgram obj = (CProgram)this.MemberwiseClone();
            obj.ListLayer = new List<CLayer>();
            foreach (CLayer nCell in ListLayer)
            {
                obj.ListLayer.Add(nCell.Clone());
            }
            obj.ListFlow = new List<CFlow>();
            foreach (CFlow nCell in ListFlow)
            {
                obj.ListFlow.Add(nCell.Clone());
            }
            return obj;
        }

        /// <summary>
        /// 计算时间总加
        /// </summary>
        public void TimeSum()//计算时间总加
        {
            ListFlow.Clear();
            TimeTol = 0;
            foreach (CLayer nLay in ListLayer)
            {
                nLay.TimeSum();
                CFlow nF = new CFlow();
                nF.LayName = nLay.Name;
                nF.LayIndex = ListLayer.IndexOf(nLay);
                nF.iCyc = nLay.iCyc;
                nF.iIndex = nLay.iIndex;
                nF.TimeTol = nLay.TimeTol;
                for (int i = 0; i < nLay.iCyc; i++)
                {
                    CFlow nCF = nF.Clone();
                    nCF.PresCyc = i;
                    ListFlow.Insert(ListFlow.Count - (nF.iCyc - 1 - i) * (nF.iIndex - 1), nCF);

                    TimeTol += nLay.TimeTol;
                }
            }
            for (int i=1;i<ListFlow.Count;i++)
            {
                ListFlow[i].TimeRunTol = ListFlow[i - 1].TimeRunTol + ListFlow[i - 1].TimeTol;
            }
        }

        /// <summary>
        /// 配方运行
        /// </summary>
        public void Run()//配方运行
        {
            rowguid = System.Guid.NewGuid();//配方的唯一标示
            LayRunIndex = -1;//当前运行层置0
            DT_Start = DateTime.Now;
            for (int i = 0; i < ListFlow.Count; i++)
            {
                ListFlow[i].TimeRun = 0;
                ListFlow[i].TimeHold = 0;
            }
        }

        public byte[] GetSendBuffAO(int iLay, int intStart, int iLen)
        {
            byte[] DataBuffer = new byte[iLen * 2];
            if (iLay == 0)
                DataBuffer[0] = 0;
            else if (iLay == ListFlow.Count - 1)
                DataBuffer[0] = 2;
            else
                DataBuffer[0] = 1;
            DataBuffer[1] = (byte)(iLay + 1);

            foreach (CLayerCell nCell in ListLayer[ListFlow[iLay].LayIndex].ListCell)
            {
                try
                {
                    if (nCell.Name == "生长室压力（Torr）")
                    {
                        int iii = 0;
                    }
                    if (nCell.CellType == ECellType.GC_AO || nCell.CellType == ECellType.GC_AO3 || nCell.CellType == ECellType.GC_Jump || nCell.CellType == ECellType.GC_datetime)
                    {
                        for (int i = 0; i < nCell.iLinker.Count; i++)
                        {
                            int ibyte = (int)nCell.iLinker[i];
                            long iValue = (long)(nCell.lngValue[i] * nCell.dRatioValue);
                            int index = (ibyte - intStart) * 2;
                            if (index >= 0 && index < DataBuffer.Length)
                            {
                                DataBuffer[index] = (byte)(iValue >> 8);
                                DataBuffer[index + 1] = (byte)iValue;
                            }
                        }
                    }
                    else if (nCell.CellType == ECellType.GC_AO2)
                    {
                        for (int i = 0; i < nCell.iLinker.Count; i++)
                        {
                            int ibyte = (int)nCell.iLinker[i];
                            long iValue = (long)(nCell.lngValue[0] * nCell.dRatioValue);
                            DataBuffer[(ibyte - intStart) * 2] = (byte)(iValue >> 8);
                            DataBuffer[(ibyte - intStart) * 2 + 1] = (byte)iValue;
                        }
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show("GetSendBuffAO:第" + (ListFlow[iLay].LayIndex + 1).ToString() + "层:" + nCell.Name + ee.Message, "错误");
                }
            }
            return DataBuffer;
        }

        public byte[] GetSendBuffDO(int iLay, int intStart, int iLen)
        {
            byte[] DataBuffer = new byte[iLen * 2];
            int[] IndBuff = new int[iLen];
            try
            {
                for (int k = 0; k < ListLayer[ListFlow[iLay].LayIndex].strRunDefault1.Length; k++)
                {
                    string[] str1 = ListLayer[ListFlow[iLay].LayIndex].strRunDefault1[k].Split(',');
                    if (str1.Length >= 2)
                    {
                        int ibyte = Convert.ToInt32(str1[0]);
                        int ibit = Convert.ToInt32(str1[1]);
                        if (ibyte >= intStart && ibyte - intStart < iLen && ibit >= 0 && ibit < 16)
                            IndBuff[ibyte - intStart] = IndBuff[ibyte - intStart] | ((1 << ibit));
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("GetSendBuffDO:AO固定值设置出错" + ee.Message, "错误");
            }
            foreach (CLayerCell nCell in ListLayer[ListFlow[iLay].LayIndex].ListCell)
            {
                try
                {
                    if (nCell.CellType == ECellType.GC_DO || nCell.CellType == ECellType.GC_H2N2Switch || nCell.CellType == ECellType.GC_IR2 || nCell.CellType == ECellType.GC_YPIVR1 || nCell.CellType == ECellType.GC_YPIVR2)
                    {
                        for (int i = 0; i < nCell.iLinker.Count / 2; i++)
                        {
                            int ibyte = (int)nCell.iLinker[i * 2];
                            int ibit = (int)nCell.iLinker[i * 2 + 1];
                            if (ibyte >= intStart && ibyte - intStart < iLen && ibit >= 0 && ibit < 16)
                            {
                                long iValue = (long)nCell.lngValue[i];
                                if (iValue == 0)
                                {
                                    IndBuff[ibyte - intStart] = IndBuff[ibyte - intStart] & (~(1 << ibit));
                                }
                                else
                                {
                                    IndBuff[ibyte - intStart] = IndBuff[ibyte - intStart] | ((1 << ibit));
                                }
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show("GetSendBuffDO:第" + (ListFlow[iLay].LayIndex + 1).ToString() + "层:" + nCell.Name + ee.Message, "错误");
                }
            }
            for (int i = 0; i < iLen; i++)
            {
                DataBuffer[i * 2] = (byte)(IndBuff[i] >> 8);
                DataBuffer[i * 2 + 1] = (byte)IndBuff[i];
            }
            return DataBuffer;
        }

        public int[] GetSendBuffOL(int iLay)
        {
            int[] DataBuffer = new int[] { 0, 0, 0, 0, 0, iLay };
            foreach (CLayerCell nCell in ListLayer[ListFlow[iLay].LayIndex].ListCell)
            {
                try
                {
                    if (nCell.CellType == ECellType.GC_OL)
                    {
                        int ibyte = Convert.ToInt32(nCell.Linker);
                        if (ibyte >= 2 && ibyte <= 4)
                            DataBuffer[ibyte] = (int)nCell.lngValue[0];
                    }
                    else if (nCell.CellType == ECellType.GC_OLMode)
                    {
                        DataBuffer[1] = (int)nCell.lngValue[0];
                    }
                    else if (nCell.CellType == ECellType.GC_Jump)
                    {
                        DataBuffer[0] = (int)nCell.lngValue[0];
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show("第" + (ListFlow[iLay].LayIndex + 1).ToString() + "层:" + nCell.Name + ee.Message, "错误");
                }
            }
            return DataBuffer;
        }

        public void LoadRulesFromXML(string filePath)
        {
            RunRules.Clear();
            VentRule.Clear();
            // 创建XmlDocument类的实例
            XmlDocument myxmldoc = new XmlDocument();
            string sXMLPath = filePath + "\\Project\\Setting.xml";
            myxmldoc.Load(sXMLPath);

            string xpath = "root/PecipeRunRules";
            XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                CRunRules nRun = new CRunRules();
                nRun.Key = item.GetAttribute("Key");
                nRun.Name = item.GetAttribute("Name");
                nRun.Linker = item.GetAttribute("Linker");
                nRun.RecipeType = item.GetAttribute("RecipeType");
                nRun.Value = Convert.ToInt32(item.GetAttribute("Value"));
                nRun.Type = Convert.ToInt32(item.GetAttribute("Type"));
                ECellType nCType;
                if (nRun.RecipeType == "GC_AO_S")
                    nCType = ECellType.GC_AO;
                else
                    nCType = (ECellType)Enum.Parse(typeof(ECellType), nRun.RecipeType);
                for (int k = 0; k < ListLayer[0].ListCell.Count; k++)
                {
                    if (ListLayer[0].ListCell[k].CellType == ECellType.GC_datetime)
                        nRun.indexDT = k;
                    if (ListLayer[0].ListCell[k].CellType == ECellType.GC_Jump)
                        nRun.indexJP = k;
                    if (ListLayer[0].ListCell[k].CellType == ECellType.GC_OLMode)
                        nRun.indexMD = k;

                    if (nCType == ListLayer[0].ListCell[k].CellType)
                    {
                        switch (nCType)
                        {
                            case ECellType.GC_AO:
                            case ECellType.GC_OL:
                                if (nRun.Linker == ListLayer[0].ListCell[k].Linker)
                                {
                                    nRun.index = k;
                                }
                                break;
                        }
                    }
                }
                RunRules.Add(nRun);
            }

            xpath = "root/Bubbler";
            childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                CRunRules nRun = new CRunRules();
                nRun.Name = item.GetAttribute("Name");
                nRun.Linker = item.GetAttribute("LayerName");
                nRun.Value = Convert.ToInt32(item.GetAttribute("MinVentTime"));
                VentRule.Add(nRun);
            }
            xpath = "root/Dopant";
            childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            foreach (XmlElement item in childNode.ChildNodes)
            {
                CRunRules nRun = new CRunRules();
                nRun.Name = item.GetAttribute("Name");
                nRun.Linker = item.GetAttribute("LayerName");
                nRun.Value = Convert.ToInt32(item.GetAttribute("MinVentTime"));
                VentRule.Add(nRun);
            }
        }

        public bool CheckExcel()
        {
            bool bRe = true;
            foreach (CLayer nLay in ListLayer)
            {
                string str1 = "第" + (ListLayer.IndexOf(nLay) + 1).ToString() + "层\r\n";
                if (!nLay.CheckExcel(ref str1))
                {
                    if (str1.Length > 100)
                    {
                        MessageBox.Show(str1.Substring(0,10)+"\r\n......\r\n错误太多", "错误");
                        bRe = false;
                    }
                    else
                    {
                        MessageBox.Show(str1, "错误");
                        bRe = false;
                    }
                }
            }
            if (!ChcekRunRules())
                bRe = false;
            if (!CheckBubbler())
                bRe = false;
            return bRe;
        }

        public bool ChcekRunRules()
        {
            bool bRe = true;
            string message = "";
            foreach (CRunRules nRun in RunRules)
            {
                switch (nRun.RecipeType)
                {
                    case "GC_AO_S":
                        if (nRun.Type == 1)
                        {
                            for (int i = 1; i < ListLayer.Count; i++)
                            {
                                if (ListLayer[i].ListCell[nRun.indexJP].lngValue[0] == 1)//渐变
                                {
                                    double iD = Math.Abs(ListLayer[i - 1].ListCell[nRun.index].lngValue[0] - ListLayer[i].ListCell[nRun.index].lngValue[0]);
                                    double iDT =  iD / ListLayer[i].ListCell[nRun.indexDT].lngValue[0];
                                    if (iDT > nRun.Value)
                                    {
                                        message += nRun.Linker + "第" + (i + 1).ToString() + "层变化率" + iDT.ToString("f1") + ">" + nRun.Name + nRun.Value.ToString() + "\r\n";
                                        bRe = false;
                                    }
                                }
                                else
                                {
                                    double iD = Math.Abs(ListLayer[i - 1].ListCell[nRun.index].lngValue[0] - ListLayer[i].ListCell[nRun.index].lngValue[0]);
                                    if (iD > nRun.Value)
                                    {
                                        message += nRun.Linker + "第" + (i + 1).ToString() + "层跳变值" + iD.ToString("f1") + ">" + nRun.Name + nRun.Value.ToString() + "\r\n";
                                        bRe = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ListLayer.Count; i++)
                            {
                                double iD = ListLayer[i].ListCell[nRun.index].lngValue[0];
                                if (iD > nRun.Value)
                                {
                                    message += nRun.Linker + "第" + (i + 1).ToString() + "层值" + iD.ToString("f2") + ">" + nRun.Name + nRun.Value.ToString() + "\r\n";
                                    bRe = false;
                                }
                            }
                        }

                        break;
                    case "GC_AO":
                        if (nRun.Type == 1)
                        {
                            for (int i = 1; i < ListLayer.Count; i++)
                            {
                                if (ListLayer[i].ListCell[nRun.indexJP].lngValue[0] == 1)//渐变
                                {
                                    double iD = Math.Abs(ListLayer[i - 1].ListCell[nRun.index].lngValue[0] - ListLayer[i].ListCell[nRun.index].lngValue[0]);
                                    double iDT = 60 * iD / ListLayer[i].ListCell[nRun.indexDT].lngValue[0];
                                    if (iDT > nRun.Value)
                                    {
                                        message += nRun.Linker + "第" + (i + 1).ToString() + "层变化率" + iDT.ToString("f1") + ">" + nRun.Name + nRun.Value.ToString() + "\r\n";
                                        bRe = false;
                                    }
                                }
                                else
                                {
                                    double iD = Math.Abs(ListLayer[i - 1].ListCell[nRun.index].lngValue[0] - ListLayer[i].ListCell[nRun.index].lngValue[0]);
                                    if (iD > nRun.Value)
                                    {
                                        message += nRun.Linker + "第" + (i + 1).ToString() + "层跳变值" + iD.ToString("f1") + ">" + nRun.Name + nRun.Value.ToString() + "\r\n";
                                        bRe = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ListLayer.Count; i++)
                            {
                                double iD = ListLayer[i].ListCell[nRun.index].lngValue[0];
                                if (iD > nRun.Value)
                                {
                                    message += nRun.Linker + "第" + (i + 1).ToString() + "层值" + iD.ToString("f2") + ">" + nRun.Name + nRun.Value.ToString() + "\r\n";
                                    bRe = false; 
                                }
                            }
                        }
                        
                        break;
                    case "GC_OLMode":
                        for (int i = 1; i < ListLayer.Count; i++)
                        {
                            if (ListLayer[i].ListCell[nRun.indexMD].lngValue[0] == 1)
                                continue;
                            if (ListLayer[i-1].ListCell[nRun.indexMD].lngValue[0] == 1)
                                continue;
                            if (nRun.Linker == "Ramp")//最大温度变化率
                            {
                                if (ListLayer[i].ListCell[nRun.indexJP].lngValue[0] == 0)
                                    continue;
                                for (int j = 1; j <= 3; j++)
                                {
                                    double iD = Math.Abs(ListLayer[i - 1].ListCell[nRun.indexMD + j].lngValue[0] - ListLayer[i].ListCell[nRun.indexMD + j].lngValue[0]);
                                    double iDT = 60 * iD / ListLayer[i].ListCell[nRun.indexDT].lngValue[0];
                                    if (iDT > nRun.Value)
                                    {
                                        message += ListLayer[i].ListCell[nRun.indexMD + j].Name + "第" + (i + 1).ToString() + "层变化率" + iDT.ToString("f1") + ">" + nRun.Name + nRun.Value.ToString() + "\r\n";
                                        bRe = false;
                                    }
                                }
                            }
                            else if (nRun.Linker == "Jump")//最大温度跳变范围
                            {
                                if (ListLayer[i].ListCell[nRun.indexJP].lngValue[0] == 1)
                                    continue;
                                for (int j = 1; j <= 3; j++)
                                {
                                    double iD = Math.Abs(ListLayer[i - 1].ListCell[nRun.indexMD + j].lngValue[0] - ListLayer[i].ListCell[nRun.indexMD + j].lngValue[0]);
                                    if (iD > nRun.Value)
                                    {
                                        message += ListLayer[i].ListCell[nRun.indexMD + j].Name + "第" + (i + 1).ToString() + "层跳变值" + iD.ToString("f1") + ">" + nRun.Name + nRun.Value.ToString() + "\r\n";
                                        bRe = false;
                                    }
                                }
                            }
                        }
                        break;
                    case "GC_OL":
                        for (int i = 1; i < ListLayer.Count; i++)
                        {
                            if (ListLayer[i].ListCell[nRun.indexJP].lngValue[0] == 0)
                                continue;
                            if (ListLayer[i].ListCell[nRun.indexMD].lngValue[0] == 0)
                                continue;
                            if (ListLayer[i-1].ListCell[nRun.indexMD].lngValue[0] == 0)
                                continue;
                            double iD = Math.Abs(ListLayer[i - 1].ListCell[nRun.index].lngValue[0] - ListLayer[i].ListCell[nRun.index].lngValue[0]);
                            double iDT = iD / ListLayer[i].ListCell[nRun.indexDT].lngValue[0];
                            if (iDT > nRun.Value)
                            {
                                message += ListLayer[i].ListCell[nRun.index].Name + "第" + (i + 1).ToString() + "层变化率" + iDT.ToString("f2") + ">" + nRun.Name + nRun.Value.ToString() + "\r\n";
                                bRe = false;
                            }
                        }
                        break;
                }
            }
            if (!bRe)
            {
                MessageBox.Show(message, "配方运行规则");
            }
            return bRe;
        }

        public bool CheckBubbler()
        {
            bool bRe = true;
            string message = "";
            foreach (CRunRules nRun in VentRule)
            {
                for (int k = 0; k < ListLayer[0].ListCell.Count; k++)
                {
                    if (nRun.Linker == ListLayer[0].ListCell[k].Name)
                    {
                        double iVent = 0;
                        bool bVent = false;
                        for (int i = 0; i < ListLayer.Count; i++)
                        {
                            if (ListLayer[i].ListCell[k].StrValue == "V")
                            {
                                iVent += ListLayer[i].ListCell[2].lngValue[0];
                                bVent = true;
                            }
                            else
                            {
                                if (bVent)
                                {
                                    if (iVent < nRun.Value && ListLayer[i].ListCell[k].StrValue == "R")
                                    {
                                        message += nRun.Linker + "第" + i.ToString() + "层Vent时间" + iVent.ToString() + "s<最少Vent时间" + nRun.Value.ToString() + "s\r\n";
                                        bRe = false;
                                    }
                                }
                                bVent = false;
                                iVent = 0;
                            }
                        }
                        break;
                    }
                }
            }
            if (!bRe)
            {
                MessageBox.Show(message, "配方运行规则");
            }
            return bRe;
        }

        /// <summary> 导出当前页DataGridView中的数据到EXcel中 
        /// 导出当前页DataGridView中的数据到EXcel中 
        /// </summary> 
        /// <param name="strName">strName</param> 
        public bool ExportTOExcel(string strName)
        {
            if (ListLayer.Count == 0)
            {
                MessageBox.Show("没有数据可供导出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (strName.Length != 0)
                return false;
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                System.Reflection.Missing miss = System.Reflection.Missing.Value;

                excel.Visible = false;//若是true，则在导出的时候会显示EXcel界面。 
                excel.AlertBeforeOverwriting = false;//关闭修改之后是否保存

                if (excel == null)
                {
                    MessageBox.Show("EXCEL无法启动！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                Workbook book;
                if (File.Exists(strName))
                    book = excel.Workbooks.Open(strName, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss);//打开Excel
                else
                    book = excel.Workbooks.Add(miss);//打开Excel
                Worksheet sheet = book.Sheets[1] as Worksheet;//得到Excel的第一个sheet
                sheet.Name = "Sheet1";
                //生成Excel中列头名称
                excel.Cells[1, 1] = "管理层";
                for (int i = 0; i < ListLayer.Count; i++)
                {
                    excel.Cells[1, i + 2] = (i + 1).ToString();
                }

                //把DataGridView当前页的数据保存在Excel中
                for (int i = 0; i < ListLayer[0].ListCell.Count; i++)
                {
                    string str1 = CStrPublicFun.Get2StrTo1(ListLayer[0].ListCell[i].Name, ListLayer[0].ListCell[i].Linker, 40, ' ');
                    excel.Cells[i + 2, 1] = str1.Trim();

                    for (int j = 0; j < ListLayer.Count; j++)
                    {
                        excel.Cells[i + 2, j + 2] = ListLayer[j].ListCell[i].StrValue;
                    }
                }

                for (int i = 0; i < ListLayer[0].ListIndex.Count - 1; i++)
                {
                    CExcel.CellsBackColor(excel, ListLayer[0].ListIndex[i] + 2, 1, ListLayer[0].ListIndex[i] + 2, ListLayer.Count + 1, Color.Gold);
                    CExcel.CellsBackColor(excel, ListLayer[0].ListIndex[i] + 3, 1, ListLayer[0].ListIndex[i + 1] + 1, ListLayer.Count + 1, CExcel.BlackColor[i]);
                }

                CExcel.ColumnAutoFit(excel, 1, ListLayer.Count + 1);
                CExcel.CellsLineStyle(excel, 1, 1, ListLayer[0].ListIndex[5] + 1, ListLayer.Count + 1);
                CExcel.CellsAlignment(excel, 1, 1, ListLayer[0].ListIndex[5] + 1, 1, Constants.xlRight, Constants.xlCenter);
                CExcel.CellsAlignment(excel, 1, 2, ListLayer[0].ListIndex[5] + 1, ListLayer.Count + 1, Constants.xlCenter, Constants.xlCenter);
                if (File.Exists(strName))
                    book.Save();
                else
                    book.SaveAs(strName, miss, miss, miss, miss, miss, XlSaveAsAccessMode.xlShared, miss, miss, miss, miss, miss);

                excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                excel.Quit();
                MessageBox.Show(ex.Message, "警告");
                return false;
            }
        }

        /// <summary> 导出当前页DataGridView中的数据到EXcel中 
        /// 导出当前页DataGridView中的数据到EXcel中 
        /// </summary> 
        /// <param name="strName">strName</param> 
        public bool ExportTOExcel2(string strName,ref string sMsg)
        {
            if (ListLayer.Count == 0)
            {
                sMsg = "没有数据可供导出！";
                return false;
            }
            if (strName.Length == 0)
            {
                sMsg = "文件路径["+strName+"]不对！";
                return false;
            }
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                System.Reflection.Missing miss = System.Reflection.Missing.Value;

                excel.Visible = false;//若是true，则在导出的时候会显示EXcel界面。 
                excel.AlertBeforeOverwriting = false;//关闭修改之后是否保存

                if (excel == null)
                {
                    sMsg = "EXCEL无法启动！";
                    return false;
                }
                Workbook book;
                if (File.Exists(strName))
                    book = excel.Workbooks.Open(strName, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss, miss);//打开Excel
                else
                    book = excel.Workbooks.Add(miss);//打开Excel
                Worksheet sheet = book.Sheets[1] as Worksheet;//得到Excel的第一个sheet
                sheet.Name = "Sheet1";
                //生成Excel中列头名称
                excel.Cells[1, 1] = "管理层";
                for (int i = 0; i < ListLayer.Count; i++)
                {
                    excel.Cells[1, i + 2] = (i + 1).ToString();
                }

                //把DataGridView当前页的数据保存在Excel中
                for (int i = 0; i < ListLayer[0].ListCell.Count; i++)
                {
                    //string str1 = CStrPublicFun.Get2StrTo1(ListLayer[0].ListCell[i].Name, ListLayer[0].ListCell[i].Linker, 40, ' ');
                    //excel.Cells[i + 2, 1] = str1.Trim();

                    for (int j = 0; j < ListLayer.Count; j++)
                    {
                        excel.Cells[i + 2, j + 2] = ListLayer[j].ListCell[i].StrValue;
                    }
                }

 /*               for (int i = 0; i < ListLayer[0].ListIndex.Count - 1; i++)
                {
                    CExcel.CellsBackColor(excel, ListLayer[0].ListIndex[i] + 2, 1, ListLayer[0].ListIndex[i] + 2, ListLayer.Count + 1, Color.Gold);
                    CExcel.CellsBackColor(excel, ListLayer[0].ListIndex[i] + 3, 1, ListLayer[0].ListIndex[i + 1] + 1, ListLayer.Count + 1, CExcel.BlackColor[i]);
                }
                */
               // CExcel.ColumnAutoFit(excel, 1, ListLayer.Count + 1);
              //  CExcel.CellsLineStyle(excel, 1, 1, ListLayer[0].ListIndex[5] + 1, ListLayer.Count + 1);
              //  CExcel.CellsAlignment(excel, 1, 1, ListLayer[0].ListIndex[5] + 1, 1, Constants.xlRight, Constants.xlCenter);
              //  CExcel.CellsAlignment(excel, 1, 2, ListLayer[0].ListIndex[5] + 1, ListLayer.Count + 1, Constants.xlCenter, Constants.xlCenter);
                if (File.Exists(strName))
                    book.Save();
                else
                    book.SaveAs(strName, miss, miss, miss, miss, miss, XlSaveAsAccessMode.xlShared, miss, miss, miss, miss, miss);

                excel.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                GC.Collect();
                return true;
            }
            catch (Exception ex)
            {
                excel.Quit();
                sMsg = ex.Message;
                return false;
            }
        }
    }

    public class CSubProgram
    {
        public string Name = "";
        public string Desc = "";
        public string[] LayerList;
        [Browsable(true), Description("层列表"), Category("Design"), DisplayName("层列表")]
        public string sLayerList
        {
            set 
            {
                value = value.TrimEnd(',');
                LayerList = value.Split(',');
            }
            get
            {
                string str1 = "";
                for (int i = 0; i < LayerList.Length; i++)
                {
                    str1 += LayerList[i] + ",";
                }
                return str1;
            }
        }

        /// <summary>从xml中读取现有的层
        /// 从xml中读取现有的层
        /// </summary>
        /// <param name="node"></param>
        public void LoadFromXML(XmlElement node)
        {
            try
            {
                if (node.HasAttribute("Name"))
                {
                    Name = node.GetAttribute("Name");
                }
                if (node.HasAttribute("Desc"))
                {
                    Desc = node.GetAttribute("Desc");
                }
                if (node.HasAttribute("sLayerList"))
                {
                    sLayerList = node.GetAttribute("sLayerList");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CSubProgram.LoadFromExcel:" + ex.Message);
            }
        }
    }
}
