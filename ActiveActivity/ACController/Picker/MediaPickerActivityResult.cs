using System;
using System.IO;
using Android.App;
using Android.Content;


namespace ActiveActivity.ACController.Picker {
	public class MediaPickerActivityResult : ActivityResult {

		#region Properties
		public Android.Net.Uri SelectedMediaUri { get { return Data?.Data; } }
		#endregion


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ActiveActivity.ACController.Picker.MediaPickerActivityResult"/> class.
		/// </summary>
		/// <param name="resultCode">Result code.</param>
		/// <param name="requestCode">Request code.</param>
		/// <param name="data">Data.</param>
		public MediaPickerActivityResult (Result resultCode, int requestCode, Intent data) : base (resultCode, requestCode, data) { }
		#endregion

		#region Methods
		/// <summary>
		/// Gets the media stream.
		/// </summary>
		/// <returns>The media stream.</returns>
		/// <param name="context">Context.</param>
		public Stream GetMediaStream (Context context)
		{
			var uri = SelectedMediaUri;
			if (uri == null)
				return null;

			return context.ContentResolver.OpenInputStream (uri);
		}
		#endregion
	}
}
