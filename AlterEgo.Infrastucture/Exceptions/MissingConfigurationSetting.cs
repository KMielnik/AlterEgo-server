using System;

namespace AlterEgo.Infrastructure.Exceptions
{
    public class MissingConfigurationSetting : ApplicationException
    {
        public string SettingName { get; private set; }
        public string SettingsScopeName { get; private set; }
        public MissingConfigurationSetting(string settingName, string settingScopeName) 
            : base($"Missing setting in configuration [{settingScopeName}/{settingName}]")
        {
            SettingName = settingName;
            SettingsScopeName = settingScopeName;
        }
    }
}
