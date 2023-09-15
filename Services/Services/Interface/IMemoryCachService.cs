using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Interface
{
    public interface IMemoryCachService
    {
        public Task AddDataToCache(List<string> item, string key);
        public Task<List<string>> GetDataFromCache(string key);
    }
}
