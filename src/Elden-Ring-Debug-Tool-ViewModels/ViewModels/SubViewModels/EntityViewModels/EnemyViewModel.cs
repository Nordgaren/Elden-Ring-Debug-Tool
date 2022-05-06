using Erd_Tools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class EnemyViewModel : ViewModelBase
    {
        private Enemy _targetEnemy;

        public EnemyViewModel(Enemy targetEnemy)
        {
            _targetEnemy = targetEnemy;

            Pointer = _targetEnemy.TargetEnemyInsPtr;

            Handle = _targetEnemy.Handle;

            HpMax = _targetEnemy.HpMax;
            HpBase = _targetEnemy.HpBase;
            FpMax = _targetEnemy.FpMax;
            FpBase = _targetEnemy.FpBase;
            StaminaMax = _targetEnemy.StaminaMax;
            StaminaBase = _targetEnemy.StaminaBase;
        }
        public void UpdateEnemy()
        {
            _reading = true;
            ChrType = _targetEnemy.ChrType;
            TeamType = _targetEnemy.TeamType;

            Name = _targetEnemy.Name;
            Model = _targetEnemy.Model;

            Hp = _targetEnemy.Hp;
            Fp = _targetEnemy.Fp;
            Stamina = _targetEnemy.Stamina;

            PoisonMax = _targetEnemy.PoisonMax;
            RotMax = _targetEnemy.RotMax;
            BleedMax = _targetEnemy.BleedMax;
            FrostMax = _targetEnemy.FrostMax;
            BlightMax = _targetEnemy.BlightMax;
            SleepMax = _targetEnemy.SleepMax;
            MadnessMax = _targetEnemy.MadnessMax;


            Poison = _targetEnemy.Poison;
            Rot = _targetEnemy.Rot;
            Bleed = _targetEnemy.Bleed;
            Frost = _targetEnemy.Frost;
            Blight = _targetEnemy.Blight;
            Sleep = _targetEnemy.Sleep;
            Madness = _targetEnemy.Madness;
  
            _reading = false;
        }

        bool _reading { get; set; }

        #region EnemyIns
        private string _pointer;
        public string Pointer
        {
            get => _pointer;
            set => SetField(ref _pointer, value);
        }

        private int _handle;
        public int Handle
        {
            get => _handle;
            set => SetField(ref _handle, value);
        }

        private int _chrType;
        public int ChrType
        {
            get => _chrType;
            set
            {
                if (SetField(ref _chrType, value))
                {
                    _targetEnemy.ChrType = value;
                }
            }
        }

        private byte _teamType;
        public byte TeamType
        {
            get => _teamType;
            set
            {
                if (SetField(ref _teamType, value))
                {
                    _targetEnemy.TeamType = value;
                }
            }
        } 
        #endregion

        #region Data
        private string _name;
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private string _model;
        public string Model
        {
            get => _model;
            set => SetField(ref _model, value);
        }

        #region Data
        private int _hp;
        public int Hp
        {
            get => _hp;
            set
            {
                if (SetField(ref _hp, value) && !_reading)
                {
                    _targetEnemy.Hp = value;
                }
            }
        }

        private int _hpMax;
        public int HpMax
        {
            get => _hpMax;
            set => SetField(ref _hpMax, value);
        }

        private int _hpBase;
        public int HpBase
        {
            get => _hpBase;
            set => SetField(ref _hpBase, value);
        }

        private int _fp;
        public int Fp
        {
            get => _fp;
            set
            {
                if (SetField(ref _fp, value) && !_reading)
                {
                    _targetEnemy.Fp = value;
                }
            }
        }

        private int _fpMax;
        public int FpMax
        {
            get => _fpMax;
            set => SetField(ref _fpMax, value);
        }

        private int _fpBase;
        public int FpBase
        {
            get => _fpBase;
            set => SetField(ref _fpBase, value);
        }

        private int _stamina;
        public int Stamina
        {
            get => _stamina;
            set => SetField(ref _stamina, value);
        }

        private int _staminaMax;
        public int StaminaMax
        {
            get => _staminaMax;
            set => SetField(ref _staminaMax, value);
        }
        #endregion

        private int _staminaBase;
        public int StaminaBase
        {
            get => _staminaBase;
            set => SetField(ref _staminaBase, value);
        } 
        #endregion

        #region Resistences

        private int _poison;
        public int Poison
        {
            get => PoisonMax - _poison;
            set => SetField(ref _poison, value);
        }

        private int _poisonMax;
        public int PoisonMax
        {
            get => _poisonMax;
            set => SetField(ref _poisonMax, value);
        }

        private int _rot;
        public int Rot
        {
            get => RotMax - _rot;
            set => SetField(ref _rot, value);
        }

        private int _rotMax;
        public int RotMax
        {
            get => _rotMax;
            set => SetField(ref _rotMax, value);
        }

        private int _bleed;
        public int Bleed
        {
            get => BleedMax - _bleed;
            set => SetField(ref _bleed, value);
        }

        private int _bleedMax;
        public int BleedMax
        {
            get => _bleedMax;
            set => SetField(ref _bleedMax, value);
        }


        private int _frost;
        public int Frost
        {
            get => FrostMax - _frost;
            set => SetField(ref _frost, value);
        }

        private int _frostMax;
        public int FrostMax
        {
            get => _frostMax;
            set => SetField(ref _frostMax, value);
        }

        private int _blight;
        public int Blight
        {
            get => BlightMax - _blight;
            set => SetField(ref _blight, value);
        }

        private int _blightMax;
        public int BlightMax
        {
            get => _blightMax;
            set => SetField(ref _blightMax, value);
        }

        private int _sleep;
        public int Sleep
        {
            get => SleepMax - _sleep;
            set => SetField(ref _sleep, value);
        }

        private int _sleepMax;
        public int SleepMax
        {
            get => _sleepMax;
            set => SetField(ref _sleepMax, value);
        }

        private int _madness;
        public int Madness
        {
            get => MadnessMax - _madness;
            set => SetField(ref _madness, value);
        }

        private int _madnessMax;
        public int MadnessMax
        {
            get => _madnessMax;
            set => SetField(ref _madnessMax, value);
        }


        #endregion




    }
}
