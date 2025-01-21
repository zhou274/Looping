




using UnityEngine;
using System.Collections;

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Attached to like button
	/// </summary>
	public class ButtonLike : MonoBehaviour 
	{
		public string facebookApp = "fb://profile/515431001924232" ;
		public string facebookAddress = "https://www.facebook.com/appadvisory";

		public void OnClickedFacebookLikeButton()
		{
			float startTime;
			startTime = Time.timeSinceLevelLoad;

			//open the facebook app
			Application.OpenURL(facebookApp);

			if (Time.timeSinceLevelLoad - startTime <= 1f)
			{
				//fail. Open safari.
				Application.OpenURL(facebookAddress);
			}
		}
	}
}