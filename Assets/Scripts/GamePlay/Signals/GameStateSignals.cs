namespace Features.Signals
{
    public class ExitToMapSignal {}
    public class ChangeLevelSignal
    {
        public int Config { get; }

        public ChangeLevelSignal(int config)
        {
            Config = config;
        }
    }
    
    public class ConfigChangedSignal
    {
        public int Config { get; }

        public ConfigChangedSignal(int config)
        {
            Config = config;
        }
    }
}