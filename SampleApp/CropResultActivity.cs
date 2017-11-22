using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Interop;

namespace SampleApp
{
    [Activity(MainLauncher = false, Theme = "@style/Theme.AppCompat")]
    public class CropResultActivity : AppCompatActivity
    {

        /**
     * The image to show in the activity.
     */
        public static Bitmap Image;

        private ImageView _imageView;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_crop_result);

            _imageView = ((ImageView)FindViewById(Resource.Id.resultImageView));
            _imageView.SetBackgroundResource(Resource.Drawable.backdrop);

            var intent = Intent;
            if (Image != null)
            {
                _imageView.SetImageBitmap(Image);
                var sampleSize = intent.GetIntExtra("SAMPLE_SIZE", 1);
                var ratio = (int)(10 * Image.Width / (double)Image.Height) / 10d;
                var byteCount = 0;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.HoneycombMr1)
                {
                    byteCount = Image.ByteCount / 1024;
                }
                var desc = $"({Image.Width}, {Image.Height}), Sample: {sampleSize}, Ratio: {ratio}, Bytes: {byteCount}K";
                FindViewById<TextView>(Resource.Id.resultImageText).Text = desc;
            }
            else
            {
                var imageUri = intent.GetParcelableExtra("URI").JavaCast<Android.Net.Uri>();
                if (imageUri != null)
                {
                    _imageView.SetImageURI(imageUri);
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

        [Export("onImageViewClicked")]
        public void onImageViewClicked(View view)
        {
            releaseBitmap();
            Finish();
        }

        private void releaseBitmap()
        {
            if (Image != null)
            {
                Image.Recycle();
                Image = null;
            }
        }
    }
}