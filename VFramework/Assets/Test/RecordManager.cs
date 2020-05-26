using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordManager
{
    static Dictionary<string, RecordTable> s_RecordCache = new Dictionary<string, RecordTable>();

    public static RecordTable GetData(string RecordName)
    {
        if (s_RecordCache.ContainsKey(RecordName))
        {
            return s_RecordCache[RecordName];
        }

        RecordTable record = null;

        record = new RecordTable();

        s_RecordCache.Add(RecordName, record);

        return record;
    }
}
