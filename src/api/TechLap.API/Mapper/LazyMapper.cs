using AutoMapper;
using TechLap.API.Mapper.MappingProfiles;

namespace TechLap.API.Mapper
{
    public static class LazyMapper
    {
        private readonly static Lazy<IMapper> _lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod?.IsPublic == true || p.GetMethod?.IsAssembly == true;

                cfg.AddProfile<ProductMappingProfile>();
            });

            return config.CreateMapper();
        });

        public static IMapper Mapper => _lazy.Value;
    }
}
