using AutoMapper;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using lab.RedisCacheSql.Config;
using lab.RedisCacheSql.Core;
using lab.RedisCacheSql.Helpers;
using lab.RedisCacheSql.Models;
using lab.RedisCacheSql.RedisManagers;
using lab.RedisCacheSql.Repository;
using lab.RedisCacheSql.ViewModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;

namespace lab.RedisCacheSql.Managers
{
    public class PatientProfileManager : IPatientProfileManager
    {
        private readonly AppConfig _appConfig;
        private readonly IPatientProfileRepository _iPatientProfileRepository;
        private readonly IMapper _iMapper;

        private readonly IRedisConnectionFactory _iRedisConnectionFactory;
        

        public PatientProfileManager(IOptions<AppConfig> appConfig
            , IPatientProfileRepository iPatientProfileRepository
            , IMapper iMapper
            , IRedisConnectionFactory iRedisConnectionFactory)
        {
            _appConfig = appConfig.Value;
            _iPatientProfileRepository = iPatientProfileRepository;
            _iMapper = iMapper;
            _iRedisConnectionFactory = iRedisConnectionFactory;
        }
        
        public async Task<PatientProfileViewModel> GetPatientProfileAsync()
        {
            try
            {
                var dataIEnumerable = await _iPatientProfileRepository.GetPatientProfilesAsync();
                var data = dataIEnumerable.FirstOrDefault();
                return _iMapper.Map<PatientProfile, PatientProfileViewModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PatientProfileViewModel> GetPatientProfileAsync(long id)
        {
            try
            {
                var data = await _iPatientProfileRepository.GetPatientProfileAsync(id);
                return _iMapper.Map<PatientProfile, PatientProfileViewModel>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DataTablesResponse> GetDataTablesResponseAsync(IDataTablesRequest request)
        {
            try
            {
                var viewModelIEnumerable = this.GetRedisPatientProfiles();
                //var viewModelIEnumerable = _iMapper.Map<IEnumerable<PatientProfile>, IEnumerable<PatientProfileViewModel>>(modelIEnumerable);

                // Global filtering.
                // Filter is being manually applied due to in-memmory (IEnumerable) data.
                // If you want something rather easier, check IEnumerableExtensions Sample.

                int dataCount = viewModelIEnumerable.Count();
                int filteredDataCount = 0;
                IEnumerable<PatientProfileViewModel> dataPage;
                if (viewModelIEnumerable.Count() > 0 && request != null)
                {
                    var filteredData = String.IsNullOrWhiteSpace(request.Search.Value)
                    ? viewModelIEnumerable
                    : viewModelIEnumerable.Where(_item => _item.FirstName.Contains(request.Search.Value));

                    dataCount = filteredData.Count();

                    // Paging filtered data.
                    // Paging is rather manual due to in-memmory (IEnumerable) data.
                    dataPage = filteredData.Skip(request.Start).Take(request.Length);

                    filteredDataCount = filteredData.Count();
                }
                else
                {
                    var filteredData = viewModelIEnumerable;

                    dataCount = filteredData.Count();

                    dataPage = filteredData;

                    filteredDataCount = filteredData.Count();
                }

                // Response creation. To create your response you need to reference your request, to avoid
                // request/response tampering and to ensure response will be correctly created.
                var response = DataTablesResponse.Create(request, dataCount, filteredDataCount, dataPage);

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PatientProfileViewModel>> GetPatientProfilesAsync()
        {
            try
            {
                var data = await _iPatientProfileRepository.GetPatientProfilesAsync();
                return _iMapper.Map<IEnumerable<PatientProfile>, IEnumerable<PatientProfileViewModel>>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result> InsertPatientProfileAsync(PatientProfileViewModel model)
        {
            try
            {
                var data = _iMapper.Map<PatientProfileViewModel, PatientProfile>(model);

                var saveChange = await _iPatientProfileRepository.InsertPatientProfileAsync(data);

                if (saveChange > 0)
                {
                    return Result.Ok(MessageHelper.Save);
                }
                else
                {
                    return Result.Fail(MessageHelper.SaveFail);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result> InsertPatientProfileAsync(List<PatientProfileViewModel> modelList)
        {
            try
            {
                var dataList = _iMapper.Map<List<PatientProfileViewModel>, List<PatientProfile>>(modelList);

                var saveChange = await _iPatientProfileRepository.InsertPatientProfileAsync(dataList);

                if (saveChange > 0)
                {
                    return Result.Ok(MessageHelper.Save);
                }
                else
                {
                    return Result.Fail(MessageHelper.SaveFail);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result> UpdatePatientProfileAsync(PatientProfileViewModel model)
        {
            try
            {
                var data = _iMapper.Map<PatientProfileViewModel, PatientProfile>(model);

                var saveChange = await _iPatientProfileRepository.UpdatePatientProfileAsync(data);

                if (saveChange > 0)
                {
                    return Result.Ok(MessageHelper.Update);
                }
                else
                {
                    return Result.Fail(MessageHelper.UpdateFail);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result> DeletePatientProfileAsync(long id)
        {
            try
            {
                var model = await GetPatientProfileAsync(id);
                if (model != null)
                {
                    var data = _iMapper.Map<PatientProfileViewModel, PatientProfile>(model);

                    var saveChange = await _iPatientProfileRepository.DeletePatientProfileAsync(data);

                    if (saveChange > 0)
                    {
                        return Result.Ok(MessageHelper.Delete);
                    }
                    else
                    {
                        return Result.Fail(MessageHelper.DeleteFail);
                    }
                }
                else
                {
                    return Result.Fail(MessageHelper.DeleteFail);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<PatientProfileViewModel> GetRedisPatientProfiles()
        {
            try
            {
                //_iRedisConnectionFactory.DeleteAllDatabase();
                var redisDataManager = new RedisDataManager<RedisModel>(_iRedisConnectionFactory);

                var redisModel = redisDataManager.Get(_appConfig.PatientProfileRedisKey);
                if (redisModel != null)
                {
                    if (!string.IsNullOrEmpty(redisModel.Key))
                    {
                        var redisDatas = JsonConvert.DeserializeObject<IEnumerable<PatientProfile>>(redisModel.Value);
                        return _iMapper.Map<IEnumerable<PatientProfile>, IEnumerable<PatientProfileViewModel>>(redisDatas.ToList());
                    }
                }

                //InsertRedisPatientProfilesAsync();
                return Enumerable.Empty<PatientProfileViewModel>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async void InsertRedisPatientProfilesAsync()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var redisDataManager = new RedisDataManager<RedisModel>(_iRedisConnectionFactory);

                    var patientProfiles = await _iPatientProfileRepository.GetPatientProfilesAsync();

                    if (patientProfiles != null && patientProfiles.Any())
                    {
                        string serializedPatientProfiles = JsonConvert.SerializeObject(patientProfiles);
                        var redisModel = new RedisModel() { Key = _appConfig.PatientProfileRedisKey, Value = serializedPatientProfiles };
                        redisDataManager.Save(_appConfig.PatientProfileRedisKey, redisModel);
                    }

                });
            }
            catch (StackExchange.Redis.RedisConnectionException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

    public interface IPatientProfileManager
    {
        Task<PatientProfileViewModel> GetPatientProfileAsync();
        Task<PatientProfileViewModel> GetPatientProfileAsync(long id);
        Task<DataTablesResponse> GetDataTablesResponseAsync(IDataTablesRequest request);
        Task<IEnumerable<PatientProfileViewModel>> GetPatientProfilesAsync();
        Task<Result> InsertPatientProfileAsync(List<PatientProfileViewModel> modelList);
        Task<Result> InsertPatientProfileAsync(PatientProfileViewModel model);
        Task<Result> UpdatePatientProfileAsync(PatientProfileViewModel model);
        Task<Result> DeletePatientProfileAsync(long id);
        IEnumerable<PatientProfileViewModel> GetRedisPatientProfiles();
    }
}
