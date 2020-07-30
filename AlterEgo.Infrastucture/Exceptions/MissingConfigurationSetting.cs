using System;

namespace AlterEgo.Infrastucture.Exceptions
{
    public class MissingConfigurationSetting : ApplicationException
    {
        public string SettingName { get; private set; }
        public string SettingsScopeName { get; private set; }
        public MissingConfigurationSetting(string settingName, string settingScopeName) : base("Missing setting in configuration")
        {
            SettingName = settingName;
            SettingsScopeName = settingScopeName;
        }
    }
}
