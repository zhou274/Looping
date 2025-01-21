using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if AADOTWEEN
using DG.Tweening;
#endif

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Class attached to the Player GameObject in the hierarchy, or to the hazards. In charge to handle the Player controls and detect touch and collision, or to move the hazards around the circle.
	/// </summary>
	public class Ball : MonoBehaviour
	{
		/// <summary>
		/// True if it's an hazard (ie. spawned and can be destroy by the player, and can destroy the player).
		/// False if it's the player.
		/// </summary>
		public bool is_hazard = false;
		/// <summary>
		/// True if the ball is jumping.
		/// </summary>
		public bool is_jumping = false;
		/// <summary>
		/// Reference to the parent of the SpriteRenderer.
		/// </summary>
		public Transform playerSprite;
		/// <summary>
		/// Reference to the SpriteRenderer.
		/// </summary>
		public SpriteRenderer sr;
		/// <summary>
		/// Reference to the GameManager.
		/// </summary>
		GameManager gameManager;
		/// <summary>
		/// Reference to the SoundManager.
		/// </summary>
		SoundManager soundManager;
		/// <summary>
		/// Some initializations.
		/// </summary>
		void Awake()
		{
			gameManager = FindObjectOfType<GameManager>();
			soundManager = FindObjectOfType<SoundManager>();

			sr.color = Color.white;
		}
		/// <summary>
		/// Return the current rotation of the Ball, on the Z axis.
		/// </summary>
		public float GetRotation()
		{
			return transform.eulerAngles.z;
		}
		/// <summary>
		/// Apply a position to the Ball.
		/// </summary>
		public void DOPosition(float x, float y)
		{
			transform.position = Vector3.zero;

			if(is_hazard)
			{
				name = "HAZARD";

				is_jumping = true;

				transform.eulerAngles = new Vector3(0, 0, Util.GetRandomNumber(0, 360));

				sr.transform.localScale = Vector2.one * Util.GetRandomNumber(1.2f, 2.5f);

				var posFinal = new Vector3(x, y, 2f);


				var posStart = 5 * posFinal;

				playerSprite.localPosition = posStart;



				sr.color = gameManager.currentMainColor;

				#if AADOTWEEN
				playerSprite.DOLocalMove(posFinal, 2)
					.SetEase(Ease.OutQuad)
					.OnComplete(() => {
						is_jumping = false;
						DOStart();
					});
				#endif

				GameManager.OnMainColorChanged += OnMainColorChanged;


			}
			else
			{
				name = "PLAYER";

				var posFinal = new Vector3(x + SpriteSize() - FindObjectOfType<Circle>().real_width, y + SpriteSize() - FindObjectOfType<Circle>().real_width, 2f);

				playerSprite.position = posFinal;
			}
		}
		/// <summary>
		/// When the Ball is disable, unsusbcribe to the events subscribe before.
		/// </summary>
		void OnDisable()
		{
			GameManager.OnMainColorChanged -= OnMainColorChanged;
			GameManager.OnPlayerJumpFinished -= OnPlayerJumpFinished;
		}
		/// <summary>
		/// When the Ball is destroyed, unsusbcribe to the events subscribe before.
		/// </summary>
		void OnDestroy()
		{
			GameManager.OnMainColorChanged -= OnMainColorChanged;
			GameManager.OnPlayerJumpFinished -= OnPlayerJumpFinished;
		}
		/// <summary>
		/// Method called when the Ball player finished a Jump.
		/// </summary>
		void OnPlayerJumpFinished()
		{
			if(is_jumping)
				return;

			is_jumping = true;
			#if AADOTWEEN
			sr.transform.DOLocalMoveX(sr.transform.localPosition.x + 0.5f, 0.2f).SetLoops(2,LoopType.Yoyo)
				.SetDelay(0.02f)
				.OnUpdate(() => {
					is_jumping = true;
				})
				.OnComplete(() => {
					is_jumping = false;
				});
			#endif
		}
		/// <summary>
		/// Method called when the main color is changed in the game. Please Refer to GameManager OnMainColorChanged() method.
		/// </summary>
		void OnMainColorChanged(Color c)
		{
			sr.color = c;
		}
		/// <summary>
		/// Return the sprite size in float (because it's a circle, so height = width).
		/// </summary>
		float SpriteSize()
		{
			return  - sr.sprite.bounds.size.x * 0.5f * sr.transform.localScale.x;
		}
		/// <summary>
		/// Start the animation of the Ball. The animation will not be the same if the Ball if the Player or an Hazard.
		/// </summary>
		public void DOStart()
		{
			float ratio = 1;

			if(is_hazard)
			{
				//			ratio = Util.GetRandomNumber(1.2f, 2.0f);
				//			ratio = sr.transform.localScale.x ;
				ratio = sr.transform.localScale.x * 0.5f;
				GameManager.OnPlayerJumpFinished += OnPlayerJumpFinished;
			}

			#if AADOTWEEN
			transform.DORotate(new Vector3(0,0,	transform.eulerAngles.z + 360f), gameManager.timeForCompleteCirclePlayer * ratio, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1,LoopType.Incremental);
			#endif
		}

		/// <summary>
		/// Activate the touch detection if the Ball is a Player. Never call if the Ball is an Hazard.
		/// </summary>
		public void ActivateTouchControl()
		{
			InputTouch.OnTouchedDown -= OnTouchDown;

			if(is_hazard)
				return;

			InputTouch.OnTouchedDown += OnTouchDown;
		}
		/// <summary>
		/// Desactivate the touch detection (= unsusbscribe to touch event) if the Ball is a Player. Never call if the Ball is an Hazard.
		/// </summary>
		public void DesactivateTouchControl()
		{
			if(is_hazard)
				return;

			InputTouch.OnTouchedDown -= OnTouchDown;
		}
		/// <summary>
		/// Method called when the player touch the screen. 
		/// </summary>
		public void OnTouchDown (TouchDirection td)
		{
			if(is_hazard || is_jumping)
				return;

			is_jumping = true;

			#if AADOTWEEN
			playerSprite.transform.DOMove(new Vector3(-playerSprite.transform.position.x, -playerSprite.transform.position.y, playerSprite.transform.position.z), gameManager.playerJumpSpeedInSeconds)
				.OnStart(() => {
					is_jumping = true;
				})
				.OnUpdate(() => {
					is_jumping = true;
				})
				.OnComplete(() => {

					is_jumping = true;

					DOVirtual.DelayedCall(0.05f, () => {
						is_jumping = false;
					});

					soundManager.PlayMetal();

					GameManager.DOPlayerJumpFinished();
				});
			#endif
		}
		/// <summary>
		/// Method called when a trigger enter event happened in the collider (child of the Ball)
		/// If the Ball is an Hazard => Nothign happens!
		/// If the Ball is the player and triggered with an hazard:
		/// - Game Over is the player is not jumping and touch an Hazard (who is a Ball too).
		/// - Destroy an Hazard (who is a Ball too) if the player is jumping
		/// If the Ball is the player and triggered with a DotToCollect:
		/// - Collect the DotToCollect GameObject
		/// </summary>
		public void DOOnTriggerEnter2D(Collider2D other)
		{
			if(is_hazard)
				return;

			if(other.GetComponent<CollisionDetection>() != null)
			{		
				if(is_jumping)
				{
					gameManager.DOParticle(other.transform.position);
					soundManager.PlayHit();
					Destroy(other.GetComponent<CollisionDetection>().myPlayer.gameObject);
				}
				else
				{
					Destroy(other.gameObject);
                    FindObjectOfType<GameManager>().ShowGameOverPanel();

                    //FindObjectOfType<GameManager>().GameOver(gameManager.player.sr.transform);
				}
			}
			else
			{
				DotToCollect ob = other.GetComponentInParent<DotToCollect>();

				if(ob != null)
				{
					if(ob.isItem)
					{
						ob.DOCollect();

					}
					else
					{
                        Destroy(other.gameObject);
                        FindObjectOfType<GameManager>().ShowGameOverPanel();
                        
                        //FindObjectOfType<GameManager>().GameOver(ob.sr.transform);
                    }
				}
				else
				{
					Debug.LogWarning("DOOnTriggerEnter2D with something else than an obstacle : " + other.name);
				}
			}
		}
	}
}