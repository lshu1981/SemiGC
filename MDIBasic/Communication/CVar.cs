using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Drawing.Design;
using System.Data;
using System.Xml;
using LSSCADA.Control;
using System.Drawing;
using System.Diagnostics;

namespace LSSCADA
{
    //1：整型；2：无符号整型；3：长整型；4：无符号长整型；5：浮点型；6：双精度型；7：字符型；8：无符号字符型；9：布尔型；10：字符串型；11：二进制型。
    public enum EVarTYPE { Var_INT = 1, Var_UINT, Var_LINT, Var_ULINT, Var_FLOAT, Var_DOUBLE, Var_CHAR, Var_UCHAR, Var_BOOL, Var_STRING, Var_BLOB };

    public enum EDAType
    {
        DA_YC = 1,  //遥测
        DA_YX = 2,  //遥信
        DA_YT = 3,  //遥调
        DA_YK = 4,  //遥控
        DA_YM = 5,  //遥脉
        DA_YS = 6,  //其他        
    };

    //	1：循环报文  2：定时报文 ；3：召唤报文
    public enum EMsgType { Msg_Loop = 1, Msg_Time = 2, Msg_Call = 3 };

    //	0：映射网络变量；2：全局内存变量 ；3：OPC变量
    public enum EVarType { NetVar = 0, DeviceVar = 1, MemoryVar = 2, OPCVar = 3 };
    public enum EType { Net = 0, Memory = 3 };
    public class CUnitTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) { return null; }

        CUnitTypeConverter()
        {
        }
    }

    public class CUnitConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) { return null; }

        CUnitConverter()
        {
        }
    };

    public class CValTypeConverter : StringConverter
    {

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) { return null; }

        CValTypeConverter()
        {
        }
    }
    public class CVTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) { return null; }

        CVTypeConverter()
        {
        }
    }

    [Description("编码转换公式"), TypeConverter(typeof(CExpressEditConvert))]
    public class CExpressEdit
    {
        //public	static CExpressEditForm  editForm = null;
        public String szExpress;
        public CVar currentVar;
        public CExpression FExpression;            //显示方式

        public CExpressEdit Clone()
        {
            CExpressEdit obj = (CExpressEdit)this.MemberwiseClone();
            if (FExpression != null)
                obj.FExpression = FExpression.Clone();
            return obj;

        }
        public CExpressEdit Edit() { return null; }
        public CExpressEdit(String str1)
        {
            szExpress = str1;
        }
        public string GetValue()
        {
            return FExpression.execStr();
        }
        public void GetDeviceVar(string StaName)
        {
            FExpression = new CExpression();
            FExpression.ExpressType = LCExpressType.Expression;
            FExpression.Exipression = szExpress;
            FExpression.GetDeviceVar(StaName);
        }
    }

    public class CExpressEditConvert : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.Equals(Type.GetType("DMS.DMS_CommModel.CExpressEdit")))
                return true;
            TypeConverter aa = new TypeConverter();
            return aa.CanConvertTo(context, destinationType);
        }

        public override Object ConvertTo(ITypeDescriptorContext context,
                                            CultureInfo culture,
                                            Object value,
                                            Type destinationType)
        {
            if (destinationType.Equals(Type.GetType("System.String"))
                    && value.GetType().Equals(Type.GetType("DMS.DMS_CommModel.CExpressEdit")))
            {
                CExpressEdit ExpressEdit = (CExpressEdit)(value);
                return ExpressEdit.szExpress;
            }
            TypeConverter aa = new TypeConverter();
            return aa.ConvertTo(context, culture, value, destinationType);
        }
    };

    [System.Security.Permissions.PermissionSetAttribute
    (System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute
    (System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
    public class CExpressEditor : UITypeEditor
    {
        public CExpressEditor() { }
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
        {
            CExpressEdit expressEdit = (CExpressEdit)(value);
            if (expressEdit != null)
                return (Object)expressEdit.Edit();
            else
                return (Object)expressEdit;
        }
    };

    public struct SVarValue
    {
        public DateTime? Date_Time;
        public Double Value_d;
        public SVarValue(double aa)
        {
            Date_Time = null;
            Value_d = aa;
        }
    }

    public class CVar
    {
        public string StaName = ""; //子站名称

        public String Name = "";        //变量名称
        public String Description = ""; //变量描述
        public EDAType  DAType;          //1YC 2YX 3YT 4YK 5YM 6YS
        public EVarTYPE VarType;         //1：整型；2：无符号整型；3：长整型；4：无符号长整型；5：浮点型；6：双精度型；7：字符型；8：无符号字符型；9：布尔型；10：字符串型；11：二进制型。
        public String Unit;				//单位(可获取单位名称)

        public int ByteAddr = 0;
        public int BitAddr = 0;

        public Single[] IntTag = new Single[10];        //变量参数
        /// <summary>
        /// 参数说明
        /// Modbus  0报文编号 1寄存器字地址 2寄存器位地址  
        /// PLC 
        /// </summary>

        public String[] StrTag= new string [5];           //变量参数 字符型

        public int Length = 16;         //变量长度 缺省值32（1~32）
        public int bHighLow = 1;	//1 高字节在前 0高字节在后
        public byte[] bHighByte = new byte[4];
        public Single BaseValue = 0;    //基数
        public Single RatioValue = 0;   //系数   实际值=采集值*RatioValue+BaseValue


        public int Readable;            //是否可读  1可读
        public int Writeable;           //是否可写  1可写
        public string Default;          //全局内存变量初值
        public int SaveToDB;            //是否保存数据库  1存储

        /// <summary>
        /// DI报警类型（0,1等级为报警 2等级为警告）
        /// </summary>
        public string DIAlarmType
        {
            get
            {
                if (DAType == EDAType.DA_YC || Writeable == 1)
                    return "";
                if (nVarAlarm == null)
                    return "";
                if (nVarAlarm.ListVarState.Count >0)
                {
                    if (nVarAlarm.VarStatepriority ==2)
                        return "2警告";
                    else
                        return "1报警";
                }
                else
                    return "";
            }
        }
        /// <summary>
        /// DI报警值（1为0变1报警 0为1变0报警）
        /// </summary>
        public string DIAlarmValue
        {
            get
            {
                if (DAType == EDAType.DA_YC || Writeable == 1)
                    return "";
                if (nVarAlarm == null)
                    return "";
                if (nVarAlarm.ListVarState.Count > 0)
                {
                    string str1 = "";
                    foreach (CVarState node in nVarAlarm.ListVarState)
                    {
                        str1 += "," + node.newvalue.ToString();
                    }
                    return str1.Substring(1);
                }
                else
                    return "";
            }
        }

        public string MySqlType
        {
            get
            {
                switch (VarType)
                {
                    case EVarTYPE.Var_INT:
                        return "int(11)";
                    case EVarTYPE.Var_UINT:
                        return "int(11) unsigned";
                    case EVarTYPE.Var_LINT:
                        return "bigint(20)";
                    case EVarTYPE.Var_ULINT:
                        return "bigint(20) unsigned";
                    case EVarTYPE.Var_FLOAT:
                        return "float";
                    case EVarTYPE.Var_DOUBLE:
                        return "double";
                    case EVarTYPE.Var_CHAR:   //"字符型";
                        return "tinyint(4)";
                    case EVarTYPE.Var_UCHAR://"无符号字符型";
                        return "tinyint(4) unsigned";
                    case EVarTYPE.Var_BOOL:// "布尔型";
                        return "tinyint(4)";
                    case EVarTYPE.Var_STRING://"字符串型";
                        return "text";
                    case EVarTYPE.Var_BLOB://"二进制型";
                        return "tinyblob";
                    default:
                        return "int(11)";
                }
            }
        }
        public SVarValue VarNewValue;      //数据
        public SVarValue VarOldValue;      //数据
        
        public List<SVarValue> ListValue = new List<SVarValue>();
        public CExpressEdit ExpressW = null;		//请求编码转换公式
        public CExpressEdit ExpressR = null;		//响应编码转换公式

        public int iDBByteNum = 0;                      //保存历史数据长度
        //public CStation ToStation;		//对应子站
        public String _ToStationName;
        public Guid _ToStationID;
        
        public String MinValue;
        public String MaxValue;

        public Int64[] PLCValue=new long[6];//PLC存储数据

        public CVarAlarm nVarAlarm = null;

        public CVar() { }
        //实现内部函数

        //public	virtual bool SaveToDB(DbCommand command) override;
        public virtual bool LoadFromNode(XmlElement Node)
        {
            Name = Node.GetAttribute("Name");
            Description = Node.GetAttribute("Description");
            DAType = (EDAType)Convert.ToInt32(Node.GetAttribute("DAType"));
            VarType = (EVarTYPE)Convert.ToInt32(Node.GetAttribute("VarType"));
            Unit = Node.GetAttribute("Unit");
            ByteAddr =Convert.ToInt32(Node.GetAttribute("ByteAddr"));
            BitAddr = Convert.ToInt32(Node.GetAttribute("BitAddr"));

            for (int i = 0; i < 10; i++)
            {
                IntTag[i] = Convert.ToSingle(Node.GetAttribute("IntTag" + i.ToString())); 
            }
            for (int i = 0; i < 5; i++)
            {
                StrTag[i] = Node.GetAttribute("StrTag"+ i.ToString()); 
            }

            Length = Convert.ToInt32(Node.GetAttribute("Length"));
            bHighLow = Convert.ToInt32(Node.GetAttribute("bHighLow"),16);
            bHighByte = BitConverter.GetBytes(bHighLow);
            BaseValue = Convert.ToSingle(Node.GetAttribute("BaseValue"));
            RatioValue = Convert.ToSingle(Node.GetAttribute("RatioValue"));
            Readable = Convert.ToInt32(Node.GetAttribute("Readable"));
            Writeable = Convert.ToInt32(Node.GetAttribute("Writeable"));
            Default = Node.GetAttribute("Value");
            SaveToDB = Convert.ToInt32(Node.GetAttribute("SaveToDB"));

            if (StrTag[3].Length > 0)
            {
                ExpressR = new CExpressEdit(StrTag[3]);
            }
            if (StrTag[4].Length > 0)
            {
                ExpressW = new CExpressEdit(StrTag[4]);
            }
            switch (VarType)
            {
                case EVarTYPE.Var_INT:
                    iDBByteNum = sizeof(int);
                    break;
                case EVarTYPE.Var_UINT:
                    iDBByteNum = sizeof(uint);
                    break;
                case EVarTYPE.Var_LINT:
                    iDBByteNum = sizeof(long);
                    break;
                case EVarTYPE.Var_ULINT:
                    iDBByteNum = sizeof(ulong);
                    break;
                case EVarTYPE.Var_FLOAT:
                    iDBByteNum = sizeof(float);
                    break;
                case EVarTYPE.Var_DOUBLE:
                    iDBByteNum = sizeof(double);
                    break;
                case EVarTYPE.Var_CHAR:   //"字符型";
                    iDBByteNum = sizeof(char);
                    break;
                case EVarTYPE.Var_UCHAR://"无符号字符型";
                    iDBByteNum = sizeof(char);
                    break;
                case EVarTYPE.Var_BOOL:// "布尔型";
                    iDBByteNum = sizeof(bool);
                    break;
                case EVarTYPE.Var_STRING://"字符串型";
                    iDBByteNum = 0;
                    break;
                case EVarTYPE.Var_BLOB://"二进制型";
                    iDBByteNum =1;
                    break;
                default:
                    iDBByteNum =0;
                    break;
            }
            return true;
        }
        //public	virtual void CopyTo(INode  _Node) override;
        public virtual CVar Clone()
        {
            CVar obj = (CVar)this.MemberwiseClone();
            if (nVarAlarm != null)
                obj.nVarAlarm = nVarAlarm.Clone();
            if (ExpressR != null)
                obj.ExpressR = ExpressR.Clone();
            if (ExpressW != null)
                obj.ExpressW = ExpressW.Clone();

            obj.ListValue = new List<SVarValue>();
            obj.IntTag = new Single[10];
            for (int i = 0; i < 10; i++)
            {
                obj.IntTag[i] = IntTag[i];
            }
            //{
            //    obj.ListValue.Add(nValue);
            //}
            return obj;
        }
        public virtual String GetErrors()
        {
            String Errors = "";
            return Errors;
        }

        // 实现IVar接口
        public virtual int GetVarType() { return 0; }

        /// <summary>
        /// 获取值的品质
        /// </summary>
        /// <returns></returns>
        public bool GetValueQuality
        {
            get
            {
                try
                {
                    if (DAType == EDAType.DA_YS && ExpressR != null)
                        return true;
                    else
                    {
                        if (VarNewValue.Date_Time != null)
                            return true;
                        else
                            return false;
                    }
                }
                catch (Exception ex) { return false; }
            }
        }

        public virtual String GetStringValue()
        {
            try
            {
             //   if (DAType == EDAType.DA_YS && ExpressR != null)
             //       return ExpressR.GetValue();

                switch (VarType)
                {
                    case EVarTYPE.Var_INT:
                    case EVarTYPE.Var_UINT:
                    case EVarTYPE.Var_LINT:
                    case EVarTYPE.Var_ULINT:
                        return (Convert.ToInt64( VarNewValue.Value_d)).ToString();
                    case EVarTYPE.Var_FLOAT:
                    case EVarTYPE.Var_DOUBLE:
                        return VarNewValue.Value_d.ToString();
                    case EVarTYPE.Var_CHAR:   //"字符型";
                    case EVarTYPE.Var_UCHAR://"无符号字符型";
                        return Convert.ToChar(VarNewValue.Value_d).ToString();
                    case EVarTYPE.Var_BOOL:// "布尔型";
                        return Convert.ToBoolean(VarNewValue.Value_d).ToString();
                    case EVarTYPE.Var_STRING://"字符串型";
                        return "";
                    case EVarTYPE.Var_BLOB://"二进制型";
                        return "";
                    default:
                        return "";
                }
            }
            catch (Exception ex) { return ""; }
        }

        public virtual String GetStrDataValue()
        {
            try
            {
                //   if (DAType == EDAType.DA_YS && ExpressR != null)
                //       return ExpressR.GetValue();
                SVarValue newValue;
                if (ListValue.Count > 0)
                    newValue = ListValue[ListValue.Count - 1];
                else
                    newValue = new SVarValue();

                    switch (VarType)
                    {
                        case EVarTYPE.Var_INT:
                        case EVarTYPE.Var_UINT:
                        case EVarTYPE.Var_LINT:
                        case EVarTYPE.Var_ULINT:
                            return (Convert.ToInt64(newValue.Value_d)).ToString();
                        case EVarTYPE.Var_FLOAT:
                        case EVarTYPE.Var_DOUBLE:
                            return newValue.Value_d.ToString();
                        case EVarTYPE.Var_CHAR:   //"字符型";
                        case EVarTYPE.Var_UCHAR://"无符号字符型";
                            return Convert.ToChar(newValue.Value_d).ToString();
                        case EVarTYPE.Var_BOOL:// "布尔型";
                            return Convert.ToBoolean(newValue.Value_d).ToString();
                        case EVarTYPE.Var_STRING://"字符串型";
                            return "";
                        case EVarTYPE.Var_BLOB://"二进制型";
                            return "";
                        default:
                            return "";
                    }
            }
            catch (Exception ex) { return ""; }
        }

        public virtual String GetStringValue(int point)
        {
            try
            {
                string fp = "f" + point.ToString().Trim();
                return GetStringValue(fp);
            }
            catch (Exception ex) { return ""; }
        }

        public virtual String GetStringValue(string fp)
        {
            try
            {
               // if (DAType == EDAType.DA_YS && ExpressR != null)
                //{
               //     string str1 = ExpressR.GetValue();
              //      return Convert.ToDouble(str1).ToString(fp);
              //  }
                switch (VarType)
                {
                    case EVarTYPE.Var_INT:
                    case EVarTYPE.Var_UINT:
                    case EVarTYPE.Var_LINT:
                    case EVarTYPE.Var_ULINT:
                        return (Convert.ToInt64(VarNewValue.Value_d)).ToString(fp);
                    case EVarTYPE.Var_FLOAT:
                    case EVarTYPE.Var_DOUBLE:
                        return VarNewValue.Value_d.ToString(fp);
                    case EVarTYPE.Var_CHAR:   //"字符型";
                    case EVarTYPE.Var_UCHAR://"无符号字符型";
                        return Convert.ToChar(VarNewValue.Value_d).ToString();
                    case EVarTYPE.Var_BOOL:// "布尔型";
                        return Convert.ToBoolean(VarNewValue.Value_d).ToString();
                    case EVarTYPE.Var_STRING://"字符串型";
                        return "";
                    case EVarTYPE.Var_BLOB://"二进制型";
                        return "";
                    default:
                        return "";
                }
            }
            catch (Exception ex) { return ""; }
        }
        public virtual String GetStringValue(int point, int iLen)
        {
            string str1 = GetStringValue(point);
            int iPT = str1.IndexOf('.');
            if (iPT + 2 <= iLen)
                return str1;
            else if (iPT > iLen)
            {
                return str1.Substring(0, iPT);
            }
            else
            {
                return str1.Substring(0, iPT);
            }

        }

        public virtual bool GetBoolValue()
        {
            return Convert.ToBoolean(VarNewValue.Value_d);
        }

        public virtual Int64 GetInt64Value()
        {
            try
            {
               // if (DAType == EDAType.DA_YS && ExpressR != null)
               //     return Convert.ToInt64(Convert.ToDouble(ExpressR.GetValue()));

                    switch (VarType)
                    {
                        case EVarTYPE.Var_INT:
                        case EVarTYPE.Var_UINT:
                        case EVarTYPE.Var_LINT:
                        case EVarTYPE.Var_ULINT:
                        case EVarTYPE.Var_BOOL:// "布尔型";
                        case EVarTYPE.Var_FLOAT:
                        case EVarTYPE.Var_DOUBLE:
                        case EVarTYPE.Var_CHAR:   //"字符型";
                        case EVarTYPE.Var_UCHAR://"无符号字符型";
                            return Convert.ToInt64(VarNewValue.Value_d);
                        case EVarTYPE.Var_STRING://"字符串型";
                            return 0;
                        case EVarTYPE.Var_BLOB://"二进制型";
                            return 0;
                        default:
                            return 0;
                    }
            }
            catch (Exception ex) { return 0; }
        }

        public virtual Double GetDoubleValue()
        {
            try
            {
                //if (DAType == EDAType.DA_YS && ExpressR != null)
                //    return Convert.ToDouble (ExpressR.GetValue());

                switch (VarType)
                    {
                        case EVarTYPE.Var_INT:
                        case EVarTYPE.Var_UINT:
                        case EVarTYPE.Var_LINT:
                        case EVarTYPE.Var_ULINT:
                        case EVarTYPE.Var_BOOL:// "布尔型";
                        case EVarTYPE.Var_FLOAT:
                        case EVarTYPE.Var_DOUBLE:
                        case EVarTYPE.Var_CHAR:   //"字符型";
                        case EVarTYPE.Var_UCHAR://"无符号字符型";
                            return (Double)VarNewValue.Value_d;
                        case EVarTYPE.Var_STRING://"字符串型";
                            return 0;
                        case EVarTYPE.Var_BLOB://"二进制型";
                            return 0;
                        default:
                            return 0;
                    }
            }
            catch (Exception ex) { return 0; }

        }

        public virtual Double GetDoubleDataValue()
        {
            try
            {
                SVarValue newValue;
                if (ListValue.Count > 0)
                    newValue = ListValue[ListValue.Count - 1];
                else
                    newValue = new SVarValue();
                switch (VarType)
                {
                    case EVarTYPE.Var_INT:
                    case EVarTYPE.Var_UINT:
                    case EVarTYPE.Var_LINT:
                    case EVarTYPE.Var_ULINT:
                    case EVarTYPE.Var_BOOL:// "布尔型";
                    case EVarTYPE.Var_FLOAT:
                    case EVarTYPE.Var_DOUBLE:
                    case EVarTYPE.Var_CHAR:   //"字符型";
                    case EVarTYPE.Var_UCHAR://"无符号字符型";
                        return (Double)newValue.Value_d;
                    case EVarTYPE.Var_STRING://"字符串型";
                        return 0;
                    case EVarTYPE.Var_BLOB://"二进制型";
                        return 0;
                    default:
                        return 0;
                }
            }
            catch (Exception ex) { return 0; }

        }
        public virtual Double GetDoubleValue(DateTime DT_N)
        {
            try
            {
                //if (DAType == EDAType.DA_YS && ExpressR != null)
                //    return Convert.ToDouble(ExpressR.GetValue());
                if (ListValue.Count == 0)
                    return 0;
                if (ListValue.Count == 1)
                    return ListValue[0].Value_d;
                int i = ListValue.Count-1;
                for (int j = ListValue.Count - 1; j >= 0; j--)
                {
                    if (DT_N <= ListValue[j].Date_Time)
                    {
                        i = j;
                    }
                    else
                        break;
                }
                return ListValue[i].Value_d;
            }
            catch (Exception ex) { return 0; }

        }

        public virtual void SetValue(Int64 value)
        {
            //ValueInt64 = value;
            SVarValue newValue = new SVarValue();
            newValue.Date_Time = DateTime.Now;
            if (StrTag[2] == "Int16")
            {
                value = (short)value;
            }
            if (StrTag[2] == "float754")
            {
                byte[] bytes = BitConverter.GetBytes((int)value);
                Single sinV = BitConverter.ToSingle(bytes, 0);
                newValue.Value_d = (double)(sinV * RatioValue + BaseValue);
            }
            else
            {
                newValue.Value_d = (double)(value * RatioValue + BaseValue);
            }
            VarOldValue = VarNewValue;
            VarNewValue = newValue;

            if (nVarAlarm != null)//配置了报警，且品质是好的情况下，产生报警
            {
                CheckAlarm();
            }
        }

        public virtual void SetValue(Single value)
        {
            //ValueInt64 = value;
            SVarValue newValue = new SVarValue();
            newValue.Date_Time = DateTime.Now;
            newValue.Value_d = (double)(value * RatioValue + BaseValue);
            VarOldValue = VarNewValue;
            VarNewValue = newValue;

            if (nVarAlarm != null)//配置了报警，且品质是好的情况下，产生报警
            {
                CheckAlarm();
            }
        }

        public virtual void SetExtendVarValue()
        {
            //ValueInt64 = value;
            SVarValue newValue = new SVarValue();
            newValue.Date_Time = DateTime.Now;
            newValue.Value_d = (double)(Convert.ToDouble(ExpressR.GetValue()) * RatioValue + BaseValue);
            VarOldValue = VarNewValue;
            VarNewValue = newValue;

            if (nVarAlarm != null)//配置了报警，且品质是好的情况下，产生报警
            {
                CheckAlarm();
            }
        }

        public void CheckAlarm()
        {
            if (VarOldValue.Date_Time!=null)
            {
                //越限报警
                nVarAlarm.GetELimitAlarm(VarNewValue.Value_d);
                //变位报警
                nVarAlarm.GetVarStateAlarmMsg(VarOldValue.Value_d,VarNewValue.Value_d);
            }
        }

        public virtual int GetDataType() {	return (int)VarType;}

        public string GetTypeName() 
        {
            string sName = "";
            switch (DAType)
            {
                case EDAType.DA_YC :
                    sName = "[遥测]";
                    break;
                case EDAType.DA_YX:
                    sName = "[遥信]";
                    break;
                case EDAType.DA_YT:
                    sName = "[遥调]";
                    break;
                case EDAType.DA_YK:
                    sName = "[遥控]";
                    break;
                case EDAType.DA_YM:
                    sName = "[遥脉]";
                    break;
                case EDAType.DA_YS:
                    sName = "[其他]";
                    break;
                default:
                    break;
            }
            switch (VarType)
            {
                case EVarTYPE.Var_INT:
                    sName += "整型";
                    break;
                case EVarTYPE.Var_UINT:
                    sName += "无符号整型";
                    break;
                case EVarTYPE.Var_LINT:
                    sName += "长整型";
                    break;
                case EVarTYPE.Var_ULINT:
                    sName += "无符号长整型";
                    break;
                case EVarTYPE.Var_FLOAT:
                    sName += "浮点型";
                    break;
                case EVarTYPE.Var_DOUBLE:
                    sName += "双精度型";
                    break;
                case EVarTYPE.Var_CHAR:
                    sName += "字符型";
                    break;
                case EVarTYPE.Var_UCHAR:
                    sName += "无符号字符型";
                    break;
                case EVarTYPE.Var_BOOL:
                    sName += "布尔型";
                    break;
                case EVarTYPE.Var_STRING:
                    sName += "字符串型";
                    break;
                case EVarTYPE.Var_BLOB:
                    sName += "二进制型";
                    break;
                default:
                    sName += "类型不确定";
                    break;
            }
            return sName;
        }

        public void SetDIAlarm(string sType,string sValue)
        {
            if (sType == "")
            {
                if (nVarAlarm == null)
                    return;
                nVarAlarm.ListVarState.Clear();
                if (nVarAlarm.GetNull() == 0)
                {
                    nVarAlarm = null;
                    return;
                }
            }
            if (nVarAlarm == null)
                nVarAlarm = new CVarAlarm();
            nVarAlarm.ListVarState.Clear();
            int iRp = Convert.ToInt32(sType.Substring(0, 1));
            string sAlarm = "";
            if (iRp == 2)
                sAlarm = "警告";
            else
                sAlarm = "报警";
            nVarAlarm.VarStatepriority = iRp;
            sValue = sValue.Replace(':',',');
            sValue = sValue.Replace(';', ',');
            sValue = sValue.Replace('；', ',');
            sValue = sValue.Replace('：', ',');
            sValue = sValue.Replace('.', ',');
            string[] str2 = sValue.Split(',');
            for (int i = 0; i < str2.Length; i++)
            {
                try
                {
                    int iNew =Math.Abs( Convert.ToInt32(str2[i]));
                    int iOld = 1 - iNew;
                    if (iOld < 0)
                        continue;
                    bool bAdd = true;
                    foreach (CVarState node in nVarAlarm.ListVarState)
                    {
                        if (node.oldvalue == iOld && node.newvalue == iNew)
                        {
                            bAdd = false;
                            break;
                        }
                    }
                    if (bAdd)
                    {
                        CVarState nState = new CVarState();
                        nState.oldvalue = iOld;
                        nState.newvalue = iNew;
                        nState.text = Name + "." + Description + " " + iOld.ToString() + "-" + iNew.ToString() + sAlarm;
                        nVarAlarm.ListVarState.Add(nState);
                    }
                }
                catch
                { }
            }
        }
    }
}
