namespace Gen3.Data
{
    public abstract class DataListChangedItem
    {
        public int Index { get; set; }

        public IGen3DataList NewDataList { get; set; }
        public object NewItem { get; internal set; }

        public IGen3DataList OldDataList { get; set; }
        public object OldItem { get; internal set; }

    }
}