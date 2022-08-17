using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickleAfterImage : MonoBehaviour
{
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
        transform.parent = null;
        StartCoroutine(Expand());
    }

    private IEnumerator Expand()
    {
        float vis = 1;
        
        while(vis > 0)
        {
            transform.localScale += 50 * Time.deltaTime * Vector3.one;
            vis -= Time.deltaTime;
            mat.SetFloat("_Visibility", vis);
            yield return null;
        }
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
