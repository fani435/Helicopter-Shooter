using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    public float AscendDescendSpeed;
    public float ForwardBackwardSpeed;
    public float TurnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Ascend/Descend
        if(Input.GetKey(KeyCode.Space))
        {
            Ascend();
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Descend();
        }

        //Forward/Backward
        if (Input.GetKey(KeyCode.W))
        {
            MoveForward();
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveBackward();
        }

        //Rotate
        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }
    }

    #region Movement Functions
    public void Ascend()
    {
        transform.Translate(Vector3.up * AscendDescendSpeed * Time.deltaTime);
    }

    public void Descend()
    {
        transform.Translate(Vector3.down * AscendDescendSpeed * Time.deltaTime);
    }

    public void MoveForward()
    {
        transform.Translate(Vector3.forward * ForwardBackwardSpeed * Time.deltaTime);
    }

    public void MoveBackward()
    {
        transform.Translate(Vector3.back * ForwardBackwardSpeed * Time.deltaTime);
    }

    public void RotateLeft()
    {
        transform.Rotate(Vector3.up, -TurnSpeed * Time.deltaTime);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.up, TurnSpeed * Time.deltaTime);
    }
    #endregion
}
