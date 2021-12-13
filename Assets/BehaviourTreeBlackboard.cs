using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeBlackboard : ScriptableObject
{
    public List<string> keyTypes = new List<string>{"int", 
                                                    "float", 
                                                    "bool",
                                                    "string",
                                                    "GameObject",
                                                    "Vector3",
                                                    "Vector2"
                                                    };
    public List<string> allKeyNames = new List<string>();
    public Dictionary<string, int> intDict = new Dictionary<string, int>();
    public Dictionary<string, float> floatDict = new Dictionary<string, float>();
    public Dictionary<string, bool> boolDict = new Dictionary<string, bool>();
    public Dictionary<string, Vector3> vector3Dict = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector2> vector2Dict = new Dictionary<string, Vector2>();
    public Dictionary<string, GameObject> gameObjectDict = new Dictionary<string, GameObject>();
    public Dictionary<string, string> stringDict = new Dictionary<string, string>();

    public List<string> GetKeyTypes(){return keyTypes;}
    public List<string> GetAllKeyNames(){return allKeyNames;}

    private void AddKeyName(string keyName){
        allKeyNames.Add(keyName);
    }
    string GetUniqueKeyName(){
        string initKeyName = "NewKey";
        string keyName = initKeyName;
        int counter = 0;
        while(allKeyNames.Contains(keyName)){
            keyName =  initKeyName + counter.ToString();
            counter++;
        }
        return keyName;
    }

    public void RemoveKey(string keyName){
        if (allKeyNames.Contains(keyName)){

            if (intDict.ContainsKey(keyName)){
                intDict.Remove(keyName);
            }
            else if (boolDict.ContainsKey(keyName)){
                boolDict.Remove(keyName);
            }
            else if (floatDict.ContainsKey(keyName)){
                floatDict.Remove(keyName);
            }
            else if (gameObjectDict.ContainsKey(keyName)){
                gameObjectDict.Remove(keyName);
            }
            else if (stringDict.ContainsKey(keyName)){
                stringDict.Remove(keyName);
            }
            else if (vector3Dict.ContainsKey(keyName)){
                vector3Dict.Remove(keyName);
            }
            else if (vector2Dict.ContainsKey(keyName)){
                vector2Dict.Remove(keyName);
            }
            else{
                throw new System.Exception("Key in allKeyNames but not found in dictionaries.");
            }
        }
    }

    public void AddKey(string keyType){
        if (keyTypes.Contains(keyType)){
            string keyName = GetUniqueKeyName();
            allKeyNames.Add(keyName);
            switch(keyType){
                case "int":
                    intDict.Add(keyName, 0);
                    break;
                case "bool":
                    boolDict.Add(keyName, false);
                    break;
                case "float":
                    floatDict.Add(keyName, 0);
                    break;
                case "GameObject":
                    gameObjectDict.Add(keyName, null);
                    break;
                case "string":
                    stringDict.Add(keyName, "");
                    break;
                case "vector3":
                    vector3Dict.Add(keyName, new Vector3(0,0,0));
                    break;
                case "vector2":
                    vector2Dict.Add(keyName, new Vector2(0,0));
                    break;
            }

        }
    }

}
