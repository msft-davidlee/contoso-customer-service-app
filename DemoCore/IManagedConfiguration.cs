namespace DemoCore
{
    public interface IManagedConfiguration
    {
        string Version { get; }
        string MachineName { get; }
        string Details { get; }
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

        public string Details
        {
            get { return $"{_version} | {_machineName}"; }
        }

        public string Version
        {
            get
            {
                return string.IsNullOrEmpty(_version) ? "0" : _version;
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
