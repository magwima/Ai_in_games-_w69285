using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToGoal : MonoBehaviour {

    public Transform Goal;
    public float SpaceBetween = 1.5f;

	void Update () {
        if (Vector3.Distance(Goal.position, transform.position) >= SpaceBetween)
        {
            Vector3 direction = Goal.position - transform.position;
            transform.Translate(direction * Time.deltaTime);
        }
	}
}
