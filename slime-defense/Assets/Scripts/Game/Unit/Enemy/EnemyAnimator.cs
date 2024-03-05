public partial class Enemy
{
    public class Animator
    {
        private readonly Enemy enemy;

        private UnityEngine.Animator animator;

        public void PlayMove()
        {
            animator.Play("move");
        }

        public void PlayDeath()
        {
            animator.Play("death");
        }

        public Animator(Enemy enemy)
        {
            this.enemy = enemy;
            animator = enemy.GetComponent<UnityEngine.Animator>();
        }
    }
}