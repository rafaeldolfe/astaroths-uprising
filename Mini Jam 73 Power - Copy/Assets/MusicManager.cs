using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource source;
    public AudioClip regularMusic;
    public AudioClip dangerousMusic;
    public AudioClip finalMusic;

    private GlobalEventManager gem;
    void Awake()
    {
        List<Type> depTypes = ProgramUtils.GetMonoBehavioursOnType(this.GetType());
        List<MonoBehaviour> deps = new List<MonoBehaviour>
            {
                (gem = FindObjectOfType(typeof(GlobalEventManager)) as GlobalEventManager),
            };
        if (deps.Contains(null))
        {
            throw ProgramUtils.DependencyException(deps, depTypes);
        }
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        source.clip = regularMusic;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
