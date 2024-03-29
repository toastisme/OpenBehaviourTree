using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Assertions;
using System;

namespace OpenBehaviourTree{
[Serializable]
public class BehaviourTreeBlackboard : ScriptableObject
{

    /**
    * \class OpenBehaviourTree.BehaviourTreeBlackboard
    * Blackboard for the BehaviourTree class.
    * Allows nodes to share variables.
    */

    public List<string> keyTypes = new List<string>{"int", 
                                                    "float", 
                                                    "bool",
                                                    "string",
                                                    "GameObject",
                                                    "Vector3",
                                                    "Vector2"
                                                    };

    bool modifyingKeys = false;

    [SerializeField]
    public SerializableOrderedDictionary allKeyNames;
    [SerializeField]
    public SerializableDictionary<string, int> intKeys;
    [SerializeField]
    public SerializableDictionary<string, float> floatKeys;
    [SerializeField]
    public SerializableDictionary<string, bool> boolKeys;
    [SerializeField]
    public SerializableDictionary<string, Vector3> vector3Keys;
    [SerializeField]
    public SerializableDictionary<string, Vector2> vector2Keys;
    [SerializeField]
    public SerializableDictionary<string, GameObject> gameObjectKeys;
    [SerializeField]
    public SerializableDictionary<string, string> stringKeys;

    public SerializableDictionary<string, int> GetIntKeys(){return intKeys;}
    public SerializableDictionary<string, float> GetFloatKeys(){return floatKeys;}
    public SerializableDictionary<string, bool> GetBoolKeys(){return boolKeys;}
    public SerializableDictionary<string, Vector3> GetVector3Keys(){return vector3Keys;}
    public SerializableDictionary<string, Vector2> GetVector2Keys(){return vector2Keys;}
    public SerializableDictionary<string, GameObject> GetGameObjectKeys(){return gameObjectKeys;}
    public SerializableDictionary<string, string> GetStringKeys(){return stringKeys;}

    public List<string> GetKeyTypes(){return keyTypes;}

    public SerializableOrderedDictionary GetAllKeyNames(){
        if (allKeyNames == null){
            allKeyNames = new SerializableOrderedDictionary();
        }
        return allKeyNames;
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


    // Gettters/Setters used by GameObjects with a BehaviourTree
    public int GetInt(string keyName){return intKeys[keyName];}
    public bool GetBool(string keyName){return boolKeys[keyName];}
    public float GetFloat(string keyName){return floatKeys[keyName];}
    public GameObject GetGameObject(string keyName){return gameObjectKeys[keyName];}
    public string GetString(string keyName){return stringKeys[keyName];}
    public Vector3 GetVector3(string keyName){return vector3Keys[keyName];}
    public Vector2 GetVector2(string keyName){return vector2Keys[keyName];}
    public void SetInt(string keyName, int val){intKeys[keyName] = val;}
    public void SetBool(string keyName, bool val){boolKeys[keyName] = val;}
    public void SetFloat(string keyName, float val){floatKeys[keyName] = val;}
    public void SetGameObject(string keyName, GameObject val){gameObjectKeys[keyName] = val;}
    public void SetString(string keyName, string val){stringKeys[keyName] = val;}
    public void SetVector3(string keyName, Vector3 val){vector3Keys[keyName] = val;}
    public void SetVector2(string keyName, Vector2 val){vector2Keys[keyName] = val;}

    KeyValuePair<int, string> GetKeyIdxAndType(string keyName){

        /**
         * Returns idx of keyName in allKeyNames and its type
         */

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
            GUI.changed = true;
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
                    if (intKeys == null){
                        intKeys = new SerializableDictionary<string, int>();
                    }
                    intKeys.Add(keyName, 0);
                    allKeyNames.Add(keyName, "int");
                    break;
                case "bool":
                    if (boolKeys == null){
                        boolKeys = new SerializableDictionary<string, bool>();
                    }
                    boolKeys.Add(keyName, false);
                    allKeyNames.Add(keyName, "bool");
                    break;
                case "float":
                    if (floatKeys == null){
                        floatKeys = new SerializableDictionary<string, float>();
                    }
                    floatKeys.Add(keyName, 0);
                    allKeyNames.Add(keyName, "float");
                    break;
                case "GameObject":
                    if (gameObjectKeys == null){
                        gameObjectKeys = new SerializableDictionary<string, GameObject>();
                    }
                    gameObjectKeys.Add(keyName, null);
                    allKeyNames.Add(keyName, "GameObject");
                    break;
                case "string":
                    if (stringKeys == null){
                        stringKeys = new SerializableDictionary<string, string>();
                    }
                    stringKeys.Add(keyName, "");
                    allKeyNames.Add(keyName, "string");
                    break;
                case "Vector3":
                    if (vector3Keys == null){
                        vector3Keys = new SerializableDictionary<string, Vector3>();
                    }
                    vector3Keys.Add(keyName, new Vector3(0,0,0));
                    allKeyNames.Add(keyName, "Vector3");
                    break;
                case "Vector2":
                    if (vector2Keys == null){
                        vector2Keys = new SerializableDictionary<string, Vector2>();
                    }
                    vector2Keys.Add(keyName, new Vector2(0,0));
                    allKeyNames.Add(keyName, "Vector2");
                    break;
            }

        }
    }

    public bool ModifyingKeys(){return modifyingKeys;}

    public float GetWeightValue(string weightKey){

        /**
         * Used by ProbabilityWeight nodes.
         */

        if (intKeys.ContainsKey(weightKey)){
            return (float)intKeys[weightKey];
        }
        else if (floatKeys.ContainsKey(weightKey)){
            return floatKeys[weightKey];
        }
        else{
            throw new Exception("weightKey not in intKeys or floatKeys.");
        }
    }

}
}
