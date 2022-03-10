using lab.RedisCacheSql.Core;
using lab.RedisCacheSql.Helpers;
using lab.RedisCacheSql.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab.RedisCacheSql.Managers
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IConfiguration _iConfiguration;
        private readonly IWebHostEnvironment _iWebHostEnvironment;

        public EmployeeManager(IConfiguration iConfiguration, IWebHostEnvironment iWebHostEnvironment)
        {
            _iConfiguration = iConfiguration;
            _iWebHostEnvironment = iWebHostEnvironment;
        }

        public async Task<List<EmployeeViewModel>> GetEmployeesAsync()
        {
            try
            {
                JsonDataStoreHelper jsonDataStoreHelper = new JsonDataStoreHelper(_iWebHostEnvironment, "employee.json");
                var employeeJson = await jsonDataStoreHelper.ReadJsonData();
                var employeeViewModelList = JsonConvert.DeserializeObject<List<EmployeeViewModel>>(employeeJson);
                return employeeViewModelList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<EmployeeViewModel> GetEmployeeByIdAsync(int employeeId)
        {
            try
            {
                JsonDataStoreHelper jsonDataStoreHelper = new JsonDataStoreHelper(_iWebHostEnvironment, "employee.json");
                var employeeJson = await jsonDataStoreHelper.ReadJsonData();
                var employeeViewModelList = JsonConvert.DeserializeObject<List<EmployeeViewModel>>(employeeJson);
                var employeeViewModel = employeeViewModelList.FirstOrDefault(x => x.EmployeeId == employeeId);
                return employeeViewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Result> InsertAsync(EmployeeViewModel viewModel)
        {
            try
            {
                if (viewModel.EmployeeId == 0)
                {
                    if (viewModel != null)
                    {
                        JsonDataStoreHelper jsonDataStoreHelper = new JsonDataStoreHelper(_iWebHostEnvironment, "employee.json");
                        var viewModelList = await GetEmployeesAsync();
                        viewModelList.Add(viewModel);
                        string viewModelListJson = JsonConvert.SerializeObject(viewModelList);
                        var isWrite = await jsonDataStoreHelper.WriteJsonData(viewModelListJson);
                        return Result.Ok(MessageHelper.Save);
                    }
                    else
                    {
                        return Result.Fail(MessageHelper.SaveFail);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Result.Fail(MessageHelper.SaveFail);
        }

        public async Task<Result> UpdateAsync(EmployeeViewModel viewModel)
        {
            try
            {
                if (viewModel.EmployeeId > 0)
                {
                    if (viewModel != null)
                    {
                        JsonDataStoreHelper jsonDataStoreHelper = new JsonDataStoreHelper(_iWebHostEnvironment, "employee.json");
                        var viewModelList = await GetEmployeesAsync();
                        var viemModelRemove = viewModelList.FirstOrDefault(x => x.EmployeeId == viewModel.EmployeeId);
                        viewModelList.Remove(viemModelRemove);
                        viewModelList.Add(viewModel);
                        string viewModelListJson = JsonConvert.SerializeObject(viewModelList);
                        var isWrite = await jsonDataStoreHelper.WriteJsonData(viewModelListJson);
                        return Result.Ok(MessageHelper.Update);
                    }
                    else
                    {
                        return Result.Fail(MessageHelper.UpdateFail);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Result.Fail(MessageHelper.UpdateFail);
        }

        public async Task<Result> DeleteAsync(EmployeeViewModel viewModel)
        {
            try
            {
                if (viewModel.EmployeeId > 0)
                {
                    if (viewModel != null)
                    {
                        JsonDataStoreHelper jsonDataStoreHelper = new JsonDataStoreHelper(_iWebHostEnvironment, "employee.json");
                        var viewModelList = await GetEmployeesAsync();
                        var viemModelRemove = viewModelList.FirstOrDefault(x => x.EmployeeId == viewModel.EmployeeId);
                        viewModelList.Remove(viemModelRemove);
                        string viewModelListJson = JsonConvert.SerializeObject(viewModelList);
                        var isWrite = await jsonDataStoreHelper.WriteJsonData(viewModelListJson);
                        return Result.Ok(MessageHelper.Delete);
                    }
                    else
                    {
                        return Result.Fail(MessageHelper.DeleteFail);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Result.Fail(MessageHelper.DeleteFail);
        }
    }

    public interface IEmployeeManager
    {
        Task<List<EmployeeViewModel>> GetEmployeesAsync();
        Task<EmployeeViewModel> GetEmployeeByIdAsync(int employeeId);
        Task<Result> InsertAsync(EmployeeViewModel viewModel);
        Task<Result> UpdateAsync(EmployeeViewModel viewModel);
        Task<Result> DeleteAsync(EmployeeViewModel viewModel);
    }
}
