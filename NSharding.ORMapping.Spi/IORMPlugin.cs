using NSharding.DataAccess.Spi;
using NSharding.DomainModel.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSharding.ORMapping.Spi
{
    public interface IORMPlugin
    {
        object MapToObject(QueryResultSet resultSet, NSharding.DomainModel.Spi.DomainModel model, DomainObject domainObject);

        List<object> MapToObjects(QueryResultSet resultSet, NSharding.DomainModel.Spi.DomainModel model, DomainObject domainObject);
    }
}
