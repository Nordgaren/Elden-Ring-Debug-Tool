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
            return _faceDataViewModel.Setup && base.CanExecute(parameter);
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
            byte[] values = new byte[240];
            Array.Fill(values, (byte)0);

            for (int i = 0; i < bytes.Length; i++)
            {
                if (FieldOffsets.TryGetValue(i, out var index))
                {
                    bytes[index] = bytes[i];
                }
            }

            foreach (KeyValuePair<int, int> pair in FieldOffsets)
            {
                if (pair.Value >= 113)
                {
                    Console.WriteLine($" {{ {pair.Key}, {pair.Value + 2} }},");
                }
                
                Console.WriteLine($" {{ {pair.Key}, {pair.Value} }},");
            }

            string json = JsonSerializer.Serialize(values);
            File.WriteAllText(path, json);
        }

        private static Dictionary<int, int> FieldOffsets = new()
        {
            { 0, 0 },
            { 4, 60 },
            { 8, 35 },
            { 12, 74 },
            { 16, 67 },
            { 20, 85 },
            { 24, 89 },
            { 28, 81 },
            { 32, 113 },
            { 33, 114 },
            { 34, 115 },
            { 35, 116 },
            { 36, 117 },
            { 37, 118 },
            { 38, 119 },
            { 39, 120 },
            { 40, 121 },
            { 41, 122 },
            { 42, 123 },
            { 43, 124 },
            { 44, 125 },
            { 45, 126 },
            { 46, 127 },
            { 47, 128 },
            { 48, 129 },
            { 49, 130 },
            { 50, 131 },
            { 51, 132 },
            { 52, 133 },
            { 53, 134 },
            { 54, 135 },
            { 55, 136 },
            { 56, 137 },
            { 57, 138 },
            { 58, 139 },
            { 59, 140 },
            { 60, 141 },
            { 61, 142 },
            { 62, 143 },
            { 63, 144 },
            { 64, 145 },
            { 65, 146 },
            { 66, 147 },
            { 67, 148 },
            { 68, 149 },
            { 69, 150 },
            { 70, 151 },
            { 71, 152 },
            { 72, 153 },
            { 73, 154 },
            { 74, 155 },
            { 75, 156 },
            { 76, 157 },
            { 77, 158 },
            { 78, 159 },
            { 79, 160 },
            { 80, 161 },
            { 81, 162 },
            { 82, 163 },
            { 83, 164 },
            { 84, 165 },
            { 85, 166 },
            { 86, 167 },
            { 87, 168 },
            { 88, 169 },
            { 89, 170 },
            { 90, 171 },
            { 91, 172 },
            { 92, 173 },
            { 93, 174 },
            { 94, 175 },
            { 95, 176 },
            { 96, 177 },
            { 97, 178 },
            { 98, 179 },
            { 99, 180 },
            { 100, 181 },
            { 101, 182 },
            { 102, 183 },
            { 103, 184 },
            { 104, 185 },
            { 105, 186 },
            { 106, 187 },
            { 107, 188 },
            { 108, 189 },
            { 109, 190 },
            { 110, 191 },
            { 111, 192 },
            { 112, 193 },
            { 113, 194 },
            { 114, 195 },
            { 115, 196 },
            { 116, 197 },
            { 117, 198 },
            { 118, 199 },
            { 119, 200 },
            { 120, 201 },
            { 121, 202 },
            { 122, 203 },
            { 123, 204 },
            { 124, 205 },
            { 125, 206 },
            { 126, 207 },
            { 127, 208 },
            { 128, 209 },
            { 129, 210 },
            { 130, 211 },
            { 131, 212 },
            { 132, 213 },
            { 133, 214 },
            { 134, 215 },
            { 135, 216 },
            { 136, 217 },
            { 137, 218 },
            { 138, 219 },
            { 139, 220 },
            { 140, 221 },
            { 141, 222 },
            { 142, 223 },
            { 143, 224 },
            { 144, 225 },
            { 145, 226 },
            { 146, 227 },
            { 147, 228 },
            { 148, 229 },
            { 149, 230 },
            { 150, 231 },
            { 151, 232 },
            { 152, 233 },
            { 153, 234 },
            { 154, 235 },
            { 155, 236 },
            { 156, 237 },
            { 157, 238 },
            { 158, 239 },
            { 160, 99 },
            { 161, 100 },
            { 162, 101 },
            { 163, 102 },
            { 164, 103 },
            { 165, 104 },
            { 166, 105 },
            { 167, 1 },
            { 168, 2 },
            { 169, 3 },
            { 170, 4 },
            { 171, 5 },
            { 172, 6 },
            { 173, 7 },
            { 174, 8 },
            { 175, 9 },
            { 176, 10 },
            { 177, 11 },
            { 178, 12 },
            { 179, 13 },
            { 180, 14 },
            { 181, 15 },
            { 182, 16 },
            { 183, 17 },
            { 184, 18 },
            { 185, 19 },
            { 186, 20 },
            { 187, 21 },
            { 188, 22 },
            { 189, 23 },
            { 190, 24 },
            { 191, 25 },
            { 192, 26 },
            { 193, 27 },
            { 194, 28 },
            { 195, 29 },
            { 196, 30 },
            { 197, 90 },
            { 198, 91 },
            { 199, 92 },
            { 200, 93 },
            { 201, 94 },
            { 202, 95 },
            { 203, 96 },
            { 204, 97 },
            { 205, 98 },
            { 206, 31 },
            { 207, 32 },
            { 208, 33 },
            { 209, 34 },
            { 210, 36 },
            { 211, 37 },
            { 212, 38 },
            { 213, 39 },
            { 214, 40 },
            { 215, 41 },
            { 216, 42 },
            { 217, 43 },
            { 218, 44 },
            { 219, 45 },
            { 220, 46 },
            { 221, 47 },
            { 222, 48 },
            { 223, 49 },
            { 224, 50 },
            { 225, 51 },
            { 226, 52 },
            { 227, 53 },
            { 228, 54 },
            { 229, 55 },
            { 230, 56 },
            { 231, 57 },
            { 232, 58 },
            { 233, 59 },
            { 234, 61 },
            { 235, 62 },
            { 236, 63 },
            { 237, 64 },
            { 238, 65 },
            { 239, 66 },
            { 240, 68 },
            { 241, 69 },
            { 242, 70 },
            { 243, 71 },
            { 244, 72 },
            { 245, 73 },
            { 246, 75 },
            { 247, 76 },
            { 248, 77 },
            { 249, 78 },
            { 250, 79 },
            { 251, 80 },
            { 252, 82 },
            { 253, 83 },
            { 254, 84 },
            { 255, 86 },
            { 256, 87 },
            { 257, 88 },
            { 258, 106 },
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