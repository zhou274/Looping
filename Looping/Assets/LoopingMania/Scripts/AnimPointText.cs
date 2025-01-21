

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if AADOTWEEN
using DG.Tweening;
#endif

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Attached to the GameObject UIController/PointTextContainer/pointText. In change of the animation of the point UI element.
	/// </summary>
	public class AnimPointText : MonoBehaviour 
	{
		/// <summary>
		/// Some initializations.
		/// </summary>
		void Awake()
		{
			GetComponent<Text>().SetAlpha(0);
		}
		/// <summary>
		/// Some initializations.
		/// </summary>
		void Start()
		{
			GetComponent<Text>().SetAlpha(0);

			#if AADOTWEEN
			GetComponent<RectTransform>().DOLocalMoveY(Screen.height * 2f, 0.1f).OnComplete(() => {
			});
			#endif
		}
		/// <summary>
		/// Anim the point UI Text from out of the screen to in the screen.
		/// </summary>
		public void DoAnimPointIn()
		{
			GetComponent<Text>().SetAlpha(1);
			#if AADOTWEEN
			GetComponent<RectTransform>().DOLocalMoveY(0, 0.5f);
			#endif
		}
		/// <summary>
		/// Anim the point UI Text from in the screen to out of the screen.
		/// </summary>
		public void DoAnimPointOut()
		{
			#if AADOTWEEN
			GetComponent<RectTransform>().DOLocalMoveY(Screen.height * 2f, 0.3f).OnComplete(() => {
			});
			#endif
		}
	}
}