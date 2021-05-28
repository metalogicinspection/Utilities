using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using Metalogic.DataUtil;
using Metaphase.Editors;

namespace Metalogic.UI.Editors
{
    //Extension methods must be defined in a static class 
    public static class ControlExtension
    {
        public static void SetCaption(this IEnovaEdit control)
        {
            var container = control.FindContainer();

            if (container == null || string.IsNullOrEmpty(control.Caption))
            {
                return;
            }

            container.Text = control.Caption;
        }

        public static LayoutControlItem FindContainer(this IEnovaEdit control)
        {
            BaseEdit edit;
            LayoutControl parent = null;
            if ((edit = control as BaseEdit) == null
                || (parent = edit.Parent as LayoutControl) == null
                || parent.Root == null)
            {
                return null;
            }

            var queue = new Queue<LayoutControlGroup>();
            queue.Enqueue(parent.Root);
            while (queue.Any())
            {
                var current = queue.Dequeue();

                LayoutControlItem retValue;
                if ((retValue = current.Items.OfType<LayoutControlItem>()
                                .FirstOrDefault(x => x.Control == edit)) != null)
                {
                    return retValue;
                }

                foreach (var group in current.Items.OfType<LayoutControlGroup>())
                {
                    queue.Enqueue(group);
                }
            }

            return null;
        }

        public static T FindControl<T>(this Control ctrl) where T : Control
        {
            var q = new Queue<Control>();
            q.Enqueue(ctrl);

            while (q.Any())
            {
                var cur = q.Dequeue();

                var curAsT = cur as T;
                if (curAsT != null && cur != ctrl)
                {
                    return curAsT;
                }

                foreach (Control curChild in cur.Controls)
                {
                    q.Enqueue(curChild);
                }
            }
            return null;

        }

        public static Control FindControl(this Control ctrl, string childControlClassFullName)
        {
            var q = new Queue<Control>();
            q.Enqueue(ctrl);

            while (q.Any())
            {
                var cur = q.Dequeue();

                if (cur.GetType().FullName.Equals(childControlClassFullName)
                    && cur != ctrl)
                {
                    q.Clear();
                    return cur;
                }

                foreach (Control curChild in cur.Controls)
                {
                    q.Enqueue(curChild);
                }
            }
            return null;
        }

        public static Control FindControl(this Control ctrl, string childControlClassFullName, string controlName)
        {
            var q = new Queue<Control>();
            q.Enqueue(ctrl);

            while (q.Any())
            {
                var cur = q.Dequeue();

                if (cur.GetType().FullName.Equals(childControlClassFullName)
                    && controlName.ToLower().Equals(cur.Name.ToLower())
                    && cur != ctrl)
                {
                    q.Clear();
                    return cur;
                }

                foreach (Control curChild in cur.Controls)
                {
                    q.Enqueue(curChild);
                }
            }
            return null;
        }

        public static T FindControl<T>(this Control control, string controlName) where T : Control
        {
            var queue = new Queue<Control>();
            var visited = new HashSet<Control>();
            queue.Enqueue(control);
            while (queue.Any())
            {
                var current = queue.Dequeue();

                foreach (var c in from Control c in current.Controls
                                  where !visited.Contains(c)
                                  select c)
                {
                    queue.Enqueue(c);
                }

                T currentAsT;
                if ((currentAsT = current as T) != null
                    && controlName.ToLower().Equals(currentAsT.Name.ToLower()))
                {
                    visited.Clear();
                    queue.Clear();
                    return currentAsT;
                }

                visited.Add(current);
            }

            queue.Clear();
            visited.Clear();
            return null;
        }

        public static IEnovaEdit FindEdit(this Control ctrl, string queryTableName, string queryFieldName)
        {
            var queue = new Queue<Control>();
            var visited = new HashSet<Control>();
            queue.Enqueue(ctrl);
            while (queue.Any())
            {
                var current = queue.Dequeue();

                foreach (var c in from Control c in current.Controls
                                  where !visited.Contains(c)
                                  select c)
                {
                    queue.Enqueue(c);
                }

                IEnovaEdit currentAsEdit;
                if ((currentAsEdit = current as IEnovaEdit) != null
                    && currentAsEdit.QueryTableName.ToLower().Equals(queryTableName.ToLower())
                    && currentAsEdit.QueryFiledName.ToLower().Equals(queryFieldName.ToLower()))
                {
                    visited.Clear();
                    queue.Clear();
                    return currentAsEdit;
                }

                visited.Add(current);
            }
            return null;
        }

        public static List<IEnovaEdit> FindEdits(this Control ctrl, string queryTableName, string queryFieldName)
        {
            var queue = new Queue<Control>();
            var visited = new HashSet<Control>();
            var retValue = new List<IEnovaEdit>();
            queue.Enqueue(ctrl);
            while (queue.Any())
            {
                var current = queue.Dequeue();

                foreach (var c in from Control c in current.Controls
                                  where !visited.Contains(c)
                                  select c)
                {
                    queue.Enqueue(c);
                }

                IEnovaEdit currentAsEdit;
                if ((currentAsEdit = current as IEnovaEdit) != null
                    && currentAsEdit.QueryTableName.ToLower().Equals(queryTableName.ToLower())
                    && currentAsEdit.QueryFiledName.ToLower().Equals(queryFieldName.ToLower()))
                {
                    retValue.Add(currentAsEdit);
                }

                visited.Add(current);
            }

            visited.Clear();
            queue.Clear();
            return retValue;
        }

        // This is the extension method. 
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined. 
        public static void BindDataSet(this Control ctrl, DataSet data, EventHandler editValueEventHandler)
        {
            var allControlsInDetail = ctrl.GetAllChildren<IEnovaEdit>();

            foreach (var unit in allControlsInDetail)
            {
                var table = data.Tables[unit.QueryTableName];
                var col = table?.Columns[unit.QueryFiledName];
                if (col == null)
                {
                    continue;
                }

                unit.DataBindings.Clear();
                unit.DataBindings.Add("EditValue", table, unit.QueryFiledName);
                unit.DataBindings[0].DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

                EnovaPicklistEdit pskEdit;
                if (col.DataType.IsSubclassOf(typeof(PicklistItem)) && 
                    (pskEdit = unit as EnovaPicklistEdit)!= null)
                {
                    pskEdit.PickListType = col.DataType;
                    pskEdit.Properties.ValueMember = null;
                }

                if (editValueEventHandler != null)
                {
                    unit.EditValueChanged -= editValueEventHandler;
                    unit.EditValueChanged += editValueEventHandler;
                }
            }
        }
        

        public static List<Control> GetAllChildren(this Control control)
        {
            var queue = new Queue<Control>();
            var visited = new HashSet<Control>();
            queue.Enqueue(control);
            while (queue.Any())
            {
                var current = queue.Dequeue();
                var currentChildAsFlag = current as IDataBindingFromParentControl;
                if (currentChildAsFlag == null || currentChildAsFlag.UseParentBinding)
                {
                    foreach (var c in from Control c in current.Controls
                                      where !visited.Contains(c)
                                      select c)
                    {
                        queue.Enqueue(c);
                    }
                }
                visited.Add(current);
            }

            queue.Clear();
            visited.Remove(control);
            return visited.ToList();
        }

        public static List<T> GetAllChildren<T>(this Control control) where T : class
        {
            return GetAllChildren(control).OfType<T>().ToList();
        }

        public static void BindDataFromOtherControl(this Control ctrl, Control listenDataChangeFromControl)
        {
            var allControlsInDetail = ctrl.GetAllChildren<IEnovaEdit>();
            var allControlsInOther = listenDataChangeFromControl.GetAllChildren<IEnovaEdit>();

            var pairs = from x in allControlsInDetail
                        join y in allControlsInOther
                            on new { A = x.QueryFiledName.ToLower(), B = x.QueryTableName.ToLower() }
                            equals new { A = y.QueryFiledName.ToLower(), B = y.QueryTableName.ToLower() }
                        select new { X = x, Y = y };

            foreach (var pair in pairs)
            {
                pair.X.EditValue = pair.Y.EditValue;
            }
            allControlsInDetail.Clear();
            allControlsInOther.Clear();
        }
    }
}
