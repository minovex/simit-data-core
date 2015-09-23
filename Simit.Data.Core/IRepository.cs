namespace Simit.Data.Core
{
    #region Using Directives
    using System.Collections.Generic;
    using System.Data;
    #endregion

    public interface IRepository<TEntity, TID> 
        where TEntity : class
        where TID : struct
    {
        #region Body
        void Create(List<TEntity> items, IDbTransaction transaction = null);
        void Create(TEntity item, IDbTransaction transaction = null);
        void Update(TEntity item, IDbTransaction transaction = null);
        void Update(List<TEntity> items, IDbTransaction transaction = null);
        void Delete(List<TID> idList, IDbTransaction transaction = null);
        void Delete(TID id, IDbTransaction transaction = null);
        #endregion
    }
}