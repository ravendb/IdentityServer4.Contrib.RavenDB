using System.Runtime.CompilerServices;
using AutoMapper;
using IdentityServer4.RavenDB.Storage.Entities;

[assembly: InternalsVisibleTo("IdentityServer4.RavenDB.IntegrationTests")]
namespace IdentityServer4.RavenDB.Storage.Mappers
{
    internal static class ApiResourceMappers
    {
        private static readonly IMapper _mapper;
        
        static ApiResourceMappers()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
                .CreateMapper();
        }

        public static ApiResource ToEntity(this Models.ApiResource model)
        {
            return model == null ? null : _mapper.Map<ApiResource>(model);
        }

        public static Models.ApiResource ToModel(this ApiResource entity)
        {
            return entity == null ? null : _mapper.Map<Models.ApiResource>(entity);
        }
    }
}
