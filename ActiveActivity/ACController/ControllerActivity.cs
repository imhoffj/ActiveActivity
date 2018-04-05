using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

namespace ActiveActivity.ACController {

	public abstract class ControllerActivity<TController> : AppCompatActivity where TController : ActivityController {

		#region Properties
		public string AssociatedActivityId { get; private set; }
		internal Action<Bundle> OnCreateHandler { get; set; }
		internal Action<int, Result, Intent> OnActivityResultHandler { get; set; }
		internal Action<Bundle> OnSaveInstanceStateHandler { get; set; }
		internal Action<Bundle> OnRestoreInstanceStateHandler { get; set; }
		internal Action OnDestroyHandler { get; set; }
		internal Action OnStartHandler { get; set; }
		internal Action OnStopHandler { get; set; }
		internal Action OnPauseHandler { get; set; }
		internal Action OnResumeHandler { get; set; }
		internal Action OnBackPressedHandler { get; set; }
		internal Action OnRestartHandler { get; set; }
		internal Action OnLowMemoryHandler { get; set; }
		internal Action OnPostResumeHandler { get; set; }
		internal Action OnAttachedToWindowHandler { get; set; }
		internal Action OnDetachedFromWindowHandler { get; set; }
		internal Action<int, string [], Android.Content.PM.Permission []> OnRequestPermissionsResultHandler { get; set; }
		internal Action<Android.Views.IContextMenu, Android.Views.View, Android.Views.IContextMenuContextMenuInfo> OnCreateContextMenuHandler { get; set; }
		internal Action<Intent> OnNewIntentHandler { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Gets the controller.
		/// </summary>
		/// <returns>The controller.</returns>
		public TController GetController ()
		{
			var info = (AssociatedActivityInfo<TController>)ActivityControllerRegistry.Get (AssociatedActivityId);
			return info.ActivityController;
		}

		/// <summary>
		/// Starts the activity for result async.
		/// </summary>
		/// <returns>The activity for result async.</returns>
		/// <param name="activityType">Activity type.</param>
		public Task<ActivityResult> StartActivityForResultAsync (Type activityType)
		{
			var intent = new Android.Content.Intent (this, activityType);
			return StartActivityForResultAsync (intent);
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
			var controller = GetController ();
			return controller.StartActivityForResultAsync (intent, requestCode);
		}

		/// <summary>
		/// Gets or sets the on create handler.
		/// </summary>
		/// <value>The on create handler.</value>
		protected override void OnCreate (Bundle savedInstanceState)
		{
			AssociatedActivityId = savedInstanceState?.GetString (ActivityControllerRegistry.ASSOC_ACTIVITY_ID)
						     ?? Intent?.GetStringExtra (ActivityControllerRegistry.ASSOC_ACTIVITY_ID)
						     ?? Guid.NewGuid ().ToString ();

			ActivityControllerRegistry.Associate<TController> (AssociatedActivityId, this);

			base.OnCreate (savedInstanceState);
			OnCreateHandler?.Invoke (savedInstanceState);
		}

		/// <summary>
		/// Ons the activity result.
		/// </summary>
		/// <param name="requestCode">Request code.</param>
		/// <param name="resultCode">Result code.</param>
		/// <param name="data">Data.</param>
		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			OnActivityResultHandler?.Invoke (requestCode, resultCode, data);
		}

		/// <summary>
		/// Ons the state of the save instance.
		/// </summary>
		/// <param name="outState">Out state.</param>
		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutString (ActivityControllerRegistry.ASSOC_ACTIVITY_ID, AssociatedActivityId);

			OnSaveInstanceStateHandler?.Invoke (outState);
			base.OnSaveInstanceState (outState);
		}

		/// <summary>
		/// Ons the state of the restore instance.
		/// </summary>
		/// <param name="savedInstanceState">Saved instance state.</param>
		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			OnRestoreInstanceStateHandler?.Invoke (savedInstanceState);
			base.OnRestoreInstanceState (savedInstanceState);
		}

		/// <summary>
		/// Ons the destroy.
		/// </summary>
		protected override void OnDestroy ()
		{
			OnDestroyHandler?.Invoke ();
			base.OnDestroy ();
		}

		/// <summary>
		/// Ons the start.
		/// </summary>
		protected override void OnStart ()
		{
			base.OnStart ();
			OnStartHandler?.Invoke ();
		}

		/// <summary>
		/// Ons the stop.
		/// </summary>
		protected override void OnStop ()
		{
			OnStopHandler?.Invoke ();
			base.OnStop ();
		}

		/// <summary>
		/// Ons the pause.
		/// </summary>
		protected override void OnPause ()
		{
			OnPauseHandler?.Invoke ();
			base.OnPause ();
		}

		/// <summary>
		/// Ons the resume.
		/// </summary>
		protected override void OnResume ()
		{
			base.OnResume ();
			OnResumeHandler?.Invoke ();
		}

		/// <summary>
		/// Ons the back pressed.
		/// </summary>
		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			OnBackPressedHandler?.Invoke ();
		}

		/// <summary>
		/// Ons the restart.
		/// </summary>
		protected override void OnRestart ()
		{
			base.OnRestart ();
			OnRestartHandler?.Invoke ();
		}

		/// <summary>
		/// Ons the low memory.
		/// </summary>
		public override void OnLowMemory ()
		{
			base.OnLowMemory ();
			OnLowMemoryHandler?.Invoke ();
		}

		/// <summary>
		/// Ons the post resume.
		/// </summary>
		protected override void OnPostResume ()
		{
			base.OnPostResume ();
			OnPostResumeHandler?.Invoke ();
		}

		/// <summary>
		/// Ons the attached to window.
		/// </summary>
		public override void OnAttachedToWindow ()
		{
			base.OnAttachedToWindow ();
			OnAttachedToWindowHandler?.Invoke ();
		}

		/// <summary>
		/// Ons the detached from window.
		/// </summary>
		public override void OnDetachedFromWindow ()
		{
			base.OnDetachedFromWindow ();
			OnDetachedFromWindowHandler?.Invoke ();
		}


		/// <summary>
		/// Ons the request permissions result.
		/// </summary>
		/// <param name="requestCode">Request code.</param>
		/// <param name="permissions">Permissions.</param>
		/// <param name="grantResults">Grant results.</param>
		public override void OnRequestPermissionsResult (int requestCode, string [] permissions, Android.Content.PM.Permission [] grantResults)
		{
			base.OnRequestPermissionsResult (requestCode, permissions, grantResults);
			OnRequestPermissionsResultHandler?.Invoke (requestCode, permissions, grantResults);
		}

		/// <summary>
		/// Ons the create context menu.
		/// </summary>
		/// <param name="menu">Menu.</param>
		/// <param name="v">V.</param>
		/// <param name="menuInfo">Menu info.</param>
		public override void OnCreateContextMenu (Android.Views.IContextMenu menu, Android.Views.View v, Android.Views.IContextMenuContextMenuInfo menuInfo)
		{
			base.OnCreateContextMenu (menu, v, menuInfo);
			OnCreateContextMenuHandler?.Invoke (menu, v, menuInfo);
		}

		/// <summary>
		/// Ons the new intent.
		/// </summary>
		/// <param name="intent">Intent.</param>
		protected override void OnNewIntent (Intent intent)
		{
			base.OnNewIntent (intent);
			OnNewIntentHandler?.Invoke (intent);
		}
		#endregion
	}
}
