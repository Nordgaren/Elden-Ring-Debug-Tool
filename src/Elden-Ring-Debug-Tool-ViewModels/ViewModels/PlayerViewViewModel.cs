using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using Erd_Tools.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Target View")]
    public class PlayerViewViewModel : ViewModelBase
    {
        internal ErdHook Hook { get; private set; }

        public PlayerViewViewModel()
        {

        }
        public void InitViewModel(ErdHook hook)
        {
            Hook = hook;
            PlayerIns = new EnemyViewModel(new Enemy(Hook.PlayerIns, Hook));
        }

        public void UpdateViewModel()
        {
            //if (PlayerIns?.Hp == 0)
            //{
            //    PlayerIns = null;
            //    LockHp = false;
            //    LockFp = false;
            //    LockStam = false;
            //    LockTarget = false;
            //}

            //if (Hook.CurrentTargetHandle != -1 && Hook.CurrentTargetHandle != PlayerIns?.Handle && !LockTarget)
            //{
            //    Enemy enemy = Hook.GetTarget();

            //    if (enemy != null)
            //        PlayerIns = new EnemyViewModel(enemy);
            //}

            PlayerIns.UpdateEnemy();

            if (LockHp)
                PlayerIns.Hp = _lockHpValue;

            if (LockFp)
                PlayerIns.Fp = _lockFpValue;

            if (LockStam)
                PlayerIns.Stamina = _lockStamValue;
        }

        private bool _lockTarget;
        public bool LockTarget
        {
            get => _lockTarget;
            set => SetField(ref _lockTarget, value);
        }

        private EnemyViewModel _playerIns;
        public EnemyViewModel PlayerIns
        {
            get => _playerIns;
            set => SetField(ref _playerIns, value);
        }

        private int _lockHpValue { get; set; }

        private bool _lockHp;
        public bool LockHp
        {
            get => _lockHp;
            set
            {
                if (SetField(ref _lockHp, value))
                {
                    if (value)
                        _lockHpValue = PlayerIns.Hp;
                }
            }
        }
        private int _lockFpValue { get; set; }
        private bool _lockFp;
        public bool LockFp
        {
            get => _lockFp;
            set
            {
                if (SetField(ref _lockFp, value))
                {
                    if (value)
                        _lockFpValue = PlayerIns.Fp;
                }
            }
        }

        private int _lockStamValue { get; set; }
        private bool _lockStam;
        public bool LockStam
        {
            get => _lockStam;
            set
            {
                if (SetField(ref _lockStam, value))
                {
                    if (value)
                        _lockStamValue = PlayerIns.Stamina;
                }
            }
        }
    }
}
