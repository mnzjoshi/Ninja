using UnityEngine;
using System.Collections;

public class Apple : MonoBehaviour
{
    [SerializeField]
    private GameObject splashReference;
    private Vector3 randomPos = new Vector3(Random.Range(-1, 1), Random.Range(0.3f, 0.7f), Random.Range(-6.5f, -7.5f));
	private Vector3 randomPosi;
	private GUIText scoreReference;

    void Start()
    {
		scoreReference = GameObject.Find("Score").GetComponent<GUIText>();
    }
    
    void Update()
    {
        /* Remove fruit if out of view */
        if (gameObject.transform.position.y < -36)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
		randomPosi = new Vector3 ((Random.Range(-1, 1)), (GameObject.Find ("Sphere").transform.position.y), Random.Range (-6.5f, -7.5f));
		if(other.gameObject.name == "Sphere" || other.gameObject.name=="Sphere1")
        {
			Camera.main.GetComponent<AudioSource>().Play();
			Destroy(gameObject);

			Instantiate(splashReference, randomPos , transform.rotation);

			/* Update Score */

			scoreReference.text = (int.Parse(scoreReference.text) + 1).ToString();
        }
    }
}