using System;
using Android.App;
using Android.Content;

namespace ActiveActivity.ACController {
	/// <summary>
	/// Activity result.
	/// </summary>
	public interface IActivityResult {
		Result ResultCode { get; }
		int RequestCode { get; }
		Intent Data { get; }
	}

	public interface ITypedActivityResult : IActivityResult {
		void Parse (Result resultCode, int requestCode, Intent data);
	}
}
