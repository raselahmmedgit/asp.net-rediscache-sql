using GCSideLoading.Core.EntityModel;
using GCSideLoading.Core.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core.DAL
{
    public class GCMiscRepository : BaseRepository<GCMisc>
    {
        public GCMiscRepository() : base(typeof(GCMisc).Name)
        {

        }
        public async Task<bool> insertNewRedisKey(string redisKey)
        {
            try
            {
                var item = await GetItemAsync(c => c.ConfigDataType == AppConstants.ConfigDataType.InsertedRedisKeyToCosmos);
                if (item == null)
                {
                    GCMisc gCMisc = new GCMisc();
                    gCMisc.ConfigDataType = AppConstants.ConfigDataType.InsertedRedisKeyToCosmos;
                    gCMisc.DataList = new List<string>()
                    {
                        redisKey
                    };
                    await CreateItemAsync(gCMisc);
                }
                else
                {
                    var dataList = item.DataList;
                    if(dataList == null)
                    {
                        dataList = new List<string>();
                    }
                    if(!dataList.Contains(redisKey))
                    {
                        dataList.Add(redisKey);
                    }
                    item.DataList = dataList;
                    await UpdateItemAsync(item.Id, item);
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
