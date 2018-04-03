using System;
using System.Threading;

namespace ActiveActivity.ACTask {
	static class ActivityIdProvider {
		static long IdentityTagPool;

		public static long GetNextId () => Interlocked.Increment (ref IdentityTagPool);
	}
}
