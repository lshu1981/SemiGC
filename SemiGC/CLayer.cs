using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Xml;

namespace SemiGC
{
    public enum ECellType
    {
        GC_head=-1,
        GC_AO=0,
        GC_AO2,
        GC_AO3,
        GC_datetime,
        GC_DO,
        GC_H2N2Switch,
        GC_IR2,
        GC_Jump,
        GC_Null,
        GC_string,
        GC_YPIVR1,
        GC_YPIVR2,
        GC_OL,
        GC_OLMode
    }
    public class CLayerStatic
    {
        public static List<string> SCellType = new List<string> { "浮点数", "整数", "字符串", "时间值" };
        public static ECellType SDescToECellType(string str1)
        {
            try
            {
                int i = SCellType.IndexOf(str1);
                return (ECellType)i;
            }
            catch(Exception ex)
            {
                return ECellType.GC_head;
            }
        }
        public static bool ObjectEquel(object obj1, object obj2)
        {
            Type type1 = obj1.GetType();
            Type type2 = obj2.GetType();

            System.Reflection.PropertyInfo[] properties1 = type1.GetProperties();
            System.Reflection.PropertyInfo[] properties2 = type2.GetProperties();

            bool IsMatch = true;
            for (int i = 0; i < properties1.Length; i++)
            {
                string s = properties1[i].DeclaringType.Name;
                if (properties1[i].GetValue(obj1, null).ToString() != properties2[i].GetValue(obj2, null).ToString())
                {
                    IsMatch = false;
                    break;
                }
            }

            return IsMatch;
        }
    }

    public class CLayerCell
    {
        public int ID;    //行数
        public string Name;
        public ECellType CellType;  //控制项格式

        public string Linker;//关联到变量
        public List<int> iLinker = new List<int>();
        public List<int> iCapacity = new List<int>();
        public string ValueType ="INT";
        public string RatioValue = "1";//变比
        public double dRatioValue//变比
        {
            get
            {
                try
                {
                    return Convert.ToDouble(RatioValue);
                }
                catch (Exception ex)
                {
                    return 1;
                }
            }
        }
        public string sRange = ""; //取值范围
        public Color BackColor = Color.LightGray;

        private string strValue = "";  //缺省值
        private TimeSpan timValue = TimeSpan.Parse("00:00:00");
        private bool bErr = false;
        private string sErr = "";
        public bool bVisible = true;//是否隐藏

        public int iHeadFirst = 0;
        public int iHeadLast = 0;
        public string sSubProgram = ""; //子程序名称
        public bool bSave = false;      //层是否需要保存

        [Browsable(true), Description("值"), Category("Design"), DisplayName("值")]
        public String StrValue
        {
            get
            {
                if (bErr)
                    return sErr;
                try
                {
                    switch (CellType)
                    {
                        case ECellType.GC_datetime:
                            return timValue.ToString("c");
                        default:
                            return strValue;
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            set
            {
                bErr = false;
                try
                {
                    switch (CellType)
                    {
                        case ECellType.GC_datetime:
                            value.TrimStart('\'');
                            if (value == "")
                                timValue = new TimeSpan(0, 0, 0);
                            else
                                timValue = TimeSpan.Parse(value);
                            break;
                        case ECellType.GC_head:
                        case ECellType.GC_string:
                            strValue = value;
                            break;
                        default:
                            strValue = value.ToUpper();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    bErr = true;
                    sErr = value + ":" + ex.Message;
                }
            }
        }

        public double[] lngValue
        {
            get
            {
                double[] lngV;
                if (bErr)
                    return new double[] { 0 };
                try
                {
                    switch (CellType)
                    {
                        case ECellType.GC_datetime:
                            lngV = new double[1];
                            lngV[0] = (long)timValue.TotalSeconds;
                            break;
                        case ECellType.GC_AO:
                        case ECellType.GC_AO2:
                        case ECellType.GC_AO3:
                        case ECellType.GC_OL:
                        case ECellType.GC_OLMode:
                        case ECellType.GC_Jump:
                            string str2 = strValue.Replace(';', ',');
                            string[] str1 = str2.Split(',');
                            lngV = new double[str1.Length];
                            for (int i = 0; i < str1.Length; i++)
                            {
                                lngV[i] = Convert.ToDouble(str1[i]);
                            }
                            break;
                        case ECellType.GC_DO:
                            lngV = new double[1] { 0 };
                            if (strValue == "R")
                                lngV[0] = 1;
                            break;
                        case ECellType.GC_H2N2Switch:
                            lngV = new double[2] { 0, 0 };
                            if (strValue == "H2")
                                lngV[0] = 1;
                            else
                                lngV[1] = 1;
                            break;
                        case ECellType.GC_IR2:
                            lngV = new double[2] { 0, 0 };
                            if (strValue == "R")
                                lngV[0] = 1;
                            else
                                lngV[1] = 1;
                            break;
                        case ECellType.GC_YPIVR1:
                            lngV = new double[4] { 0, 0, 0, 0 };
                            if (strValue == "R")
                                lngV = new double[4] { 1, 1, 1, 0 };
                            else if (strValue == "I")
                                lngV = new double[4] { 0, 0, 0, 1 };
                            else if (strValue == "V")
                                lngV = new double[4] { 1, 1, 0, 1 };
                            break;
                        case ECellType.GC_YPIVR2:
                            lngV = new double[4] { 0, 0, 0, 0 };
                            if (strValue == "R")
                                lngV = new double[4] { 0, 1, 1, 0 };
                            else if (strValue == "I")
                                lngV = new double[4] { 1, 0, 0, 1 };
                            else if (strValue == "V")
                                lngV = new double[4] { 0, 1, 0, 1 };
                            break;
                        default:
                            lngV = new double[] { 0 };
                            break;
                    }
                    return lngV;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public CLayerCell(ECellType eType, string _name, int _ID)
        {
            CellType = eType;
            Name = _name;
            ID = _ID;
        }
        public CLayerCell(string eType, string _name, int _ID)
        {
            CellType = (ECellType)Enum.Parse(typeof(ECellType), eType);
            Name = _name;
            ID = _ID;
        }
        public CLayerCell()
        {
            CellType = ECellType.GC_string;
        }

        public CLayerCell Clone()
        {
            CLayerCell obj = (CLayerCell)this.MemberwiseClone();
            return obj;
        }

        public bool CheckExcel(string sNew, ref string message)
        {
              bool bRe = true;
            sNew = sNew.ToUpper();
            try
            {
                message = Name + ":值=(" + sNew + ") ";
                switch (CellType)
                {
                    case ECellType.GC_datetime:
                        sNew.TrimStart('\'');
                        TimeSpan DT = TimeSpan.Parse(sNew);

                        if ((float)DT.TotalSeconds > 43200)
                        {
                            message += "超过最大值";
                            bRe = false;
                        }
                        if ((float)DT.TotalSeconds < 0)
                        {
                            message += "少于最小值";
                            bRe = false;
                        }
                        if (bRe)
                            message = DT.ToString("c");
                        break;
                    case ECellType.GC_AO:
                    case ECellType.GC_AO2:
                        if (ValueType == "INT")
                        {
                            long[] lngV = new long[1] { 0 };
                            lngV[0] = Convert.ToInt64(sNew);
                            if (lngV[0] > iCapacity[0])
                            {
                                message += "超出容量" + iCapacity[0].ToString();
                                bRe = false;
                            }
                            else if (lngV[0] < 0)
                            {
                                message += "不能是负值";
                                bRe = false;
                            }
                        }
                        else
                        {
                            double[] lngV = new double[1] { 0 };
                            lngV[0] = Convert.ToDouble(sNew);
                            if (lngV[0] > iCapacity[0])
                            {
                                message += "超出容量" + iCapacity[0].ToString();
                                bRe = false;
                            }
                            else if (lngV[0] < 0)
                            {
                                message += "不能是负值";
                                bRe = false;
                            }
                        }
                        break;
                    case ECellType.GC_AO3:
                        string str2 = sNew.Replace(';', ',');
                        string[] str1 = str2.Split(',');
                        if (str1.Length < iLinker.Count)
                        {
                            message += "数据量少;";
                            bRe = false;
                        }
                        if (ValueType == "INT")
                        {
                            long[] lngV = new long[str1.Length];
                            for (int i = 0; i < Math.Min(str1.Length, iLinker.Count); i++)
                            {
                                lngV[i] = Convert.ToInt64(str1[i]);
                                if (lngV[i] > iCapacity[i])
                                {
                                    message += "值" + i.ToString() + "超出容量" + iCapacity[i].ToString();
                                    bRe = false;
                                }
                                else if (lngV[i] < 0)
                                {
                                    message += "不能是负值";
                                    bRe = false;
                                }

                            }
                        }
                        else
                        {
                            double[] lngV = new double[str1.Length];
                            for (int i = 0; i < Math.Min(str1.Length, iLinker.Count); i++)
                            {
                                lngV[i] = Convert.ToDouble(str1[i]);
                                if (lngV[i] > iCapacity[i])
                                {
                                    message += "值" + i.ToString() + "超出容量" + iCapacity[i].ToString();
                                    bRe = false;
                                }
                                else if (lngV[i] < 0)
                                {
                                    message += "不能是负值";
                                    bRe = false;
                                }
                            }
                        }
                        break;
                    case ECellType.GC_DO:
                    case ECellType.GC_H2N2Switch:
                    case ECellType.GC_IR2:
                    case ECellType.GC_YPIVR1:
                    case ECellType.GC_YPIVR2:
                        str1 = sRange.Split(',');
                        for (int i = 0; i < str1.Length; i++)
                        {
                            if (str1[i] == sNew)
                                return true;
                        }
                        message += "非预期的值:" + sRange;
                        bRe = false;
                        break;
                    case ECellType.GC_Jump:
                    case ECellType.GC_OLMode:
                        long[] lngVMode = new long[1] { 0 };
                        lngVMode[0] = Convert.ToInt64(sNew);
                        if (lngVMode[0] != 0 && lngVMode[0] != 1)
                        {
                            message += "非预期的值:0,1";
                            bRe = false;
                        }
                        break;
                    case ECellType.GC_OL:
                        if (ValueType == "INT")
                        {
                            long[] lngV = new long[1] { 0 };
                            lngV[0] = Convert.ToInt64(sNew);
                            if (lngV[0] < 0)
                            {
                                message += "不能是负值";
                                bRe = false;
                            }
                        }
                        else
                        {
                            double[] lngV = new double[1] { 0 };
                            lngV[0] = Convert.ToDouble(sNew);
                            if (lngV[0] < 0)
                            {
                                message += "不能是负值";
                                bRe = false;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                message += ex.Message;
                bRe = false;
            }
            return bRe;
        }
    }

    public class CLayer
    {
        public List<CLayerCell> ListCell = new List<CLayerCell>();
        public List<int> ListIndex = new List<int> ();
        string _Name = "";
        public int iCyc
        {
            get
            {
                try
                {
                    if (ListCell[0].StrValue != "")
                    {
                        string str2 = ListCell[0].StrValue.Replace(';', ',');
                        str2 = str2.Replace(':', ',');
                        str2 = str2.Replace('：', ',');
                        str2 = str2.Replace('；', ',');
                        str2 = str2.Replace('，', ',');
                        str2 = str2.Replace(' ', ',');
                        string[] str1 = str2.Split(',');
                        return int.Parse(str1[0]);
                    }
                    else
                    {
                        return 1;
                    }
                }
                catch(Exception ex)
                {
                    return 1;
                }
            }
        }
        public int iIndex
        {
            get
            {
                try
                {
                    if (ListCell[0].StrValue != "")
                    {
                        string str2 = ListCell[0].StrValue.Replace(';', ',');
                        str2 = str2.Replace(':', ',');
                        str2 = str2.Replace('：', ',');
                        str2 = str2.Replace('；', ',');
                        str2 = str2.Replace('，', ',');
                        str2 = str2.Replace(' ', ',');
                        string[] str1 = str2.Split(',');
                        return int.Parse(str1[1]);
                    }
                    else
                    {
                        return -11;
                    }
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }
        public double TimeTol = 0;
        public DateTime TimeStart ;
        public double TimeRun = 0;
        public String[] strRunDefault1;//固定的DO列表
        [Browsable(true), Description("名称"), Category("Design"), DisplayName("名称")]
        public String Name
        {
            get
            {
                if (_Name == "")
                {
                    return ListCell[1].StrValue;  
                }
                return _Name; 
            }
            set
            {
                _Name = value;
            }
        }
        public String RunDefault1//固定的DO列表
        {
            set
            {
                strRunDefault1 = value.Split(';');
            }
        }
        /// <summary>初始化层控制项
        /// 初始化层控制项
        /// </summary>

        public void InitFromXml(string filePath)
        {
            try
            {
                ListCell.Clear();
                ListIndex.Clear();
                XmlDocument myxmldoc = new XmlDocument();
                string filePath1 = filePath + @"\Project\Layer.xml";

                myxmldoc.Load(filePath1);

                string xpath = "root/ListCell";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                RunDefault1 = childNode.GetAttribute("RunDefault1");

                xpath = "root/ListCell/row";
                XmlNodeList mynodes = myxmldoc.SelectNodes(xpath);
                foreach (XmlElement node in mynodes)
                {
                    int sAtt = Convert.ToInt32(node.GetAttribute("ID"));
                    string sType = node.GetAttribute("RecipeType");
                    string sName = node.GetAttribute("Name");
                    CLayerCell nCell = new CLayerCell(sType, sName, sAtt);
                    nCell.Linker = node.GetAttribute("Linker");
                    nCell.StrValue = node.GetAttribute("InitialValue");
                    nCell.ValueType = node.GetAttribute("ValueType");
                    nCell.RatioValue = node.GetAttribute("RatioValue");
                    nCell.sRange = node.GetAttribute("sRange");

                    ListCell.Add(nCell);
                    if (nCell.CellType == ECellType.GC_head)
                    {
                        ListIndex.Add(ListCell.IndexOf(nCell));
                    }

                    //if (nCell.CellType == ECellType.GC_AO || nCell.CellType == ECellType.GC_AO2 || nCell.CellType == ECellType.GC_AO3)
                    //{
                    //    nCell.StrValue = "";
                    //}

                }
                ListIndex.Add(ListCell.Count);
                for (int i = 0; i < ListIndex.Count - 1; i++)
                {
                    ListCell[ListIndex[i]].iHeadFirst = ListIndex[i] + 1;
                    ListCell[ListIndex[i]].iHeadLast = ListIndex[i + 1] - 1;
                }

                //读取PortInf_Table，获取对应的地址和容量
                filePath1 = filePath + @"\Project\IO\PortInf_Table.xml";
                myxmldoc.Load(filePath1);
                xpath = "IO/VariableInf_Table";
                childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);

                foreach (CLayerCell nCell in ListCell)
                {
                    nCell.iLinker.Clear();
                    nCell.iCapacity.Clear();
                    switch (nCell.CellType)
                    {
                        case ECellType.GC_AO:
                        case ECellType.GC_AO2:
                        case ECellType.GC_AO3:
                            string[] sAB1 = nCell.Linker.Split(',');
                            for (int i = 0; i < sAB1.Length; i++)
                            {
                                int k = 0;
                                foreach (XmlElement item in childNode.ChildNodes)
                                {
                                    string sVarName = item.GetAttribute("Name");
                                    if (sAB1[i] == item.GetAttribute("Name") && item.GetAttribute("Driver") == "NJ301")
                                    {
                                        nCell.iLinker.Add(Convert.ToInt32(item.GetAttribute("ByteAddr")) + 5000);
                                        nCell.iCapacity.Add(Convert.ToInt32(item.GetAttribute("IntTag9")));
                                        k++;
                                        break;
                                    }
                                }
                                if (k == 0)
                                {
                                    nCell.iLinker.Add(0);
                                    nCell.iCapacity.Add(0);
                                }
                            }
                            break;
                        case ECellType.GC_DO:
                        case ECellType.GC_YPIVR1:
                        case ECellType.GC_YPIVR2:
                        case ECellType.GC_H2N2Switch:
                        case ECellType.GC_IR2:
                            string[] sAB2 = nCell.Linker.Split(',');
                            for (int i = 0; i < sAB2.Length; i++)
                            {
                                int k = 0;
                                foreach (XmlElement item in childNode.ChildNodes)
                                {
                                    string sVarName = item.GetAttribute("Name");
                                    if (sAB2[i] == item.GetAttribute("Name") && item.GetAttribute("Driver") == "NJ301")
                                    {
                                        nCell.iLinker.Add(Convert.ToInt32(item.GetAttribute("ByteAddr")) - 1000);
                                        nCell.iLinker.Add(Convert.ToInt32(item.GetAttribute("BitAddr")));
                                        k++;
                                        break;
                                    }
                                }
                                if (k == 0)
                                {
                                    nCell.iLinker.Add(-1000);
                                    nCell.iLinker.Add(0);
                                }
                            }
                            break;
                        case ECellType.GC_datetime:
                            nCell.iLinker.Add(6052);
                            break;
                        case ECellType.GC_Jump:
                            nCell.iLinker.Add(6053);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CLayer.InitFromXml:" + ex.Message);
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
                foreach (CLayerCell nCell in ListCell)
                {
                    if (node.HasAttribute("ID" + nCell.ID.ToString("D3")))
                    {
                        nCell.StrValue = node.GetAttribute("ID" + nCell.ID.ToString("D3"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CLayer.LoadFromExcel:" + ex.Message);
            }

        }
        /// <summary>层复制
        /// 层复制
        /// </summary>
        /// <returns></returns>
        public CLayer Clone()
        {
            CLayer obj = (CLayer)this.MemberwiseClone();
            obj.ListCell = new List<CLayerCell>();
            foreach (CLayerCell nCell in ListCell)
            {
                obj.ListCell.Add(nCell.Clone());
            }
            return obj;
        }

        public bool CheckExcel(ref string message)
        {
            bool bRe = true;
            foreach (CLayerCell nCell in ListCell)
            {
                string str1 = "";
                if (!nCell.CheckExcel(nCell.StrValue, ref str1))
                {
                    message += str1 + "\r\n";
                    bRe = false;
                }
            }
            return bRe;
        }

        public void Clear()
        {
            foreach (CLayerCell nCell in ListCell)
            {
                nCell.StrValue = "";
            }
            _Name = "";
        }
        
        public void SaveToXML()//保存层到xml中
        {
            if (ListCell[1].StrValue == "" || ListCell[1].StrValue == string.Empty)
            {
                MessageBox.Show("层名称为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string filePath =frmRecipe. sAppPath + @"\Project\Layer.xml";
            XmlDocument myxmldoc = new XmlDocument();
            myxmldoc.Load(filePath);

            string xpath = "root/LayerList/Layer";
            XmlNodeList mynodes = myxmldoc.SelectNodes(xpath);
            foreach (XmlElement node in mynodes)
            {
                string sN = node.GetAttribute("ID002");
                if (sN == ListCell[1].StrValue)
                {
                    for (int i = 0; i < ListCell.Count; i++)
                    {
                        string sName = "ID" + ListCell[i].ID .ToString("D3");
                            node.SetAttribute(sName, ListCell[i].StrValue);
                    }
                    myxmldoc.Save(filePath);
                    return;
                }
            }

            xpath = "root/LayerList";
            XmlElement ListNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
            XmlElement nLayNode = myxmldoc.CreateElement("Layer"); // 创建根节点album
            for (int i = 0; i < ListCell.Count; i++)
            {
                string sName = "ID" + ListCell[i].ID.ToString("D3");
                nLayNode.SetAttribute(sName, ListCell[i].StrValue);
            }
            ListNode.AppendChild(nLayNode);
            myxmldoc.Save(filePath);
        }

        public void SaveToDefault()//保存层到xml中缺省层
        {
            try
            {
                string filePath = frmRecipe.sAppPath + @"\Project\Layer.xml";
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(filePath);

                string xpath = "root/ListCell";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement node in childNode.ChildNodes)
                {
                    string sName = node.GetAttribute("Name");
                    for (int i = 0; i < ListCell.Count; i++)
                    {
                        if (sName == ListCell[i].Name)
                        {
                            node.SetAttribute("Linker", ListCell[i].Linker);
                            node.SetAttribute("RecipeType", ListCell[i].CellType.ToString());
                            node.SetAttribute("ValueType", ListCell[i].ValueType);
                            node.SetAttribute("InitialValue", ListCell[i].StrValue);
                            node.SetAttribute("RatioValue", ListCell[i].RatioValue);
                            node.SetAttribute("sRange", ListCell[i].sRange);
                        }
                    }
                }
                myxmldoc.Save(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CLayer.InitFromXml:" + ex.Message);
            }
        }

        public void TimeSum()
        {
            TimeTol = 0;
            TimeRun = 0;
            try
            {
                TimeTol = TimeSpan.Parse(ListCell[2].StrValue).TotalSeconds;
            }
            catch
            { 
            }
        }
    }
}
