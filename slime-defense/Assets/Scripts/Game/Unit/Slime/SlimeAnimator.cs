using System.Collections;

public partial class Slime
{
    public class Animator
    {
        private Slime slime;
        private UnityEngine.Animator animator;

        public Animator(Slime slime)
        {
            this.slime = slime;
            animator = slime.GetComponent<UnityEngine.Animator>();
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
                var ratio = clip.clip.length / slime.maxStats.GetStat(Stats.Key.AttackDelay);
                animator.SetFloat("atkspeed", ratio);
            }
        }
    }
}