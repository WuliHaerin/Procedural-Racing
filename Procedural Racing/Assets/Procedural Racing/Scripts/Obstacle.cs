using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
	
	//game manager reference
	GameManager manager;
	
	void Start(){
		//find the game manager
		manager = GameObject.FindObjectOfType<GameManager>();
	}

    private void Update()
    {
        if (manager.isInvincible)
        {
            GetComponent<Collider>().enabled = false;
        }
		else
		{
            GetComponent<Collider>().enabled = true;
        }
    }

    void OnCollisionEnter(Collision other){
		//if the player hits this obstacle, end the game
		if (other.gameObject.transform.root.CompareTag("Player") && !manager.isInvincible)
			manager.PreDie();
	}
}
