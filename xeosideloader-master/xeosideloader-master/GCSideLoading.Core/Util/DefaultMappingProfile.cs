using AutoMapper;
using GCSideLoading.Core.EnitityModel;
using GCSideLoading.Core.ViewModel;

namespace GCSideLoading.Core
{
    public class DefaultMappingProfile : Profile
    {
        public DefaultMappingProfile()
        {
            
            CreateMap<ClientProfile, ClientProfileViewModel>();
            CreateMap<ClientProfileViewModel, ClientProfile>();

        }
    }

}
