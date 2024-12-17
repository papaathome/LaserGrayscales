using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;

using Caliburn.Micro;
using As.Applications.Validation;
using As.Applications.Data.Images;
using As.Applications.Config;
using Microsoft.Win32;

namespace As.Applications.ViewModels
{
    public class ImageViewModel : Screen, IDataErrorInfo
    {
        public ImageViewModel()
        {
            _dataErrorInfo = new(this);
        }

        #region Properties
        /// <summary>
        /// List of available image types
        /// </summary>
        public List<ImageType> Images { get; } =
        [
            ImageType.Line,
            ImageType.Box,
            ImageType.Square,
            //ImageType.Card not yet available.
        ];

        /// <summary>
        /// Selected image type.
        /// </summary>
        public ImageType CurrentImage
        {
            get => _current_image;
            set
            {
                if (SetEnum(ref _current_image, value))
                {
                    NotifyOfPropertyChange(nameof(HaveDescription));
                    NotifyOfPropertyChange(nameof(Description));
                    NotifyOfPropertyChange(nameof(LinesPerCm));
                    NotifyOfPropertyChange(nameof(CanLinesPerCm));
                    NotifyOfPropertyChange(nameof(ImagePath));
                    NotifyOfPropertyChange(nameof(CanImagePath));
                    NotifyOfPropertyChange(nameof(CanSelectImagePath));
                }
            }
        }
        ImageType _current_image = ImageType.Line;

        /// <summary>
        /// Short description of the selected image type.
        /// </summary>
        public string Description => CurrentImage.Description();

        public bool HaveDescription => !string.IsNullOrEmpty(Description);

        /// <summary>
        /// Image width in mm, must be greater than 0.
        /// </summary>
        public double Width
        {
            get => _width;
            set => Set(ref _width, value, value.ValidateMinimum(0, open: true));
        }
        double _width = 0.001f;

        /// <summary>
        /// Image Height in mm, must be greater than 0.
        /// </summary>
        public double Height
        {
            get => _height;
            set => Set(ref _height, value, value.ValidateMinimum(0, open: true));
        }
        double _height = 0.001f;

        /// <summary>
        /// For image type Square and Card only: number of lines in lines per cm.
        /// </summary>
        public double LinesPerCm
        {
            get => _lines_per_cm;
            set => Set(ref _lines_per_cm, value, value.ValidateMinimum(0));
        }
        double _lines_per_cm = 20;

        public bool CanLinesPerCm =>
            CurrentImage == ImageType.Square ||
            CurrentImage == ImageType.Card;

        /// <summary>
        /// For image type Card only: path to image file.
        /// </summary>
        public string ImagePath
        {
            get => _image_path;
            set => Set(ref _image_path, value, value.ValidateFileExists(noempty: (CurrentImage == ImageType.Card)));
        }
        string _image_path = "";

        public bool CanImagePath => CurrentImage == ImageType.Card;
        #endregion Properties

        #region Actions
        /// <summary>
        /// For image type Square only: set the image file path.
        /// </summary>
        public void SelectImagePath()
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                FileName = ImagePath,
                DefaultExt = Path.GetExtension(ImagePath) ?? ".bmp",
                Filter = "Image files |*.bmp;*.gif;*.jpg;*.png|all|*.*"
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                ImagePath = dlg.FileName;
            }
        }

        public bool CanSelectImagePath => CurrentImage == ImageType.Card;

        public Image GetImage()
        {
            switch (CurrentImage)
            {
                case ImageType.Line:
                    return new Line()
                    {
                        Width = Width,
                        Height = Height,
                        LinesPerCm = 20,
                        ImagePath = "",
                    };
                case ImageType.Box:
                    return new Box()
                    {
                        Width = Width,
                        Height = Height,
                        LinesPerCm = 20,
                        ImagePath = "",
                    };
                case ImageType.Square:
                    return new Square()
                    {
                        Width = Width,
                        Height = Height,
                        LinesPerCm = LinesPerCm,
                        ImagePath = "",
                    };
                case ImageType.Card:
                    return new Card()
                    {
                        Width = Width,
                        Height = Height,
                        LinesPerCm = LinesPerCm,
                        ImagePath = ImagePath,
                    };
                default:
                    throw new ConfigException($"bug: GetImage fail, image type not recognised; image type = \"{CurrentImage}\"");
            }
        }

        public void SetImage(Image image )
        {
            switch (image.GetType().Name)
            {
                case "Line":
                    var line = (image as Line) ?? throw new ConfigException($"bug: SetImage fail, can not casst image type; image type = \"{image.GetType().Name}\"");
                    CurrentImage = ImageType.Line;
                    Width = line.Width;
                    Height = line.Height;
                    LinesPerCm = 20;
                    ImagePath = "";
                    break;
                case "Box":
                    var box = (image as Box) ?? throw new ConfigException($"bug: SetImage fail, can not casst image type; image type = \"{image.GetType().Name}\"");
                    CurrentImage = ImageType.Box;
                    Width = box.Width;
                    Height = box.Height;
                    LinesPerCm = 20;
                    ImagePath = "";
                    break;
                case "Square":
                    var square = (image as Square) ?? throw new ConfigException($"bug: SetImage fail, can not casst image type; image type = \"{image.GetType().Name}\"");
                    CurrentImage = ImageType.Square;
                    Width = square.Width;
                    Height = square.Height;
                    LinesPerCm = square.LinesPerCm;
                    ImagePath = "";
                    break;
                case "Card":
                    var card = (image as Card) ?? throw new ConfigException($"bug: SetImage fail, can not casst image type; image type = \"{image.GetType().Name}\"");
                    CurrentImage = ImageType.Card;
                    Width = card.Width;
                    Height = card.Height;
                    LinesPerCm = card.LinesPerCm;
                    ImagePath = card.ImagePath;
                    break;
                default:
                    throw new ConfigException($"bug: SetImage fail, image type not recognised; image type = \"{image.GetType().Name}\"");
            }
        }
        #endregion Axtions

        #region IDataErrorInfo
        readonly DataErrorInfo _dataErrorInfo;

        /// <inheritdoc/>
        public string Error => _dataErrorInfo.Error;

        /// <inheritdoc/>
        public string this[string propertyName]
        {
            get => _dataErrorInfo[propertyName];
            internal set => _dataErrorInfo[propertyName] = value;
        }

        internal bool Set<T>(ref T lvalue, T value, string? error = null, [CallerMemberName] string propertyName = "") where T : IEquatable<T>
        {
            if (!_dataErrorInfo.TrySet(ref lvalue, value, error, propertyName: propertyName)) return false;
            NotifyOfPropertyChange(propertyName);
            return true;
        }

        internal bool SetEnum<T>(ref T lvalue, T value, string? error = null, [CallerMemberName] string propertyName = "") where T : Enum
        {
            if (!_dataErrorInfo.TrySetEnum(ref lvalue, value, error, propertyName: propertyName)) return false;
            NotifyOfPropertyChange(propertyName);
            return true;
        }
        #endregion
    }
}
