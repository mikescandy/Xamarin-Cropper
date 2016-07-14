using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Interop;

namespace App4
{
    [Activity(Label = "App4", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat")]
    public class CropResultActivity : AppCompatActivity
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

        [Export("onImageViewClicked")]
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
}