using Game.Infrastructure.Database.Context;
using Game.Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Database.Repositories
{
    public class BaseRepository<T> where T : BaseEntity
    {
        private DatabaseContext _db;

        public BaseRepository(DatabaseContext db) => _db = db;

        public T? GetAsNoTracking(Guid idToFind)
        {
            IQueryable<T> query = _db.Set<T>();
            T? result = query.AsNoTracking().FirstOrDefault(record => record.Id == idToFind);

            return result;
        }

        public T? Get(Guid idToFind)
        {
            IQueryable<T> query = _db.Set<T>();
            T? result = query.FirstOrDefault(record => record.Id == idToFind);

            return result;
        }

        public List<T> GetAll()
        {
            IQueryable<T> query = _db.Set<T>();
            List<T> result = query.Where(record => record.IsDeleted == false).ToList();

            return result;
        }

        public T Save(T entityObject)
        {
            if (!RecordExists(entityObject.Id))
            {
                _db.Entry(entityObject).State = EntityState.Added;
                entityObject.Updated = DateTime.UtcNow;
                entityObject.Created = entityObject.Updated;
            }
            else
                Add(entityObject);

            _db.SaveChanges();

            return entityObject;
        }

        public void Delete(Guid id)
        {
            if (RecordExists(id))
            {
                IQueryable<T> query = _db.Set<T>();
                T result = query.First(record => record.Id == id);

                _db.Entry(result).State = EntityState.Modified;
                result.IsDeleted = true;
                result.Updated = DateTime.UtcNow;
                _db.SaveChanges();
            }
        }

        public bool RecordExists(Guid? idToFind)
        {
            var result = false;
            if (idToFind == null) return result;

            if (_db.Set<T>().Find(idToFind) != null)
                result = true;
            return result;
        }

        public Guid Add(T entityObject)
        {
            _db.Add(entityObject);
            _db.SaveChanges();

            return entityObject.Id;
        }
    }
}
