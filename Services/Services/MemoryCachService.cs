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
        public async Task AddReceiversToCache(List<EmailRecord> item)
        {
            try
            {
                var key = Resource.CacheKeyOfReceivers;
                var existingReceivers = await _cache.GetStringAsync(key);

                HashSet<string> existingUsers;

                if (string.IsNullOrEmpty(existingReceivers))
                {
                    existingUsers = new HashSet<string>();
                }
                else
                {
                    existingUsers = JsonConvert.DeserializeObject<HashSet<string>>(existingReceivers);
                }
                foreach (var user in item)
                {
                    if (!existingUsers.Contains((user.Id).ToString()))
                        existingUsers.Add((user.Id).ToString());
                }
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddDays(10)
                };

                // Serialize the HashSet to JSON before storing it
                var serializedOnlineUsers = JsonConvert.SerializeObject(existingUsers);
                await _cache.SetStringAsync(key, serializedOnlineUsers, options);

            }
            catch (Exception ex)
            {
                throw new Exception(Resource.GeneralErrorTryAgain, ex);
            }
        }

        public async Task<List<string>> GetReceiversFromCache(string key)
        {
            try
            { 
                var existingReceivers = await _cache.GetStringAsync(key);

                HashSet<string> existingUsers;

                if (string.IsNullOrEmpty(existingReceivers))
                {
                    existingUsers = new HashSet<string>();
                }
                else
                {
                    existingUsers = JsonConvert.DeserializeObject<HashSet<string>>(existingReceivers);
                }

                if (!existingUsers.Any())
                    return null;
                    
                    return existingUsers.ToList(); 
            }
            catch (Exception ex)
            {
                throw new Exception(Resource.GeneralErrorTryAgain, ex);
            }
        }

    }
}
