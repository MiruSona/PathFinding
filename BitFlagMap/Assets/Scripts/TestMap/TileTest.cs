using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileTest : MonoBehaviour, IPointerClickHandler
{
    private Image _image;
    private RectTransform _rectTrans;

    private int _colorIndex = 0;
    private Color[] _colors = new Color[]
    {
        Color.gray, Color.black, Color.blue, Color.red, Color.yellow
    };

    public void Init()
    {
        _image = GetComponent<Image>();
        _rectTrans = GetComponent<RectTransform>();

        _image.color = _colors[0];
    }

    private void ChangeToNextColor()
    {
        if (_colorIndex + 1 >= _colors.Length)
            _colorIndex = 0;
        else
            _colorIndex++;

        _image.color = _colors[_colorIndex];
    }

    private void ChangeToPrevColor()
    {
        if (_colorIndex - 1 < 0)
            _colorIndex = _colors.Length - 1;
        else
            _colorIndex--;

        _image.color = _colors[_colorIndex];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ChangeToNextColor();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ChangeToPrevColor();
        }
    }
}
