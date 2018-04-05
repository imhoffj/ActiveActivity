using System;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Support.V7.App;
using Android.Widget;
using Xamarin.Android;
using ActiveActivity.ACController;
using ActiveActivity.ACTask;
using System.Threading.Tasks;

namespace ActiveActivity {

	[Activity (MainLauncher = true, Label = "Active Activity ", Theme = "@style/Theme.AppCompat")]
	public class MainActivity : ControllerActivity<MainActivity.MainController> {

		public class MainController : ActivityController {

			#region Fields
			private static string HELLO_MESSAGE = "Hello";
			#endregion

			#region Properties
			public string DisplayName { get; private set; }
			TextView ContactLabel => ActivityScope.Instance.FindViewById<TextView> (Resource.Id.contactNameLabel);
			#endregion


			/// <summary>
			/// Ons the create.
			/// </summary>
			/// <param name="savedInstanceState">Saved instance state.</param>
			protected override void OnCreate (Android.OS.Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				SetContentView (Resource.Layout.Main);

				FindViewById<Button> (Resource.Id.standardButton).Click += GetContactStandard;
				FindViewById<Button> (Resource.Id.simpleButton).Click += GetContactSimple;
			}

			/// <summary>
			/// Gets the contact standard.
			/// </summary>
			/// <param name="sender">Sender.</param>
			/// <param name="e">E.</param>
			async void GetContactStandard (object sender, EventArgs e)
			{
				// Create the intent
				var contactPickerIntent = new Intent (Intent.ActionPick, ContactsContract.CommonDataKinds.Phone.ContentUri);

				// Await the started activity for its result
				var result = await StartActivityForResultAsync (contactPickerIntent);

				// Parse out the selected contact uri
				var contactUri = result?.Data?.Data;

				if (contactUri != null) {
					ContactLabel.Text = DisplayName = GetDisplayName (contactUri);
					await SayHelloAsync (ActivityScope);
				}
			}


			/// <summary>
			/// Gets the contact simple.
			/// </summary>
			/// <param name="sender">Sender.</param>
			/// <param name="e">E.</param>
			async void GetContactSimple (object sender, EventArgs e)
			{
				var result = await PickContactAsync ();
				var contactUri = result.SelectedContactUri;

				if (contactUri != null) {
					ContactLabel.Text = DisplayName = GetDisplayName (contactUri);
					await SayHelloAsync (ActivityScope);
				}
			}

			/// <summary>
			/// Gets the display name.
			/// </summary>
			/// <returns>The display name.</returns>
			/// <param name="uri">URI.</param>
			string GetDisplayName (Android.Net.Uri uri)
			{
				var c = Activity.ContentResolver.Query (uri, null, null, null, null);
				c.MoveToFirst ();
				return c.GetString (c.GetColumnIndex (ContactsContract.ContactNameColumns.DisplayNamePrimary));
			}


			/// <summary>
			/// Saies the hello async.
			/// </summary>
			/// <returns>The hello async.</returns>
			async ActivityTask SayHelloAsync (ActivityScope scope)
			{
				await Task.Delay (3000); // Medium network call
				ContactLabel.Text = HELLO_MESSAGE + " " + DisplayName;
			}


		}
	}

}

