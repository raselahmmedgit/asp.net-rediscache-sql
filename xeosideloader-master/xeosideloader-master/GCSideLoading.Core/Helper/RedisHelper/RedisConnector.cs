using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using StackExchange.Redis;
using Microsoft.Rest;
using Microsoft.Azure.Management.Redis;
using Microsoft.Azure.Management.Redis.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GCSideLoading.Core.Helper.RedisHelper
{
    public class RedisConnector
    {
        
        private static string subscriptionId = "9c3273b2-0976-416b-8132-c9de75cd430e"; //Get this from the Azure Portal
        private static string resourceGroupName = "Redis-auto-scalling";//Get this from the Azure Portal
        private static string cacheName = "interactive-scalling"; // This is just the name (without redis.cache.windows.net)
        private static string redisSKUName = "Premium"; //Basic/ Standard/ Premium
        private static string redisSKUFamily = "P";
        public static int redisSKUCapacity = 1; //Cache Size 0-6. More details http://azure.microsoft.com/en-us/pricing/details/cache/
        private static string redisCacheRegion = "Central US";
        public static bool isScalling = false;

        // Use the following link to learn how to setup this app to access Active Directory
        // https://msdn.microsoft.com/en-us/library/azure/dn790557.aspx#bk_portal
        private static string tenantId = "3c21c26e-8c2e-4dee-bd64-f8a9e4d79ef7";
        private static string clientId = "2322e1ee-abfc-4c4d-9dff-a4ede699d5fe";
        private static string redirectUri = "";
        private static string clientSecrets = "35-2pT46WbyYE.Tyl1JMy3~FB89ni9mS_1";

        private static string host = "interactive-scalling.redis.cache.windows.net";
        private static int port = 6380;
        private static string password = "0A3eGfGpmLIoRMfPyxqm3YrRwyHHEcUNiCTanaRPCfE=";
        private static string connectionString = "interactive-scalling.redis.cache.windows.net:6380,password=0A3eGfGpmLIoRMfPyxqm3YrRwyHHEcUNiCTanaRPCfE=,ssl=True,abortConnect=False";
        private static RedisManagementClient client = null;
        public static string redisCacheStatus = "";
        public static string redisModeStatus = "";
        private static string[] displayItems = new string[]
        {
            "used_memory_human","maxmemory_human","server_load"
        };
        private static string GetAuthorizationHeader()
        {
            try
            {
                AuthenticationResult result = null;

                var context = new AuthenticationContext(String.Format("https://login.windows.net/{0}", tenantId));
                result = context.AcquireTokenAsync(
                    "https://management.core.windows.net/",
                    new ClientCredential(clientId, clientSecrets)).Result;

                if (result == null)
                {
                    throw new InvalidOperationException("Failed to obtain the JWT token");
                }

                string token = result.AccessToken;
                return token;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Lazy<ConnectionMultiplexer> ScallingConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            try
            {
                var options = ConfigurationOptions.Parse(connectionString);
                options.ConnectRetry = 5;
                options.AllowAdmin = true;
                var connect = ConnectionMultiplexer.Connect(options);
                return connect;
            }
            catch (Exception)
            {
                throw;
            }
        });
        private static Lazy<ConnectionMultiplexer> DataConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            try
            {
                var options = ConfigurationOptions.Parse(connectionString);
                options.ConnectRetry = 5;
                options.AllowAdmin = true;
                var connect = ConnectionMultiplexer.Connect(options);
                return connect;
            }
            catch (Exception)
            {
                throw;
            }
        });
        private static Lazy<ConnectionMultiplexer> CacheInfoConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            try
            {
                var options = ConfigurationOptions.Parse(connectionString);
                options.ConnectRetry = 5;
                options.AllowAdmin = true;
                //options.ConnectTimeout = 15;
                //options.AbortOnConnectFail = false;
                var connect = ConnectionMultiplexer.Connect(options);
                return connect;
            }
            catch (Exception)
            {
                throw;
            }
        });

        public static void InitiateRedisClient()
        {
            try
            {
                if (client == null)
                {
                    string token = GetAuthorizationHeader();
                    TokenCredentials creds = new TokenCredentials(token);
                    client = new RedisManagementClient(creds);
                    client.SubscriptionId = subscriptionId;
                }
            }
            catch(Exception)
            {

            }
               
        }
        public static void DownScaling()
        {
            redisSKUCapacity = 1;
            isScalling = true;
            StartScaling();
            redisModeStatus = "Down Scaling...";
        }
        public static void UpScaling()
        {
            redisSKUCapacity = 2;
            isScalling = true;
            StartScaling();
            redisModeStatus = "Up Scaling...";
        }
        private static RedisUpdateParameters GetAutoScalingSettings()
        {
            try
            {
                var redisParams = new RedisUpdateParameters()
                {
                    Sku = new Sku(redisSKUName, redisSKUFamily, redisSKUCapacity)
                };
                return redisParams;
            }
            catch (Exception)
            {

            }
            return null;
        }
        public async static void InsertData()
        {
            try
            {
                redisModeStatus = "Inserting Data...";
                IDatabase cache = DataConnection.Value.GetDatabase();
                string status = RedisCacheScalingStatus();
                //statusLabelValueText.Text = status;            
                List<JObject> list = new List<JObject>();
                var n = 50000;
                if (!string.IsNullOrEmpty(status) && status.Equals("Scaling"))
                {
                    n = 5000;
                }
                for (int i = 0; i < n; i++)
                {
                    JObject emp = new JObject();
                    emp.Add("id", "V-" + i);
                    emp.Add("name", "virendra");
                    emp.Add("address", "Indore");
                    emp.Add("Country", "India");
                    list.Add(emp);
                }
                // create the document into DocumentDb  
                if (!string.IsNullOrEmpty(status) 
                    //&& !status.Equals("Scaling") && !isScalling
                    )
                {
                    long counter = 0;
                    var counterRedis = await cache.StringGetAsync("redisDataCounter");
                    if (!counterRedis.HasValue)
                        counter = 1;
                    else
                    {
                        counter = int.Parse(counterRedis);
                        counter++;
                    }
                    var entryInRedis = await cache.StringSetAsync("redisEmp-" + counter, JsonConvert.SerializeObject(list));
                    await cache.StringSetAsync("redisDataCounter", counter);
                }
            }
            catch (Exception)
            {

            }
           
        }
        public async static void RemoveData()
        {
            try
            {
                redisModeStatus = "Removing Data...";
                IDatabase cache = DataConnection.Value.GetDatabase();
                string status = RedisCacheScalingStatus();
                //statusLabelValueText.Text = status;            
                // create the document into DocumentDb  
                if (!string.IsNullOrEmpty(status) 
                    //&& !status.Equals("Scaling")
                    )
                {
                    long counter = 0;
                    var counterRedis = await cache.StringGetAsync("redisDataCounter");
                    if (!counterRedis.HasValue)
                        return;
                    cache.KeyDelete("redisEmp-" + counterRedis);
                    counter = long.Parse(counterRedis);
                    counter--;
                    await cache.StringSetAsync("redisDataCounter", counter);
                }
            }
            catch (Exception)
            {

            }
            
        }

        private static void StartScaling()
        {
            try
            {
                client.Redis.Update(resourceGroupName, cacheName, GetAutoScalingSettings());
            }
            catch (Exception)
            {

            }
        }
        public static string RedisCacheScalingStatus()
        {
            try
            {
                var redisCache = client.Redis.Get(resourceGroupName, cacheName);
                if (redisCache != null)
                {
                    return redisCache.ProvisioningState;
                }
            }
            catch (Exception)
            {

            }
            
            return string.Empty;
        }
        public static Dictionary<string,string> GetCacheStatus()
        {
            try
            {
                var server = CacheInfoConnection.Value.GetServer(host, port);
                //var ms =  server.MemoryStats();
                //var cl =  server.ClientList();
                //var Dz = server.DatabaseSize();
                var info = server.Info();
                Dictionary<string, string> dataItems = new Dictionary<string, string>();
                foreach (var item in info)
                {

                    //Console.WriteLine("     " + item.Key);
                    //Console.WriteLine("     =======================================");
                    foreach (var data in item)
                    {
                        //Console.WriteLine("     " + data.Key + " : " + data.Value);
                        if (item != null && !string.IsNullOrEmpty(item.Key) && displayItems.Contains(data.Key))
                        {
                            dataItems.Add(data.Key, data.Value);
                        }
                    }
                    //Console.WriteLine("     ------------------------------------");
                    //Console.WriteLine("");
                }

                return dataItems;
            }
            catch (Exception)
            {

            }
            return null;
            
        }
  
    }
}
