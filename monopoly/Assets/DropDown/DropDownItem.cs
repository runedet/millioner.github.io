using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DropDownItem : MonoBehaviour
{
    [SerializeField] TMP_Text Label;
    [SerializeField] Image Image;
    UnityAction<int> Action;
    int ID;

    [SerializeField] void Choice() => Action.Invoke(ID);

    public void SetColor(Color color) => Image.color = color;

    public string GetLabel() => Label.text;

    public void SetItem(int id, UnityAction<int> action, string label, Color color)
    {
        SetColor(color);
        ID = id;
        Action = action;
        Label.text = label;
    }
}