using Game;
using TMPro;
using Utilities;

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Services;

namespace SettingPopup
{
    public class SettingView : MonoBehaviour
    {
        //[SerializeField] private TMP_Text highScoreTMP;
        //[SerializeField] private TMP_Text watermelonCountTMP;
        [SerializeField] private TMP_Text confirmButtonTMP;

        private ParamServices paramServices;
        private event Action OnConfirmButtonClicked;

        private void Awake()
        {
            var gameServiceObj = GameObject.FindGameObjectWithTag(Constants.ServicesTag);
            if (gameServiceObj != null)
            {
                var gameServices = gameServiceObj.GetComponent<GameServices>();
				paramServices = gameServices.GetService<ParamServices>();
            }
            else
            {
                SceneManager.LoadScene(Constants.HomeScene);
            }
        }

        private void Start()
        {
             //highScoreTMP.text = PlayerPrefs.GetInt(Constants.HighScore, 0).ToString();
             //watermelonCountTMP.text = PlayerPrefs.GetInt(Constants.WatermelonCount, 0).ToString();

            if(paramServices.PopupTypeParam == PopupType.SettingPopup)
            {
                OnConfirmButtonClicked = OnContinueButtonClick;
                confirmButtonTMP.text = "Continue";
            }
            else
            {
                OnConfirmButtonClicked = OnPlayAgainButtonClick;
                confirmButtonTMP.text = "Play Again";
            }
        }


        public void OnConfirmButtonClick()
        {
            OnConfirmButtonClicked?.Invoke();
        }

        private void OnPlayAgainButtonClick()
        {
            SceneManager.LoadScene(Constants.GameScene);
        }

        private void OnContinueButtonClick()
        {
            //Time.timeScale = 1.0f;
            PopupHelpers.Close();
        }

        public void OnHomeButtonClick()
        {
            SceneManager.LoadScene(Constants.HomeScene);
        }
    }
}
