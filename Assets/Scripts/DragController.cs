using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DragController : MonoBehaviour
{
    bool _isDragActive = false;
    Vector2 _screenPosition;
    Vector3 _worldPosition;
    public Draggrable _lastDragged;

    /// <summary>
    /// Calls before start method
    /// </summary>
    private void Awake()
    {
        DragController[] dragControllers = FindObjectsOfType<DragController>();
        if (dragControllers.Length > 1)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if(_isDragActive)
        {
            if(Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                Drop();
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mouseInput = Input.mousePosition;
            _screenPosition = new Vector2(mouseInput.x,mouseInput.y);
        }
        else if (Input.touchCount > 0)
        {
            _screenPosition = Input.GetTouch(0).position;
        }
        else
        {
            return;
        }

        _worldPosition = Camera.main.ScreenToWorldPoint(_screenPosition);

        if (_isDragActive)
        {
            Drag();
        }
        else
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(_worldPosition, Vector2.zero);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.tag == "Level_1_Image" ||
                        hit.collider.tag == "Level_2_Image" ||
                        hit.collider.tag == "Level_3_Image")
                    {
                        Draggrable draggrable = hit.collider.gameObject.GetComponent<Draggrable>();

                        if (draggrable != null)
                        {
                            _lastDragged = draggrable;
                            InitDrag();
                        }
                    }
                }
            }
        }
    }

    private void InitDrag()
    {
        _lastDragged.LastPosition = _lastDragged.transform.position;
        UpdateDragStatus(true);
    }

    private void Drag()
    {
        _lastDragged.transform.position = new Vector2(_worldPosition.x,_worldPosition.y);
    }

    private void Drop()
    {
        UpdateDragStatus(false);
    }

    private void UpdateDragStatus(bool isDragging)
    {
        _isDragActive = _lastDragged.IsDragging = isDragging;
        _lastDragged.gameObject.layer = isDragging ? Layer.Dragging : Layer.Default;
    }
}
