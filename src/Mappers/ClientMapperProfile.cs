using System.Security.Claims;
using AutoMapper;
using IdentityServer4.Models;

namespace IdentityServer4.RavenDB.Storage.Mappers
{
    public class ClientMapperProfile : Profile
    {
        public ClientMapperProfile()
        {
            CreateMap<Entities.Client, Models.Client>()
                .ForMember(dest => dest.ProtocolType, opt => opt.Condition(srs => srs != null))
                .ForMember(x => x.AllowedIdentityTokenSigningAlgorithms, opts => opts.ConvertUsing(AllowedSigningAlgorithmsConverter.Converter, x => x.AllowedIdentityTokenSigningAlgorithms))
                .ReverseMap()
                .ForMember(x => x.AllowedIdentityTokenSigningAlgorithms, opts => opts.ConvertUsing(AllowedSigningAlgorithmsConverter.Converter, x => x.AllowedIdentityTokenSigningAlgorithms));

            CreateMap<Entities.ClientClaim, ClientClaim>(MemberList.None)
                .ConstructUsing(src => new ClientClaim(src.Type, src.Value, ClaimValueTypes.String))
                .ReverseMap();
        }
    }
}
