using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.DataAccess.Service
{
    /// <summary>
    /// 结果集映射服务
    /// </summary>
    public class ResultMappingService
    {
        /// <summary>
        /// 获取领域模型结果集映射
        /// </summary>
        /// <param name="model">领域模型</param>
        /// <returns>领域模型结果集映射</returns>
        public ResultMapping GetResultMapping(DomainModel.Spi.DomainModel model)
        {
            if (model == null)
                throw new ArgumentNullException("ResultMappingService.GetResultMapping.model");

            return ResultMappingFactory.GetInstance().CreateOrGetResultMapping(model);
        }
    }
}
