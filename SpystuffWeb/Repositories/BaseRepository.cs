using Repositories.Config;
using log4net;

namespace Repositories
{
    public interface IBaseRepositoryConfig
    {
        void ForceLoadConfig(string configName);
    }

    public class BaseRepository<TConfig> : IBaseRepositoryConfig 
    {
        protected TConfig _config;

        protected readonly ILog Logger = LogManager.GetLogger("BaseRepository");

        /// <summary>
        /// Default constructor that also handles loading the configuration.
        /// </summary>
        protected BaseRepository()
        {
            Logger = LogManager.GetLogger(GetType().Name);
            LoadConfig(typeof(TConfig).Name);
        }

        public void ForceLoadConfig(string configName)
        {
            LoadConfig(configName); 
        }

        protected virtual void LoadConfig(string configName)
        {
            _config = Global.GetConfig<TConfig>(configName);
        }
    }
}
