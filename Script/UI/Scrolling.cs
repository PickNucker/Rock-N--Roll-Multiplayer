using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scrolling : MonoBehaviour
{
    [SerializeField] float speed = .5f;
    [SerializeField] float maxDistane = -3000f;
    RectTransform rects;
    Vector2 startPos;

    private void Start()
    {

        startPos = transform.position;
    }

    void Update()
    {
        float newPos = Mathf.Repeat(Time.time * speed, maxDistane);
        transform.position = startPos + Vector2.right * newPos;
    }
}
