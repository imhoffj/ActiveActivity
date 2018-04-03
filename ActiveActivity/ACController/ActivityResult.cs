using System;
using Android.App;
using Android.Content;

namespace ActiveActivity.ACController {

	/// <summary>
	/// Activity result.
	/// </summary>
	public class ActivityResult : IActivityResult {

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ActiveActivity.ActivityController.ActivityResult"/> class.
		/// </summary>
		/// <param name="resultCode">Result code.</param>
		/// <param name="requestCode">Request code.</param>
		/// <param name="data">Data.</param>
		public ActivityResult (Result resultCode, int requestCode, Intent data)
		{
			ResultCode = resultCode;
			RequestCode = requestCode;
			Data = data;
		}

		public Result ResultCode { get; private set; }
		public int RequestCode { get; private set; }
		public Intent Data { get; private set; }

		Result IActivityResult.ResultCode => throw new NotImplementedException ();
	}
}
