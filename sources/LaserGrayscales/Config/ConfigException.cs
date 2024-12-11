namespace As.Applications.Config
{
    public class ConfigException : Exception
    {
        public ConfigException() : base() { }

        public ConfigException(string message) : base(message) { }

        public ConfigException(string message, Exception innerException) : base(message, innerException) { }
    }
}
