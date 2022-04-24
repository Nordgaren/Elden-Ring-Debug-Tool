namespace Elden_Ring_Debug_Tool_WPF
{
    interface ICellControl
    {
        public string FieldName { get; set; }
        public string Value { get; }

        public abstract void UpdateField();
    }
}
    