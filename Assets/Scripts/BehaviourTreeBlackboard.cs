using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Assertions;

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
    public OrderedDictionary allKeyNames = new OrderedDictionary();
    public Dictionary<string, int> intKeys = new Dictionary<string, int>();
    public Dictionary<string, float> floatKeys = new Dictionary<string, float>();
    public Dictionary<string, bool> boolKeys = new Dictionary<string, bool>();
    public Dictionary<string, Vector3> vector3Keys = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector2> vector2Keys = new Dictionary<string, Vector2>();
    public Dictionary<string, GameObject> gameObjectKeys = new Dictionary<string, GameObject>();
    public Dictionary<string, string> stringKeys = new Dictionary<string, string>();

    public Dictionary<string, int> GetIntKeys(){return intKeys;}
    public Dictionary<string, float> GetFloatKeys(){return floatKeys;}
    public Dictionary<string, bool> GetBoolKeys(){return boolKeys;}
    public Dictionary<string, Vector3> GetVector3Keys(){return vector3Keys;}
    public Dictionary<string, Vector2> GetVector2Keys(){return vector2Keys;}
    public Dictionary<string, GameObject> GetGameObjectKeys(){return gameObjectKeys;}
    public Dictionary<string, string> GetStringKeys(){return stringKeys;}

    public List<string> GetKeyTypes(){return keyTypes;}
    public OrderedDictionary GetAllKeyNames(){return allKeyNames;}
    bool modifyingKeys = false;

    private void AddKeyName(string keyName, string keyType){
        allKeyNames.Add(keyName, keyType);
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
            switch(allKeyNames[keyName]){
                case "int":
                    intKeys.Remove(keyName);
                    break;
                case "bool":
                    boolKeys.Remove(keyName);
                    break;
                case "float":
                    floatKeys.Remove(keyName);
                    break;
                case "GameObject":
                    gameObjectKeys.Remove(keyName);
                    break;
                case "string":
                    stringKeys.Remove(keyName);
                    break;
                case "Vector3":
                    vector3Keys.Remove(keyName);
                    break;
                case "Vector2":
                    vector2Keys.Remove(keyName);
                    break;
                default:
                    throw new System.Exception("Key in allKeyNames but not found in dictionaries.");
            }
            allKeyNames.Remove(keyName);
        }
        else{
            Debug.LogWarning("Tried to remove a key that does not exist.");
        }
    }

    KeyValuePair<int, string> GetKeyIdxAndType(string keyName){
        IDictionaryEnumerator allKeysEnumerator = allKeyNames.GetEnumerator();
        int idx = 0;
        while(allKeysEnumerator.MoveNext()){
            if (allKeysEnumerator.Key.ToString() == keyName){
                return new KeyValuePair<int, string>(idx, allKeysEnumerator.Value.ToString());
            }
            idx++;
        }
        throw new System.Exception("Could not find key in allKeyNames.");
    }

    public void RenameKey(string oldKeyName, string newKeyName){
        modifyingKeys = true;
        if (allKeyNames.Contains(oldKeyName)){
            KeyValuePair<int, string> allKeyIdxAndType = GetKeyIdxAndType(oldKeyName);
            switch(allKeyIdxAndType.Value){
                case "int":
                    int intVal = intKeys[oldKeyName];
                    intKeys.Remove(oldKeyName);
                    intKeys.Add(newKeyName, intVal);
                    break;
                case "bool":
                    bool boolVal = boolKeys[oldKeyName];
                    boolKeys.Remove(oldKeyName);
                    boolKeys.Add(newKeyName, boolVal);
                    break;
                case "float":
                    float floatVal = floatKeys[oldKeyName];
                    floatKeys.Remove(oldKeyName);
                    floatKeys.Add(newKeyName, floatVal);
                    break;
                case "GameObject":
                    GameObject goVal = gameObjectKeys[oldKeyName];
                    gameObjectKeys.Remove(oldKeyName);
                    gameObjectKeys.Add(newKeyName, goVal);
                    break;
                case "string":
                    string stringVal = stringKeys[oldKeyName];
                    stringKeys.Remove(oldKeyName);
                    stringKeys.Add(newKeyName, stringVal);
                    break;
                case "Vector3":
                    Vector3 v3Val = vector3Keys[oldKeyName];
                    vector3Keys.Remove(oldKeyName);
                    vector3Keys.Add(newKeyName, v3Val);
                    break;
                case "Vector2":
                    Vector2 v2Val = vector2Keys[oldKeyName];
                    vector2Keys.Remove(oldKeyName);
                    vector2Keys.Add(newKeyName, v2Val);
                    break;
                default:
                    throw new System.Exception("Key " + oldKeyName + " in allKeyNames but not found in dictionaries.");
            }
            allKeyNames.Remove(oldKeyName);
            allKeyNames.Insert(allKeyIdxAndType.Key, newKeyName, allKeyIdxAndType.Value);
        }
        modifyingKeys = false;
    }

    public void AddKey(string keyType, string keyName=""){
        if (keyTypes.Contains(keyType)){
            if (keyName == ""){
                keyName = GetUniqueKeyName();
            }
            else{
                Assert.IsFalse(allKeyNames.Contains(keyName));
            }
            switch(keyType){
                case "int":
                    intKeys.Add(keyName, 0);
                    allKeyNames.Add(keyName, "int");
                    break;
                case "bool":
                    boolKeys.Add(keyName, false);
                    allKeyNames.Add(keyName, "bool");
                    break;
                case "float":
                    floatKeys.Add(keyName, 0);
                    allKeyNames.Add(keyName, "float");
                    break;
                case "GameObject":
                    gameObjectKeys.Add(keyName, null);
                    allKeyNames.Add(keyName, "GameObject");
                    break;
                case "string":
                    stringKeys.Add(keyName, "");
                    allKeyNames.Add(keyName, "string");
                    break;
                case "Vector3":
                    vector3Keys.Add(keyName, new Vector3(0,0,0));
                    allKeyNames.Add(keyName, "Vector3");
                    break;
                case "Vector2":
                    vector2Keys.Add(keyName, new Vector2(0,0));
                    allKeyNames.Add(keyName, "Vector2");
                    break;
            }

        }
    }

    public bool ModifyingKeys(){return modifyingKeys;}

}
