


using UnityEngine;
using System;
using System.Collections;
#if AADOTWEEN
using DG.Tweening;
#endif

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Attached to ObstaclePrefab prefab in the prefab folder. Represent the little dot the player (Who is a Ball) have to collect around the Circle.
	/// </summary>
	public class DotToCollect : MonoBehaviour 
	{
		public bool isItem = true;

		public Collider2D _collider;

		public Transform obstacleSprite;

		public SpriteRenderer sr;

		GameManager gameManager;

		public Vector3 defaultPosition = Vector3.zero;

		void Awake()
		{
			transform.position = Vector3.zero;
			this.gameManager = FindObjectOfType<GameManager>();
			obstacleSprite.gameObject.SetActive(true);
			_collider.enabled = false;
		}

		public Vector3 spritepos;

		public void Init(float rotation)
		{
			_collider.enabled = false;

			defaultPosition = new Vector3(gameManager.radiusBorder,0,2f);
			obstacleSprite.position = defaultPosition;

			sr.transform.localPosition = new Vector3(+ SpriteSize() - FindObjectOfType<Circle>().real_width, 0, 0);
			sr.transform.localEulerAngles = new Vector3(0,0,0); 

			transform.eulerAngles = new Vector3(0, 0, rotation);

			spritepos = sr.transform.position;

			var p = obstacleSprite.localPosition;

			obstacleSprite.localPosition = new Vector3(0, 0, p.z);

			#if AADOTWEEN
			obstacleSprite.transform.DOLocalMove(p, gameManager.timeAnimItemInDotToCollect)
				.SetEase(Ease.Linear)
				.OnComplete(() => {
					_collider.enabled = true;

					sr.transform.DOScale(Vector3.one * 0.9f, 0.3f).SetLoops(-1,LoopType.Yoyo);
				});
			#endif
		}

		void OnDotColorChanged(Color c)
		{
			sr.color = c;
		}
		float SpriteSize()
		{
			return  - sr.sprite.bounds.size.x * 0.5f * sr.transform.localScale.x;
		}

		public void DOCollect()
		{
			_collider.enabled = false;

			gameManager.Add1Point();


			float time = 0.5f * Vector2.Distance(obstacleSprite.position, new Vector2(Camera.main.orthographicSize,0));



			#if AADOTWEEN
			sr.transform.DOLocalMoveX(sr.transform.localPosition.x + 0.3f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);

			obstacleSprite.DOMoveX(Camera.main.orthographicSize, time);

			obstacleSprite.DOMoveY(0, time);


			sr.DOFade(0, time).OnComplete(() => {
				Destroy(gameObject);
			});
			#endif
		}
	}
}