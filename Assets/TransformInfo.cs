using UnityEngine;
using UnityEngine.UI;

public class TransformInfo : MonoBehaviour
{
    [SerializeField]
    private Transform _transform = null;

    [SerializeField]
    private Text _text = null;

    [SerializeField]
    private float _maxWidth = 2;
    private float _maxHeight = 2;

    void Update()
    {
        _text.text =
            "WIDTH: " + (_transform.lossyScale.x / _maxWidth) + "\n" +
            "HEIGHT: " + (_transform.lossyScale.y / _maxHeight) + "\n" +
            "ANGLE: " + _transform.eulerAngles.z;
    }
}
