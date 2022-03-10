using GCSideLoading.Core.DAL;
using GCSideLoading.Core.EnitityModel;
using GCSideLoading.Core.EntityModel;
using GCSideLoading.Core.Util;
using log4net;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCSideLoading.Core.BLL.Redis
{
    public class RedisDataReaderManager
    {
        private LeaderboardRepository leaderboardRepository;
        private GameDataRepository gameDataRepository;
        private AwardedCouponRepository awardedCouponRepository;
        private GCMiscRepository miscRepository;
        private ILog log;
        long totalLeaderboardEntry = 0;
        long totalGameDataEntry = 0;
        long totalAwardedCouponEntry = 0;
        public RedisDataReaderManager()
        {
            leaderboardRepository = new LeaderboardRepository();
            gameDataRepository = new GameDataRepository();
            awardedCouponRepository = new AwardedCouponRepository();
            miscRepository = new GCMiscRepository();
            log = LogManager.GetLogger(typeof(RedisDataReaderManager));
        }
        public async Task<bool> readRedisDataAndStoreCosmosDB()
        {
            try
            {
                log.Info("readRedisDataAndStoreCosmosDB started");
                IServer server = RedisConnectionManager.GetServer();
                IDatabase database = RedisConnectionManager.GetDatabase();
                long totalKey = 0;
                totalLeaderboardEntry = 0;
                totalGameDataEntry = 0;
                totalAwardedCouponEntry = 0;
                var keys = await getKeyList(server) ;
                foreach (var key in keys)
                {
                    try
                    {
                        totalKey++;
                        var keyType = database.KeyType(key);
                        string redisKey = key.ToString();
                        log.Debug("key: " + redisKey + " , Key Type: " + keyType);
                        var keyDataList = redisKey.Split('.');
                        if (keyDataList[1] == "leaderboards")
                        {
                            await insertLeaderBoardData(keyDataList, redisKey, keyType, database);
                        }
                        else if (keyDataList[2] == "awarded-coupons")
                        {
                            await InsertAwardedCouponData(keyDataList, redisKey, keyType, database);
                        }
                        else
                        {
                            await insertGameData(keyDataList, redisKey, keyType, database);
                        }
                    }
                    catch(Exception ex)
                    {
                        log.Error("invalid key exception: "+ex);
                    }

                }
                log.Info("TotalKey: " + totalKey);
                log.Info("totalLeaderboardEntry: " + totalLeaderboardEntry);
                log.Info("totalGameDataEntry: " + totalGameDataEntry);
                log.Info("totalAwardedCouponEntry: " + totalAwardedCouponEntry);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return true;
        }
        public async Task<bool> insertLeaderBoardData(String[] keyDataList, string key, RedisType keyType, IDatabase database)
        {
            try
            {
                GCLeaderboard leaderboard = new GCLeaderboard();
                leaderboard.MappedRedisKey = key;
                leaderboard.RedisKeyType = keyType.ToDescriptionAttr(); 
                leaderboard.Cid = keyDataList[0];
                if (keyDataList.Length > 2)
                {
                    leaderboard.Gid = keyDataList[2];
                }
                if (keyDataList.Length > 3)
                {
                    leaderboard.Sid = keyDataList[3];
                }
                if(keyDataList.Length>4)
                {
                    leaderboard.DataType = keyDataList[4];
                }
                bool isExist = leaderboardRepository.checkIfItemExistByRedisKey(leaderboard);
                if(isExist)
                {
                    await miscRepository.insertNewRedisKey(key);
                    return true;
                }

                ICollection<String> ListDatas = null;
                ICollection<SortedListData> SortedListDatas = null;
                ICollection<HashListData> HashListDatas = null;

                if (keyType == RedisType.Hash)
                {
                    //RedisValue[] hashEntries = cache.HashValues(key);
                    RedisValue[] hashkeys = database.HashKeys(key);
                    ICollection<Award> awards = new List<Award>();
                    ICollection<GCUser> GCUsers = new List<GCUser>();
                    HashListDatas = new List<HashListData>();
                    for (int i = 0; i < hashkeys.Length; i++)
                    {
                        var hashKey = hashkeys[i];
                        var hashData = database.HashGet(key, hashKey);
                        log.Debug("hash key: " + hashKey + ", Hash Value: " + hashData);
                        if (keyDataList.Length > 4)
                        {
                            if (keyDataList[4] == "data")
                            {
                                var user = JsonConvert.DeserializeObject<GCUser>(hashData);
                                user.uid = hashKey;
                                GCUsers.Add(user);
                                leaderboard.UserData = GCUsers;
                            }
                            else if (keyDataList[4] == "award-status")
                            {
                                Award award = new Award();
                                award.Uid = hashKey;
                                award.AwardStatus = hashData;
                                awards.Add(award);
                                leaderboard.AwardStatus = awards;
                            }
                            else
                            {
                                HashListData hashListData = new HashListData();
                                hashListData.key = hashKey;
                                hashListData.value = hashData;
                                HashListDatas.Add(hashListData);
                            }
                        }
                        else
                        {
                            HashListData hashListData = new HashListData();
                            hashListData.key = hashKey;
                            hashListData.value = hashData;
                            HashListDatas.Add(hashListData);
                        }
                    }
                }
                else if (keyType == RedisType.List)
                {
                    ListDatas = new List<string>();
                    RedisValue[] hashEntries = database.ListRange(key);
                    for (int i = 0; i < hashEntries.Length; i++)
                    {
                        var value = hashEntries[i];
                        ListDatas.Add(value);
                        log.Debug("Redis Value: " + value);
                    }
                }
                else if (keyType == RedisType.Set)
                {
                    ListDatas = new List<string>();
                    RedisValue[] hashEntries = database.SetMembers(key);
                    for (int i = 0; i < hashEntries.Length; i++)
                    {
                        var value = hashEntries[i];
                        ListDatas.Add(value);
                        log.Debug("Redis Value: " + value);
                    }
                }
                else if (keyType == RedisType.SortedSet)
                {
                    SortedListDatas = new List<SortedListData>();
                    SortedSetEntry[] hashEntries = database.SortedSetRangeByRankWithScores(key);
                    for (int i = 0; i < hashEntries.Length; i++)
                    {
                        var entry = hashEntries[i];
                        SortedListData sortedListData = new SortedListData();
                        sortedListData.key = entry.Element;
                        sortedListData.value = entry.Score;
                        SortedListDatas.Add(sortedListData);
                        log.Debug("SortedSetEntry Element: " + entry.Element + ", score: " + entry.Score);

                    }
                }
                else if (keyType == RedisType.String)
                {
                    ListDatas = new List<string>();
                    string value = database.StringGet(key);
                    ListDatas.Add(value);
                    log.Debug("value: " + value);

                }
                leaderboard.ListData = ListDatas;
                leaderboard.SortedListData = SortedListDatas;
                leaderboard.HashListData = HashListDatas;
                totalLeaderboardEntry++;
                await leaderboardRepository.CreateItemAsync(leaderboard);
                await miscRepository.insertNewRedisKey(key);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return true;
        }
        public async Task<bool> insertGameData(String[] keyDataList, string key, RedisType keyType, IDatabase database)
        {
            try
            {
                GCGameData gameData = new GCGameData();
                gameData.MappedRedisKey = key;
                gameData.RedisKeyType = keyType.ToDescriptionAttr();
                gameData.Cid = keyDataList[0];
                if(keyDataList.Length > 1)
                {
                    gameData.Gid = keyDataList[1];
                }
                if (keyDataList.Length > 2)
                {
                    gameData.Sid = keyDataList[2];
                }
                if (keyDataList.Length > 3)
                {
                    gameData.DataType = keyDataList[3];
                    if (keyDataList[3] == "card")
                    {
                        if (keyDataList.Length > 4)
                        {
                            gameData.CardId = keyDataList[4];
                        }
                        if (keyDataList.Length > 5)
                        {
                            gameData.SubmitedCardAction = keyDataList[5];
                        }
                    }
                }

                bool isExist = gameDataRepository.checkIfItemExistByRedisKey(gameData);
                if (isExist)
                {
                    await miscRepository.insertNewRedisKey(key);
                    return true;
                }

                ICollection<String> ListDatas = null;
                ICollection<SortedListData> SortedListDatas = null;
                ICollection<HashListData> hashListDatas = null;

                if (keyType == RedisType.Hash)
                {
                    //RedisValue[] hashEntries = cache.HashValues(key);
                    hashListDatas = new List<HashListData>();
                    RedisValue[] hashkeys = database.HashKeys(key);
                    for (int i = 0; i < hashkeys.Length; i++)
                    {
                        var hashKey = hashkeys[i];
                        var hashData = database.HashGet(key, hashKey);
                        log.Debug("hash key: " + hashKey + ", Hash Value: " + hashData);
                        HashListData userAnswer = new HashListData();
                        userAnswer.key = hashKey;
                        userAnswer.value = hashData;
                        hashListDatas.Add(userAnswer);
                    }
                }
                else if (keyType == RedisType.List)
                {
                    ListDatas = new List<string>();
                    RedisValue[] hashEntries = database.ListRange(key);
                    for (int i = 0; i < hashEntries.Length; i++)
                    {
                        var value = hashEntries[i];
                        log.Debug("Redis Value: " + value);
                        ////file.WriteLine("Redis Value: " + value);
                        ListDatas.Add(value);
                    }
                }
                else if (keyType == RedisType.Set)
                {
                    ListDatas = new List<string>();
                    RedisValue[] hashEntries = database.SetMembers(key);
                    for (int i = 0; i < hashEntries.Length; i++)
                    {
                        var value = hashEntries[i];
                        log.Debug("Redis Value: " + value);
                        ////file.WriteLine("Redis Value: " + value);
                        ListDatas.Add(value);
                    }
                }
                else if (keyType == RedisType.SortedSet)
                {
                    SortedListDatas = new List<SortedListData>();
                    SortedSetEntry[] hashEntries = database.SortedSetRangeByRankWithScores(key);
                    for (int i = 0; i < hashEntries.Length; i++)
                    {
                        var entry = hashEntries[i];
                        var element = entry.Element;
                        var score = entry.Score;
                        log.Debug("SortedSetEntry Element: " + element + ", score: " + score);
                        ////file.WriteLine("SortedSetEntry Element: " + element + ", score: " + score);
                        SortedListData userAnswer = new SortedListData();
                        userAnswer.key = element;
                        userAnswer.value = score;
                        SortedListDatas.Add(userAnswer);
                    }
                }
                else if (keyType == RedisType.String)
                {
                    ListDatas = new List<string>();
                    string value = database.StringGet(key);
                    log.Debug("value: " + value);
                    ////file.WriteLine("value: " + database.StringGet(key));
                    ListDatas.Add(value);
                }
                gameData.ListData = ListDatas;
                gameData.SortedListData = SortedListDatas;
                gameData.HashListData = hashListDatas;
                totalGameDataEntry++;
                await gameDataRepository.CreateItemAsync(gameData);
                await miscRepository.insertNewRedisKey(key);
            }
            catch (Exception ex)
            {
                log.Debug(ex);
            }
            return true;
        }
        public async Task<bool> InsertAwardedCouponData(String[] keyDataList, string key, RedisType keyType, IDatabase database)
        {
            try
            {
                GCAwardedCoupon awardedCoupon = new GCAwardedCoupon();
                awardedCoupon.MappedRedisKey = key;
                awardedCoupon.RedisKeyType = keyType.ToDescriptionAttr();
                awardedCoupon.Cid = keyDataList[0];
                if(keyDataList.Length > 1)
                {
                    awardedCoupon.Gid = keyDataList[1];
                }
                if (keyDataList.Length > 3)
                {
                    awardedCoupon.sid = keyDataList[3];
                }

                bool isExist = awardedCouponRepository.checkIfItemExistByRedisKey(awardedCoupon);
                if (isExist)
                {
                    await miscRepository.insertNewRedisKey(key);
                    return true;
                }

                ICollection<String> ListDatas = null;
                ICollection<SortedListData> SortedListDatas = null;
                ICollection<HashListData> hashListDatas = null;

                if (keyType == RedisType.Hash)
                {
                    hashListDatas = new List<HashListData>();
                    RedisValue[] hashkeys = database.HashKeys(key);
                    for (int i = 0; i < hashkeys.Length; i++)
                    {
                        var hashKey = hashkeys[i];
                        var value = database.HashGet(key, hashKey);
                        HashListData sortedListData = new HashListData();
                        sortedListData.key = hashKey;
                        sortedListData.value = value;
                        hashListDatas.Add(sortedListData);
                        log.Debug("hash key: " + hashKey + ", Hash Value: " + value);
                    }
                }
                else if (keyType == RedisType.List)
                {
                    RedisValue[] hashEntries = database.ListRange(key);
                    ICollection<GCCoupon> coupons = new List<GCCoupon>();
                    for (int i = 0; i < hashEntries.Length; i++)
                    {
                        var value = hashEntries[i];
                        log.Debug("Redis Value: " + value);
                        var coupon = JsonConvert.DeserializeObject<GCCoupon>(value);
                        coupons.Add(coupon);
                    }
                    awardedCoupon.GCCoupons = coupons;
                }
                else if (keyType == RedisType.Set)
                {
                    ListDatas = new List<string>();
                    RedisValue[] hashEntries = database.SetMembers(key);
                    for (int i = 0; i < hashEntries.Length; i++)
                    {
                        var value = hashEntries[i];
                        ListDatas.Add(value);
                        log.Debug("Redis Value: " + value);
                    }
                }
                else if (keyType == RedisType.SortedSet)
                {
                    SortedListDatas = new List<SortedListData>();
                    SortedSetEntry[] hashEntries = database.SortedSetRangeByRankWithScores(key);
                    for (int i = 0; i < hashEntries.Length; i++)
                    {
                        var entry = hashEntries[i];
                        SortedListData sortedListData = new SortedListData();
                        sortedListData.key = entry.Element;
                        sortedListData.value = entry.Score;
                        SortedListDatas.Add(sortedListData);
                        log.Debug("SortedSetEntry Element: " + entry.Element + ", score: " + entry.Score);

                    }
                }
                else if (keyType == RedisType.String)
                {
                    ListDatas = new List<string>();
                    string value = database.StringGet(key);
                    log.Debug("value: " + value);
                    ListDatas.Add(value);

                }
                awardedCoupon.ListData = ListDatas;
                awardedCoupon.SortedListData = SortedListDatas;
                awardedCoupon.HashListData = hashListDatas;
                totalAwardedCouponEntry++;
                await awardedCouponRepository.CreateItemAsync(awardedCoupon);
                await miscRepository.insertNewRedisKey(key);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return true;
        }
        public async Task<List<string>> getKeyList(IServer server)
        {
            try
            {
                var notInsertedKeyList = new List<string>();
                var redisKeys = getRedisKeys(server);
                if(redisKeys == null || redisKeys.Count == 0)
                {
                    return new List<string>();
                }
                var insertedKeyList = new List<string>();
                var totalColonKey = new List<string>();
                var totalActionKey = new List<string>();
                var totalLengthLess3Key = new List<string>();
                var keyListItem = await miscRepository.GetItemAsync(c => c.ConfigDataType == AppConstants.ConfigDataType.InsertedRedisKeyToCosmos);
                if (keyListItem != null)
                {
                    insertedKeyList = keyListItem.DataList;
                }
                foreach (var redisKey in redisKeys)
                {
                    if (redisKey.Contains(":"))
                    {
                        totalColonKey.Add(redisKey);
                        //file.Write("Action: Key not parsed because contains colon");
                        log.Debug("Action: Key not parsed because contains colon");
                    }
                    else if (redisKey.Contains("action"))
                    {
                        //file.WriteLine(" Action: Key not parsed because action entr");
                        totalActionKey.Add(redisKey);
                        log.Debug(" Action: Key not parsed because action entr");
                    }
                    else
                    {
                        try
                        {
                            String[] keyDataList = redisKey.Split('.');
                            if (keyDataList.Length >= 3)
                            {
                                if (!insertedKeyList.Contains(redisKey))
                                {
                                    notInsertedKeyList.Add(redisKey);
                                }
                            }
                            else
                            {
                                totalLengthLess3Key.Add(redisKey);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }
                }
                log.Info("totalkey: " + redisKeys.Count);
                log.Info("totalInsertedKey: " + insertedKeyList.Count);
                log.Info("totalNotInsertedKey: " + notInsertedKeyList.Count);
                log.Info("totalColonContainsKey: " + totalColonKey.Count);
                log.Info("totalActionContainsKey: " + totalActionKey.Count);
                //var keyListToInsertCosmos = await checkIsItemExist(notInsertedKeyList);
                return notInsertedKeyList;

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return new List<string>();
        }
        public List<string> getRedisKeys(IServer server)
        {
            List<string> redisKeys = new List<string>();
            try
            {
                var keys = server.Keys();
                foreach (var key in keys)
                {
                    redisKeys.Add(key.ToString());
                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
            return redisKeys;
        }
        public async Task<List<String>> checkIsItemExist(List<string> redisKey)
        {
            List<string> notInsertedKeys = new List<string>();
            List<string> insertedKeys = new List<string>();
            try
            {
                foreach(var key in redisKey)
                {
                    String[] keyDataList = key.Split('.');
                    if (keyDataList[1] == "leaderboards")
                    {
                        GCLeaderboard leaderboard = new GCLeaderboard();
                        leaderboard.Cid = keyDataList[0];
                        if (keyDataList.Length > 2)
                        {
                            leaderboard.Gid = keyDataList[2];
                        }
                        if (keyDataList.Length > 3)
                        {
                            leaderboard.Sid = keyDataList[3];
                        }
                        if (keyDataList.Length > 4)
                        {
                            leaderboard.DataType = keyDataList[4];
                        }
                        bool isExist = leaderboardRepository.checkIfItemExist(leaderboard);
                        if (!isExist)
                        {
                            notInsertedKeys.Add(key);
                        }
                        else
                        {
                            insertedKeys.Add(key);
                            await miscRepository.insertNewRedisKey(key);
                        }
                    }
                    else if (keyDataList[2] == "awarded-coupons")
                    {
                        GCAwardedCoupon awardedCoupon = new GCAwardedCoupon();
                        awardedCoupon.Cid = keyDataList[0];
                        if (keyDataList.Length > 1)
                        {
                            awardedCoupon.Gid = keyDataList[1];
                        }
                        if (keyDataList.Length > 3)
                        {
                            awardedCoupon.sid = keyDataList[3];
                        }

                        bool isExist = awardedCouponRepository.checkIfItemExist(awardedCoupon);
                        if (!isExist)
                        {
                            notInsertedKeys.Add(key);
                        }
                        else
                        {
                            insertedKeys.Add(key);
                            await miscRepository.insertNewRedisKey(key);

                        }
                    }
                    else
                    {
                        GCGameData gameData = new GCGameData();
                        gameData.Cid = keyDataList[0];
                        if (keyDataList.Length > 1)
                        {
                            gameData.Gid = keyDataList[1];
                        }
                        if (keyDataList.Length > 2)
                        {
                            gameData.Sid = keyDataList[2];
                        }
                        if (keyDataList.Length > 3)
                        {
                            gameData.DataType = keyDataList[3];
                            if (keyDataList[3] == "card")
                            {
                                if (keyDataList.Length > 4)
                                {
                                    gameData.CardId = keyDataList[4];
                                }
                                if (keyDataList.Length > 5)
                                {
                                    gameData.SubmitedCardAction = keyDataList[5];
                                }
                            }
                        }

                        bool isExist = gameDataRepository.checkIfItemExist(gameData);
                        if (!isExist)
                        {
                            notInsertedKeys.Add(key);
                        }
                        else
                        {
                            insertedKeys.Add(key);
                            await miscRepository.insertNewRedisKey(key);

                        }
                    }
                }
              

            }
            catch(Exception ex)
            {

            }
            return notInsertedKeys;
        }
        
        public async void DataStatus()
        {
            try
            {
                IServer server = RedisConnectionManager.GetServer();
                IDatabase database = RedisConnectionManager.GetDatabase();
                var keys = getKeyList(server);
                //long total = keys.Count;
                int leaderBoardEntryCount = leaderboardRepository.TotalRowCount();
                int gameDataEntryCount = gameDataRepository.TotalRowCount();
                int awardedCouponEntryCount = awardedCouponRepository.TotalRowCount();
            }
            catch(Exception ex)
            {
                log.Debug(ex);
            }
        }
       
    }
}
