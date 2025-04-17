using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class AscendVFX : MonoBehaviour
{
    public GameObject player;
    public GameObject ascendSphere;
    public Material ascendSphereMaterial;
    public UniversalRendererData urp;

    public float cutoffHeight;
    private float playerHeight;
    private float dissolve;

    public bool isAscending;

    List<ScriptableRendererFeature> rendererFeatures;

    // Start is called before the first frame update
    void Start()
    {
        rendererFeatures = urp.rendererFeatures;

        rendererFeatures[0].SetActive(false);
        rendererFeatures[1].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        playerHeight = player.transform.position.y;

        if(!isAscending)
        {
            dissolve = 0;
            ascendSphereMaterial.SetFloat("_CutoffHeight", cutoffHeight - playerHeight);
        }
        else
        {
            dissolve = dissolve + Time.deltaTime * 10;
            ascendSphereMaterial.SetFloat("_CutoffHeight", (cutoffHeight - playerHeight) + dissolve);
        }
    }

    public void EnableRendererFeatures()
    {
        rendererFeatures[0].SetActive(true);
        rendererFeatures[1].SetActive(true);
        
        isAscending = true;
    }

    public void DisableRendererFeatures()
    {
        rendererFeatures[0].SetActive(false);
        rendererFeatures[1].SetActive(false);

        isAscending = false;

    }
}
