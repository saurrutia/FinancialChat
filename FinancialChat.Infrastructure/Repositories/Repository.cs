using FinancialChat.Domain.Entities;
using FinancialChat.Domain.Repositories;
using FinancialChat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinancialChat.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly FinancialChatDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(FinancialChatDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> FindByConditionReadOnlyFirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbSet.Where(expression).AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
