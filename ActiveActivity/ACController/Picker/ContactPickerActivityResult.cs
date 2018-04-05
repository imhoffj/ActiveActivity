using System;
using Android.App;
using Android.Content;

namespace ActiveActivity.ACController.Picker {

	/// <summary>
	/// Contact picker activity result.
	/// </summary>
	public class ContactPickerActivityResult : ActivityResult {

		#region Properties
		public Android.Net.Uri SelectedContactUri { get { return Data?.Data; } }
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ActiveActivity.ACController.Picker.ContactPickerActivityResult"/> class.
		/// </summary>
		/// <param name="resultCode">Result code.</param>
		/// <param name="requestCode">Request code.</param>
		/// <param name="data">Data.</param>
		public ContactPickerActivityResult (Result resultCode, int requestCode, Intent data) : base (resultCode, requestCode, data) { }
		#endregion
	}
}
