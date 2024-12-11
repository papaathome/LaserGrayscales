namespace As.Applications.Data
{
    public static class ToString
    {
        public static string Format(string name, string content = "")
        {
            return (string.IsNullOrEmpty(content))
                ? $"{name}:[]"
                : $"{name}:[ {content} ]";
        }
    }
}
