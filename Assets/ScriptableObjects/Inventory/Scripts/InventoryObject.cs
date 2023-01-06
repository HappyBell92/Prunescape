using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public ItemDatabaseObject database;
    public Inventory container;

    public void AddItem(Item _item, int _amount)
    {
        //if(_item.buffs.Length > 0)
        //{
        //    container.Items.Add(new InventorySlot(_item.id, _item, _amount));
        //    return;
        //}

        if (!_item.canStack)
        {
            container.Items.Add(new InventorySlot(_item.id, _item, _amount));
            return;
        }

        for (int i = 0; i < container.Items.Count; i++)
        {
            if(container.Items[i].item.id == _item.id)
            {
                container.Items[i].AddAmount(_amount);
                return;
            }
        }
        container.Items.Add(new InventorySlot(_item.id, _item, _amount));
    }

    [ContextMenu("Save")]
    public void Save()
    {
        Debug.Log("Inventory Saved");
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file, saveData);
        file.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if(File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            Debug.Log("Inventory Loaded");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }
    }

    [ContextMenu("Clear Inventory")]
    public void Clear()
    {
        container = new Inventory();
    }
}

[System.Serializable]
public class Inventory
{
    public List<InventorySlot> Items = new List<InventorySlot>();
}

[System.Serializable]
public class InventorySlot
{
    public int ID;
    public Item item;
    public int amount;
    public InventorySlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }

    
}
