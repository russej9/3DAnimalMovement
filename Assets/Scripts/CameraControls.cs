using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public GameObject animal;
    private float speed = 2f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow)) //uses arrow keys to move camera
        {
            transform.Translate(new Vector3(speed * Time.unscaledDeltaTime, 0, 0), Space.World);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-speed * Time.unscaledDeltaTime, 0, 0), Space.World);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, 0, -speed * Time.unscaledDeltaTime), Space.World);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, 0, speed * Time.unscaledDeltaTime), Space.World);
        }
        if (Input.GetMouseButton(1))    //use right click to change camera
        {
            //transform.Rotate(Input.GetAxis("Mouse X") * speed, Input.GetAxis("Mouse Y") * speed, 0, Space.World);
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"), Space.World);
            transform.Rotate(Vector3.right, Input.GetAxis("Mouse Y"));
        }

        gameObject.transform.Translate(0, Input.GetAxis("Mouse ScrollWheel") * -1.5f, 0, Space.World); //mouse zooming and scaling of animal
        animal.transform.localScale = new Vector3(animal.transform.localScale.x - Input.GetAxis("Mouse ScrollWheel") * 0.03f, animal.transform.localScale.y - Input.GetAxis("Mouse ScrollWheel") * 0.03f, animal.transform.localScale.z - Input.GetAxis("Mouse ScrollWheel") * 0.03f);

    }
}
