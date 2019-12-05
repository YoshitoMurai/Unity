using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizumo : MonoBehaviour
{
    [SerializeField] public float _strandLength = 0.8f;

    // つながる範囲
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _strandLength);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
