using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Equipment,
    Default
}

public enum Attributes
{
    AtkBonus,
    StrBonus,
    DfcBonus,
    MgcBonus,
    RngBonus
}
public abstract class ItemObject : ScriptableObject
{
    public bool stackable;
    public int id;
    public Sprite uiDisplay;
    public GameObject groundItem;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    public ItemBuff[] buffs;

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable]
public class Item
{
    public bool canStack;
    public string name;
    public int id;
    public ItemBuff[] buffs;
    public Item(ItemObject item)
    {
        name = item.name;
        id = item.id;
        canStack = item.stackable;
        buffs = new ItemBuff[item.buffs.Length];
        for(int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.buffs[i].min, item.buffs[i].max);
            buffs[i].attribute = item.buffs[i].attribute;
        }
    }
}

[System.Serializable]
public class ItemBuff
{
    public Attributes attribute;
    public int value;
    public int min;
    public int max;
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }
    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}
