using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ObjectContainer : MonoBehaviour
{

    [System.Serializable]
    public struct ObjectStruct
    {
        public string name;
        public int index;
        public Sprite sprite;

        public int goodObjectValue;
        public int badObjectValue;
    }

    public ObjectStruct[] objet;


    public ObjectStruct GetObjectWithIndex(int index)
    {
        return objet[index];
    }

    private void OnValidate()
    {
        int objetLength = objet.Length;
        for (int i = 0; i < objetLength; i++)
        {
            objet[i].index = i;
        }
    }
}
