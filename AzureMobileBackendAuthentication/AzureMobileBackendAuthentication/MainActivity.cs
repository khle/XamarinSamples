using Android.App;
using Android.OS;
using Android.Widget;
using System;
using Android.Support.V7.App;
using Microsoft.WindowsAzure.MobileServices;
using Android.Webkit;

namespace AzureMobileBackendAuthentication
{
    [Activity(Label = "@string/AppName", MainLauncher = true, Icon = "@drawable/AppLogo")]
    public class MainActivity : AppCompatActivity
    {
        Button GmailBtn, FbBtn, TwitterBtn, LogoutBtn;
        MobileServiceClient MobileService;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            
            GmailBtn = FindViewById<Button>(Resource.Id.GmailButton);
            FbBtn = FindViewById<Button>(Resource.Id.FbButton);
            TwitterBtn = FindViewById<Button>(Resource.Id.TwitterButton);
            LogoutBtn = FindViewById<Button>(Resource.Id.LogoutButton);
            LogoutBtn.Enabled = false;

            MobileService = new MobileServiceClient(Constants.AZUREMOBILEAPPSURL);
            WireEvents();
        }
        
        void WireEvents()
        {
            GmailBtn.Click += delegate { Provider_Validation(Constants.GMAIL); };
            FbBtn.Click += delegate { Provider_Validation(Constants.FACEBOOK); };
            TwitterBtn.Click += delegate { Provider_Validation(Constants.TWITTER); }; 
            LogoutBtn.Click += Logout;
        }

        void Provider_Validation(string authValue)
        {
            switch (authValue)
            {
                case Constants.GMAIL:
                    Authentcation(MobileServiceAuthenticationProvider.Google);
                    break;

                case Constants.FACEBOOK:
                    Authentcation(MobileServiceAuthenticationProvider.Facebook);
                    break;
                    
                case Constants.TWITTER:
                    Authentcation(MobileServiceAuthenticationProvider.Twitter);
                    break;
            }
        }

        async void Authentcation(MobileServiceAuthenticationProvider provider)
        {
            try
            {
                //Single line to display Sign-In form for providers..
                await MobileService.LoginAsync(this, provider);
                FbBtn.Enabled = GmailBtn.Enabled = TwitterBtn.Enabled = false;
                LogoutBtn.Enabled = true;
            }
            catch (Exception)
            {
                Toast.MakeText(this, Constants.FAIL_AUTH, ToastLength.Short).Show();
            }
        }


        private async void Logout(object sender, EventArgs e)
        {
            CookieManager.Instance.RemoveAllCookie();
            await MobileService.LogoutAsync();

            Toast.MakeText(this, Constants.LOGGED_OUT, ToastLength.Short).Show();
  
            FbBtn.Enabled = GmailBtn.Enabled = TwitterBtn.Enabled = true;
            LogoutBtn.Enabled = false;
        }
    }
}