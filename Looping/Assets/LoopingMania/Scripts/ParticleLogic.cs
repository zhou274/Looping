




using UnityEngine;
using System.Collections;

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Class in charge to desactivate the particles.
	/// </summary>
	public class ParticleLogic : MonoBehaviour 
	{
		void OnEnable()
		{
			StartCoroutine("waitForDesactivate");
		}

		void OnDisable()
		{
			StopCoroutine("waitForDesactivate");
		}

		IEnumerator waitForDesactivate()
		{
			yield return new WaitForSeconds(1);
			gameObject.SetActive(false);
		}
	}
}