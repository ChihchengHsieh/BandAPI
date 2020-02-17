using AutoMapper;
using BandAPI.Entities;
using BandAPI.Helpers;
using BandAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.Profiles
{
    public class BandsProfile : Profile
    {
        public BandsProfile()
        {
            CreateMap<Band, BandDto>().ForMember( // Defining the Map function
                dest => dest.FoundedYearsAgo,
                opt => opt.MapFrom(src => $"{src.Founded.ToString("yyyy")} ({src.Founded.GetYearsAgo()} years ago)")
                );

            CreateMap<BandForCreatingDto, Band>();
        }
    }
}
