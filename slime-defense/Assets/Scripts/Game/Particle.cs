using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    //service
    private ObjectPool objectPool => ServiceProvider.Get<ObjectPool>();

    private ParticleSystem p;
    private Poolable poolable;

    private void Awake()
    {
        p = GetComponent<ParticleSystem>();
        poolable = GetComponent<Poolable>();
    }

    public void Play()
    {
        p.Play();
        this.Invoke(() => poolable.Pool(), p.main.duration);   
    }
}