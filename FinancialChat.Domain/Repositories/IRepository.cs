using FinancialChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinancialChat.Domain.Repositories
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity> FindByConditionReadOnlyFirstOrDefault(Expression<Func<TEntity, bool>> expression);
    }
}
