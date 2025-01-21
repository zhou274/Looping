

using UnityEngine;
using System;
using System.Collections;
#if AADOTWEEN
using DG.Tweening;
#endif

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Class in charge to draw the circle.
	/// </summary>
	public class Circle : MonoBehaviour
	{
		/// <summary>
		/// Reference to the GameManager.
		/// </summary>
		GameManager gameManager;
		/// <summary>
		/// Number of segment in the LineRenderer to draw the circle.
		/// </summary>
		int segments = 100;
		/// <summary>
		/// Reference to the LineRenderer.
		/// </summary>
		public LineRenderer line;
		/// <summary>
		/// Radius of the Circle.
		/// </summary>
		private float radius;
		/// <summary>
		/// Width of the border of the Circle.
		/// </summary>
		[NonSerialized] public float real_width = 0.1f;
		/// <summary>
		/// Some initialization.
		/// </summary>
		void Awake()
		{
			this.gameManager = FindObjectOfType<GameManager>();
		}
		/// <summary>
		/// Draw the Circle and susbcribe to the GameManager event: OnMainColorChanged.
		/// </summary>
		public void DOStart (float radius)
		{
			this.radius = radius;

			line = gameObject.GetComponent<LineRenderer>();

			line.SetVertexCount (segments + 1);
			line.useWorldSpace = false;

			float angle = 20f;
			float z = 0f;
			float x;
			float y;

			for (int i = 0; i < (segments + 1); i++)
			{
				x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
				y = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;

				line.SetPosition (i,new Vector3(x,y,z) );

				angle += (360f / segments);
			}

			line.SetColors(gameManager.currentMainColor, gameManager.currentMainColor);
			line.SetWidth(real_width * 2,real_width * 2f);

			GameManager.OnMainColorChanged += OnMainColorChanged;
		}
		/// <summary>
		/// Unsusbcribe to the GameManager event: OnMainColorChanged when the GameObject is disabled.
		/// </summary>
		void OnDisable()
		{
			GameManager.OnMainColorChanged -= OnMainColorChanged;
		}
		/// <summary>
		/// Unsusbcribe to the GameManager event: OnMainColorChanged when the GameObject is destroyed.
		/// </summary>
		void OnDestroy()
		{
			GameManager.OnMainColorChanged -= OnMainColorChanged;
		}
		/// <summary>
		/// Called when the GameManager event: OnMainColorChanged is called.
		/// </summary>
		void OnMainColorChanged(Color c)
		{
			if(line != null)
				line.SetColors(c,c);
		}
	}
}