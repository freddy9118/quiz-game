using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GamePanelController : MonoBehaviour
{
    private GameObject _firstQuizCardObject;
    private GameObject _secondQuizCardObject;
    
    private List<QuizData> _quizDataList;
    
    private int _lastGeneratedQuizIndex;
    private int _lastStageIndex;
    
    private Queue<QuizCardController> _quizCardQueue = new Queue<QuizCardController>();

    public void AddQuizCardObject(QuizData? quizData, bool isInit = false)
    {
        QuizCardController tempQuizCardController = null;
        
        // Step.1
        // First 영역의 카드 제거
        void RemoveFirstQuizCard(Action onCompleted = null)
        {
            tempQuizCardController = _quizCardQueue.Dequeue();
            tempQuizCardController.SetQuizCardPosition(QuizCardController.QuizCardPositionType.Remove,
                true, onCompleted);
        }
        
        // Step.2
        // Second 영역의 카드를 First 영역으로 이동
        void SecondQuizCardToFirst(Action onCompleted = null)
        {
            var firstQuizCardController = _quizCardQueue.Peek();
            firstQuizCardController.SetQuizCardPosition(QuizCardController.QuizCardPositionType.First,
                true, onCompleted);
        }
        
        // Step.3
        // 새로운 퀴즈 카드를 Second 영역에 생성
        void AddNewQuizCard(Action onCompleted = null)
        {
            if (quizData.HasValue)
            {
                var quizCardObject = ObjectPool.Instance.GetObject();
                var quizCardController = quizCardObject.GetComponent<QuizCardController>();
                quizCardController.SetQuiz(quizData.Value, OnCompletedQuiz);
                _quizCardQueue.Enqueue(quizCardController);
                quizCardController.SetQuizCardPosition(QuizCardController.QuizCardPositionType.Second,
                    true, onCompleted);
            }
        }

        
        // 애니메이션 처리
        if (_quizCardQueue.Count > 0)
        {
            if (isInit)
            {
                SecondQuizCardToFirst();
                AddNewQuizCard();
            }
            else
            {
                RemoveFirstQuizCard(() =>
                    SecondQuizCardToFirst( () => 
                        AddNewQuizCard(() =>
                        {
                            if (tempQuizCardController != null) 
                                ObjectPool.Instance.ReturnObject(tempQuizCardController.gameObject);
                        })));
            }
        }
        else
        {
            AddNewQuizCard();
        }
    }
    
    private void Start()
    {
        _lastStageIndex = UserInformations.LastStageIndex;
        InitQuizCards(_lastStageIndex);
    }

    private void InitQuizCards(int stageIndex)
    {
        _quizDataList = QuizDataController.LoadQuizData(stageIndex);
        
        AddQuizCardObject(_quizDataList[0], true);
        AddQuizCardObject(_quizDataList[1], true);
        
        // _firstQuizCardObject = ObjectPool.Instance.GetObject();
        // _firstQuizCardObject.GetComponent<QuizCardController>()
        //     .SetQuiz(_quizDataList[0], 0, OnCompletedQuiz);
        //
        // _secondQuizCardObject = ObjectPool.Instance.GetObject();
        // _secondQuizCardObject.GetComponent<QuizCardController>()
        //     .SetQuiz(_quizDataList[1], 1, OnCompletedQuiz);
        //
        // SetQuizCardPosition(_firstQuizCardObject, 0);
        // SetQuizCardPosition(_secondQuizCardObject, 1);
        //
        // // 마지막으로 생성된 퀴즈 인덱스
        // _lastGeneratedQuizIndex = 1;
    }

    private void OnCompletedQuiz(int cardIndex)
    {
        if (cardIndex < _quizDataList.Count - 2)
        {
            AddQuizCardObject(_quizDataList[cardIndex + 2]);            
        }
        else
        {
            AddQuizCardObject(null);
            if (cardIndex == _quizDataList.Count - 1)
            {
                // TODO: 스테이지 클리어
                _lastStageIndex++;
                
                // 새로운 스테이지 시작
                if (_lastStageIndex < Constants.MAX_STAGE_COUNT)
                    InitQuizCards(_lastStageIndex);
            }
        }
        
        // if (cardIndex >= _quizDataList.Count)
        // {
        //     if (_lastStageIndex >= Constants.MAX_STAGE_COUNT - 1)
        //     {
        //         // TODO: 올 클리어 연출
        //         GameManager.Instance.QuitGame();
        //     }
        //     else
        //     {
        //         // TODO: 스테이지 클리어 연출
        //         _lastStageIndex += 1;
        //         InitQuizCards(_lastStageIndex);
        //         return;   
        //     }
        // }
        // else
        // {
        //     // ChangeQuizCard();
        //     if (_quizDataList.Count > cardIndex + 1)
        //         AddQuizCardObject(_quizDataList[cardIndex + 1]);
        //     else
        //         AddQuizCardObject(null);
        // }
    }

    // private void SetQuizCardPosition(GameObject quizCardObject, int index)
    // {
    //     var quizCardTransform = quizCardObject.GetComponent<RectTransform>();
    //     if (index == 0)
    //     {
    //         quizCardTransform.anchoredPosition = new Vector2(0, 0);
    //         quizCardTransform.localScale = Vector3.one;
    //         quizCardTransform.SetAsLastSibling();
    //         
    //         quizCardObject.GetComponent<QuizCardController>()
    //             .SetVisible(true);
    //     }
    //     else if (index == 1)
    //     {
    //         quizCardTransform.anchoredPosition = new Vector2(0, 160);
    //         quizCardTransform.localScale = Vector3.one * 0.9f;
    //         quizCardTransform.SetAsFirstSibling();
    //         
    //         quizCardObject.GetComponent<QuizCardController>()
    //             .SetVisible(false);
    //     }
    // }

    // private void ChangeQuizCard()
    // {
        // if (_lastGeneratedQuizIndex >= Constants.MAX_QUIZ_COUNT) return;
        //
        // var temp = _firstQuizCardObject;
        // _firstQuizCardObject = _secondQuizCardObject;
        // _secondQuizCardObject = ObjectPool.Instance.GetObject();
        //
        // if (_lastGeneratedQuizIndex < _quizDataList.Count - 1)
        // {
        //     _lastGeneratedQuizIndex++;
        //     _secondQuizCardObject.GetComponent<QuizCardController>()
        //         .SetQuiz(_quizDataList[_lastGeneratedQuizIndex], _lastGeneratedQuizIndex, OnCompletedQuiz);
        // }
        //
        // SetQuizCardPosition(_firstQuizCardObject, 0);
        // SetQuizCardPosition(_secondQuizCardObject, 1);
        //
        // ObjectPool.Instance.ReturnObject(temp);
    // }
}