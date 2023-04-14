using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEditor.UIElements;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Rigidbody),typeof(LineRenderer))]
public class Draw : MonoBehaviour
{
     Rigidbody rb;
     LineRenderer lr;
    public float TimeforNextRay;
    public List<GameObject> WayPoints;
    private float timer=0;
    private int currentWaypoint = 0;
    private int wayIndex;
    private bool move; //karakter hareketi
    private bool touchStartedOnPlayer; //başlangıç noktasını karaktere vericez
    private float speed = 3f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        wayIndex = 1;
        move = false;
        touchStartedOnPlayer = false;
    }

    public void OnMouseDown()
    {
        lr.enabled = true;
        touchStartedOnPlayer = true;
        lr.positionCount = 1;
        lr.SetPosition(0,transform.position);//karaketerin konumunu başlangıç noktasına
    }

    //sprite
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButton(0) && timer > TimeforNextRay &&
            touchStartedOnPlayer) // her timefornextraya göre çizebilcez
        {
            Vector3 WorldFromMouse =
                Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100));
            Vector3 direction = WorldFromMouse - Camera.main.transform.position*Time.deltaTime*speed;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, direction, out hit, 100f))
            {
                Debug.DrawLine(Camera.main.transform.position, direction, Color.red, 1f);
                GameObject newWayPoint = new GameObject("WayPoint");
                newWayPoint.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                
                if (WayPoints.Count > 0 && Vector3.Distance(WayPoints[WayPoints.Count - 1].transform.position,
                        newWayPoint.transform.position) < 0.3f)
                {
                    Destroy(newWayPoint);
                    return;
                }
                WayPoints.Add(newWayPoint);
                lr.positionCount = wayIndex + 1;
                lr.SetPosition(wayIndex, newWayPoint.transform.position);
                timer = 0;
                wayIndex++;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            touchStartedOnPlayer = false;
            move = true;
        }

        if (move&& WayPoints.Count>0)
        {
            //Debug.Log("curren"+ currentWaypoint);
            //Debug.Log("aa" +WayPoints[currentWaypoint].name);
            transform.LookAt(WayPoints[currentWaypoint].transform);
            rb.MovePosition(WayPoints[currentWaypoint].transform.position);
            if (transform.position == WayPoints[currentWaypoint].transform.position)
                currentWaypoint++;
            
            if (currentWaypoint == WayPoints.Count)
            {
                move = false;
                foreach (var item in WayPoints)
                    Destroy(item);
                
                WayPoints.Clear();
                wayIndex = 1;
                currentWaypoint = 0;

            }
        }
    }
}
