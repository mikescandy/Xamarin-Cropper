using Com.Theartofdev.Edmodo.Cropper;

namespace App4
{
    public class CropImageViewOptions
    {

        public CropImageView.ScaleType scaleType = CropImageView.ScaleType.CenterInside;

        public CropImageView.CropShape cropShape = CropImageView.CropShape.Rectangle;

        public CropImageView.Guidelines guidelines = CropImageView.Guidelines.OnTouch;

        public Android.Util.Pair aspectRatio = new Android.Util.Pair(1, 1);

        public bool autoZoomEnabled;

        public int maxZoomLevel;

        public bool fixAspectRatio;

        public bool showCropOverlay;

        public bool showProgressBar;
    }
}