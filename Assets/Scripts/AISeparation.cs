using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISeparation : MonoBehaviour {

    GameObject[] AI;
    public float SpaceBetween = 1.5f;

	void Start () {
        AI = GameObject.FindGameObjectsWithTag("AI");
	}

	void Update () {
		foreach(GameObject go in AI)
        {
            if(go != gameObject)
            {
                float distance = Vector3.Distance(go.transform.position, this.transform.position);
                if(distance <= SpaceBetween)
                {
                    Vector3 direction = transform.position - go.transform.position;
                    transform.Translate(direction * Time.deltaTime);
                }
            }
        }
	}
}
