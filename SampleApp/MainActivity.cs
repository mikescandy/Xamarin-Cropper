using Android.Support.V7.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Theartofdev.Edmodo.Cropper;
using Android.App;
using Android.Support.V4.Widget;
using Android.Content.PM;
using Android;
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

            if (requestCode == CropImage.PickImageChooserRequestCode && resultCode == Result.Ok)
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
}