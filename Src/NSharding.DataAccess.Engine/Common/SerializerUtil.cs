using System;
using System.Xml;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// Summary description for SerializerUtil.
    /// </summary>
    public class SerializerUtil
    {
        // Element
        /// <summary>
        /// Add a child element with the specific name to the given parent element 
        /// and return the child element.
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name)
        {
            XmlElement element = parent.OwnerDocument.CreateElement(parent.Prefix, name, parent.NamespaceURI);
            parent.AppendChild(element);
            return element;
        }

        #region DateTime
        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, DateTime value)
        {
            return AddElement(parent, name, value, DateTime.MinValue);
        }

        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// If the given value is null then the default value is used.  
        /// If the value is null then this method will not add the child element and will return null.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <param name="defaultValue">The default value (if the value is null)</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, DateTime value, DateTime defaultValue)
        {
            if (value == DateTime.MinValue)
            {
                value = defaultValue;
            }

            XmlElement child = null;

            if (value != DateTime.MinValue)
            {
                child = AddElement(parent, name);
                child.InnerText = value.ToString();
            }

            return child;
        }
        #endregion

        #region Integer
        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, int value)
        {
            return AddElement(parent, name, value, int.MinValue);
        }

        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// If the given value is null then the default value is used.  
        /// If the value is null then this method will not add the child element and will return null.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <param name="defaultValue">The default value (if the value is null)</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, int value, int defaultValue)
        {
            if (value == int.MinValue)
            {
                value = defaultValue;
            }

            XmlElement child = null;

            if (value != int.MinValue)
            {
                child = AddElement(parent, name);
                child.InnerText = value.ToString();
            }

            return child;
        }
        #endregion

        #region String
        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, string value)
        {
            return AddElement(parent, name, value, null);
        }

        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// If the given value is null then the default value is used.  
        /// If the value is null then this method will not add the child element and will return null.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <param name="defaultValue">The default value (if the value is null)</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, string value, string defaultValue)
        {
            if (value == null)
            {
                value = defaultValue;
            }

            XmlElement child = null;

            if (value != null)
            {
                child = AddElement(parent, name);
                child.InnerText = value;
            }

            return child;
        }
        #endregion

        #region URI
        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, Uri value)
        {
            return AddElement(parent, name, value, null);
        }

        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// If the given value is null then the default value is used.  
        /// If the value is null then this method will not add the child element and will return null.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <param name="defaultValue">The default value (if the value is null)</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, Uri value, Uri defaultValue)
        {
            XmlElement child = null;

            if (value == null)
            {
                value = defaultValue;
            }

            if (value != null)
            {
                child = AddElement(parent, name);
                child.InnerText = value.AbsoluteUri;
            }

            return child;
        }
        #endregion

        #region TimeSpan
        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, TimeSpan value)
        {
            return AddElement(parent, name, value, TimeSpan.Zero);
        }

        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// If the given value is null then the default value is used.  
        /// If the value is null then this method will not add the child element and will return null.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <param name="defaultValue">The default value (if the value is null)</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, TimeSpan value, TimeSpan defaultValue)
        {
            XmlElement child = null;

            if (value == TimeSpan.Zero)
            {
                value = defaultValue;
            }

            if (value != TimeSpan.Zero)
            {
                child = AddElement(parent, name);
                child.InnerText = value.ToString();
            }

            return child;
        }
        #endregion TimeSpan

        #region Enum
        /// <summary>
        /// Add a child element with the specific name and the given value to the given parent element 
        /// and return the child element.  
        /// This method will use the namespace of the parent element for the child element's namespace.
        /// </summary>
        /// <param name="parent">The parent element</param>
        /// <param name="name">The new child element name</param>
        /// <param name="value">The value</param>
        /// <returns>The child element</returns>
        public static XmlElement AddElement(XmlElement parent, string name, Enum value)
        {
            XmlElement child = null;
            child = AddElement(parent, name);
            child.InnerText = value.ToString();
            return child;
        }
        #endregion Element

        public static void ImportXml(XmlElement parent, string text)
        {
            if (text == null || text.Length == 0)
            {
                return;
            }
            XmlDocumentFragment documentFragment = parent.OwnerDocument.CreateDocumentFragment();
            documentFragment.InnerText = text;
            parent.AppendChild(documentFragment);
        }
    }
}
