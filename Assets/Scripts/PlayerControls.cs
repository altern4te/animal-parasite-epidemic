using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private float speed = 10.0f;
    private float horizontalInput;
    private float verticalInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");        
        verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput, Space.World);
        transform.Translate(Vector3.right * Time.deltaTime * speed * verticalInput, Space.World);

        transform.Translate(Vector3.forward * Time.deltaTime * speed * -horizontalInput, Space.World);
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput, Space.World);

        transform.rotation = Quaternion.Euler (new Vector3(0f,-AngleBetweenTwoPoints(Camera.main.WorldToViewportPoint (transform.position), (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition)),0f));
    }

    void OnCollisionEnter(Collision hurdle)
    {
        if(hurdle.collider.tag != "Tile")
        {
            StartCoroutine(colReact());
        }
    }

    IEnumerator colReact()
    {
        speed = -5.0f;
        yield return new WaitForSeconds(0.1f);
        speed = 10.0f;
    }
 
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {    return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;   }
    
}
