using AutoMapper;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;

namespace Common.Services.Implementation
{
    public class ServiceGeneric<TEntity, TEntityRead, TEntityCreate,TEntityUpdate> :
        IServiceGeneric<TEntityRead, TEntityCreate, TEntityUpdate>
        where TEntity : class
        where TEntityCreate : class
        where TEntityRead : class
        where TEntityUpdate : class
    {
        private readonly IRepository<TEntity> _work;
        private readonly IMapper _mapper;

        public ServiceGeneric(IRepository<TEntity> work, IMapper mapper)
        {
            _work = work;
            _mapper = mapper;
        }

        public async Task<object> Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new Exception("L'id ne peut pas être un Guid Vide");
            }
            var obj = await _work.GetById(id);
            return _mapper.Map<TEntityRead>(obj);
        }

        public async Task<IEnumerable<TEntityRead>> Get()
        {
            var obj = await _work.GetListAsync();
            return _mapper.Map<IEnumerable<TEntityRead>>(obj);
        }

        public async Task<object> Create(TEntityCreate entity)
        {
            var model = _mapper.Map<TEntity>(entity);
            await _work.AddAsync(model);
            return await _work.Complete();
            
        }

        public async Task<object> Update(Guid key, Delta<TEntityUpdate> entity)
        {
            if (key == Guid.Empty)
            {
                throw new Exception("L'Id ne peux pas être un guid vide!");
            }
            var entityToUpdate = await _work.GetById(key);
            if (entityToUpdate == null)
            {
                throw new Exception($"Entité avec l'ID {key} n'existe pas dans la base de donneés veuillez verifier!");
            }
            var entityDto = _mapper.Map<TEntityUpdate>(entityToUpdate);
            entity.Patch(entityDto);
            _mapper.Map(entityDto, entityToUpdate);
            return await _work.Complete();
        }

        public async Task<object> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new Exception("L'id ne peux pas être un Guid vide!");
            }
            var entity = await _work.GetById(id);
            if (entity == null)
            {
                throw new Exception("Entité avec l'ID {id} n'existe pas dans la base de donneés!");
            }
            _work.Remove(entity);
            return await _work.Complete();
        }
    }
}