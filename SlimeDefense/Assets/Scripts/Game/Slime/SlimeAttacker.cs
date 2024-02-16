using System.Linq;
using UnityEngine;

public partial class Slime
{
    public class SlimeAttacker
    {
        private Slime slime;

        public SlimeAttacker(Slime slime)
        {
            this.slime = slime;
        }

        public bool IsEnemyInRange()
        {
            return Physics
                .OverlapCapsule(
                    slime.transform.position + (Vector3.up * 100), 
                    slime.transform.position + (Vector3.down * 100), 
                    slime.stat.GetStat("attack range"), 
                    LayerMask.GetMask("Enemy"))
                .Any();
        }
    }
}