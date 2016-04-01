using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class collision : MonoBehaviour {

		void OnCollisionEnter(Collision other)
	{

		if(other.gameObject.name == "Sphere" || other.gameObject.name=="Sphere1")
		{

			Debug.Log ("collided");
			Invoke( "Load", 5 );




		}

	}
	void Load(){
		SceneManager.LoadScene ("Scene1");
	}
}
