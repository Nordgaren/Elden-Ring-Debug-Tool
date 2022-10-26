using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using Erd_Tools.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    [Description("Target View")]
    public class PlayerViewViewModel : ViewModelBase
    {
        internal ErdHook Hook { get; private set; }

        public PlayerViewViewModel() { }

        public void InitViewModel(ErdHook hook)
        {
            Hook = hook;
            if (Hook.PlayerIns.Resolve() != IntPtr.Zero)
                PlayerIns = new EnemyViewModel(new Enemy(Hook.PlayerIns, Hook));
        }

        public void UpdateViewModel()
        {
            if (PlayerIns == null && Hook.PlayerIns.Resolve() != IntPtr.Zero)
                PlayerIns = new EnemyViewModel(new Enemy(Hook.PlayerIns, Hook));

            if (PlayerIns == null) return;

            // if (PlayerIns?.Hp == 0)
            // {
            //     PlayerIns = null;
            //     LockHp = false;
            //     LockFp = false;
            //     LockStam = false;
            //     LockTarget = false;
            // }

            //if (Hook.CurrentTargetHandle != -1 && Hook.CurrentTargetHandle != PlayerIns?.Handle && !LockTarget)
            //{
            //    Enemy enemy = Hook.GetTarget();

            //    if (enemy != null)
            //        PlayerIns = new EnemyViewModel(enemy);
            //}

            Name = Hook.Name;
            PlayerIns.UpdateEnemy();

            if (LockHp) PlayerIns.Hp = _lockHpValue;

            if (LockFp) PlayerIns.Fp = _lockFpValue;

            if (LockStam) PlayerIns.Stamina = _lockStamValue;

            if (LockTeam) PlayerIns.TeamType = _lockTeamValue;

            if (LockChr) PlayerIns.ChrType = _lockChrValue;
        }

        private bool _lockTarget;
        public bool LockTarget { get => _lockTarget; set => SetField(ref _lockTarget, value); }

        private EnemyViewModel _playerIns;
        public EnemyViewModel? PlayerIns { get => _playerIns; set => SetField(ref _playerIns, value); }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (SetField(ref _name, value))
                {
                    Hook.Name = Name;
                }
            }
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
                    if (value) _lockHpValue = PlayerIns.Hp;
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
                    if (value) _lockFpValue = PlayerIns.Fp;
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
                    if (value) _lockStamValue = PlayerIns.Stamina;
                }
            }
        }

        private byte _lockTeamValue { get; set; }
        private bool _lockTeamBool;

        public bool LockTeam
        {
            get => _lockTeamBool;
            set
            {
                if (SetField(ref _lockTeamBool, value))
                {
                    if (value) _lockTeamValue = PlayerIns.TeamType;
                }
            }
        }

        private int _lockChrValue { get; set; }
        private bool _lockChrBool;

        public bool LockChr
        {
            get => _lockChrBool;
            set
            {
                if (SetField(ref _lockChrBool, value))
                {
                    if (value) _lockChrValue = PlayerIns.ChrType;
                }
            }
        }
    }
}