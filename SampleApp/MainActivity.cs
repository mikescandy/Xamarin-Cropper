using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Theartofdev.Edmodo.Cropper;
using Java.Interop;
using Uri = Android.Net.Uri;

namespace SampleApp
{
    [Activity(Label = "@string/app_title", MainLauncher = true, Theme = "@style/Theme.AppCompat", Icon = "@drawable/ic_launcher")]
    public class MainActivity : AppCompatActivity
    {

        //region: Fields and Consts

        private DrawerLayout _drawerLayout;

        private ActionBarDrawerToggle _drawerToggle;

        private MainFragment _currentFragment;

        private Uri _cropImageUri;

        private CropImageViewOptions _cropImageViewOptions = new CropImageViewOptions();
        //endregion

        public void SetCurrentFragment(MainFragment fragment)
        {
            _currentFragment = fragment;
        }

        public void SetCurrentOptions(CropImageViewOptions options)
        {
            _cropImageViewOptions = options;
            UpdateDrawerTogglesByOptions(options);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            _drawerLayout = (DrawerLayout)FindViewById(Resource.Id.drawer_layout);

            _drawerToggle = new ActionBarDrawerToggle(this, _drawerLayout, Resource.String.main_drawer_open, Resource.String.main_drawer_close);
            _drawerToggle.DrawerIndicatorEnabled = true;
            _drawerLayout.SetDrawerListener(_drawerToggle);

            if (savedInstanceState == null)
            {
                SetMainFragmentByPreset(CropDemoPreset.Rect);
            }
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            _drawerToggle.SyncState();
            _currentFragment.UpdateCurrentCropViewOptions();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (_drawerToggle.OnOptionsItemSelected(item))
            {
                return true;
            }
            if (_currentFragment != null && _currentFragment.OnOptionsItemSelected(item))
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
                    _cropImageUri = imageUri;

                    RequestPermissions(new[] { Manifest.Permission.ReadExternalStorage }, CropImage.PickImagePermissionsRequestCode);
                }
                else
                {

                    _currentFragment.setImageUri(imageUri);
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
                if (_cropImageUri != null && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    _currentFragment.setImageUri(_cropImageUri);
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
                        RequestPermissions(new[] { Manifest.Permission.Camera }, CropImage.CameraCapturePermissionsRequestCode);
                    }
                    else
                    {
                        CropImage.StartPickImageActivity(this);
                    }
                    _drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_oval:
                    SetMainFragmentByPreset(CropDemoPreset.Circular);
                    _drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_rect:
                    SetMainFragmentByPreset(CropDemoPreset.Rect);
                    _drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_customized_overlay:
                    SetMainFragmentByPreset(CropDemoPreset.CustomizedOverlay);
                    _drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_min_max_override:
                    SetMainFragmentByPreset(CropDemoPreset.MinMaxOverride);
                    _drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_scale_center:
                    SetMainFragmentByPreset(CropDemoPreset.ScaleCenterInside);
                    _drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_toggle_scale:
                    _cropImageViewOptions.ScaleType = _cropImageViewOptions.ScaleType == CropImageView.ScaleType.FitCenter
                            ? CropImageView.ScaleType.CenterInside : _cropImageViewOptions.ScaleType == CropImageView.ScaleType.CenterInside
                            ? CropImageView.ScaleType.Center : _cropImageViewOptions.ScaleType == CropImageView.ScaleType.Center
                            ? CropImageView.ScaleType.CenterCrop : CropImageView.ScaleType.FitCenter;
                    _currentFragment.SetCropImageViewOptions(_cropImageViewOptions);
                    UpdateDrawerTogglesByOptions(_cropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_shape:
                    _cropImageViewOptions.CropShape = _cropImageViewOptions.CropShape == CropImageView.CropShape.Rectangle
                            ? CropImageView.CropShape.Oval : CropImageView.CropShape.Rectangle;
                    _currentFragment.SetCropImageViewOptions(_cropImageViewOptions);
                    UpdateDrawerTogglesByOptions(_cropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_guidelines:
                    _cropImageViewOptions.Guidelines = _cropImageViewOptions.Guidelines == CropImageView.Guidelines.Off
                            ? CropImageView.Guidelines.On : _cropImageViewOptions.Guidelines == CropImageView.Guidelines.On
                            ? CropImageView.Guidelines.OnTouch : CropImageView.Guidelines.Off;
                    _currentFragment.SetCropImageViewOptions(_cropImageViewOptions);
                    UpdateDrawerTogglesByOptions(_cropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_aspect_ratio:
                    if (!_cropImageViewOptions.FixAspectRatio)
                    {
                        _cropImageViewOptions.FixAspectRatio = true;
                        _cropImageViewOptions.AspectRatio = (1, 1);
                    }
                    else
                    {
                        if (_cropImageViewOptions.AspectRatio.AspectRatioX == 1 && _cropImageViewOptions.AspectRatio.AspectRatioY == 1)
                        {
                            _cropImageViewOptions.AspectRatio = (4, 3);
                        }
                        else if (_cropImageViewOptions.AspectRatio.AspectRatioX == 4 && _cropImageViewOptions.AspectRatio.AspectRatioY == 3)
                        {
                            _cropImageViewOptions.AspectRatio = (16, 9);
                        }
                        else if (_cropImageViewOptions.AspectRatio.AspectRatioX == 16 && _cropImageViewOptions.AspectRatio.AspectRatioY == 9)
                        {
                            _cropImageViewOptions.AspectRatio = (9, 16);
                        }
                        else
                        {
                            _cropImageViewOptions.FixAspectRatio = false;
                        }
                    }
                    _currentFragment.SetCropImageViewOptions(_cropImageViewOptions);
                    UpdateDrawerTogglesByOptions(_cropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_auto_zoom:
                    _cropImageViewOptions.AutoZoomEnabled = !_cropImageViewOptions.AutoZoomEnabled;
                    _currentFragment.SetCropImageViewOptions(_cropImageViewOptions);
                    UpdateDrawerTogglesByOptions(_cropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_max_zoom:
                    _cropImageViewOptions.MaxZoomLevel = _cropImageViewOptions.MaxZoomLevel == 4 ? 8
                            : _cropImageViewOptions.MaxZoomLevel == 8 ? 2 : 4;
                    _currentFragment.SetCropImageViewOptions(_cropImageViewOptions);
                    UpdateDrawerTogglesByOptions(_cropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_set_initial_crop_rect:
                    _currentFragment.SetInitialCropRect();
                    _drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_reset_crop_rect:
                    _currentFragment.ResetCropRect();
                    _drawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_toggle_multitouch:
                    _cropImageViewOptions.Multitouch = !_cropImageViewOptions.Multitouch;
                    _currentFragment.SetCropImageViewOptions(_cropImageViewOptions);
                    UpdateDrawerTogglesByOptions(_cropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_show_overlay:
                    _cropImageViewOptions.ShowCropOverlay = !_cropImageViewOptions.ShowCropOverlay;
                    _currentFragment.SetCropImageViewOptions(_cropImageViewOptions);
                    UpdateDrawerTogglesByOptions(_cropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_show_progress_bar:
                    _cropImageViewOptions.ShowProgressBar = !_cropImageViewOptions.ShowProgressBar;
                    _currentFragment.SetCropImageViewOptions(_cropImageViewOptions);
                    UpdateDrawerTogglesByOptions(_cropImageViewOptions);
                    break;
                default:
                    Toast.MakeText(this, "Unknown drawer option clicked", ToastLength.Long).Show();
                    break;
            }
        }

        private void SetMainFragmentByPreset(CropDemoPreset demoPreset)
        {
            var fragmentManager = SupportFragmentManager;
            fragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, MainFragment.NewInstance(demoPreset))
                    .Commit();
        }

        private void UpdateDrawerTogglesByOptions(CropImageViewOptions options)
        {
            FindViewById<TextView>(Resource.Id.drawer_option_toggle_scale).Text = Resources.GetString(Resource.String.drawer_option_toggle_scale, options.ScaleType.Name());
            FindViewById<TextView>(Resource.Id.drawer_option_toggle_shape).Text = Resources.GetString(Resource.String.drawer_option_toggle_shape, options.CropShape.Name());
            FindViewById<TextView>(Resource.Id.drawer_option_toggle_guidelines).Text = Resources.GetString(Resource.String.drawer_option_toggle_guidelines, options.Guidelines.Name());
            FindViewById<TextView>(Resource.Id.drawer_option_toggle_multitouch).Text = Resources.GetString(Resource.String.drawer_option_toggle_multitouch, options.Multitouch.ToString());
            FindViewById<TextView>(Resource.Id.drawer_option_toggle_show_overlay).Text = Resources.GetString(Resource.String.drawer_option_toggle_show_overlay, options.ShowCropOverlay.ToString());
            FindViewById<TextView>(Resource.Id.drawer_option_toggle_show_progress_bar).Text = Resources.GetString(Resource.String.drawer_option_toggle_show_progress_bar, options.ShowProgressBar.ToString());

            var aspectRatio = "FREE";
            if (options.FixAspectRatio)
            {
                aspectRatio = options.AspectRatio.AspectRatioX + ":" + options.AspectRatio.AspectRatioY;
            }
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_aspect_ratio)).Text = Resources.GetString(Resource.String.drawer_option_toggle_aspect_ratio, aspectRatio);

            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_auto_zoom)).Text = Resources.GetString(Resource.String.drawer_option_toggle_auto_zoom, options.AutoZoomEnabled ? "Enabled" : "Disabled");
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_max_zoom)).Text = Resources.GetString(Resource.String.drawer_option_toggle_max_zoom, options.MaxZoomLevel);
        }
    }
}