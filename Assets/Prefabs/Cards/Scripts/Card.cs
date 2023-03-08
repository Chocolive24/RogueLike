using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    [SerializeField] protected Board _board;

    [SerializeField] protected GameObject _CardLimit;
    
    protected Vector3 _originalPos;

    protected BoxCollider2D _boxCollider2D;

    protected bool _isPlayed = false;
    protected bool _onMouseDrag = false;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _originalPos = transform.position;
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
       
    }
    
    private void CheckIfPlayed()
    {
        if (transform.position.y - (_boxCollider2D.bounds.size.y / 2) 
            > _CardLimit.transform.position.y && !_isPlayed && !_onMouseDrag)
        {
            Play();
        }
    }

    protected virtual void Play()
    {
        _isPlayed = true;
        StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
        Debug.Log("Card is played");
        
        yield return new WaitForSeconds(1f);
        
        Destroy(gameObject);
    }
    
    private void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
        transform.rotation = Quaternion.identity;
    }

    private void OnMouseUp()
    {
        CheckIfPlayed();
        
        if (_board.IsMouseOnEmptyCardLoc)
        {
            transform.position = _board.CardLocPos;
        }
        else
        {
            if (!_isPlayed)
            {
                transform.position = _originalPos;
            }
        }
    }
}