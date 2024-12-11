namespace As.Applications.Data.Images
{
    /// <summary>
    /// Known type of images, each with a width and a height.
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// One line.
        /// </summary>
        Line,

        /// <summary>
        /// Box without a filling.
        /// </summary>
        Box,

        /// <summary>
        /// Square with a gray level filling.
        /// </summary>
        Square,

        /// <summary>
        /// Full gray scale image.
        /// </summary>
        Card
    }

    public static class ImageTypeEx
    {
        public static string Description(this ImageType me)
        {
            switch (me)
            {
                case ImageType.Line: return "One line in the x direction.";
                case ImageType.Box: return "Box without any filling";
                case ImageType.Square: return "Box filled with a number of lines in the x direction";
                case ImageType.Card: return "Rectangular picture, black and white";
                default: return "";
            }
        }
    }
}
