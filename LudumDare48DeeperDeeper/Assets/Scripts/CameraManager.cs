using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public GameObject cameraLookPoint; //set in inspector
    public AudioSource tileMoveSound;
    public AudioSource tileClickSound;

    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * 10;
            //provinceInfo.FaceCamera();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * Time.deltaTime * 10;
            //provinceInfo.FaceCamera();
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * Time.deltaTime * 10;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.forward * Time.deltaTime * 10;
        }
    }
    
}
