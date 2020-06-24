using AutoMapper;
using IdentityServer4.RavenDB.Storage.Entities;

namespace IdentityServer4.RavenDB.Storage.Mappers
{
    public class ApiResourceMapperProfile : Profile
    {
        public ApiResourceMapperProfile()
        {
            CreateMap<ApiResource, Models.ApiResource>(MemberList.Destination)
                .ConstructUsing(src => new Models.ApiResource())
                .ForMember(x => x.ApiSecrets, opts => opts.MapFrom(x => x.Secrets))
                //.ForMember(x => x.AllowedAccessTokenSigningAlgorithms, opts => opts.ConvertUsing(AllowedSigningAlgorithmsConverter.Converter, x => x.AllowedAccessTokenSigningAlgorithms))
                .ReverseMap()
                //.ForMember(x => x.AllowedAccessTokenSigningAlgorithms, opts => opts.ConvertUsing(AllowedSigningAlgorithmsConverter.Converter, x => x.AllowedAccessTokenSigningAlgorithms))
                ;

            CreateMap<Secret, Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();
        }
    }
}
