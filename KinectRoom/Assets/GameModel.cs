using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FaceData
{
    public int happy;
    public int wearingGlasses;
    public int roll;
    public int pitch;
    public int yaw;
    public ulong id;
    public ulong time;

    public static FaceData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<FaceData>(jsonString);
    }
}

public class GameModel {

    private static GameModel instance;
    public static GameModel Instance {
        get
        {
            if (instance == null) instance = new GameModel();
            return instance;
        }
    }

    public FaceData faceData = null;
}
