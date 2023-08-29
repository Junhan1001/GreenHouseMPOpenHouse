//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TestComponentObstacle : MonoBehaviour
//{
//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.gameObject.layer == 7)
//        {
//            // Disable the path
//            PathFinder.instance.graphData.GetPath(int.Parse(collision.name)).isOpen = false;
//            //collision.GetComponent<SpriteRenderer>().color = Color.red;
//        }
//    }

//    private void OnTriggerStay2D(Collider2D collision)
//    {
//        if (collision.gameObject.layer == 7)
//        {
//            // Disable the path
//            PathFinder.instance.graphData.GetPath(int.Parse(collision.name)).isOpen = false;
//            //collision.GetComponent<SpriteRenderer>().color = Color.red;
//        }
//    }

//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        if (collision.gameObject.layer == 7)
//        {
//            // Enable the path
//            PathFinder.instance.graphData.GetPath(int.Parse(collision.name)).isOpen = true;
//            //collision.GetComponent<SpriteRenderer>().color = Color.blue;
//        }
//    }
//}
