using System.Collections;
using UnityEngine;

public partial class Slime
{
    public class SlimeAnimator
    {
        private Slime slime;
        private Animator animator;

        public SlimeAnimator(Slime slime)
        {
            this.slime = slime;
            animator = slime.GetComponent<Animator>();
        }

        public void PlayLookAround()
        {
            animator.SetTrigger("lookaround");
        }

        public void PlayAttack(string name)
        {
            slime.StartCoroutine(Routine(name));
            IEnumerator Routine(string name)
            {
                animator.Play(name);
                yield return null;
                var clip = animator.GetCurrentAnimatorClipInfo(0)[0];
                var ratio = clip.clip.length / slime.stat.attackDelay;
                animator.SetFloat("atkspeed", ratio);
            }
        }
    }
}