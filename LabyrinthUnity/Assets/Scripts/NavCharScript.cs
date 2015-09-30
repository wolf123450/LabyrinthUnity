using UnityEngine;
using System.Collections;

public class NavCharScript : MonoBehaviour
{

    NavMeshAgent agent;
    public Transform target;
    public float surfaceOffset = 1.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.name);
                target.transform.position = hit.point;// + hit.normal * surfaceOffset;
                agent.SetDestination(hit.point);
                
                
            }
        }
    }
}
