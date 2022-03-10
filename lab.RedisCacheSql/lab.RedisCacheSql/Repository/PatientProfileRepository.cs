using lab.RedisCacheSql.Config;
using lab.RedisCacheSql.Data;
using lab.RedisCacheSql.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab.RedisCacheSql.Repository
{
    public class PatientProfileRepository : IPatientProfileRepository
    {
        private AppDbContext _context;
        public PatientProfileRepository()
        {
            _context = new AppDbContext();
        }
        public PatientProfileRepository(AppDbContext context)
        {
            _context = context;
        }
        public PatientProfileRepository(IDbContextFactory<AppDbContext> factory)
        {
            _context = factory.CreateDbContext();
        }
        
        public async Task<PatientProfile> GetPatientProfileAsync(long patientProfileId)
        {
            return await _context.PatientProfile.SingleOrDefaultAsync(x => x.PatientProfileId == patientProfileId);
        }

        public async Task<IEnumerable<PatientProfile>> GetPatientProfilesAsync()
        {
            return await _context.PatientProfile.ToListAsync();
        }

        public async Task<int> InsertOrUpdatetPatientProfileAsync(PatientProfile model)
        {
            if (model.PatientProfileId > 0)
            {
                await _context.PatientProfile.AddAsync(model);
            }
            else
            {
                _context.PatientProfile.Update(model);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> InsertPatientProfileAsync(PatientProfile model)
        {
            try
            {
                if (model.PatientProfileId > 0)
                {
                    await _context.PatientProfile.AddAsync(model);
                }
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdatePatientProfileAsync(PatientProfile model)
        {
            if (model.PatientProfileId > 0)
            {
                _context.PatientProfile.Update(model);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeletePatientProfileAsync(PatientProfile model)
        {
            if (model.PatientProfileId > 0)
            {
                _context.PatientProfile.Remove(model);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> InsertPatientProfileAsync(List<PatientProfile> PatientProfileList)
        {
            try
            {
                // Create an instance and save the entity to the database

                _context.AddRange(PatientProfileList);

                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    public interface IPatientProfileRepository
    {
        Task<PatientProfile> GetPatientProfileAsync(long patientProfileId);
        Task<IEnumerable<PatientProfile>> GetPatientProfilesAsync();
        Task<int> InsertOrUpdatetPatientProfileAsync(PatientProfile model);
        Task<int> InsertPatientProfileAsync(PatientProfile model);
        Task<int> UpdatePatientProfileAsync(PatientProfile model);
        Task<int> DeletePatientProfileAsync(PatientProfile model);
        Task<int> InsertPatientProfileAsync(List<PatientProfile> PatientProfileList);
    }
}
