﻿using MagicVillaApi.Data;
using MagicVillaApi.Models.Villa;
using MagicVillaApi.Models.VillaNumber;
using MagicVillaApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVillaApi.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber> , IVillaNumberRepository
    {
        private readonly ApplicationDbContext _db;
        public VillaNumberRepository(ApplicationDbContext db) :base(db) 
        {
                _db= db;
        }

        public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
        {
            entity.UpdatedDate=DateTime.Now;
            _db.VillaNumbers.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
      


    }
}
