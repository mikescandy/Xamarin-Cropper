using System;
using Android.Support.V7.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Theartofdev.Edmodo.Cropper;
using Android.App;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Content.PM;
using Android;
using Java.Lang;
using Android.Net;
using Android.Util;
using Java.Interop;

namespace App4
{
    [Activity(Label = "App4", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat")]
    public class MainActivity : AppCompatActivity
    {

        //region: Fields and Consts

        DrawerLayout mDrawerLayout;

        private ActionBarDrawerToggle mDrawerToggle;

        private MainFragment mCurrentFragment;

        private Android.Net.Uri mCropImageUri;

        private CropImageViewOptions mCropImageViewOptions = new CropImageViewOptions();
        //endregion

        public void setCurrentFragment(MainFragment fragment)
        {
            mCurrentFragment = fragment;
        }

        public void setCurrentOptions(CropImageViewOptions options)
        {
            mCropImageViewOptions = options;
            updateDrawerTogglesByOptions(options);
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            mDrawerLayout = (DrawerLayout)FindViewById(Resource.Id.drawer_layout);

            mDrawerToggle = new ActionBarDrawerToggle(this, mDrawerLayout, Resource.String.main_drawer_open, Resource.String.main_drawer_close);
            mDrawerToggle.DrawerIndicatorEnabled = true;
            mDrawerLayout.SetDrawerListener(mDrawerToggle);

            if (savedInstanceState == null)
            {
                setMainFragmentByPreset(CropDemoPreset.RECT);
            }
        }


        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
            mCurrentFragment.updateCurrentCropViewOptions();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (mDrawerToggle.OnOptionsItemSelected(item))
            {
                return true;
            }
            if (mCurrentFragment != null && mCurrentFragment.OnOptionsItemSelected(item))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == CropImage.PickImagePermissionsRequestCode && resultCode == Result.Ok)
            {
                var imageUri = CropImage.GetPickImageResultUri(this, data);

                // For API >= 23 we need to check specifically that we have permissions to read external storage,
                // but we don't know if we need to for the URI so the simplest is to try open the stream and see if we get error.
                var requirePermissions = false;
                if (CropImage.IsReadExternalStoragePermissionsRequired(this, imageUri))
                {

                    // request permissions and handle the result in onRequestPermissionsResult()
                    requirePermissions = true;
                    mCropImageUri = imageUri;

                    RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, CropImage.PickImagePermissionsRequestCode);
                }
                else
                {

                    mCurrentFragment.setImageUri(imageUri);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == CropImage.CameraCapturePermissionsRequestCode)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    CropImage.StartPickImageActivity(this);
                }
                else
                {
                    Toast.MakeText(this, "Cancelling, required permissions are not granted", ToastLength.Long).Show();
                }
            }
            if (requestCode == CropImage.PickImagePermissionsRequestCode)
            {
                if (mCropImageUri != null && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    mCurrentFragment.setImageUri(mCropImageUri);
                }
                else
                {
                    Toast.MakeText(this, "Cancelling, required permissions are not granted", ToastLength.Long).Show();
                }
            }
        }





        [Export("onDrawerOptionClicked")]

        public void onDrawerOptionClicked(View view)
        {
            switch (view.Id)
            {
                case Resource.Id.drawer_option_load:
                    if (CropImage.IsExplicitCameraPermissionRequired(this))
                    {
                        RequestPermissions(new string[] { Manifest.Permission.Camera }, CropImage.CameraCapturePermissionsRequestCode);
                    }
                    else
                    {
                        CropImage.StartPickImageActivity(this);
                    }
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_oval:
                    setMainFragmentByPreset(CropDemoPreset.CIRCULAR);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_rect:
                    setMainFragmentByPreset(CropDemoPreset.RECT);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_customized_overlay:
                    setMainFragmentByPreset(CropDemoPreset.CUSTOMIZED_OVERLAY);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_min_max_override:
                    setMainFragmentByPreset(CropDemoPreset.MIN_MAX_OVERRIDE);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_scale_center:
                    setMainFragmentByPreset(CropDemoPreset.SCALE_CENTER_INSIDE);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_toggle_scale:
                    mCropImageViewOptions.scaleType = mCropImageViewOptions.scaleType == CropImageView.ScaleType.FitCenter
                            ? CropImageView.ScaleType.CenterInside : mCropImageViewOptions.scaleType == CropImageView.ScaleType.CenterInside
                            ? CropImageView.ScaleType.Center : mCropImageViewOptions.scaleType == CropImageView.ScaleType.Center
                            ? CropImageView.ScaleType.CenterCrop : CropImageView.ScaleType.FitCenter;
                    mCurrentFragment.setCropImageViewOptions(mCropImageViewOptions);
                    updateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_shape:
                    mCropImageViewOptions.cropShape = mCropImageViewOptions.cropShape == CropImageView.CropShape.Rectangle
                            ? CropImageView.CropShape.Oval : CropImageView.CropShape.Rectangle;
                    mCurrentFragment.setCropImageViewOptions(mCropImageViewOptions);
                    updateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_guidelines:
                    mCropImageViewOptions.guidelines = mCropImageViewOptions.guidelines == CropImageView.Guidelines.Off
                            ? CropImageView.Guidelines.On : mCropImageViewOptions.guidelines == CropImageView.Guidelines.On
                            ? CropImageView.Guidelines.OnTouch : CropImageView.Guidelines.Off;
                    mCurrentFragment.setCropImageViewOptions(mCropImageViewOptions);
                    updateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_aspect_ratio:
                    if (!mCropImageViewOptions.fixAspectRatio)
                    {
                        mCropImageViewOptions.fixAspectRatio = true;
                        mCropImageViewOptions.aspectRatio = new Android.Util.Pair(1, 1);
                    }
                    else
                    {
                        if ((int)mCropImageViewOptions.aspectRatio.First == 1 && (int)mCropImageViewOptions.aspectRatio.Second == 1)
                        {
                            mCropImageViewOptions.aspectRatio = new Android.Util.Pair(4, 3);
                        }
                        else if ((int)mCropImageViewOptions.aspectRatio.First == 4 && (int)mCropImageViewOptions.aspectRatio.Second == 3)
                        {
                            mCropImageViewOptions.aspectRatio = new Android.Util.Pair(16, 9);
                        }
                        else if ((int)mCropImageViewOptions.aspectRatio.First == 16 && (int)mCropImageViewOptions.aspectRatio.Second == 9)
                        {
                            mCropImageViewOptions.aspectRatio = new Android.Util.Pair(9, 16);
                        }
                        else
                        {
                            mCropImageViewOptions.fixAspectRatio = false;
                        }
                    }
                    mCurrentFragment.setCropImageViewOptions(mCropImageViewOptions);
                    updateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_auto_zoom:
                    mCropImageViewOptions.autoZoomEnabled = !mCropImageViewOptions.autoZoomEnabled;
                    mCurrentFragment.setCropImageViewOptions(mCropImageViewOptions);
                    updateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_max_zoom:
                    mCropImageViewOptions.maxZoomLevel = mCropImageViewOptions.maxZoomLevel == 4 ? 8
                            : mCropImageViewOptions.maxZoomLevel == 8 ? 2 : 4;
                    mCurrentFragment.setCropImageViewOptions(mCropImageViewOptions);
                    updateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_set_initial_crop_rect:
                    mCurrentFragment.setInitialCropRect();
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_reset_crop_rect:
                    mCurrentFragment.resetCropRect();
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_toggle_show_overlay:
                    mCropImageViewOptions.showCropOverlay = !mCropImageViewOptions.showCropOverlay;
                    mCurrentFragment.setCropImageViewOptions(mCropImageViewOptions);
                    updateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_show_progress_bar:
                    mCropImageViewOptions.showProgressBar = !mCropImageViewOptions.showProgressBar;
                    mCurrentFragment.setCropImageViewOptions(mCropImageViewOptions);
                    updateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                default:
                    Toast.MakeText(this, "Unknown drawer option clicked", ToastLength.Long).Show();
                    break;
            }
        }

        private void setMainFragmentByPreset(CropDemoPreset demoPreset)
        {
            var fragmentManager = SupportFragmentManager;
            fragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, MainFragment.newInstance(demoPreset))
                    .Commit();
        }

        private void updateDrawerTogglesByOptions(CropImageViewOptions options)
        {
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_scale)).Text = Resources.GetString(Resource.String.drawer_option_toggle_scale, options.scaleType.Name());
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_shape)).Text = Resources.GetString(Resource.String.drawer_option_toggle_shape, options.cropShape.Name());
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_guidelines)).Text = Resources.GetString(Resource.String.drawer_option_toggle_guidelines, options.guidelines.Name());
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_show_overlay)).Text = Resources.GetString(Resource.String.drawer_option_toggle_show_overlay, options.showCropOverlay.ToString());
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_show_progress_bar)).Text = Resources.GetString(Resource.String.drawer_option_toggle_show_progress_bar, options.showProgressBar.ToString());

            var aspectRatio = "FREE";
            if (options.fixAspectRatio)
            {
                aspectRatio = options.aspectRatio.First + ":" + options.aspectRatio.Second;
            }
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_aspect_ratio)).Text = Resources.GetString(Resource.String.drawer_option_toggle_aspect_ratio, aspectRatio);

            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_auto_zoom)).Text = Resources.GetString(Resource.String.drawer_option_toggle_auto_zoom, options.autoZoomEnabled ? "Enabled" : "Disabled");
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_max_zoom)).Text = Resources.GetString(Resource.String.drawer_option_toggle_max_zoom, options.maxZoomLevel);
        }
    }

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

public class CropResultActivity : Activity
{

    /**
     * The image to show in the activity.
     */
    public static Bitmap mImage;

    private ImageView imageView;


    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        RequestWindowFeature(WindowFeatures.NoTitle);
        SetContentView(Resource.Layout.activity_crop_result);

        imageView = ((ImageView)FindViewById(Resource.Id.resultImageView));
        imageView.SetBackgroundResource(Resource.Drawable.backdrop);

        if (mImage != null)
        {
            imageView.SetImageBitmap(mImage);
            double ratio = ((int)(10 * mImage.Width / (double)mImage.Height)) / 10d;
            int byteCount = 0;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.HoneycombMr1)
            {
                byteCount = mImage.ByteCount / 1024;
            }
            var desc = "(" + mImage.Width + ", " + mImage.Height + "), Ratio: " + ratio + ", Bytes: " + byteCount + "K";
            ((TextView)FindViewById(Resource.Id.resultImageText)).Text = desc;
        }
        else
        {
            Intent intent = Intent;
            var imageUri = (Android.Net.Uri)intent.GetParcelableExtra("URI");
            if (imageUri != null)
            {
                imageView.SetImageURI(imageUri);
            }
            else
            {
                Toast.MakeText(this, "No image is set to show", ToastLength.Long).Show();
            }
        }
    }


    public override void OnBackPressed()
    {
        releaseBitmap();
        base.OnBackPressed();
    }

    public void onImageViewClicked(View view)
    {
        releaseBitmap();
        Finish();
    }

    private void releaseBitmap()
    {
        if (mImage != null)
        {
            mImage.Recycle();
            mImage = null;
        }
    }
}

public enum CropDemoPreset
{
    RECT,
    CIRCULAR,
    CUSTOMIZED_OVERLAY,
    MIN_MAX_OVERRIDE,
    SCALE_CENTER_INSIDE,
    CUSTOM
}
}

