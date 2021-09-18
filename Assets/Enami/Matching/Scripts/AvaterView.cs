using UnityEngine;
using UnityEngine.UI;

public class AvaterView : MonoBehaviour
{
    [SerializeField] private Image _image;
    void Start()
    {
        // ランダムな座標に自身のアバター（ネットワークオブジェクト）を生成する
        _image.transform.position = new Vector3(Random.Range(0f, 1000f), Random.Range(0f, 1000f));
    }
    
}
