using AutoMapper;
using System;
using UrlShortenerAPI.DTOs;
using UrlShortenerAPI.Models;

namespace UrlShortenerAPI.Mappings
{
    // 1. MUST inherit from AutoMapper's "Profile" class
    public class UrlMappingProfile : Profile
    {
        public UrlMappingProfile()
        {
            // The translation: CreateMap<Source, Destination>()
            // Translating FROM the Database Entity TO the Response DTO
            CreateMap<ShortenedUrl, UrlResponseDto>();
            CreateMap<UpdateUrlDto, ShortenedUrl>();
        }
    }
}
