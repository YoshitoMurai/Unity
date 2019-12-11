using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
   public bool isRotate = true;
  void Update() {
        if(isRotate) transform.Rotate(new Vector3(10,60,120) * Time.deltaTime*0.3f);
  }
}
