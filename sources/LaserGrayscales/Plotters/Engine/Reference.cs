namespace As.Applications.Plotters.Engine
{
    public enum Reference
    {
        /// <summary>
        /// (0, 0) is machine (0, 0) position.
        /// </summary>
        Absolute = 0,

        /// <summary>
        /// (0, 0) is current position.
        /// </summary>
        Relative = 1,

        // (0, 0) is grid (#0, #0)
        GridRelative = 2,

        // (0, 0) is top op stack position.
        StackRelative = 3,
    }
}
