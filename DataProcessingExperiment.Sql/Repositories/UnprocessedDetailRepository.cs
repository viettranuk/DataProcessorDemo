﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProcessingExperiment.Models.DTOs;
using DataProcessingExperiment.Services.Interfaces;
using DataProcessingExperiment.Services.Interfaces.IRepositories;

namespace DataProcessingExperiment.Sql.Repositories
{
    public partial class BaseReadOnlyRepository<TContext> : IBaseReadOnlyRepository 
        where TContext : DbContext
    {
        public List<UnprocessedDetailDto> GetUnprocessedDetailsByFileId(int fileId)
        {
            try
            {
                var allUnprocessedDetails =
                    GetQueryable<UnprocessedDetail>(f => f.FileId == fileId)
                        .Select(f => f.ToUnprocessedDetailDto())
                        .ToList();

                return allUnprocessedDetails;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return null;
            }
        }
    }

    public partial class BaseRepository<TContext> : BaseReadOnlyRepository<TContext>, IBaseRepository 
        where TContext : DbContext
    {
        public async Task AddUnprocessedDetailAsync(string unprocessedValues)
        {
            try
            {
                _context.Configuration.AutoDetectChangesEnabled = false;

                await _context.Database.ExecuteSqlCommandAsync(
                    "Insert Into dbo.UnprocessedDetails (FileId, LineData) Values " + unprocessedValues);

                await _context.SaveChangesAsync();

                _context.Dispose();
                //context = new TContext();
                //context.Configuration.AutoDetectChangesEnabled = false;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
            }
        }        
    }
}