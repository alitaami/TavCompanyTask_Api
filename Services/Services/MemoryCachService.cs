using Common.Resources;
using Entities.Models;
using Entities.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Services.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class MemoryCachService : IMemoryCachService
    {
        private readonly IDistributedCache _cache;
        private readonly CacheSettings _cacheSettings;
        public MemoryCachService(IDistributedCache cache, IOptions<CacheSettings> cacheSettings)
        {
            _cache = cache;
            _cacheSettings = cacheSettings.Value;
        }
        public async Task  AddDataToCache(List<string> item,string key)
        {
            try
            { 
                var existingData = await _cache.GetStringAsync(key);

                HashSet<string> res;

                if (string.IsNullOrEmpty(existingData))
                {
                    res = new HashSet<string>();
                }
                else
                {
                    res = JsonConvert.DeserializeObject<HashSet<string>>(existingData);
                }
                foreach (var i in item)
                {
                    if (!res.Contains(i))
                        res.Add(i);
                }
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddDays(10)
                };

                // Serialize the HashSet to JSON before storing it
                var serializedOnlineUsers = JsonConvert.SerializeObject(res);
                await _cache.SetStringAsync(key, serializedOnlineUsers, options);
            }
            catch (Exception ex)
            {
                throw new Exception(Resource.GeneralErrorTryAgain, ex);
            }
        }

        public async Task<List<string>> GetDataFromCache(string key)
        {
            try
            { 
                var Data = await _cache.GetStringAsync(key);

                HashSet<string> existingData;

                if (string.IsNullOrEmpty(Data))
                {
                    existingData = new HashSet<string>();
                }
                else
                {
                    existingData = JsonConvert.DeserializeObject<HashSet<string>>(Data);
                }

                if (!existingData.Any())
                    return null;
                    
                    return existingData.ToList(); 
            }
            catch (Exception ex)
            {
                throw new Exception(Resource.GeneralErrorTryAgain, ex);
            }
        }

    }
}
