using UnityEngine;
using System.Collections;

namespace AppAdvisory.UI
{
	/// <summary>
	/// Attached to rate button
	/// </summary>
	public class ButtonRate : MonoBehaviour 
	{
		public bool isAmazon = false;

		/// <summary>
		/// URL of the iOS game. Find it on iTunes Connect.
		/// </summary>
		public string iosRateURL = "fb://profile/515431001924232";
		/// <summary>
		/// URL of the Android game. Find it on Google Play.
		/// </summary>
		public string androidRateURL = "https://www.facebook.com/appadvisory";
		/// <summary>
		/// URL of the Amazon game. Find it on the Amazon Developer Console.
		/// </summary>
		public string amazonRateURL = "https://www.facebook.com/appadvisory";

		/// <summary>
		/// If player clicks on the rate button, we call this method.
		/// </summary>
		public void OnClickedRate()
		{
			string URL = "";

			#if UNITY_IOS
			URL = iosRateURL;
			#else
			URL = androidRateURL;
			if(isAmazon)
				URL = amazonRateURL;
			#endif

			Application.OpenURL(URL);
		}
	}
}