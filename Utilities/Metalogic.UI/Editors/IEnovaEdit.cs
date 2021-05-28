using Metalogic.UI;

namespace Metaphase.Editors
{
    public interface IEnovaEdit : IDataBindingFromParentControl, IMetadataControl
    {
        string Caption
        {
            get;
            set;
        }
        
        System.Windows.Forms.ControlBindingsCollection DataBindings { get; }

    }
}