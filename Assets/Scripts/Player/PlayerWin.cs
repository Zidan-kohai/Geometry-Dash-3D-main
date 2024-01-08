using GD3D.Audio;
using GD3D.ObjectPooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Player
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerWin : PlayerScript
    {
        [Header("Main")]
        [SerializeField] private LayerMask deathLayer;
        private bool _touchingDeath;
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {

            base.Update();

            // Detect if the player is touching deadly stuff
            _touchingDeath = Physics.OverlapBox(transform.position, transform.localScale / 2 + (Vector3.one / 15), transform.rotation, deathLayer).Length >= 1;

            // Die if we are touching death stuff
            if (_touchingDeath)
            {
                Win();
            }

        }

        /// <summary>
        /// Makes the player explode, plays the death sound effect, disables the mesh and respawns the player afterwards.
        /// </summary>
        public void Win()
        {
            // Don't die again if we have already died
            if (player.IsDead)
            {
                return;
            }

            // Spawn death effect
            //PoolObject obj = _pool.SpawnFromPool(transform.position);

            // Remove after a second
            //obj.RemoveAfterTime(1);

            // Play death sound
            SoundManager.PlaySound("Player Explode", 1);

            // Invoke on death event
            player.InvokeWinEvent();
        }
    }
}
