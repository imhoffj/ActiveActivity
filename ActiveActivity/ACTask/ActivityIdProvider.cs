using System;
using System.Threading;

namespace ActiveActivity.ACTask {

	static class ActivityIdProvider {

		#region Fields
		private static long _IdentityTagPool;
		#endregion

		#region Properties
		public static long GetNextId () => Interlocked.Increment (ref _IdentityTagPool);
		#endregion
	}
}
