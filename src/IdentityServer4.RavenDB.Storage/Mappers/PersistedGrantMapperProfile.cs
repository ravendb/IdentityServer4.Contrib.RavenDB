using AutoMapper;
using IdentityServer4.RavenDB.Storage.Helpers;

namespace IdentityServer4.RavenDB.Storage.Mappers
{
    /// <summary>
    /// Defines entity/model mapping for persisted grants.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    internal class PersistedGrantMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="PersistedGrantMapperProfile">
        /// </see>
        /// </summary>
        public PersistedGrantMapperProfile()
        {
            CreateMap<Entities.PersistedGrant, Models.PersistedGrant>(MemberList.Destination)
                .ReverseMap()
                .ForMember(x => x.Key, options => 
                    options.MapFrom(x => CryptographyHelper.CreateHash(x.Key)));
        }
    }
}
