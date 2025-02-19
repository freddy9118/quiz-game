using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class HeartPanelController : MonoBehaviour
{
    [SerializeField] private GameObject _heartRemoveImageObject;
    [SerializeField] private TMP_Text _heartCountText;
    
    [SerializeField] private AudioClip _heartRemoveAudioClip;
    [SerializeField] private AudioClip _heartAddAudioClip;
    [SerializeField] private AudioClip _heartEmptyAudioClip;
    
    private AudioSource _audioSource;
    
    private int _heartCount;
    
    // 1. 하트 추가 연출
    // 2. 하트 감소 연출
    // 3. 하트 부족 연출

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _heartRemoveImageObject.SetActive(false);
        InitHeartCount(10);
    }

    /// <summary>
    /// Heart Panel에 하트 수 초기화
    /// </summary>
    /// <param name="heartCount">하트 수</param>
    public void InitHeartCount(int heartCount)
    {
        _heartCount = heartCount;
        _heartCountText.text = _heartCount.ToString();
    }

    private void ChangeTextAnimation(bool isAdd, Action callback = null)
    {
        float duration = 0.2f;
        float yPos = 40f;
        
        _heartCountText.rectTransform.DOAnchorPosY(-yPos, duration);
        _heartCountText.DOFade(0, duration).OnComplete(() =>
        {
            if (isAdd)
            {
                var currentHeartCount = _heartCountText.text;
                _heartCountText.text = (int.Parse(currentHeartCount) + 1).ToString();
            }
            else
            {
                var currentHeartCount = _heartCountText.text;
                _heartCountText.text = (int.Parse(currentHeartCount) - 1).ToString();
            }
            
            // Heart Panel의 Width를 글자 수에 따라 변경
            var textLength = _heartCountText.text.Length;
            GetComponent<RectTransform>().sizeDelta = new Vector2(100 + textLength * 30f, 100f);
            
            // 새로운 하트 수 추가 애니메이션
            _heartCountText.rectTransform.DOAnchorPosY(yPos, 0);
            _heartCountText.rectTransform.DOAnchorPosY(0, duration);
            _heartCountText.DOFade(1, duration).OnComplete(() =>
            {
                Debug.Log("콜벡됨 changetxet");
                callback?.Invoke();
            });
        });
    }

    public void AddHeart(int heartCount)
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < 3; i++)
        {
            sequence.AppendCallback(() =>
            {
                ChangeTextAnimation(true);
                
                // 효과음 재생
                if (UserInformations.IsPlaySFX)
                    _audioSource.PlayOneShot(_heartAddAudioClip);
            });
            sequence.AppendInterval(0.5f);
        }
    }

    public void EmptyHeart(Action callback = null)
    {
        // 효과음 재생
        if (UserInformations.IsPlaySFX)
            _audioSource.PlayOneShot(_heartEmptyAudioClip);
        
        GetComponent<RectTransform>().DOPunchPosition(new Vector3(20f, 0, 0), 1f, 7);
        callback?.Invoke();
    }

    public void RemoveHeart(Action callback = null)
    {
        // 효과음 재생
        if (UserInformations.IsPlaySFX)
            _audioSource.PlayOneShot(_heartRemoveAudioClip);
        
        // 하트 사라지는 연출
        _heartRemoveImageObject.SetActive(true);
        _heartRemoveImageObject.transform.localScale = Vector3.zero;
        _heartRemoveImageObject.GetComponent<Image>().color = Color.white;
        
        _heartRemoveImageObject.transform.DOScale(3f, 1f);
        _heartRemoveImageObject.GetComponent<Image>().DOFade(0f, 1f);
        
        // 하트 수 텍스트 떨어지는 연출
        DOVirtual.DelayedCall(1f, () =>
        {
            ChangeTextAnimation(false, () =>
            {
                Debug.Log("콜벡됨 removeheart");
                callback?.Invoke();
            });
        });
    }
}
