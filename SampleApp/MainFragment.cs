using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Theartofdev.Edmodo.Cropper;
using Java.Lang;

namespace App4
{
    public class MainFragment : Android.Support.V4.App.Fragment, CropImageView.IOnSetImageUriCompleteListener, CropImageView.IOnGetCroppedImageCompleteListener
    {

        //region: Fields and Consts

        private CropDemoPreset mDemoPreset;

        private CropImageView mCropImageView;
        //endregion

        /**
         * Returns a new instance of this fragment for the given section number.
         */
        public static MainFragment newInstance(CropDemoPreset demoPreset)
        {
            MainFragment fragment = new MainFragment();
            Bundle args = new Bundle();
            args.PutString("DEMO_PRESET", demoPreset.ToString());
            fragment.Arguments = args;
            return fragment;
        }

        /**
         * Set the image to show for cropping.
         */
        public void setImageUri(Android.Net.Uri imageUri)
        {
            mCropImageView.SetImageUriAsync(imageUri);
            //        CropImage.activity(imageUri)
            //                .start(getContext(), this);
        }

        /**
         * Set the options of the crop image view to the given values.
         */
        public void setCropImageViewOptions(CropImageViewOptions options)
        {
            mCropImageView.SetScaleType(options.scaleType);
            mCropImageView.SetCropShape(options.cropShape);
            mCropImageView.SetGuidelines(options.guidelines);
            mCropImageView.SetAspectRatio((int)options.aspectRatio.First, (int)options.aspectRatio.Second);
            mCropImageView.SetFixedAspectRatio(options.fixAspectRatio);
            mCropImageView.ShowCropOverlay = options.showCropOverlay;
            mCropImageView.ShowProgressBar = options.showProgressBar;
            mCropImageView.AutoZoomEnabled = options.autoZoomEnabled;
            mCropImageView.MaxZoom = options.maxZoomLevel;
        }

        /**
         * Set the initial rectangle to use.
         */
        public void setInitialCropRect()
        {
            mCropImageView.CropRect = new Rect(100, 300, 500, 1200);
        }

        /**
         * Reset crop window to initial rectangle.
         */
        public void resetCropRect()
        {
            mCropImageView.ResetCropRect();
        }

        public void updateCurrentCropViewOptions()
        {
            CropImageViewOptions options = new CropImageViewOptions();
            options.scaleType = mCropImageView.GetScaleType();
            options.cropShape = mCropImageView.GetCropShape();
            options.guidelines = mCropImageView.GetGuidelines();
            options.aspectRatio = mCropImageView.AspectRatio;
            options.fixAspectRatio = mCropImageView.IsFixAspectRatio;
            options.showCropOverlay = mCropImageView.ShowCropOverlay;
            options.showProgressBar = mCropImageView.ShowProgressBar;
            options.autoZoomEnabled = mCropImageView.AutoZoomEnabled;
            options.maxZoomLevel = mCropImageView.MaxZoom;
            ((MainActivity)Activity).setCurrentOptions(options);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView;
            switch (mDemoPreset)
            {
                case CropDemoPreset.RECT:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_rect, container, false);
                    break;
                case CropDemoPreset.CIRCULAR:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_oval, container, false);
                    break;
                case CropDemoPreset.CUSTOMIZED_OVERLAY:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_customized, container, false);
                    break;
                case CropDemoPreset.MIN_MAX_OVERRIDE:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_min_max, container, false);
                    break;
                case CropDemoPreset.SCALE_CENTER_INSIDE:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_scale_center, container, false);
                    break;
                case CropDemoPreset.CUSTOM:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_rect, container, false);
                    break;
                default:
                    throw new IllegalStateException("Unknown preset: " + mDemoPreset);
            }
            return rootView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            mCropImageView = (CropImageView)view.FindViewById(Resource.Id.cropImageView);
            mCropImageView.SetOnSetImageUriCompleteListener(this);
            mCropImageView.SetOnGetCroppedImageCompleteListener(this);

            updateCurrentCropViewOptions();

            if (savedInstanceState == null)
            {
                if (mDemoPreset == CropDemoPreset.SCALE_CENTER_INSIDE)
                {
                    mCropImageView.ImageResource = Resource.Drawable.cat_small;
                }
                else
                {
                    mCropImageView.ImageResource = Resource.Drawable.cat;
                }
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.main_action_crop)
            {
                mCropImageView.GetCroppedImageAsync();
                return true;
            }
            else if (item.ItemId == Resource.Id.main_action_rotate)
            {
                mCropImageView.RotateImage(90);
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            mDemoPreset = (CropDemoPreset)System.Enum.Parse(typeof(CropDemoPreset), Arguments.GetString("DEMO_PRESET"));
            ((MainActivity)activity).setCurrentFragment(this);
        }

        public override void OnDetach()
        {
            base.OnDetach();
            if (mCropImageView != null)
            {
                mCropImageView.SetOnSetImageUriCompleteListener(null);
                mCropImageView.SetOnGetCroppedImageCompleteListener(null);
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

        public void OnGetCroppedImageComplete(CropImageView view, Bitmap bitmap, Java.Lang.Exception error)
        {
            handleCropResult(null, bitmap, error);

        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == CropImage.CropImageActivityRequestCode)
            {
                CropImage.ActivityResult result = CropImage.GetActivityResult(data);
                handleCropResult(result.Uri, null, result.Error);
            }
        }

        private void handleCropResult(Android.Net.Uri uri, Bitmap bitmap, Java.Lang.Exception error)
        {
            if (error == null)
            {
                Intent intent = new Intent(Activity, typeof(CropResultActivity));
                if (uri != null) {
                    intent.PutExtra("URI", uri);
                } else {
                    CropResultActivity.mImage = mCropImageView.GetCropShape() == CropImageView.CropShape.Oval
                        ? CropImage.ToOvalBitmap(bitmap)
                        : bitmap;
                }
                StartActivity(intent);
            } else {
                Log.Error("AIC", "Failed to crop image", error);
                Toast.MakeText(Activity, "Image crop failed: " + error.Message, ToastLength.Long).Show();
            }
        }
    }
}