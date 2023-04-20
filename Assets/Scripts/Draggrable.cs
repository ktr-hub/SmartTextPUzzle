using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggrable : MonoBehaviour
{
    public bool IsDragging = false;
    public Vector3 LastPosition;

    private DragController _controller;
    private Collider2D _collider;
    private float _movementTime = 15f;
    private System.Nullable<Vector3> _movementDestination;

    private void Start()
    {
        _controller = FindObjectOfType<DragController>();
        _collider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (_movementDestination.HasValue)
        {
            if (IsDragging)
            {
                _movementDestination = null;
            }
        }

        if(transform.position == _movementDestination)
        {
            gameObject.layer = Layer.Default;
        }
        else if(_movementDestination != null)
        {
            transform.position = Vector3.Lerp(transform.position, _movementDestination.Value, _movementTime* Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Draggrable collidedDraggable = other.GetComponent<Draggrable>();


        //Debug.Log("Entered _controller" + collidedDraggable + " : " + _controller._lastDragged);

        if (collidedDraggable != null && _controller != null
            && _controller._lastDragged != null && _controller._lastDragged.gameObject == gameObject)
        {
            ColliderDistance2D colliderDistance2D = other.Distance(_collider);
            Vector3 diff = new Vector3(colliderDistance2D.normal.x, colliderDistance2D.normal.y) * colliderDistance2D.distance;
            transform.position -= diff;
        }

        if (other.CompareTag("ValidDrop"))
        {
            _movementDestination = other.transform.position;
        }
        else
        {
            _movementDestination = LastPosition;
        }
    }

}
