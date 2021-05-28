using Gen3.Data;

namespace Metalogic.UI
{
    /// <summary>
    /// Uses on BindDataSet, if a control does not implement his interface, BindDataSet consider as continue.
    /// Continue = true: all children controls of this control will uses parent's databinding
    /// Continue = false: all children controls of this control will uses this object's databinding
    /// </summary>
    public interface IDataBindingFromParentControl
    {
        bool UseParentBinding { get; }
    }

    /// <summary>
    /// Absruct interface for control that can read write a field in DataSet
    /// </summary>
    public interface IMetadataControl
    {

        string QueryTableName
        {
            get;
            set;
        }

        string QueryFiledName
        {
            get;
            set;
        }


        //
        // Summary:
        //     Gets or sets the editor's value.
        object EditValue { get; set; }
        
        void WriteValueIntoDataModel(DataModel model);

        
        void ReadValueFromDataModel(DataModel model);


        event System.EventHandler EditValueChanged;
        
        
        DataModel Model { get; }

        bool ReadOnly { get; set; }
    }
}
