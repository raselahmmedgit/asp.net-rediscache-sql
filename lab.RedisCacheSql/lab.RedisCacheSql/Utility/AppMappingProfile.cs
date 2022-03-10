using AutoMapper;
using lab.RedisCacheSql.Models;
using lab.RedisCacheSql.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace lab.RedisCacheSql.Utility
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<PatientProfile, PatientProfileViewModel>();
            CreateMap<PatientProfileViewModel, PatientProfile>();
        }
    }

}
