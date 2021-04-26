using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWater : MonoBehaviour
{
    public bool direction;
    // Update is called once per frame
    void FixedUpdate()
    {
        if(this.transform.position.y >= 0.09f)
        {
            direction = false;
            
        }
        else if (this.transform.position.y <= -0.1f)
        {
            direction = true;
        }
        if (direction)
        {
            transform.Translate(Vector3.up * Time.fixedDeltaTime * 0.1f, Space.World);
        }
        else
        {
            transform.Translate(Vector3.down * Time.fixedDeltaTime * 0.1f, Space.World);
        }
    }
}
