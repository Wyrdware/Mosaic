using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoreSaveData
{
    private static int _saveSlot = 0;
    private static string Path()
    {
        return Application.persistentDataPath + "/saveData_"+_saveSlot + ".json";
    }
}
