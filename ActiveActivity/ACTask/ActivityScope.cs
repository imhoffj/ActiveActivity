using System;
using System.Collections.Concurrent;
using System.Threading;
using Android.App;
using Android.OS;
using Android.Runtime;

namespace ActiveActivity.ACTask {
	public class ActivityScope : IDisposable {

		enum ActivityState {
			Created,
			Started,
			Resumed,
			Paused,
			SaveInstance,
			Stopped,
			Destroyed
		}

		#region Fields
		private const string BundleIdentityKey = "5a81c10b-3f75-40a2-8108-46a5b7e8b558";
		private long _activityIdentity;
		private ActivityScopeListener _listener;
		private ConcurrentQueue<Action> _continuations = new ConcurrentQueue<Action> ();
		#endregion

		#region Properties
		public Activity Instance { get; private set; }
		internal bool IsUnavailable { get; private set; }
		#endregion

		#region Contructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ActiveActivity.ACTask.ActivityScope"/> class.
		/// </summary>
		/// <param name="activity">Activity.</param>
		ActivityScope (Activity activity)
		{
			Instance = activity;
			_activityIdentity = ActivityIdProvider.GetNextId ();
			_listener = new ActivityScopeListener ();
			_listener.ActivityStateChanged += HandleActivityStateChanged;
			activity.Application.RegisterActivityLifecycleCallbacks (_listener);
		}

		/// <summary>
		/// Of the specified activity.
		/// </summary>
		/// <returns>The of.</returns>
		/// <param name="activity">Activity.</param>
		public static ActivityScope Of (Activity activity) => new ActivityScope (activity);
		#endregion

		#region Methods
		/// <summary>
		/// Releases all resource used by the <see cref="T:ActiveActivity.ACTask.ActivityScope"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:ActiveActivity.ACTask.ActivityScope"/>.
		/// The <see cref="Dispose"/> method leaves the <see cref="T:ActiveActivity.ACTask.ActivityScope"/> in an unusable
		/// state. After calling <see cref="Dispose"/>, you must release all references to the
		/// <see cref="T:ActiveActivity.ACTask.ActivityScope"/> so the garbage collector can reclaim the memory that the
		/// <see cref="T:ActiveActivity.ACTask.ActivityScope"/> was occupying.</remarks>
		public void Dispose ()
		{
			if (_listener != null) {
				_listener.ActivityStateChanged -= HandleActivityStateChanged;
				Instance.Application.UnregisterActivityLifecycleCallbacks (_listener);
				_listener = null;
			}
		}

		/// <summary>
		/// Ops the implicit.
		/// </summary>
		/// <returns>The implicit.</returns>
		/// <param name="scope">Scope.</param>
		public static implicit operator Activity (ActivityScope scope)
		{
			return scope.Instance;
		}

		/// <summary>
		/// Ons the completed.
		/// </summary>
		/// <param name="continuation">Continuation.</param>
		internal void OnCompleted (Action continuation)
		{
			_continuations.Enqueue (continuation);
		}

		/// <summary>
		/// Handles the activity state changed.
		/// </summary>
		/// <param name="activity">Activity.</param>
		/// <param name="newState">New state.</param>
		/// <param name="savedData">Saved data.</param>
		void HandleActivityStateChanged (Activity activity, ActivityState newState, Bundle savedData)
		{
			switch (newState) {
			case ActivityState.Created:
				if (activity != null && !savedData.ContainsKey (BundleIdentityKey))
					return;
				if (savedData.GetLong (BundleIdentityKey) == _activityIdentity)
					Instance = activity;
				break;
			case ActivityState.SaveInstance:
				if (IsSameActivity (activity))
					savedData.PutLong (BundleIdentityKey, _activityIdentity);
				break;
			case ActivityState.Resumed:
			case ActivityState.Started:
				if (IsSameActivity (activity))
					SetAvailability (isAvailable: true);
				break;
			case ActivityState.Stopped:
			case ActivityState.Destroyed:
			case ActivityState.Paused:
				if (IsSameActivity (activity))
					SetAvailability (isAvailable: false);
				break;
			}
		}

		/// <summary>
		/// Ises the same activity.
		/// </summary>
		/// <returns><c>true</c>, if same activity was ised, <c>false</c> otherwise.</returns>
		/// <param name="other">Other.</param>
		bool IsSameActivity (Activity other)
		{
			return ReferenceEquals (Instance, other) || JNIEnv.IsSameObject (Instance.Handle, other.Handle);
		}


		/// <summary>
		/// Sets the availability.
		/// </summary>
		/// <param name="isAvailable">If set to <c>true</c> is available.</param>
		void SetAvailability (bool isAvailable)
		{
			if (IsUnavailable ^ isAvailable)
				return;
			IsUnavailable = !isAvailable;
			if (isAvailable)
				while (_continuations.TryDequeue (out var continuation))
					continuation ();
		}

		/// <summary>
		/// Activity scope listener.
		/// </summary>
		class ActivityScopeListener : Java.Lang.Object, Application.IActivityLifecycleCallbacks {
			public event Action<Activity, ActivityState, Bundle> ActivityStateChanged;
			public void OnActivityCreated (Activity activity, Bundle savedInstanceState) => ActivityStateChanged?.Invoke (activity, ActivityState.Created, savedInstanceState);
			public void OnActivitySaveInstanceState (Activity activity, Bundle outState) => ActivityStateChanged?.Invoke (activity, ActivityState.SaveInstance, outState);
			public void OnActivityResumed (Activity activity) => ActivityStateChanged?.Invoke (activity, ActivityState.Resumed, null);
			public void OnActivityStarted (Activity activity) => ActivityStateChanged?.Invoke (activity, ActivityState.Started, null);
			public void OnActivityPaused (Activity activity) => ActivityStateChanged?.Invoke (activity, ActivityState.Paused, null);
			public void OnActivityStopped (Activity activity) => ActivityStateChanged?.Invoke (activity, ActivityState.Stopped, null);
			public void OnActivityDestroyed (Activity activity) => ActivityStateChanged?.Invoke (activity, ActivityState.Destroyed, null);
		}
		#endregion
	}
}
