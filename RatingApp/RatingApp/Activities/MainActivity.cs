using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Com.Yalantis.Guillotine.Animation;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Views.Animations;

namespace RatingApp.Activities
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        private FrameLayout root;
        private Toolbar toolbar;
        private View contentHamburger;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            root = FindViewById<FrameLayout>(Resource.Id.root);

            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.Title = null;
            }

            View navigationLayout = LayoutInflater
                                        .From(this)
                                        .Inflate(Resource.Layout.guillotine, null);
            root.AddView(navigationLayout);

            var navigationHamburger = navigationLayout
                                        .FindViewById(Resource.Id.guillotineHamburger);

            contentHamburger = FindViewById<ImageView>(Resource.Id.contentHamburger);

            new GuillotineAnimation.GuillotineBuilder(
                                        navigationLayout, 
                                        navigationHamburger, 
                                        contentHamburger)
                                            .SetClosedOnStart(true)
                                            .SetDuration(1000)
                                            .SetStartDelay(250)
                                            .SetActionBarViewForAnimation(toolbar)
                                            .SetInterpolator(new BounceInterpolator())
                                            .Build();

            var RateAppButton = navigationLayout.FindViewById(Resource.Id.settings_group);

            RateAppButton.Click += (sender, e) => {
                   StartActivity(typeof(RatingUs_Activity));
            };
        }
    }
}
