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

	/// <summary>
	/// Typed activity result.
	/// </summary>
	public interface ITypedActivityResult : IActivityResult {
		void Parse (Result resultCode, int requestCode, Intent data);
	}
}
