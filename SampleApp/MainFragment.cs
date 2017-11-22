using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Theartofdev.Edmodo.Cropper;
using Java.Lang;

namespace SampleApp
{
    public class MainFragment : Android.Support.V4.App.Fragment, CropImageView.IOnSetImageUriCompleteListener, CropImageView.IOnCropImageCompleteListener
    {

        //region: Fields and Consts

        private CropDemoPreset _demoPreset;

        private CropImageView _cropImageView;
        //endregion

        /**
         * Returns a new instance of this fragment for the given section number.
         */
        public static MainFragment NewInstance(CropDemoPreset demoPreset)
        {
            var fragment = new MainFragment();
            var args = new Bundle();
            args.PutString("DEMO_PRESET", demoPreset.ToString());
            fragment.Arguments = args;
            return fragment;
        }

        /**
         * Set the image to show for cropping.
         */
        public void setImageUri(Android.Net.Uri imageUri)
        {
            _cropImageView.SetImageUriAsync(imageUri);
        }

        /**
         * Set the options of the crop image view to the given values.
         */
        public void SetCropImageViewOptions(CropImageViewOptions options)
        {
            _cropImageView.SetScaleType(options.ScaleType);
            _cropImageView.SetCropShape(options.CropShape);
            _cropImageView.SetGuidelines(options.Guidelines);
            _cropImageView.SetAspectRatio(options.AspectRatio.AspectRatioX, options.AspectRatio.AspectRatioY);
            _cropImageView.SetFixedAspectRatio(options.FixAspectRatio);
            _cropImageView.SetMultiTouchEnabled(options.Multitouch);
            _cropImageView.ShowCropOverlay = options.ShowCropOverlay;
            _cropImageView.ShowProgressBar = options.ShowProgressBar;
            _cropImageView.AutoZoomEnabled = options.AutoZoomEnabled;
            _cropImageView.MaxZoom = options.MaxZoomLevel;
            _cropImageView.FlippedHorizontally = options.FlipHorizontally;
            _cropImageView.FlippedVertically = options.FlipVertically;
        }

        /**
         * Set the initial rectangle to use.
         */
        public void SetInitialCropRect()
        {
            _cropImageView.CropRect = new Rect(100, 300, 500, 1200);
        }

        /**
         * Reset crop window to initial rectangle.
         */
        public void ResetCropRect()
        {
            _cropImageView.ResetCropRect();
        }

        public void UpdateCurrentCropViewOptions()
        {
            CropImageViewOptions options = new CropImageViewOptions();
            options.ScaleType = _cropImageView.GetScaleType();
            options.CropShape = _cropImageView.GetCropShape();
            options.Guidelines = _cropImageView.GetGuidelines();
            options.AspectRatio = ((int)_cropImageView.AspectRatio.First, (int)_cropImageView.AspectRatio.Second);
            options.FixAspectRatio = _cropImageView.IsFixAspectRatio;
            options.ShowCropOverlay = _cropImageView.ShowCropOverlay;
            options.ShowProgressBar = _cropImageView.ShowProgressBar;
            options.AutoZoomEnabled = _cropImageView.AutoZoomEnabled;
            options.MaxZoomLevel = _cropImageView.MaxZoom;
            options.FlipHorizontally = _cropImageView.FlippedHorizontally;
            options.FlipVertically = _cropImageView.FlippedVertically;
            ((MainActivity)Activity).SetCurrentOptions(options);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView;
            switch (_demoPreset)
            {
                case CropDemoPreset.Rect:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_rect, container, false);
                    break;
                case CropDemoPreset.Circular:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_oval, container, false);
                    break;
                case CropDemoPreset.CustomizedOverlay:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_customized, container, false);
                    break;
                case CropDemoPreset.MinMaxOverride:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_min_max, container, false);
                    break;
                case CropDemoPreset.ScaleCenterInside:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_scale_center, container, false);
                    break;
                case CropDemoPreset.Custom:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_rect, container, false);
                    break;
                default:
                    throw new IllegalStateException("Unknown preset: " + _demoPreset);
            }
            return rootView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _cropImageView = (CropImageView)view.FindViewById(Resource.Id.cropImageView);
            _cropImageView.SetOnSetImageUriCompleteListener(this);
            _cropImageView.SetOnCropImageCompleteListener(this);

            UpdateCurrentCropViewOptions();

            if (savedInstanceState == null)
            {
                if (_demoPreset == CropDemoPreset.ScaleCenterInside)
                {
                    _cropImageView.ImageResource = Resource.Drawable.cat_small;
                }
                else
                {
                    _cropImageView.ImageResource = Resource.Drawable.cat;
                }
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.main_action_crop:
                    _cropImageView.GetCroppedImageAsync();
                    return true;
                case Resource.Id.main_action_rotate:
                    _cropImageView.RotateImage(90);
                    return true;
                default:
                    if (item.ItemId == Resource.Id.main_action_flip_horizontally)
                    {
                        _cropImageView.FlipImageHorizontally();
                        return true;
                    }
                    else if (item.ItemId == Resource.Id.main_action_flip_vertically)
                    {
                        _cropImageView.FlipImageVertically();
                        return true;
                    }
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            _demoPreset = (CropDemoPreset)System.Enum.Parse(typeof(CropDemoPreset), Arguments.GetString("DEMO_PRESET"));
            ((MainActivity)activity).SetCurrentFragment(this);
        }

        public override void OnDetach()
        {
            base.OnDetach();
            if (_cropImageView != null)
            {
                _cropImageView.SetOnSetImageUriCompleteListener(null);
                _cropImageView.SetOnCropImageCompleteListener(null);
            }
        }

        public void OnSetImageUriComplete(CropImageView view, Android.Net.Uri uri, Java.Lang.Exception error)
        {
            if (error == null)
            {
                Toast.MakeText(Activity, "Image load successful", ToastLength.Long).Show();
            }
            else
            {
                Log.Error("AIC", "Failed to load image by URI", error);
                Toast.MakeText(Activity, "Image load failed: " + error.Message, ToastLength.Long).Show();
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == CropImage.CropImageActivityRequestCode)
            {
                CropImage.ActivityResult result = CropImage.GetActivityResult(data);
                HandleCropResult(result);
            }
        }

        private void HandleCropResult(CropImageView.CropResult result)
        {
            if (result.Error == null)
            {
                Intent intent = new Intent(Activity, typeof(CropResultActivity));
                intent.PutExtra("SAMPLE_SIZE", result.SampleSize);
                if (result.Uri != null) {
                    intent.PutExtra("URI", result.Uri);
                } else {
                    CropResultActivity.Image =
                        _cropImageView.GetCropShape() == CropImageView.CropShape.Oval
                            ? CropImage.ToOvalBitmap(result.Bitmap)
                            : result.Bitmap;
                }
                StartActivity(intent);
            } else {
                Log.Error("AIC", "Failed to crop image", result.Error);
                Toast.MakeText(Activity, "Image crop failed: " + result.Error.Message, ToastLength.Long).Show();
            }
        }

        public void OnCropImageComplete(CropImageView view, CropImageView.CropResult result)
        {
            HandleCropResult(result);
        }
    }
}