using UnityEngine;
using System.Collections;

namespace AppAdvisory.UI
{
	/// <summary>
	/// Class attached to button to open an url.
	/// </summary>
	public class ButtonOpenUrl : MonoBehaviour 
	{
		public string URL = "http://app-advisory.com";

		void Start()
		{
			GetComponent<UnityEngine.UI.Button>().onClick.RemoveListener(OnClickedOpenURL);
			GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClickedOpenURL);
		}

		public void OnClickedOpenURL()
		{
			Application.OpenURL(URL);
		}
	}
}

