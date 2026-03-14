using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Erd_Tools.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Save Face Data")]
    public class SaveFaceDataCommand : CommandBase, IToggleableCommand
    {
        private FaceViewViewModel _faceDataViewModel { get; }

        private bool _state;

        public bool State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

        public SaveFaceDataCommand(FaceViewViewModel debugViewViewModel)
        {
            _faceDataViewModel = debugViewViewModel;
            _faceDataViewModel.PropertyChanged += _faceViewViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _faceDataViewModel.Hook.Setup && _faceDataViewModel.Hook.Loaded && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            // Create a SaveFileDialog instance
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            // Set dialog properties
            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Text File";
            saveFileDialog1.DefaultExt = "json";
            saveFileDialog1.FileName = "face_name"; 

            // Show the dialog and check if the user clicked "OK"
            if (saveFileDialog1.ShowDialog() == false)
            {
                return;
            }

            // Get the selected file path
            string filePath = saveFileDialog1.FileName;
            SaveFaceData(filePath);
        }

        private void SaveFaceData(string path)
        {
            byte[] bytes = _faceDataViewModel.Bytes;

            Dictionary<string, byte> values = new();
            foreach (KeyValuePair<string, int> kvp in FieldOffsets)
            {
                values.Add(kvp.Key, bytes[kvp.Value]);
            }

            string json = JsonSerializer.Serialize(values);
            File.WriteAllText(path, json);
        }

        private static Dictionary<string, int> FieldOffsets = new()
        {
            { "face_partsId", 32 },
            { "skin_color_R", 199 },
            { "skin_color_G", 200 },
            { "skin_color_B", 201 },
            { "skin_gloss", 202 },
            { "skin_pores", 203 },
            { "face_beard", 204 },
            { "face_aroundEye", 205 },
            { "face_aroundEyeColor_R", 206 },
            { "face_aroundEyeColor_G", 207 },
            { "face_aroundEyeColor_B", 208 },
            { "face_cheek", 209 },
            { "face_cheekColor_R", 210 },
            { "face_cheekColor_G", 211 },
            { "face_cheekColor_B", 212 },
            { "face_eyeLine", 213 },
            { "face_eyeLineColor_R", 214 },
            { "face_eyeLineColor_G", 215 },
            { "face_eyeLineColor_B", 216 },
            { "face_eyeShadowDown", 217 },
            { "face_eyeShadowDownColor_R", 218 },
            { "face_eyeShadowDownColor_G", 219 },
            { "face_eyeShadowDownColor_B", 220 },
            { "face_eyeShadowUp", 221 },
            { "face_eyeShadowUpColor_R", 222 },
            { "face_eyeShadowUpColor_G", 223 },
            { "face_eyeShadowUpColor_B", 224 },
            { "face_lip", 225 },
            { "face_lipColor_R", 226 },
            { "face_lipColor_G", 227 },
            { "face_lipColor_B", 228 },
            { "body_hair", 238 },
            { "body_hairColor_R", 239 },
            { "body_hairColor_G", 240 },
            { "body_hairColor_B", 241 },
            { "eye_partsId", 40 },
            { "eyeR_irisColor_R", 242 },
            { "eyeR_irisColor_G", 243 },
            { "eyeR_irisColor_B", 244 },
            { "eyeR_irisScale", 245 },
            { "eyeR_cataract", 246 },
            { "eyeR_cataractColor_R", 247 },
            { "eyeR_cataractColor_G", 248 },
            { "eyeR_cataractColor_B", 249 },
            { "eyeR_scleraColor_R", 250 },
            { "eyeR_scleraColor_G", 251 },
            { "eyeR_scleraColor_B", 252 },
            { "eyeR_irisDistance", 253 },
            { "eyeL_irisColor_R", 254 },
            { "eyeL_irisColor_G", 255 },
            { "eyeL_irisColor_B", 256 },
            { "eyeL_irisScale", 257 },
            { "eyeL_cataract", 258 },
            { "eyeL_cataractColor_R", 259 },
            { "eyeL_cataractColor_G", 260 },
            { "eyeL_cataractColor_B", 261 },
            { "eyeL_scleraColor_R", 262 },
            { "eyeL_scleraColor_G", 263 },
            { "hair_partsId", 36 },
            { "hair_color_R", 22 },
            { "hair_color_B", 23 },
            { "hair_shininess", 20 },
            { "beard_partsId", 48 },
            { "eyebrow_partsId", 44 },
            { "eyebrow_color_B", 13 },
            { "eyelash_partsId", 60 },
            { "accessories_partsId", 52 },
            { "decal_partsId", 56 },
            { "decal_posX", 229 },
            { "decal_posY", 230 },
            { "decal_angle", 231 },
            { "decal_scale", 232 },
            { "decal_color_R", 233 },
            { "decal_color_G", 234 },
            { "decal_color_B", 235 },
            { "decal_gloss", 236 },
            { "decal_mirror", 237 },
            { "chrBodyScaleHead", 192 },
            { "chrBodyScaleBreast", 193 },
            { "chrBodyScaleAbdomen", 194 },
            { "chrBodyScaleRArm", 195 },
            { "chrBodyScaleRLeg", 196 },
            { "chrBodyScaleLArm", 197 },
            { "chrBodyScaleLLeg", 198 },
            { "override_eye_partsId", 29 },
            { "override_eye_irisColor", 18 },
            { "override_eye_cataract", 29 },
            { "override_eye_cataractColor", 18 },
            { "override_eye_scleraColor", 29 },
            { "override_burn_scar", 18 },
            { "pad2", 199 },
            { "age", 64 },
            { "gender", 65 },
            { "caricatureGeometry", 66 },
            { "caricatureTexture", 67 },
            { "faceGeoData00", 68 },
            { "faceGeoData01", 69 },
            { "faceGeoData02", 70 },
            { "faceGeoData03", 71 },
            { "faceGeoData04", 72 },
            { "faceGeoData05", 73 },
            { "faceGeoData06", 74 },
            { "faceGeoData07", 75 },
            { "faceGeoData08", 76 },
            { "faceGeoData09", 77 },
            { "faceGeoData10", 78 },
            { "faceGeoData11", 79 },
            { "faceGeoData12", 80 },
            { "faceGeoData13", 81 },
            { "faceGeoData14", 82 },
            { "faceGeoData15", 83 },
            { "faceGeoData16", 84 },
            { "faceGeoData17", 85 },
            { "faceGeoData18", 86 },
            { "faceGeoData19", 87 },
            { "faceGeoData20", 88 },
            { "faceGeoData21", 89 },
            { "faceGeoData22", 90 },
            { "faceGeoData23", 91 },
            { "faceGeoData24", 92 },
            { "faceGeoData25", 93 },
            { "faceGeoData26", 94 },
            { "faceGeoData27", 95 },
            { "faceGeoData28", 96 },
            { "faceGeoData29", 97 },
            { "faceGeoData30", 98 },
            { "faceGeoData31", 99 },
            { "faceGeoData32", 100 },
            { "faceGeoData33", 101 },
            { "faceGeoData34", 102 },
            { "faceGeoData35", 103 },
            { "faceGeoData36", 104 },
            { "faceGeoData37", 105 },
            { "faceGeoData38", 106 },
            { "faceGeoData39", 107 },
            { "faceGeoData40", 108 },
            { "faceGeoData41", 109 },
            { "faceGeoData42", 110 },
            { "faceGeoData43", 111 },
            { "faceGeoData44", 112 },
            { "faceGeoData45", 113 },
            { "faceGeoData46", 114 },
            { "faceGeoData47", 115 },
            { "faceGeoData48", 116 },
            { "faceGeoData49", 117 },
            { "faceGeoData50", 118 },
            { "faceGeoData51", 119 },
            { "faceGeoData52", 120 },
            { "faceGeoData53", 121 },
            { "faceGeoData54", 122 },
            { "faceGeoData55", 123 },
            { "faceGeoData56", 124 },
            { "faceGeoData57", 125 },
            { "faceGeoData58", 126 },
            { "faceGeoData59", 127 },
            { "faceGeoData60", 128 },
            { "faceTexData00", 129 },
            { "faceTexData01", 130 },
            { "faceTexData02", 131 },
            { "faceTexData03", 132 },
            { "faceTexData04", 133 },
            { "faceTexData05", 134 },
            { "faceTexData06", 135 },
            { "faceTexData07", 136 },
            { "faceTexData08", 137 },
            { "faceTexData09", 138 },
            { "faceTexData10", 139 },
            { "faceTexData11", 140 },
            { "faceTexData12", 141 },
            { "faceTexData13", 142 },
            { "faceTexData14", 143 },
            { "faceTexData15", 144 },
            { "faceTexData16", 145 },
            { "faceTexData17", 146 },
            { "faceTexData18", 147 },
            { "faceTexData19", 148 },
            { "faceTexData20", 149 },
            { "faceTexData21", 150 },
            { "faceTexData22", 151 },
            { "faceTexData23", 152 },
            { "faceTexData24", 153 },
            { "faceTexData25", 154 },
            { "faceTexData26", 155 },
            { "faceTexData27", 156 },
            { "faceTexData28", 157 },
            { "faceTexData29", 158 },
            { "faceTexData30", 159 },
            { "faceTexData31", 160 },
            { "faceTexData32", 161 },
            { "faceTexData33", 162 },
            { "faceTexData34", 163 },
            { "faceTexData35", 164 },
            { "faceGeoAsymData00", 165 },
            { "faceGeoAsymData01", 166 },
            { "faceGeoAsymData02", 167 },
            { "faceGeoAsymData03", 168 },
            { "faceGeoAsymData04", 169 },
            { "faceGeoAsymData05", 170 },
            { "faceGeoAsymData06", 171 },
            { "faceGeoAsymData07", 172 },
            { "faceGeoAsymData08", 173 },
            { "faceGeoAsymData09", 174 },
            { "faceGeoAsymData10", 175 },
            { "faceGeoAsymData11", 176 },
            { "faceGeoAsymData12", 177 },
            { "faceGeoAsymData13", 178 },
            { "faceGeoAsymData14", 179 },
            { "faceGeoAsymData15", 180 },
            { "faceGeoAsymData16", 181 },
            { "faceGeoAsymData17", 182 },
            { "faceGeoAsymData18", 183 },
            { "faceGeoAsymData19", 184 },
            { "faceGeoAsymData20", 185 },
            { "faceGeoAsymData21", 186 },
            { "faceGeoAsymData22", 187 },
            { "faceGeoAsymData23", 188 },
            { "faceGeoAsymData24", 189 },
            { "faceGeoAsymData25", 190 }
        };

        private void _faceViewViewModel_PropertyChanged(object? sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FaceViewViewModel.Setup)
                || e.PropertyName == nameof(FaceViewViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}