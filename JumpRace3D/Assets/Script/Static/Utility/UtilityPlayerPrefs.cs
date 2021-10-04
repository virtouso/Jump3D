using UnityEngine;


//made utility wrapper because most of the time you need  encoding or platform specific things
public static class UtilityPlayerPrefs
{

    public static bool CheckKeyExist(string key)
    {
        return PlayerPrefs.HasKey(key);
    }


    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }



}
