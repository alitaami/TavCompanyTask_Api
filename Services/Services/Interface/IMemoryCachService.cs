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
        public Task AddReceiversToCache(List<EmailRecord> item);
        public Task<List<string>> GetReceiversFromCache(string key);
    }
}
