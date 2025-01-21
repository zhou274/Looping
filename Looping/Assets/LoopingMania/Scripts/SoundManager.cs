




using UnityEngine;
using System.Collections;

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Class in charge to play sound in the game.
	/// </summary>
	public class SoundManager : MonoBehaviour 
	{
		/// <summary>
		/// Reference to the AudioSource.
		/// </summary>
		AudioSource _audioSource;
		/// <summary>
		/// Reference to the AudioSource.
		/// </summary>
		AudioSource audioSource
		{
			get
			{
				if(_audioSource == null)
					_audioSource = FindObjectOfType<AudioSource>();

				return _audioSource;
			}
		}
		/// <summary>
		/// Reference to the AudioClip played when the player jump.
		/// </summary>
		[SerializeField] private AudioClip soundJump;
		/// <summary>
		/// Reference to the AudioClip played when the player hit the Circle, ie. when the player ended his jump. (cf Ball)
		/// </summary>
		[SerializeField] private AudioClip soundHit;
		/// <summary>
		/// Reference to the AudioClip played when the player get a point, ie. collect a DotToCollect.
		/// </summary>
		[SerializeField] private AudioClip soundPoint;
		/// <summary>
		/// Reference to the AudioClip played when the player hit the Circle, ie. when the player ended his jump. (cf Ball)
		/// </summary>
		[SerializeField] private AudioClip soundMetal;
		/// <summary>
		/// Reference to the AudioClip played when the game is Game Over.
		/// </summary>
		[SerializeField] private AudioClip soundGameOverVoice;
		/// <summary>
		/// Reference to the AudioClip played when the player start a new game.
		/// </summary>
		[SerializeField] private AudioClip soundStartVoice;
		/// <summary>
		/// Reference to the AudioClip when we spawned new DotToCollect elements.
		/// </summary>
		[SerializeField] private AudioClip soundReloadVoice;

		public void PlayJump()
		{
			audioSource.PlayOneShot (soundJump,1f);
		}

		public void PlayPoint()
		{
			audioSource.PlayOneShot (soundPoint, 0.5f);
		}

		public void PlayMetal()
		{
			audioSource.PlayOneShot (soundMetal,1f);
		}

		public void PlayHit()
		{
			audioSource.PlayOneShot (soundHit,1f);
		}

		public void PlayGameOverVoice()
		{
			audioSource.PlayOneShot (soundGameOverVoice,1f);
		}

		public void PlayStartVoice()
		{
			audioSource.PlayOneShot (soundStartVoice,1f);
		}

		public void PlayReloadVoice()
		{
			audioSource.PlayOneShot (soundReloadVoice,1f);
		}
	}
}