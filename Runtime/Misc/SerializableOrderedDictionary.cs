using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Assertions;
using System;

 [Serializable]
 public class SerializableOrderedDictionary : OrderedDictionary, ISerializationCallbackReceiver
 {
     /**
      * Wrapper for OrderedDictionary to allow for serialization.
      */

     [SerializeField]
     private List<string> keys = new List<string>();
     
     [SerializeField]
     private List<string> values = new List<string>();
     
     // save the dictionary to lists
     public void OnBeforeSerialize()
     {
        keys.Clear();
        values.Clear();
        IDictionaryEnumerator dictEnumerator = this.GetEnumerator();
        while(dictEnumerator.MoveNext()){
            keys.Add(dictEnumerator.Key.ToString());
            values.Add(dictEnumerator.Value.ToString());
        }
     }
     
     // load dictionary from lists
     public void OnAfterDeserialize()
     {
        this.Clear();
 
        if(keys.Count != values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
 
        for(int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
     }
 }
