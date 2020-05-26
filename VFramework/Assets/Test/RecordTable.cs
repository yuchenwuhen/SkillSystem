using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordTable : Dictionary<string, Data>
{
    public void SetRecord(string key, int value)
    {
        if(this.ContainsKey(key))
        {
            this[key] = new Data(value);
        }
        else
        {
            this.Add(key, new Data(value));
        }
    }
}
