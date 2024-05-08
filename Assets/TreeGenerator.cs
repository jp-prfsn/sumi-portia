using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{

    public Vector2 range;
    public GameObject tree;

    


    // Start is called before the first frame update
    void Start()
    {
        // split the range into a 2d grid and generate a tree in every cell

        for (float x = -range.x; x < range.x; x++)
        {
            for (float y = -range.y; y < range.y; y++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
                Instantiate(tree, transform.position + new Vector3(x, y, 0) + randomOffset, Quaternion.identity, transform);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(range.x * 2, range.y * 2, 0.1f));
    }

    
}
