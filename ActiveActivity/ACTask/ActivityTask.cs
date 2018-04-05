using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ActiveActivity.ACTask {

	[AsyncMethodBuilder (typeof (ActivityScopeMethodBuilder))]
	public class ActivityTask {

		#region Fields
		private TaskCompletionSource<VoidTaskResult> _completion;
		#endregion

		#region Properties
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static ActivityScopeMethodBuilder CreateAsyncMethodBuilder () => ActivityScopeMethodBuilder.Create ();
		internal Task CompletionTask => Completion.Task;
		internal TaskCompletionSource<VoidTaskResult> Completion => _completion ?? (_completion = new TaskCompletionSource<VoidTaskResult> ());
		#endregion

		#region Methods
		/// <summary>
		/// Gets the awaiter.
		/// </summary>
		/// <returns>The awaiter.</returns>
		public TaskAwaiter GetAwaiter ()
		{
			return CompletionTask.GetAwaiter ();
		}

		/// <summary>
		/// Configures the await.
		/// </summary>
		/// <returns>The await.</returns>
		/// <param name="continueOnCapturedContext">If set to <c>true</c> continue on captured context.</param>
		public ConfiguredTaskAwaitable ConfigureAwait (bool continueOnCapturedContext)
		{
			return CompletionTask.ConfigureAwait (continueOnCapturedContext);
		}
		#endregion
	}

	struct VoidTaskResult { };
}
