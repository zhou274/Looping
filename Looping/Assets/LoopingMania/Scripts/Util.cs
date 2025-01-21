

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
#if AADOTWEEN
using DG.Tweening;
#endif
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Utility class.
	/// </summary>
	public static class Util
	{
		static System.Random random = new System.Random();

		public static double GetRandomNumber(double minimum, double maximum)
		{ 
			return random.NextDouble() * (maximum - minimum) + minimum;
		}

		public static float GetRandomNumber(float minimum, float maximum)
		{ 
			return (float)random.NextDouble() * (maximum - minimum) + minimum;
		}

		/// <summary>
		/// Real shuffle of List
		/// </summary>
		public static void Shuffle<T>(this IList<T> list)  
		{  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = random.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
		/// <summary>
		/// Set the last score, and eventually save a new best score in rhe PlayerPrefs.
		/// </summary>
		public static void SetLastScore(int score)
		{
			PlayerPrefs.SetInt("_LASTSCORE",score);

			int b = GetBestScore();

			if(score > b)
			{
				Debug.Log("new best : " + score);
				PlayerPrefs.SetInt("_BESTSCORE",score);
			}
			else
			{
				Debug.Log("NOT new best : " + score + " best is = " + GetBestScore());
			}

			PlayerPrefs.Save();
		}
		/// <summary>
		/// Get best score from PlayerPrefs.
		/// </summary>
		public static int GetBestScore()
		{
			return PlayerPrefs.GetInt("_BESTSCORE",0);
		}
		/// <summary>
		/// Get best score from PlayerPrefs.
		/// </summary>
		public static int GetLastScore()
		{
			return PlayerPrefs.GetInt("_LASTSCORE",0);
		}

		/// <summary>
		/// Clean the memory and reload the scene
		/// </summary>
		public static void ReloadLevel()
		{
			CleanMemory();

			#if UNITY_5_3_OR_NEWER
			SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
			#else
			Application.LoadLevel(Application.loadedLevel);
			#endif

			CleanMemory();
		}
		/// <summary>
		/// Clean the memory
		/// </summary>
		public static void CleanMemory()
		{
			#if AADOTWEEN
			DOTween.KillAll();
			#endif
			GC.Collect();
			Application.targetFrameRate = 60;
		}
		/// <summary>
		/// Set alpha of UI Text element.
		/// </summary>
		public static void SetAlpha(this Text text, float alpha)
		{
			Color c = text.color;
			c.a = alpha;
			text.color = c;
		}
		/// <summary>
		/// Set scale X of RectTransform element.
		/// </summary>
		public static void SetScaleX(this RectTransform rect, float scale)
		{
			var s = rect.localScale;
			s.x = scale;
			rect.localScale = s;
		}
	}
}