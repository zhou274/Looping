

using UnityEngine;
using System.Collections;

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Class attached to the sprite child of the Player GameOBject, in charge to listen if the player collide with an obstacle
	/// </summary>
	public class CollisionDetection : MonoBehaviour
	{
		public Ball myPlayer;

		/// <summary>
		/// Listen the collision. If collision: all the Player method DOOnTriggerEnter2D
		/// </summary>
		public void OnTriggerEnter2D(Collider2D other)
		{
			myPlayer.DOOnTriggerEnter2D(other);
		}
	}
}