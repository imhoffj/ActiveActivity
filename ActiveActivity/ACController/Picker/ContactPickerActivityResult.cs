using System;
using Android.App;
using Android.Content;

namespace ActiveActivity.ACController.Picker {

	/// <summary>
	/// Contact picker activity result.
	/// </summary>
	public class ContactPickerActivityResult : ActivityResult {
		public ContactPickerActivityResult (Result resultCode, int requestCode, Intent data)
			: base (resultCode, requestCode, data) { }

		public Android.Net.Uri SelectedContactUri { get { return Data?.Data; } }
	}
}
