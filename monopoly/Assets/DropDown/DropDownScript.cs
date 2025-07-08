using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

public class DropDownScript : MonoBehaviour
{
    public Color DefaultColor = Color.white, SelectColor = Color.yellow;
    enum DropType { GenerateTemplateFromMaxItems, GenerateEnernityTemplate, Fixed}
    [SerializeField] DropType type;
    [Range (2, 20)] public int MaxItems;
    [SerializeField] GameObject Template, PrefabItem;
    [SerializeField] RectTransform Parent;
    [SerializeField] TMP_Text MainLable;
    int CountIntems;
    List<DropDownItem> ItemsScripts = new();
    List<GameObject> Items = new();
    UnityAction<string> Action;

    public void Initialize(string[] options, string selected, UnityAction<string> action)
    {
        Action = action;
        MainLable.text = selected;
        GenerateOptions(options, selected);
        if (type == DropType.GenerateTemplateFromMaxItems || type == DropType.GenerateEnernityTemplate) ChangeSize(Template.GetComponent<RectTransform>());
    }

    void GenerateOptions(string[] options, string selected)
    {
        for (var k = 0; k < Items.Count; k++) Destroy(Items[k]);

        Items.Clear();
        ItemsScripts.Clear();

        CountIntems = options.Length;

        for (var i = 0; i < CountIntems; i++)
        {
            Items.Add(Instantiate(PrefabItem, Parent));
            ItemsScripts.Add(Items[i].GetComponent<DropDownItem>());
            ItemsScripts[i].SetItem(i, Select, options[i], DefaultColor);
            if (options[i] == selected) ItemsScripts[i].SetColor(SelectColor);
        }
    }

    void ChangeSize(RectTransform Objetc)
    {
        var rectPrefab = PrefabItem.GetComponent<RectTransform>().rect;
        if (type == DropType.GenerateTemplateFromMaxItems) Objetc.sizeDelta = new(Objetc.sizeDelta.x, (rectPrefab.height * MaxItems) + Parent.GetComponent<VerticalLayoutGroup>().spacing * (MaxItems - 1));
        else Objetc.sizeDelta = new(Objetc.sizeDelta.x, (rectPrefab.height * CountIntems) + Parent.GetComponent<VerticalLayoutGroup>().spacing * (CountIntems - 1));
        Template.transform.localPosition = new(Template.transform.localPosition.x, -GetComponent<RectTransform>().rect.height / 2);
    }

    void Select(int id)
    {
        MainLable.text = ItemsScripts[id].GetLabel();

        for (var i = 0; i < CountIntems; i++) ItemsScripts[i].SetColor(DefaultColor);

        ItemsScripts[id].SetColor(SelectColor);
        Interact();
        Action.Invoke(ItemsScripts[id].GetLabel());
    }

    [SerializeField] void Interact()
    {
        Template.SetActive(!Template.activeSelf);
        GetComponent<Button>().interactable = !GetComponent<Button>().interactable;
    }
}