using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    public GameObject source;
    public AudioClip toggleSound;
    Light _light;
    private LightShafts _lightShaft;

    // Use this for initialization
    void Start()
    {
        _light = GetComponent<Light>();
        _lightShaft = GetComponent<LightShafts>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _light.enabled = !_light.enabled;
            _lightShaft.enabled = !_lightShaft.enabled;
            //AudioSource.PlayClipAtPoint(toggleSound, transform.position);
        }

        transform.Rotate(source.transform.rotation.eulerAngles - transform.rotation.eulerAngles);
    }
}