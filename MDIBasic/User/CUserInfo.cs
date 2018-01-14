using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSSCADA.Database;
using System.Security.Cryptography;
using System.Xml;
using System.Diagnostics;
using System.Collections;

namespace LSSCADA
{
    public class CUserInfo
    {
        public string OldUserName = "";
        public string UserName = "";
        public int  UserID = 0;
        public bool bSuccess = false;
        public string UserRole = "";
        public List<string> ListSRight = new List<string>();
        public LSDatabase nDatabase;
        public string sXMLPath;

        public CUserInfo(string _sApp, LSDatabase _DB)
        {
            nDatabase = _DB;
            sXMLPath = _sApp + "\\Project\\User.xml";
            LoadXML();
        }
        void LoadXML()
        {
            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/UserList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                OldUserName = childNode.GetAttribute("OldUserName");
            }
            catch (Exception ee)
            {
            }
        }

        //登录
        public virtual bool Login(string sName,string sPass, ref string sRe) { return true; }
        //检查用户名和密码
        public virtual bool CheckUserNamePass(string sName, string sPass) { return true; }
        //修改密码
        public virtual bool ChangePassword(string sName, string sPass,string sNew, ref string sRe) { return true; }
        //获取权限
        public bool GetUserRole(string sRight) 
        {
            try
            {
                foreach (string str1 in ListSRight)
                {
                    if (str1 == sRight)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //删除用户
        public virtual bool DelUser(string sName, ref string sRe) { return true; }
        //添加用户
        public virtual bool AddUser(string ID, string sName, string sRole, string sPass, ref string sRe) { return true; }
        //修改用户
        public virtual bool EditUser(string sName,string sRole,string sPass, ref string sRe) { return true; }

        public virtual Dictionary<int, string> GetAllUsers() { return null; }
        public virtual ArrayList GetAllUsersArray() { return null; }
        public virtual ArrayList GetAllRole() { return null; }

        /// <summary>
        /// MD5加密函数
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        /// 
        public string md5_Encode(string str)
        {
            MD5 m = new MD5CryptoServiceProvider();
            string sHead = "ydjkzbh_7";
            string sTail = "yqqlm@gsycl";
            byte[] data = Encoding.Default.GetBytes(sHead + str + sTail);
            byte[] result = m.ComputeHash(data);
            string ret1 = "";
            try
            {
                for (int j = 0; j < result.Length; j++)
                {
                    ret1 += result[j].ToString("x").PadLeft(2, '0');
                }
                return ret1;
            }
            catch
            {
                return str;
            }
        }
    }

    public class CUserXMLUM : CUserInfo
    {
        public CUserXMLUM(string _sApp, LSDatabase _DB)
            : base(_sApp, _DB)
        {        }

        //用户登录
        public override bool Login(string sName, string sPass, ref string sRe)
        {
            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/UserList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);

                foreach (XmlElement item in childNode.ChildNodes)
                {
                    if (sName == item.GetAttribute("UserName"))
                    {
                        string sUserRole = item.GetAttribute("UserRoles");
                        string PasswordMd5 = md5_Encode(sName + sUserRole + sPass);
                        if (PasswordMd5 == item.GetAttribute("UserPassword"))
                        {
                            UserName = sName;
                            UserRole = sUserRole;
                            UserID =Convert.ToInt32( item.GetAttribute("UserID"));
                            GetUserRoleFromXML((XmlElement)myxmldoc.SelectSingleNode("root"));
                            bSuccess = true;
                            if (OldUserName != UserName)
                            {
                                OldUserName = UserName;
                                childNode.SetAttribute("OldUserName", OldUserName);
                                myxmldoc.Save(sXMLPath);
                            }
                            return true;
                        }
                        else
                        {
                            sRe = "密码错误！请重新输入";
                            return false;
                        }
                    }
                }
                sRe = "用户名'" + sName + "'不存在！请重新输入";
            }
            catch (Exception ee)
            {
                sRe = ee.Message;
            }
            return false;
        }
        //检查用户名和密码
        public override bool CheckUserNamePass(string sName, string sPass) 
        {
            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/UserList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement item in childNode.ChildNodes)
                {
                    if (sName == item.GetAttribute("UserName"))
                    {
                        string sUserRole = item.GetAttribute("UserRoles");
                        string PasswordMd5 = md5_Encode(sName + UserRole + sPass);
                        if (PasswordMd5 == item.GetAttribute("UserPassword"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
            }
            return false;
        }
        //修改口令
        public override bool ChangePassword(string sName, string sPass, string sNew, ref string sRe) 
        {
            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/UserList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement item in childNode.ChildNodes)
                {
                    if (sName == item.GetAttribute("UserName"))
                    {
                      string  sUserRole = item.GetAttribute("UserRoles");
                        string PasswordMd5 = md5_Encode(sName + UserRole + sPass);
                        if (PasswordMd5 == item.GetAttribute("UserPassword"))
                        {
                            string NewMD5 = md5_Encode(sName + UserRole + sNew);
                            item.SetAttribute("UserPassword", NewMD5);
                            myxmldoc.Save(sXMLPath);
                            return true;
                        }
                        else
                        {
                            sRe = "原密码错误！请重新输入";
                            return false;
                        }
                    }
                }
                sRe = "用户名'" + sName + "'不存在！请重新输入";
            }
            catch (Exception ee)
            {
                sRe = ee.Message;
            }
            return false;
        }
        //删除用户
        public override bool DelUser(string sName, ref string sRe)
        {
            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/UserList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);

                foreach (XmlElement item in childNode.ChildNodes)
                {
                    if (sName == item.GetAttribute("UserName"))
                    {
                        childNode.RemoveChild(item);
                        myxmldoc.Save(sXMLPath);
                        return true;
                    }
                }
                sRe = "用户名'" + sName + "'不存在！请重新选择";
            }
            catch (Exception ee)
            {
                sRe = ee.Message;
            }
            return false;
        }

        //添加用户
        public override bool AddUser(string ID, string sName, string sRole, string sPass, ref string sRe)
        {
            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/UserList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);

                foreach (XmlElement item in childNode.ChildNodes)
                {
                    if (sName == item.GetAttribute("UserName"))
                    {
                        sRe = "用户名已存在！";
                        return false;
                    }
                }
                XmlElement nUser = myxmldoc.CreateElement("row");
                nUser.SetAttribute("UserID", ID);
                nUser.SetAttribute("UserName", sName);
                nUser.SetAttribute("UserRoles", sRole);
                string PasswordMd5 = md5_Encode(sName + sRole + sPass);
                nUser.SetAttribute("UserPassword", PasswordMd5);
                nUser.SetAttribute("PasswordExpired", "0");
                nUser.SetAttribute("Expired_DT", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                nUser.SetAttribute("ValidTime", "60");
                nUser.SetAttribute("UserAddress", "");
                nUser.SetAttribute("UserTelephone", "");
                nUser.SetAttribute("UserEmail", "");
                nUser.SetAttribute("UserDescriber", "");
                nUser.SetAttribute("rowguid", System.Guid.NewGuid().ToString());
                childNode.AppendChild(nUser);

                myxmldoc.Save(sXMLPath);
                return true;
            }
            catch (Exception ee)
            {
                sRe = ee.Message;
                return false;
            }
        }
        //修改用户
        public override bool EditUser(string sName, string sRole, string sPass, ref string sRe)
        {
            try
            {
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/UserList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement item in childNode.ChildNodes)
                {
                    if (sName == item.GetAttribute("UserName"))
                    {
                        string PasswordMd5 = md5_Encode(sName + sRole + sPass);
                        item.SetAttribute("UserRoles", sRole);
                        item.SetAttribute("UserPassword", PasswordMd5);
                        myxmldoc.Save(sXMLPath);
                        return true;
                    }
                }
                sRe = "用户名'" + sName + "'不存在！请重新输入";
            }
            catch (Exception ee)
            {
                sRe = ee.Message;
            }
            return false;
        }

        //获取用户角色
        public bool GetUserRoleFromXML(XmlElement Node) 
        {
            try
            {
                string xpath = "RoleList";
                XmlElement childNode = (XmlElement)Node.SelectSingleNode(xpath);
                ListSRight.Clear();
                foreach (XmlElement item in childNode.ChildNodes)
                {
                    if (UserRole == item.GetAttribute("UserRoles"))
                    {
                        string RightField = item.GetAttribute("RightField");
                        string MD5 = item.GetAttribute("MD5");
                        string str4 = md5_Encode(UserRole + RightField);
                        if (MD5 == md5_Encode(UserRole + RightField))
                        {
                            string str1 = RightField.Replace('；',';');
                            str1 = str1.Replace('，', ';');
                            str1 = str1.Replace(',', ';');
                            string[] str2 = str1.Split(';');
                            for (int i = 0; i < str2.Length; i++)
                            {
                                if (str2[i].Length > 0)
                                {
                                    ListSRight.Add(str2[i]);
                                }
                            }
                            return true;
                        }
                        else
                        {
                            ListSRight.Clear();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message);
            }
            return false;
        }
        //获取所有用户列表
        public override Dictionary<int, string> GetAllUsers() 
        {
            try
            {
                Dictionary<int, string> ListUser = new Dictionary<int, string>();
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/UserList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement item in childNode.ChildNodes)
                {
                    string sUserName = item.GetAttribute("UserName");
                    int iUserID = Convert.ToInt32(item.GetAttribute("UserID"));
                    ListUser.Add(iUserID,sUserName);
                }
                return ListUser;
            }
            catch (Exception ee)
            {
                Console.WriteLine("ERROR: " + ee.Message);
                return null;
            }
        }
        //获取所有用户列表
        public override ArrayList GetAllUsersArray()
        {
            try
            {
                ArrayList ListUser = new ArrayList();
                string[] str1 = new string[3];
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/UserList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement item in childNode.ChildNodes)
                {
                    str1 = new string[3];
                    str1[0] = item.GetAttribute("UserID");
                    str1[1] = item.GetAttribute("UserName");
                    str1[2] = item.GetAttribute("UserRoles");
                    ListUser.Add(str1);
                }
                return ListUser;
            }
            catch (Exception ee)
            {
                Console.WriteLine("ERROR: " + ee.Message);
                return null;
            }
        }

        public override ArrayList GetAllRole()
        {
            try
            {
                ArrayList ListUser = new ArrayList();
                XmlDocument myxmldoc = new XmlDocument();
                myxmldoc.Load(sXMLPath);

                string xpath = "root/RoleList";
                XmlElement childNode = (XmlElement)myxmldoc.SelectSingleNode(xpath);
                foreach (XmlElement item in childNode.ChildNodes)
                {
                    string str1 = item.GetAttribute("UserRoles");
                    ListUser.Add(str1);
                }
                return ListUser;
            }
            catch (Exception ee)
            {
                Console.WriteLine("ERROR: " + ee.Message);
                return null;
            }
        }

    }

    public class CUserDBUM : CUserInfo
    {
        public CUserDBUM(string _sApp, LSDatabase _DB)
            : base(_sApp, _DB)
        {  }
        public override bool Login(string sName, string sPass, ref string  sRe) //组装所有读报文
        {
            return true;
        }
        //获取所有用户列表
        public override Dictionary<int, string> GetAllUsers() { return null; }
    }
}
