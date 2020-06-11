using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Repository.Infrastructure
{
    public abstract class Repository<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class
    {
        #region Ctor
        private readonly DbContext _db;
        private readonly DbSet<TEntity> _dbSet;
        public Repository(DbContext db)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>();
        }
        #endregion

        #region normal
        public void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("there is not entity");
            _db.Add(entity);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("there is not entity");
            _db.Update(entity);
        }

        public void Delete(object id)
        {
            TEntity entity = GetById(id);
            if (entity == null)
                throw new ArgumentException("there is not entity");
            _dbSet.Remove(entity);
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("there is not entity");
            _dbSet.Remove(entity);
        }

        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            IEnumerable<TEntity> entitys = _dbSet.Where(where).AsEnumerable();
            foreach (TEntity item in entitys)
            {
                if (item != null)
                {
                    _dbSet.Remove(item);
                }
            }
        }

        public TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.AsEnumerable();
        }

        public IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> filter = null,
                           Func<IQueryable<TEntity>,IOrderedQueryable<TEntity>> orderBy = null , 
                           string includeEntity = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includeEntity.Split(new char[] { ','} , StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(include);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }

        }

        public TEntity Get(Expression<Func<TEntity, bool>> where)
        {
            return _dbSet.Where(where).FirstOrDefault();
        }
        #endregion

        #region Async
        public async Task InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("there is not entity");
            await _db.AddAsync(entity);
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> filter = null,
                           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                           string includeEntity = "")
        {
            //return await _dbSet.Where(where).FirstOrDefaultAsync();

            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includeEntity.Split(new char[] { ','} , StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(include);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbSet.Where(where).FirstOrDefaultAsync();
        }
        #endregion

        #region Dispose
        private bool dispose = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!dispose)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            dispose = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~Repository()
        {
            Dispose(false);
        }
        #endregion
    }
}
