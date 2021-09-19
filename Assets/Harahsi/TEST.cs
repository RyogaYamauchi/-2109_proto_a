using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;

public class TEST : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Text _text;
    private int count=0;


    async void Start()
    {
        //var obj = await Resources.LoadAsync<GameObject>("");
        //await _button.OnClickAsync();
        //asyncとawaitはセット

         /*_button.OnClickAsObservable().Subscribe(x =>
        {
            count++;
           _text.text= count.ToString();
            //Shift+optionでアンカー
            
        }).AddTo(this);//thisが破壊したらsubscribe解除*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
