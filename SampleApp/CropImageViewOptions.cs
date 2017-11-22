using System;
using Com.Theartofdev.Edmodo.Cropper;

namespace SampleApp
{
    public class CropImageViewOptions
    {
        public CropImageView.ScaleType ScaleType { get; set; } = CropImageView.ScaleType.CenterInside;

        public CropImageView.CropShape CropShape { get; set; } = CropImageView.CropShape.Rectangle;

        public CropImageView.Guidelines Guidelines { get; set; } = CropImageView.Guidelines.OnTouch;

        public (int AspectRatioX, int AspectRatioY) AspectRatio { get; set; } = (1, 1);

        public bool AutoZoomEnabled { get; set; }

        public int MaxZoomLevel { get; set; }

        public bool FixAspectRatio { get; set; }

        public bool Multitouch  { get; set; }

        public bool ShowCropOverlay { get; set; }

        public bool ShowProgressBar { get; set; }

        public bool FlipHorizontally { get; set; }

        public bool FlipVertically { get; set; }

    }
}