using DAL.Context;
using DAL.Models;
using DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class AssetRepository : Repository<Asset>, IAssetRepository
    {
        private readonly ChallengePpiContext _context;
        public AssetRepository(ChallengePpiContext context) : base(context)
        {
            _context = context;
        }
    }
}
