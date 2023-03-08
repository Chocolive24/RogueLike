using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardLocation : MonoBehaviour
{
    private bool _isEmpty = true;
    private bool _isHighlighted = false;

    public bool IsEmpty
    {
        get => _isEmpty;
        set => _isEmpty = value;
    }

    public bool IsHighlighted
    {
        get => _isHighlighted;
        set => _isHighlighted = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        Card card = col.GetComponent<Card>();

        if (card)
        {
            _isEmpty = false;
        }
    }
}
