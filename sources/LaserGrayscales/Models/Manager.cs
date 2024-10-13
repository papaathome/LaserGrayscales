namespace As.Applications.Models
{
    internal static class Manager
    {
        public const string APPLICATION_NAME = "LaserGrayscales";

        public static bool Generate()
            => TestModel.Generate();
    }
}
