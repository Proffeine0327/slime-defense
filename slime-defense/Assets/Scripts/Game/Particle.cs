using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameScene
{
    public class Particle : MonoBehaviour
    {
        private ParticleSystem p;
        private Poolable poolable;

        private Poolable Poolable
        {
            get
            {
                if (poolable == null)
                    poolable = GetComponent<Poolable>();
                return poolable;
            }
        }

        private void Awake()
        {
            p = GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            p.Play();
            this.Invoke(() => Poolable.Pool(), p.main.duration);
        }
    }
}