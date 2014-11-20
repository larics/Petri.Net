using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for TextPropertyEditor.
	/// </summary>
	public class TextPropertyEditor : System.Drawing.Design.UITypeEditor
	{
		public TextPropertyEditor()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")] 
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
	
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")]
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{            
			// Attempts to obtain an IWindowsFormsEditorService.
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if(edSvc == null)
				return null;         
        
			// Displays a StringInputDialog Form to get a user-adjustable 
			// string value.
			TextPropertyEditorControl tpec = new TextPropertyEditorControl((string)value, edSvc);
			tpec.BackColor = SystemColors.Control;

			edSvc.DropDownControl(tpec);

			return tpec.Text;
		}
	}
}
