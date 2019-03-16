using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NSharding.DataAccess.Core
{
    /// <summary>
    /// ***类说明***
    /// </summary>
    /// <remarks>类的补充说明</remarks>
    public class QName
    {
        // Fields
        private string _localName;
        private string _prefix;

        // Methods
        public QName()
        {
        }

        public QName(string localName, string prefix)
        {
            this._localName = localName;
            this._prefix = prefix;
        }

        // Properties
        public string LocalName
        {
            get
            {
                return this._localName;
            }
            set
            {
                this._localName = value;
            }
        }

        public string Name
        {
            get
            {
                return (this._prefix + ":" + this._localName);
            }
        }

        public string Prefix
        {
            get
            {
                return this._prefix;
            }
            set
            {
                this._prefix = value;
            }
        }
    }
}
