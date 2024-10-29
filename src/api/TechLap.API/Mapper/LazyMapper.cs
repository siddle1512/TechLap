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
                cfg.AddProfile<DiscountMappingProfile>();
                cfg.AddProfile<UserMappingProfile>();
                cfg.AddProfile<OrderMappingProfile>();
                cfg.AddProfile<ChatMessageMappingProfile>();
                cfg.AddProfile<CustomerMappingProfile>();
                cfg.AddProfile<CategoryMappingProfile>();
            });

            return config.CreateMapper();
        });

        public static IMapper Mapper => _lazy.Value;
    }
}
