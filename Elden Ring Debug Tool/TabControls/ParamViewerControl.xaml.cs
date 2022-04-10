using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using static SoulsFormats.PARAMDEF;


namespace Elden_Ring_Debug_Tool
{
    /// <summary>
    /// Interaction logic for ParamViewerControl.xaml
    /// </summary>
    public partial class ParamViewerControl : DebugControl
    {
        private static XmlSerializer XML = new XmlSerializer(typeof(string[]));
        public List<ERParam> Params { get; private set; }
        public ParamViewerControl()
        {
            InitializeComponent();
        }

        public void GetParams()
        {
            Params = new List<ERParam>();
            var paramPath = $"{Util.ExeDir}/Resources/Params/";
            
            var pointerPath = $"{paramPath}/Pointers/";
            var paramPointers = Directory.GetFiles(pointerPath);
            foreach (var path in paramPointers)
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(path);
                var defPath = $"{paramPath}/Defs/{name}.xml";
                if (!File.Exists(defPath))
                    continue;

                var offsets = new string[3];
                using (FileStream stream = File.OpenRead(path))
                {
                    offsets = XML.Deserialize(stream) as string[];
                }
                var offsetInt = new int[3];

                for (int i = 0; i < offsets.Length; i++)
                {
                    offsetInt[i] = int.Parse(offsets[i], System.Globalization.NumberStyles.HexNumber);
                }

                var pointer = Hook.GetParamPointer(offsetInt);

                var paramDef = XmlDeserialize(defPath);

                Params.Add(new ERParam(pointer, paramDef, name));
            }
        }

    }
}
