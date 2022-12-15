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
    public class TargetViewViewModel : ViewModelBase
    {
        internal ErdHook Hook { get; private set; }

        public TargetViewViewModel()
        {

        }
        public void InitViewModel(ErdHook hook)
        {
            Hook = hook;
        }

        public void UpdateViewModel()
        {
            if (TargetEnemy?.Hp == 0 || (Hook.CurrentTargetHandle == -1 && !LockTarget))
            {
                TargetEnemy = null;
                LockHp = false;
                LockFp = false;
                LockStam = false;
                LockTarget = false;
            }

            if (Hook.CurrentTargetHandle != -1 && Hook.CurrentTargetHandle != TargetEnemy?.Handle && !(LockTarget && TargetEnemy != null))
            {
                
                Enemy? enemy = Hook.GetTarget();
            
                if (enemy != null)
                    TargetEnemy = new EnemyViewModel(enemy);
            }

            TargetEnemy?.UpdateEnemy();

            if (LockHp && TargetEnemy != null)
                TargetEnemy.Hp = _lockHpValue;

            if (LockFp && TargetEnemy != null)
                TargetEnemy.Fp = _lockFpValue;

            if (LockStam && TargetEnemy != null)
                TargetEnemy.Stamina = _lockStamValue;
        }

        private bool _lockTarget;
        public bool LockTarget
        {
            get => _lockTarget;
            set => SetField(ref _lockTarget, value);
        }

        private EnemyViewModel? _targetEnemy;
        public EnemyViewModel? TargetEnemy
        {
            get => _targetEnemy;
            set => SetField(ref _targetEnemy, value);
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
                        _lockHpValue = TargetEnemy.Hp;
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
                        _lockFpValue = TargetEnemy.Fp;
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
                        _lockStamValue = TargetEnemy.Stamina;
                }
            }
        }
    }
}
