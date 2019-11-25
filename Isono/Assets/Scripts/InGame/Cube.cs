using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IsonoGame.InGame
{
    public class Cube : MonoBehaviour
    {
        public Vector2 pos => new Vector2(transform.position.x, transform.position.y);
    }
}