using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    public Vector3 CardLocPos;

    public bool IsMouseOnEmptyCardLoc = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.layer == _layerMask)
            {
                CardLocation cardLoc = hit.collider.GetComponent<CardLocation>();
                
                CardLocPos = hit.collider.transform.position;

                if (cardLoc.IsEmpty)
                {
                    IsMouseOnEmptyCardLoc = true;
                }
            }
            else
            {
                IsMouseOnEmptyCardLoc = false;
            }
        }
    }
}
