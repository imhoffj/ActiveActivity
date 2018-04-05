using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace ActiveActivity.ACTask {

	public class ActivityScopeMethodBuilder {

		#region Properties
		public ActivityTask Task => task;
		public IAsyncStateMachine stateMachine { get; private set; }
		public SynchronizationContext syncContext { get; private set; }
		public ActivityScope scope { get; private set; }
		public ActivityTask task { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ActiveActivity.ActivityTask.ActivityScopeMethodBuilder"/> class.
		/// </summary>
		/// <param name="synchronizationContext">Synchronization context.</param>
		ActivityScopeMethodBuilder (SynchronizationContext synchronizationContext)
		{
			this.syncContext = synchronizationContext;
			this.task = new ActivityTask ();
			if (syncContext != null)
				syncContext.OperationStarted ();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Create this instance.
		/// </summary>
		/// <returns>The create.</returns>
		public static ActivityScopeMethodBuilder Create () => new ActivityScopeMethodBuilder (SynchronizationContext.Current);

		/// <summary>
		/// Start the specified stateMachine.
		/// </summary>
		/// <returns>The start.</returns>
		/// <param name="stateMachine">State machine.</param>
		/// <typeparam name="TStateMachine">The 1st type parameter.</typeparam>
		public void Start<TStateMachine> (ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			scope = ActivityScopeRetriever<TStateMachine>.GetScopeFromStateMachine (ref stateMachine);
			if (scope == null)
				throw new InvalidOperationException ("An async method returning AsyncTask needs to have a valid parameter of type ActivityScope");
			stateMachine.MoveNext ();
		}

		/// <summary>
		/// Sets the result.
		/// </summary>
		public void SetResult ()
		{
			if (syncContext != null)
				syncContext.OperationCompleted ();
			scope = null;
			task.Completion.SetResult (default (VoidTaskResult));
		}

		/// <summary>
		/// Sets the exception.
		/// </summary>
		/// <param name="ex">Ex.</param>
		public void SetException (Exception ex)
		{
			task.Completion.SetException (ex);
			if (syncContext != null)
				syncContext.OperationCompleted ();
		}

		/// <summary>
		/// Required for compiler
		/// </summary>
		/// <param name="stateMachine">State machine.</param>
		public void SetStateMachine (IAsyncStateMachine stateMachine)
		{
			this.stateMachine = stateMachine;
		}

		/// <summary>
		/// Awaits the on completed.
		/// </summary>
		/// <param name="awaiter">Awaiter.</param>
		/// <param name="stateMachine">State machine.</param>
		/// <typeparam name="TAwaiter">The 1st type parameter.</typeparam>
		/// <typeparam name="TStateMachine">The 2nd type parameter.</typeparam>
		public void AwaitOnCompleted<TAwaiter, TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine)
		   where TAwaiter : INotifyCompletion
		   where TStateMachine : IAsyncStateMachine
		{
			var callback = GetCompletionAction<TStateMachine> (ref stateMachine);
			awaiter.OnCompleted (callback);
		}

		/// <summary>
		/// Awaits the unsafe on completed.
		/// </summary>
		/// <param name="awaiter">Awaiter.</param>
		/// <param name="stateMachine">State machine.</param>
		/// <typeparam name="TAwaiter">The 1st type parameter.</typeparam>
		/// <typeparam name="TStateMachine">The 2nd type parameter.</typeparam>
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine)
		   where TAwaiter : ICriticalNotifyCompletion
		   where TStateMachine : IAsyncStateMachine
		{
			AwaitOnCompleted (ref awaiter, ref stateMachine);
		}

		/// <summary>
		/// Gets the completion action.
		/// </summary>
		/// <returns>The completion action.</returns>
		/// <param name="machine">Machine.</param>
		/// <typeparam name="TStateMachine">The 1st type parameter.</typeparam>
		Action GetCompletionAction<TStateMachine> (ref TStateMachine machine) where TStateMachine : IAsyncStateMachine
		{
			// If this is our first await, such that we've not yet boxed the state machine, do so now.
			if (stateMachine == null) {
				stateMachine = (IAsyncStateMachine)machine;
				stateMachine.SetStateMachine (stateMachine);
			}
			var runner = new Runner (stateMachine, scope);
			return new Action (runner.Run);
		}

		/// <summary>
		/// Runner.
		/// </summary>
		sealed class Runner {
			IAsyncStateMachine machine;
			ActivityScope scope;

			internal Runner (IAsyncStateMachine machine, ActivityScope scope)
			{
				this.machine = machine;
				this.scope = scope;
			}

			public void Run ()
			{
				if (!scope.IsUnavailable)
					machine.MoveNext ();
				else
					scope.OnCompleted (Run);
			}
		}
		#endregion
	}
}
