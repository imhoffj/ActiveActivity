using System;
using System.IO;
using Android.App;
using Android.Content;


namespace ActiveActivity.ACController.Picker {
	/// <summary>
	/// Media picker activity result.
	/// </summary>
	public class MediaPickerActivityResult : ActivityResult {
		public MediaPickerActivityResult (Result resultCode, int requestCode, Intent data)
		    : base (resultCode, requestCode, data) { }

		public Android.Net.Uri SelectedMediaUri { get { return Data?.Data; } }

		public Stream GetMediaStream (Context context)
		{
			var uri = SelectedMediaUri;
			if (uri == null)
				return null;

			return context.ContentResolver.OpenInputStream (uri);
		}
	}
}
