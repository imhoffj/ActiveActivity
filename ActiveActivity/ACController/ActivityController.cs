using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Widget;
using ActiveActivity.ACTask;

namespace ActiveActivity.ACController {

	public partial class ActivityController {

		#region Properties
		public string AssociatedActivityId { get; internal set; } = Guid.NewGuid ().ToString ();
		public ActivityScope ActivityScope { get; private set; }
		// Stores pending startActivityForResult invocations
		Dictionary<int, TaskCompletionSource<ActivityResult>> activityCompletionSources = new Dictionary<int, TaskCompletionSource<ActivityResult>> ();
		#endregion

		#region Methods
		/// <summary>
		/// Gets the activity.
		/// </summary>
		/// <returns>The activity.</returns>
		/// <typeparam name="TActivity">The 1st type parameter.</typeparam>
		/// <typeparam name="TController">The 2nd type parameter.</typeparam>
		public TController GetActivity<TActivity, TController> () where TActivity : ActivityController where TController : ControllerActivity<TActivity>
		{
			var info = ActivityControllerRegistry.Get<TActivity> (AssociatedActivityId);
			return (TController)info.ControllerActivity;
		}

		/// <summary>
		/// Gets the activity.
		/// </summary>
		/// <value>The activity.</value>
		public AppCompatActivity Activity {
			get {
				return ActivityControllerRegistry.Get (AssociatedActivityId).AppCompatActivity;
			}
		}


		/// <summary>
		/// Starts the activity for result async.
		/// </summary>
		/// <returns>The activity for result async.</returns>
		/// <param name="intent">Intent.</param>
		public Task<ActivityResult> StartActivityForResultAsync (Intent intent)
		{
			return StartActivityForResultAsync (intent, ActivityControllerRegistry.NextRequestCode ());
		}

		/// <summary>
		/// Starts the activity for result async.
		/// </summary>
		/// <returns>The activity for result async.</returns>
		/// <param name="intent">Intent.</param>
		/// <param name="requestCode">Request code.</param>
		public Task<ActivityResult> StartActivityForResultAsync (Intent intent, int requestCode)
		{
			if (activityCompletionSources.ContainsKey (requestCode))
				throw new ArgumentException ("There is already an activity request started for this requestCode", nameof (requestCode));

			var tcs = new TaskCompletionSource<ActivityResult> ();

			activityCompletionSources.Add (requestCode, tcs);

			Activity.StartActivityForResult (intent, requestCode);

			return tcs.Task;
		}

		/// <summary>
		/// Starts the activity for result async.
		/// </summary>
		/// <returns>The activity for result async.</returns>
		/// <param name="activityType">Activity type.</param>
		/// <typeparam name="TActivityResult">The 1st type parameter.</typeparam>
		public Task<TActivityResult> StartActivityForResultAsync<TActivityResult> (Type activityType) where TActivityResult : ActivityResult
		{
			var intent = new Intent (Activity, activityType);
			return StartActivityForResultAsync<TActivityResult> (intent);
		}

		/// <summary>
		/// Starts the activity for result async.
		/// </summary>
		/// <returns>The activity for result async.</returns>
		/// <param name="intent">Intent.</param>
		/// <typeparam name="TActivityResult">The 1st type parameter.</typeparam>
		public Task<TActivityResult> StartActivityForResultAsync<TActivityResult> (Intent intent) where TActivityResult : ActivityResult
		{
			return StartActivityForResultAsync<TActivityResult> (intent, ActivityControllerRegistry.NextRequestCode ());
		}

		/// <summary>
		/// Starts the activity for result async.
		/// </summary>
		/// <returns>The activity for result async.</returns>
		/// <param name="intent">Intent.</param>
		/// <param name="requestCode">Request code.</param>
		/// <typeparam name="TActivityResult">The 1st type parameter.</typeparam>
		public async Task<TActivityResult> StartActivityForResultAsync<TActivityResult> (Intent intent, int requestCode) where TActivityResult : ActivityResult
		{
			var result = await StartActivityForResultAsync (intent, requestCode);
			if (result == null)
				return null;

			return (TActivityResult)Activator.CreateInstance (typeof (TActivityResult), result.ResultCode, result.RequestCode, result.Data);
		}

		/// <summary>
		/// Wireups the handlers.
		/// </summary>
		/// <param name="activity">Activity.</param>
		/// <typeparam name="TActivityController">The 1st type parameter.</typeparam>
		internal void WireupHandlers<TActivityController> (ControllerActivity<TActivityController> activity) where TActivityController : ActivityController
		{
			activity.OnStartHandler = OnStart;
			activity.OnCreateHandler = (p) => this.OnCreate (p);
			activity.OnActivityResultHandler = HandleOnActivityResult;
			activity.OnStopHandler = OnStop;
			activity.OnResumeHandler = OnResume;
			activity.OnPauseHandler = OnPause;
			activity.OnDestroyHandler = OnDestroy;
			activity.OnRestoreInstanceStateHandler = OnRestoreInstanceState;
			activity.OnSaveInstanceStateHandler = OnSaveInstanceState;
			activity.OnDestroyHandler = OnDestroy;
			activity.OnBackPressedHandler = OnBackPressed;
			activity.OnRestartHandler = OnRestart;
			activity.OnLowMemoryHandler = OnLowMemory;
			activity.OnPostResumeHandler = OnPostResume;
			activity.OnAttachedToWindowHandler = OnAttachedToWindow;
			activity.OnDetachedFromWindowHandler = OnDetachedFromWindow;
			activity.OnRequestPermissionsResultHandler = OnRequestPermissionsResult;
			activity.OnCreateContextMenuHandler = OnCreateContextMenu;
			activity.OnNewIntentHandler = OnNewIntent;
		}

		/// <summary>
		/// Handles the on activity result.
		/// </summary>
		/// <param name="requestCode">Request code.</param>
		/// <param name="resultCode">Result code.</param>
		/// <param name="data">Data.</param>
		void HandleOnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			if (activityCompletionSources.ContainsKey (requestCode)) {
				var tcs = activityCompletionSources [requestCode];

				if (resultCode == Result.Canceled) {
					tcs.TrySetResult (null);
					return;
				}
				tcs.TrySetResult (new ActivityResult (resultCode, requestCode, data));
			} else {
				OnActivityResult (requestCode, resultCode, data);
			}
		}

		/// <summary>
		/// Ons the activity result.
		/// </summary>
		/// <param name="requestCode">Request code.</param>
		/// <param name="resultCode">Result code.</param>
		/// <param name="data">Data.</param>
		protected virtual void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
		}

		/// <summary>
		/// Ons the start.
		/// </summary>
		protected virtual void OnStart ()
		{
		}

		/// <summary>
		/// Ons the create.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected virtual void OnCreate (Bundle savedInstanceState)
		{
			ActivityScope = ActivityScope.Of (this.Activity);
		}

		/// <summary>
		/// Finish this instance.
		/// </summary>
		protected virtual void Finish ()
		{
			Activity.Finish ();
			ActivityControllerRegistry.Destroy (AssociatedActivityId);
		}

		/// <summary>
		/// Finds the view by identifier.
		/// </summary>
		/// <returns>The view by identifier.</returns>
		/// <param name="resourceId">Resource identifier.</param>
		/// <typeparam name="TView">The 1st type parameter.</typeparam>
		public TView FindViewById<TView> (int resourceId) where TView : global::Android.Views.View
		{
			return Activity.FindViewById<TView> (resourceId);
		}

		/// <summary>
		/// Sets the content view.
		/// </summary>
		/// <param name="layoutResId">Layout res identifier.</param>
		public void SetContentView (int layoutResId)
		{
			Activity.SetContentView (layoutResId);
		}

		/// <summary>
		/// Ons the state of the save instance.
		/// </summary>
		/// <param name="outState">Out state.</param>
		protected virtual void OnSaveInstanceState (Bundle outState)
		{
		}

		/// <summary>
		/// Ons the state of the restore instance.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected virtual void OnRestoreInstanceState (Bundle savedInstanceState)
		{
		}

		/// <summary>
		/// Ons the destroy.
		/// </summary>
		protected virtual void OnDestroy ()
		{
		}

		/// <summary>
		/// Ons the stop.
		/// </summary>
		protected virtual void OnStop ()
		{
		}

		/// <summary>
		/// Ons the pause.
		/// </summary>
		protected virtual void OnPause ()
		{
		}

		/// <summary>
		/// Ons the resume.
		/// </summary>
		protected virtual void OnResume ()
		{
		}

		/// <summary>
		/// Ons the back pressed.
		/// </summary>
		protected virtual void OnBackPressed ()
		{
		}

		/// <summary>
		/// Ons the restart.
		/// </summary>
		protected virtual void OnRestart ()
		{
		}

		/// <summary>
		/// Ons the low memory.
		/// </summary>
		public virtual void OnLowMemory ()
		{
		}

		/// <summary>
		/// Ons the post resume.
		/// </summary>
		protected virtual void OnPostResume ()
		{
		}

		/// <summary>
		/// Ons the attached to window.
		/// </summary>
		public virtual void OnAttachedToWindow ()
		{
		}

		/// <summary>
		/// Ons the detached from window.
		/// </summary>
		public virtual void OnDetachedFromWindow ()
		{
		}

		/// <summary>
		/// Ons the request permissions result.
		/// </summary>
		/// <param name="requestCode">Request code.</param>
		/// <param name="permissions">Permissions.</param>
		/// <param name="grantResults">Grant results.</param>
		public virtual void OnRequestPermissionsResult (int requestCode, string [] permissions, Android.Content.PM.Permission [] grantResults)
		{
		}

		/// <summary>
		/// Ons the create context menu.
		/// </summary>
		/// <param name="menu">Menu.</param>
		/// <param name="v">V.</param>
		/// <param name="menuInfo">Menu info.</param>
		public virtual void OnCreateContextMenu (Android.Views.IContextMenu menu, Android.Views.View v, Android.Views.IContextMenuContextMenuInfo menuInfo)
		{
		}

		/// <summary>
		/// Ons the new intent.
		/// </summary>
		/// <param name="intent">Intent.</param>
		protected virtual void OnNewIntent (Intent intent)
		{
		}
		#endregion
	}
}
