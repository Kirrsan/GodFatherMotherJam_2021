using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectList : MonoBehaviour

{
    int random = 0;
    int totalObjet = 1;
    public int currentObjetIndex;
    //array pour contenir toute les armes
    public GameObject[] Objets;
    
    
    // Start is called before the first frame update
    void Start()
    {

        random = Random.Range(0, Objets.Length - 1);
        Debug.Log(random);
        
        
        Objets[random].SetActive(true);
        

    }

        // Update is called once per frame
        void Update()
    {
       
    }
}
