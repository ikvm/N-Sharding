﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DomainModel.Spi
{
    /// <summary>
    /// 结果集映射项类型枚举
    /// </summary>
    public enum ResultMappingItemType
    {
        /// <summary>
        /// 通用
        /// </summary>
        Normal,

        /// <summary>
        /// 子对象关联
        /// </summary>
        SubResultMapping,

        /// <summary>
        /// 外部对象关联
        /// </summary>
        ForeignResultMapping,

        /// <summary>
        /// 枚举
        /// </summary>
        Enum,

        /// <summary>
        /// 虚拟
        /// </summary>
        Virtual
    }
}
