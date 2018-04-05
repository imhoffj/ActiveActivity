using System;
using Android.App;
using Android.Content;

namespace ActiveActivity.ACController {

	public class ActivityResult : IActivityResult {

		#region Properties
		public Result ResultCode { get; private set; }
		public int RequestCode { get; private set; }
		public Intent Data { get; private set; }

		Result IActivityResult.ResultCode => throw new NotImplementedException ();
		#endregion

		#region Methods
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
		#endregion

	}
}
