using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MainPanelController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Sprite newSprite;
    [SerializeField] private HeartPanelController heartPanel;

    /// <summary>
    /// Play Button 눌렀을 때 호출되는 메서드
    /// </summary>
    private void Start()
    {
        playButton.onClick.AddListener(OnClickPlayButton);
        heartPanel.InitHeartCount();
    }

    public void OnClickPlayButton()
    {
        float animationDuration = 0.2f;
        playButton.GetComponent<Image>().sprite = newSprite;
        playButton.GetComponent<RectTransform>().transform.DOScale(1.5f, animationDuration)
            .SetEase(Ease.OutQuad);
        playButton.GetComponent<Image>().DOFade(0, animationDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                playButton.GetComponent<RectTransform>().transform.DOScale(1f, 0);
                playButton.GetComponent<Image>().DOFade(1, 0);
                GameManager.Instance.heartCount--;
                heartPanel.RemoveHeart(() => GameManager.Instance.StartGame());
            });
    }

    #region Main Menu 버튼 클릭 함수
    /// <summary>
    /// Shop 아이콘 터치시 호출되는 메서드
    /// </summary>
    public void OnClickShopButton()
    {
    }

    /// <summary>
    /// Stage 아이콘 터치시 호출되는 메서드
    /// </summary>
    public void OnClickStageButton()
    {
    }
    
    /// <summary>
    /// Leaderboard 아이콘 터치시 호출되는 메서드
    /// </summary>
    public void OnClickLeaderboardButton()
    {
    }

    /// <summary>
    /// Settings 아이콘 터치시 호출되는 메서드
    /// </summary>
    public void OnClickSettingsButton()
    {
    }
    #endregion
}
