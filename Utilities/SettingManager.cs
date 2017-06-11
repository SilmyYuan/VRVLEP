using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace VRVLEP.Utilities
{
    /// <summary>
    /// 公共配置类，用于读取特定路径中的json文件
    /// </summary>
    public class SettingManager
    {
        public static T GetAppSettings<T>(string key) where T : class, new()
        {
            var basePath = AppContext.BaseDirectory + Path.DirectorySeparatorChar;  //appsettings.json配置文件的路径

            var settingJson = "appsettings.json";   //配置文件名
            
            return GetAppSettings<T>(key, basePath, settingJson);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">配置文件Model</typeparam>
        /// <param name="key">读取的字段名</param>
        /// <param name="basePath">配置文件所在目录</param>
        /// <param name="settingJson">配置文件的名称</param>
        /// <returns></returns>
        public static T GetAppSettings<T>(string key,string basePath,string settingJson) where T : class, new()
        {
            basePath = (basePath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? basePath : basePath + Path.DirectorySeparatorChar);

            settingJson = (settingJson.EndsWith(".json") ? settingJson : settingJson + ".json");

            IConfiguration config = new ConfigurationBuilder()                
                .SetBasePath(basePath)  //设置配置文件所在目录
                .Add(new JsonConfigurationSource { Path = settingJson, Optional = false, ReloadOnChange = true })    //设置读取的配置文件名
                .Build();

            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }

        #region 该方法被替换
        /*
        public T GetAppSettings<T>(string key) where T : class, new()
        {
            IConfiguration config = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();

            var appconfig = new ServiceCollection()
            .AddOptions()
            .Configure<T>(config.GetSection(key))
            .BuildServiceProvider()
            .GetService<IOptions<T>>()
            .Value;

            return appconfig;
        }
        */
        #endregion
    }
}
