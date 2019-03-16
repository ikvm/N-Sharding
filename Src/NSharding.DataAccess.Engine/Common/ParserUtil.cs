using System;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// SQL解析工具类
    /// </summary>
    /// <remarks>SQL解析工具类</remarks>
    public class ParserUtil
    {
        private XmlNamespaceManager nsmgr;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParserUtil() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ns">XmlNamespaceManager</param>
        public ParserUtil(XmlNamespaceManager ns)
        {
            this.nsmgr = ns;
        }

        public bool AsBool(XmlNode element, QName qname)
        {
            string str = this.AsString(element, qname);
            return (((str != null) && !(str == "")) && Convert.ToBoolean(str));
        }

        public bool AsBool(XmlNode element, string childName)
        {
            string str = this.AsString(element, childName);
            return (((str != null) && !(str == "")) && Convert.ToBoolean(str));
        }

        public DateTime AsDateTime(XmlNode element, QName qname)
        {
            string str = this.AsString(element, qname);
            if ((str != null) && !(str == string.Empty))
            {
                return Convert.ToDateTime(str);
            }
            return DateTime.Now;
        }

        public DateTime AsDateTime(XmlNode element, string childName)
        {
            string str = this.AsString(element, childName);
            if ((str != null) && !(str == string.Empty))
            {
                return Convert.ToDateTime(str);
            }
            return DateTime.Now;
        }

        public Enum AsEnum(XmlNode element, string name, Type enumType)
        {
            Enum enum2;
            string str = this.AsString(element, name);
            if (str == null)
            {
                return null;
            }

            enum2 = Enum.Parse(enumType, str) as Enum;

            return enum2;
        }

        public int AsInt(XmlNode element, string childName)
        {
            string str = this.AsString(element, childName);
            if ((str != null) && !(str == ""))
            {
                return Convert.ToInt32(str);
            }
            return 0;
        }

        public long AsLong(XmlNode element, string childName)
        {
            string str = this.AsString(element, childName);
            if ((str != null) && !(str == ""))
            {
                return Convert.ToInt64(str);
            }
            return 0L;
        }

        public string AsString(XmlNode element, QName qname)
        {
            XmlNode node = this.Child(element, qname);
            if (node != null)
            {
                return Convert.ToString(node.InnerText);
            }
            return null;
        }

        public string AsString(XmlNode element, string childName)
        {
            XmlNode node = this.Child(element, childName);
            if (node != null)
            {
                return Convert.ToString(node.InnerText);
            }
            return null;
        }

        public bool AttrBoolean(XmlElement element, string name)
        {
            string str = this.AttrStr(element, name);
            if (str == null)
            {
                return false;
            }
            return Convert.ToBoolean(str);
        }

        public Enum AttrEnum(XmlElement element, string name, Type enumType)
        {
            Enum enum2;
            string str = this.AttrStr(element, name);
            if (str == null)
            {
                return null;
            }

            enum2 = Enum.Parse(enumType, str) as Enum;

            return enum2;
        }

        public int AttrInt(XmlElement element, string name)
        {
            string str = this.AttrStr(element, name);
            if (str == null)
            {
                return 0;
            }
            return Convert.ToInt32(str);
        }

        public string AttrStr(XmlElement element, string name)
        {
            string attribute = element.GetAttribute(name, element.NamespaceURI);
            if (attribute.Length != 0)
            {
                return attribute;
            }
            return null;
        }

        public XmlNode Child(XmlNode element, QName qname)
        {
            return element.SelectSingleNode(qname.Name, this.nsmgr);
        }

        public XmlElement Child(XmlNode element, string childName)
        {
            return (element.SelectSingleNode(childName, this.nsmgr) as XmlElement);
        }

        public XmlNode Child(XmlNode element, string prefix, string childName)
        {
            return element.SelectSingleNode(prefix + ":" + childName, this.nsmgr);
        }

        public XmlNodeList Children(XmlNode element, QName qname)
        {
            return element.SelectNodes(qname.Name, this.nsmgr);
        }

        public XmlNodeList Children(XmlNode element, string childName)
        {
            return element.SelectNodes(childName, this.nsmgr);
        }

        public XmlNodeList Children(XmlNode element, string prefix, string childName)
        {
            return element.SelectNodes(prefix + ":" + childName, this.nsmgr);
        }

        // Properties
        public XmlNamespaceManager NamespaceManager
        {
            get
            {
                return this.nsmgr;
            }
            set
            {
                this.nsmgr = value;
            }
        }
    }
}