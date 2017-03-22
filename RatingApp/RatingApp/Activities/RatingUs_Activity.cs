using Android.App;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Content;
using Android.Provider;
using Java.IO;
using System;
using Android.Support.V7.App;
using RatingApp.Helpers;
using Android.Content.PM;
using System.Collections.Generic;
using Builder = Android.Support.V7.App.AlertDialog.Builder;
using RadialProgress;
using Android.Views;

namespace RatingApp.Activities
{
    [Activity(Label = "RatingActivity")]
    public class RatingUs_Activity : AppCompatActivity
    {
        public static File file;
        public static File dir;
        public static Bitmap bitmap;
        private ImageView imageView;
        private Button pictureButton;
        private TextView resultTextView;
        private bool isCaptureMode = true;
        private Builder alert;
        private RadialProgressView progressView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.rateus_layout);
            resultTextView = FindViewById<TextView>(Resource.Id.resultText);
            imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            pictureButton = FindViewById<Button>(Resource.Id.GetPictureButton);
            progressView = FindViewById<RadialProgressView>(Resource.Id.progressView);

            progressView.MinValue = 0;
            progressView.MaxValue = 5;
            if (progressView.IsDone)
                progressView.Reset();

            alert = new Builder(this);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
                pictureButton.Click += OnActionClick;
            }
        }

        #region Check whether any App is available to Take Pictures on current phone
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(
                                     intent,
                                     PackageInfoFlags.MatchDefaultOnly
                                     );
            return ((null != availableActivities) &&
                    (availableActivities.Count > 0));
        }
        #endregion

        #region Create a Directory on Android to store Pictures
        private void CreateDirectoryForPictures()
        {
            dir = new File(
                   Android.OS.Environment.GetExternalStoragePublicDirectory(
                                Android.OS.Environment.DirectoryPictures),
                                GetString(Resource.String.imageFolderName)
                           );
            if (!dir.Exists())
            {
                dir.Mkdirs();
            }
        }
        #endregion



        private void OnActionClick(object sender, EventArgs eventArgs)
        {
            //Take Camera Button clicked.
            if (!isCaptureMode)//submit new Image button clicked!
            {
                imageView.SetImageBitmap(null);
                if (bitmap != null)
                {
                    progressView.Visibility = ViewStates.Invisible;
                    bitmap.Recycle();
                    bitmap.Dispose();
                    bitmap = null;
                }
            }
            // Take Picture
            CaptureImage();
        }

        public void CaptureImage()
        {
            alert.SetTitle(GetString(Resource.String.alertTitle));
            alert.SetMessage(GetString(Resource.String.alertMessage));
            alert.SetPositiveButton(GetString(Resource.String.allow), (senderAlert, args) => {
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                file = new File(dir, String.Format
                                        (
                                           GetString(Resource.String.imageName),
                                           Guid.NewGuid()
                                        )
                                 );
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(file));
                StartActivityForResult(intent, 0);
            });
            alert.SetNegativeButton(GetString(Resource.String.deny), (senderAlert, args) => {
                Toast.MakeText(this, GetString(Resource.String.cancelled), ToastLength.Short).Show();
            });
            Dialog dialog = alert.Create();
            dialog.Show();
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                //Get the bitmap with the right rotation
                //bitmap = BitmapHelpers.GetAndRotateBitmap(file.Path);
                Bitmap bitmap = BitmapFactory.DecodeFile(file.Path);
                //Resize the picture to be under 4MB 
                //(Emotion API limitation and better for Android memory)
                bitmap = Bitmap.CreateScaledBitmap(bitmap, 500,
                                    (int)(500 * bitmap.Height / bitmap.Width),
                                    false);

                //Display the image
                imageView.SetImageBitmap(bitmap);

                //Loading message
                resultTextView.Text = GetString(Resource.String.loadText);
                pictureButton.Enabled = false;

                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    //Get a stream
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, stream);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    //Get and display the happiness score
                    var result = await Emotion_Helper.GetAverageHappinessScore(stream);
                    resultTextView.Text = GetString(Resource.String.rateMessage);
                    progressView.Value = Emotion_Helper.GetHappinessMessage(result);
                    progressView.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                resultTextView.Text = "We have an issue with the image which you have taken, Please try again once.";
            }
            finally
            {
                pictureButton.Enabled = true;
                pictureButton.Text = GetString(Resource.String.imageSubmit);
                isCaptureMode = false;
            }
        }
    }
}