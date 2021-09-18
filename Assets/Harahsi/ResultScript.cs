using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class ResultScript : MonoBehaviour
{
    [SerializeField]
    private Button _titleButton;
    [SerializeField]
    private Button _retryButton;
    [SerializeField]
    private Text _resultText;
    [SerializeField]
    private Text _rateltText;
    public GameObject loseEnemy;
    public GameObject winEnemy;
    public bool winOrLose = true;
    

    // Start is called before the first frame update
    void Start()
    {
        //_rateltText.text = "1300";
        if (winOrLose == true)
        {
            _resultText.text = "You win";
            winEnemy.SetActive(true);
            loseEnemy.SetActive(false);
        }
        else if(winOrLose == false)
        {
            _resultText.text = "You lose";
            winEnemy.SetActive(false);
            loseEnemy.SetActive(true);
        }

        _titleButton.OnClickAsObservable().Subscribe(x =>
        {
            Debug.Log("タイトル戻る");
            //タイトルに戻る処理をかく

        }).AddTo(this);

        _retryButton.OnClickAsObservable().Subscribe(x =>
        {
            Debug.Log("メインに戻る");
            //メインに戻る処理をかく

        }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
