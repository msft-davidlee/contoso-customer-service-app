namespace DemoWebsite.Core
{
    public interface IManagedConfiguration
    {
        string Version { get; }
        string MachineName { get; }
    }

    public class ManagedConfiguration : IManagedConfiguration
    {
        private readonly string _version;
        private readonly string _machineName;

        public ManagedConfiguration(string version, string machineName)
        {
            _version = version;
            _machineName = machineName;
        }

        public string Version
        {
            get
            {
                return string.IsNullOrEmpty(_version) ? "1.0" : _version;
            }
        }

        public string MachineName
        {
            get
            {
                return string.IsNullOrEmpty(_machineName) ? "unknown" : _machineName;
            }
        }
    }
}
