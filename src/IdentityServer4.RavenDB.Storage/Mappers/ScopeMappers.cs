using AutoMapper;
using IdentityServer4.RavenDB.Storage.Entities;

namespace IdentityServer4.RavenDB.Storage.Mappers
{
    /// <summary>
    /// Extension methods to map to/from entity/model for scopes.
    /// </summary>
    public static class ScopeMappers
    {
        private static readonly IMapper _mapper;

        static ScopeMappers()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ScopeMapperProfile>())
                .CreateMapper();
        }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Models.ApiScope ToModel(this ApiScope entity)
        {
            return entity == null ? null : _mapper.Map<IdentityServer4.Models.ApiScope>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static ApiScope ToEntity(this Models.ApiScope model)
        {
            return model == null ? null : _mapper.Map<ApiScope>(model);
        }
    }
}
