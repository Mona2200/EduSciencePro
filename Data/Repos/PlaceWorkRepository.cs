﻿using AutoMapper;
using EduSciencePro.Models.User;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
    public class PlaceWorkRepository : IPlaceWorkRepository
    {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;

      public PlaceWorkRepository(ApplicationDbContext db, IMapper mapper)
      {
         _db = db;
         _mapper = mapper;
      }

      public async Task<PlaceWork> GetPlaceWorkByName(string name) => await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Name == name);

      public async Task<PlaceWork> GetPlaceWorkById(Guid id) => await _db.PlaceWorks.FirstOrDefaultAsync(p => p.Id == id);

      public async Task<PlaceWork[]> GetPlaceWorks() => await _db.PlaceWorks.ToArrayAsync();

      public async Task<PlaceWork[]> GetPlaceWorksSearch(string search) => await _db.PlaceWorks.Where(p => p.Name.ToLower().Contains(search.ToLower())).ToArrayAsync();
   }

    public interface IPlaceWorkRepository
    {
      Task<PlaceWork> GetPlaceWorkByName(string name);
      Task<PlaceWork> GetPlaceWorkById(Guid id);
      Task<PlaceWork[]> GetPlaceWorks();
      Task<PlaceWork[]> GetPlaceWorksSearch(string search);
    }
}
