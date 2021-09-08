using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectList : MonoBehaviour

{
    int random = 0;
    int totalObjet = 1;
    public int currentObjetIndex;
    //array pour contenir toute les armes
    public GameObject[] Objets ;
    public GameObject objectContainer;
    public int spawnObjet;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        for (int i = 0; i < spawnObjet && i < Objets.Length; i++)
        {
            
            random = Random.Range(0, Objets.Length - 1);
            Debug.Log(random);
            while (Objets[random].activeSelf)
            {
                random++;
                random %= Objets.Length;
            }
            Objets[random].SetActive(true);
            Objets[random].transform.SetParent(objectContainer.transform, false);
            
        }
        
        



    }

        // Update is called once per frame
        void Update()
    {
       
    }
}
