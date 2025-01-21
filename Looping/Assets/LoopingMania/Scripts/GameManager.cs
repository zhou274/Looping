

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using StarkSDKSpace;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
#if AADOTWEEN
using DG.Tweening;
#endif
using AppAdvisory.UI;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
using DG.Tweening.Core.Easing;
using UnityEngine.Analytics;
#endif

#if APPADVISORY_LEADERBOARD
using AppAdvisory.social;
#endif

#if APPADVISORY_ADS
using AppAdvisory.Ads;
#endif

namespace AppAdvisory.LoopMania
{
	/// <summary>
	/// Class in charge of the logic of the game. This class will restart the level at game over, handle and save the point, and call the Ads if you import the VERY SIMPLE ADS asset available here: http://u3d.as/oWD
	/// </summary>
	public class GameManager : MonoBehaviour
	{
		#region Colors
		/// <summary>
		/// Accessible in the editor, in the GameObject _Managers in the Hierarchy view. It's the list of colors use for the Ball Hazards and the Circle.
		/// </summary>
		public List<Color> listColors;
		/// <summary>
		/// Accessible in the editor, in the GameObject _Managers in the Hierarchy view. It's the list of colors use for the Ball Player and the DotToCollect.
		/// </summary>
		public List<Color> listDotColors;
		/// <summary>
		/// Reference to the dots Material, attached to each DotToCollect sprites. Usefull to change the color of each DotToCollect in one time.
		/// </summary>
		public Material dotMaterial;
		/// <summary>
		/// The current main color = The color of the Circle + color of the DotToCollect + Particle explosion color.
		/// </summary>
		[SerializeField] private Color m_currentMainColor;
        /// <summary>
        /// The current main color = The color of the Circle + color of the DotToCollect + Particle explosion color.
        /// </summary>
        public string clickid;
        private StarkAdManager starkAdManager;
        public Color currentMainColor
		{
			get
			{
				return m_currentMainColor;
			}

			set
			{
				m_currentMainColor = value;

				if(OnMainColorChanged != null)
					OnMainColorChanged(value);
			}
		}
		/// <summary>
		/// Change the current main color main color = The color of the Circle + color of the DotToCollect + Particle explosion color.
		/// </summary>
		public void DOMainColorChange()
		{
			listColors.Shuffle();
			StartCoroutine(DoMainColorLerp(currentMainColor, listColors[0], 1f));
		}
		/// <summary>
		/// Change the current main color main color = The color of the Circle + color of the DotToCollect + Particle explosion color.
		/// </summary>
		public IEnumerator DoMainColorLerp(Color from, Color to, float time)
		{
			float timer = 0;
			while (timer <= time)
			{
				timer += Time.deltaTime;
				currentMainColor = Color.Lerp(from, to, timer / time);
				yield return null;
			}

			currentMainColor = to;
		}
		/// <summary>
		/// Change the current main color main color = The color of the Circle + color of the DotToCollect + Particle explosion color.
		/// </summary>
		public void StartDotColorChange()
		{
			dotMaterial.color = listDotColors[0];

			#if AADOTWEEN
			dotMaterial.DOColor(listDotColors[1], 1f).SetLoops(-1, LoopType.Yoyo);
			#endif
		}
		#endregion

		#region Events
		/// <summary>
		/// Event triggered when the Ball Player finishes a jump = touch the Circle.
		/// </summary>
		public delegate void OnPlayerJumpFinish();
		/// <summary>
		/// Event triggered when the Ball Player finishes a jump = touch the Circle.
		/// </summary>
		public static event OnPlayerJumpFinish OnPlayerJumpFinished;
		/// <summary>
		/// Event triggered when we change the current main color main color = The color of the Circle + color of the DotToCollect + Particle explosion color.
		/// </summary>
		public delegate void OnMainColorChange(Color c);
		/// <summary>
		/// Event triggered when we change the current main color main color = The color of the Circle + color of the DotToCollect + Particle explosion color.
		/// </summary>
		public static event OnMainColorChange OnMainColorChanged;
		/// <summary>
		/// Call this method to called all the methods who subcribe to OnPlayerJumpFinish.
		/// </summary>
		public static void DOPlayerJumpFinished()
		{
			if(OnPlayerJumpFinished != null)
				OnPlayerJumpFinished();
		}
		#endregion

		#region Particle
		/// <summary>
		/// Reference to the particle prefab.
		/// </summary>
		public GameObject particleExplosion;
		/// <summary>
		/// Spawn the particle explosion at a certain position.
		/// </summary>
		public void DOParticle(Vector3 position)
		{
			GameObject go = Instantiate(particleExplosion) as GameObject;

			go.transform.position = position;

			go.GetComponent<ParticleSystem>().startColor = currentMainColor;
		}
		#endregion

		#region Settings
		/// <summary>
		/// If you want to monetize this game, get Very Simple Ads here: http://u3d.as/oWD.
		/// </summary>
		public string VerySimpleAdsURL = "http://u3d.as/oWD";
		/// <summary>
		/// We will show an interstitial (need Very Simple Ads) each 5 Game Over. If you want to monetize this game, get Very Simple Ads here: http://u3d.as/oWD.
		/// </summary>
		public int numberOfPlayToShowInterstitial = 5;
		/// <summary>
		/// Accessible in the editor, the number of DotToCollect we will create each turn.
		/// </summary>
		public int numberOfDotsToCreate = 15;
		/// <summary>
		/// Accessible in the editor, the time in second to anim the DotToCollect at creation
		/// </summary>
		public float timeAnimItemInDotToCollect = 0.3f;
		/// <summary>
		/// Accessible in the editor, the time for the Ball Player to do a complete turn.
		/// </summary>
		public float timeForCompleteCirclePlayer = 2.5f;
		/// <summary>
		/// The orthographic camera size.
		/// </summary>
		float cameraSize = 5;
		/// <summary>
		/// Accessible in the editor, the time for the Ball Player to do a complete jump.
		/// </summary>
		public float playerJumpSpeedInSeconds = 0.12f;
		#endregion

		#region Variables
		/// <summary>
		/// The radius of the circle.
		/// </summary>
		[NonSerialized] public float radiusBorder;
		/// <summary>
		/// True if the game state is Game Over.
		/// </summary>
		bool isGameOver = false;
		#endregion

		#region Refereces
		/// <summary>
		/// Reference to the UI Text who represents the point during the game.
		/// </summary>
		public Text pointText;
		/// <summary>
		/// Reference to the prefab DotToCollect.
		/// </summary>
		public GameObject dotToCollectPrefab;
		/// <summary>
		/// Reference to the prefab Ball.
		/// </summary>
		public GameObject ballPrefab;
		/// <summary>
		/// Current point in the game.
		/// </summary>
		private int point = 0;
		/// <summary>
		/// Reference to the Ball Player.
		/// </summary>
		public Ball player;
		/// <summary>
		/// Reference to sound manager.
		/// </summary>
		SoundManager soundManager;
		/// <summary>
		/// Reference to Circle.
		/// </summary>
		Circle circle = null;
		#endregion
		/// <summary>
		/// Some initializations.
		/// </summary>
		public GameObject GameOverPanelUI;
		void Awake()
		{
			soundManager = FindObjectOfType<SoundManager>();

			#if AADOTWEEN
			if(Time.realtimeSinceStartup < 1)
				DOTween.Init();

			DOTween.KillAll();
			#endif

			GC.Collect();
			Resources.UnloadUnusedAssets();
			Application.targetFrameRate = 60;

			//To be sure we have some color defined.
			if(listColors == null || listColors.Count <= 1)
			{
				listColors = new List<Color>();
				listColors.Add(Color.blue);
				listColors.Add(Color.cyan);
				listColors.Add(Color.green);
			}
			//To be sure we have some color defined.
			if(listDotColors == null || listDotColors.Count <= 1)
			{
				listDotColors = new List<Color>();
				listDotColors.Add(Color.red);
				listDotColors.Add(Color.yellow);
			}
		}
		/// <summary>
		/// Some initializations. We will anim from far to close the MainCamera.
		/// </summary>
		void Start()
		{
			listColors.Shuffle();

			currentMainColor = listColors[0];

			Camera.main.orthographicSize = 0.01f;
			FindObjectOfType<AppAdvisory.UI.UIController>().DOAnimIN();
			#if AADOTWEEN
			Camera.main.DOOrthoSize(cameraSize,1).SetEase(Ease.InBack).OnComplete(() => {
				GC.Collect();
				Application.targetFrameRate = 60;
			});
			#endif

			FindObjectOfType<UIController>().SetBestText(Util.GetBestScore());
			FindObjectOfType<UIController>().SetLastText(Util.GetLastScore());
		}
		/// <summary>
		/// Initialize the game.
		/// Set the point = 0.
		/// Initialize the radiusBorder.
		/// Set the Ball Player position.
		/// Start the different color change.
		/// Play start FX.
		/// Anim the MainCamera.
		/// At the end of the camera animation, enable the touch controll for the Ball Player (to jump) and start the Ball Player rotation.
		/// Spaxn the first DotToCollect.
		/// </summary>
		public void DOStart()
		{
			point = 0;

			pointText.text = point.ToString ();

			player.transform.eulerAngles = Vector3.zero;

			float save = Camera.main.orthographicSize;
			Camera.main.orthographicSize = cameraSize;
			float height = 2f * cameraSize;

			float width = height * Camera.main.aspect;

			this.radiusBorder = width * 0.60f / 2f;

			Camera.main.orthographicSize = save;

			player.gameObject.SetActive(true);

			player.DOPosition(this.radiusBorder, 0);

			circle = FindObjectOfType<Circle>();

			circle.transform.position = new Vector3(0,0,98f);

			circle.DOStart(this.radiusBorder);

			StartDotColorChange();

			InvokeRepeating("InstantiateHazard", 1, 5);

			soundManager.PlayStartVoice();

			Camera.main.orthographicSize = 0.1f;
			#if AADOTWEEN
			Camera.main.DOOrthoSize(5,0.3f).SetEase(Ease.OutBack).OnComplete(() => {
				player.ActivateTouchControl();

				player.DOStart();

				DOInstantiateDotToCollect();
			});
			#endif
		}
		/// <summary>
		/// Add 1 point, play point sound, update the UI Text point.
		/// </summary>
		public void Add1Point()
		{
			point++;

			pointText.text = point.ToString();

			soundManager.PlayPoint ();

			if(point % numberOfDotsToCreate == 0)
			{
				DOInstantiateDotToCollect();

				InstantiateHazard();

				DOMainColorChange();

				soundManager.PlayReloadVoice();
			}
		}
        
        /// <summary>
        /// Instantiante a new Ball Hazard.
        /// </summary>
        void InstantiateHazard()
		{
			var p = Instantiate(ballPrefab) as GameObject;
			var pl = p.GetComponent<Ball>();
			pl.is_hazard = true;
			pl.DOPosition(this.radiusBorder, 0);
		}
		/// <summary>
		/// Instantiante numberOfDotsToCreate = 15 by default DotToCollect.
		/// </summary>
		void DOInstantiateDotToCollect()
		{
			for(int i = 0; i < numberOfDotsToCreate; i++)
			{
				DotToCollect ob = (Instantiate(dotToCollectPrefab) as GameObject).GetComponent<DotToCollect>();

				ob.gameObject.SetActive(true);

				ob.Init(i * 360 / numberOfDotsToCreate);
			}
		}
		/// <summary>
		/// If using Very Simple Leaderboard by App Advisory, report the score : http://u3d.as/qxf
		/// </summary>
		void ReportScoreToLeaderboard(int p)
		{
			#if APPADVISORY_LEADERBOARD
			LeaderboardManager.ReportScore(p);
			#else
			if(PlayerPrefs.GetInt("VERY_SIMPLE_LEADERBOARD_COUNT", 0) > 32)
			{
				print("Get very simple leaderboard to use it : http://u3d.as/qxf");
				PlayerPrefs.SetInt("VERY_SIMPLE_LEADERBOARD_COUNT", 0); 
				PlayerPrefs.Save();
			}
			else
			{
				PlayerPrefs.SetInt("VERY_SIMPLE_LEADERBOARD_COUNT", PlayerPrefs.GetInt("VERY_SIMPLE_LEADERBOARD_COUNT", 0) + 1);
				PlayerPrefs.Save();
			}

			#endif
		}
		public void OnUIAnimOutStarted()
		{
			FindObjectOfType<AppAdvisory.UI.UIController>().HideVerySimpleShare();
		}
		/// <summary>
		/// Called at Game Over. Please have a look to the method DOOnTriggerEnter2D() in Ball : 
		/// Game Over is triggered if, and only if: 
		/// If the Ball is the player and triggered with an hazard and if the player is not jumping and touch an Hazard (who is a Ball too).
		/// </summary>
		/// pu
		
		public void Continue()
		{
            ShowVideoAd("192if3b93qo6991ed0",
            (bol) => {
                if (bol)
                {

                    GameOverPanelUI.SetActive(false);
                    Time.timeScale = 1;
                    foreach (GameObject g in FindObjectsOfType(typeof(GameObject)))
                    {
                        if (g.name == "HAZARD")
                        {
                            Destroy(g);
                        }
                    }


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
            
        }
		public void EndGame()
        {
			GameOver(player.sr.transform);
        }
		public void ShowGameOverPanel()
		{
			GameOverPanelUI.SetActive(true);
			if(point>PlayerPrefs.GetInt("BestScore"))
			{
				PlayerPrefs.SetInt("BestScore", point);
			}
            GameOverPanelUI.GetComponent<GameOverPanel>().SetScore(point);
            Time.timeScale = 0;
		}
		public void GameOver(Transform t1)
		{

            Time.timeScale = 1;
            ShowInterstitialAd("1lcaf5895d5l1293dc",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
            if (isGameOver)
				return;

			//FindObjectOfType<AppAdvisory.UI.UIController>().DOTakeScreenshotWithVerySimpleShare();

			//ShowAds();

			ReportScoreToLeaderboard(point);

			isGameOver = true;

			player.DesactivateTouchControl();

			#if AADOTWEEN
			DOTween.KillAll();
			#endif

			Util.SetLastScore(point);

			FindObjectOfType<UIController>().SetBestText(Util.GetBestScore());
			FindObjectOfType<UIController>().SetLastText(Util.GetLastScore());

			StopAllCoroutines ();

			CancelInvoke();

			#if AADOTWEEN
			DOTween.KillAll();
			#endif

			soundManager.PlayGameOverVoice ();

//			FindObjectOfType<AnimPointText>().DoAnimPointOut();

			#if AADOTWEEN
			t1.DOScale(Vector3.one * 1.5f, 0.1f).SetLoops(5,LoopType.Yoyo).OnComplete(() => {
				Camera.main.DOOrthoSize(0.1f,1).SetEase(Ease.InBack).OnComplete(() => {
					Util.ReloadLevel();
				});
			});
			#endif
		}
		public void ShowAds()
		{
			int count = PlayerPrefs.GetInt("GAMEOVER_COUNT",0);
			count++;
			PlayerPrefs.SetInt("GAMEOVER_COUNT",count);
			PlayerPrefs.Save();

			#if APPADVISORY_ADS
			if(count > numberOfPlayToShowInterstitial)
			{
			#if UNITY_EDITOR
			print("count = " + count + " > numberOfPlayToShowINterstitial = " + numberOfPlayToShowInterstitial);
			#endif
			if(AdsManager.instance.IsReadyInterstitial())
			{
			#if UNITY_EDITOR
				print("AdsManager.instance.IsReadyInterstitial() == true ----> SO ====> set count = 0 AND show interstial");
			#endif
				PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
				AdsManager.instance.ShowInterstitial();
			}
			else
			{
			#if UNITY_EDITOR
				print("AdsManager.instance.IsReadyInterstitial() == false");
			#endif
			}

		}
		else
		{
			PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
		}
		PlayerPrefs.Save();
			#else
		if(count >= numberOfPlayToShowInterstitial)
		{
			Debug.LogWarning("To show ads, please have a look to Very Simple Ad on the Asset Store, or go to this link: " + VerySimpleAdsURL);
			Debug.LogWarning("Very Simple Ad is already implemented in this asset");
			Debug.LogWarning("Just import the package and you are ready to use it and monetize your game!");
			Debug.LogWarning("Very Simple Ad : " + VerySimpleAdsURL);
			PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
		}
		else
		{
			PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
		}
		PlayerPrefs.Save();
			#endif
	}
        public void getClickid()
        {
            var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
            if (launchOpt.Query != null)
            {
                foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                    if (kv.Value != null)
                    {
                        Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                        if (kv.Key.ToString() == "clickid")
                        {
                            clickid = kv.Value.ToString();
                        }
                    }
                    else
                    {
                        Debug.Log(kv.Key + "<-参数-> " + "null ");
                    }
            }
        }

        public void apiSend(string eventname, string clickid)
        {
            TTRequest.InnerOptions options = new TTRequest.InnerOptions();
            options.Header["content-type"] = "application/json";
            options.Method = "POST";

            JsonData data1 = new JsonData();

            data1["event_type"] = eventname;
            data1["context"] = new JsonData();
            data1["context"]["ad"] = new JsonData();
            data1["context"]["ad"]["callback"] = clickid;

            Debug.Log("<-data1-> " + data1.ToJson());

            options.Data = data1.ToJson();

            TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
               response => { Debug.Log(response); },
               response => { Debug.Log(response); });
        }


        /// <summary>
        /// </summary>
        /// <param name="adId"></param>
        /// <param name="closeCallBack"></param>
        /// <param name="errorCallBack"></param>
        public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
        {
            starkAdManager = StarkSDK.API.GetStarkAdManager();
            if (starkAdManager != null)
            {
                starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
            }
        }

        /// <summary>
        /// 播放插屏广告
        /// </summary>
        /// <param name="adId"></param>
        /// <param name="errorCallBack"></param>
        /// <param name="closeCallBack"></param>
        public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
        {
            starkAdManager = StarkSDK.API.GetStarkAdManager();
            if (starkAdManager != null)
            {
                var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
                mInterstitialAd.Load();
                mInterstitialAd.Show();
            }
        }
    }
}