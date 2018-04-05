using System;
using System.Threading.Tasks;
using ActiveActivity.ACController.Picker;
using Android.Content;
using Android.Provider;

namespace ActiveActivity.ACController {

	public partial class ActivityController {

		#region Fields
		private static string MIME_TYPE_IMAGE = "image/*";
		private static string MIME_TYPE_VIDEO = "video/*";
		#endregion


		#region Methods
		/// <summary>
		/// Picks the contact async.
		/// </summary>
		/// <returns>The contact async.</returns>
		public Task<ContactPickerActivityResult> PickContactAsync ()
		{
			var contactPickerIntent = new Intent (Intent.ActionPick, ContactsContract.CommonDataKinds.Phone.ContentUri);
			return StartActivityForResultAsync<ContactPickerActivityResult> (contactPickerIntent);
		}

		/// <summary>
		/// Picks the photo async.
		/// </summary>
		/// <returns>The photo async.</returns>
		/// <param name="title">Title.</param>
		public Task<MediaPickerActivityResult> PickPhotoAsync (string title)
		{
			var intent = Intent.CreateChooser (new Intent (Intent.ActionPick).SetType (MIME_TYPE_IMAGE), title);

			return StartActivityForResultAsync<MediaPickerActivityResult> (intent);
		}

		/// <summary>
		/// Takes the photo async.
		/// </summary>
		/// <returns>The photo async.</returns>
		/// <param name="title">Title.</param>
		public Task<MediaPickerActivityResult> TakePhotoAsync (string title)
		{
			var intent = Intent.CreateChooser (new Intent (MediaStore.ActionImageCapture).SetType (MIME_TYPE_IMAGE), title);

			return StartActivityForResultAsync<MediaPickerActivityResult> (intent);
		}

		/// <summary>
		/// Picks the video async.
		/// </summary>
		/// <returns>The video async.</returns>
		/// <param name="title">Title.</param>
		public Task<MediaPickerActivityResult> PickVideoAsync (string title)
		{
			var intent = Intent.CreateChooser (new Intent (Intent.ActionPick).SetType (MIME_TYPE_VIDEO), title);

			return StartActivityForResultAsync<MediaPickerActivityResult> (intent);
		}

		/// <summary>
		/// Takes the video async.
		/// </summary>
		/// <returns>The video async.</returns>
		/// <param name="title">Title.</param>
		/// <param name="videoQuality">Video quality.</param>
		/// <param name="videoSizeLimit">Video size limit.</param>
		/// <param name="videoDurationLimit">Video duration limit.</param>
		public Task<MediaPickerActivityResult> TakeVideoAsync (string title, int? videoQuality = null, int? videoSizeLimit = null, TimeSpan? videoDurationLimit = null)
		{
			var intent = Intent.CreateChooser (new Intent (MediaStore.ActionImageCapture).SetType (MIME_TYPE_VIDEO), title);

			if (videoQuality.HasValue)
				intent.PutExtra (MediaStore.ExtraVideoQuality, videoQuality.Value);
			if (videoSizeLimit.HasValue)
				intent.PutExtra (MediaStore.ExtraSizeLimit, videoSizeLimit.Value);
			if (videoDurationLimit.HasValue)
				intent.PutExtra (MediaStore.ExtraDurationLimit, (int)videoDurationLimit.Value.TotalSeconds);

			return StartActivityForResultAsync<MediaPickerActivityResult> (intent);
		}
		#endregion
	}
}
