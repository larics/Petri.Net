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
	/// Summary description for ResponsePlacesEditor.
	/// </summary>
	public class ResponsePlacesEditor : System.Drawing.Design.UITypeEditor
	{
		public ResponsePlacesEditor()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")] 
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
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

			ResponseOptions ro = (ResponseOptions)context.Instance;

			ResponsePlacesEditorForm form = new ResponsePlacesEditorForm(ro.Document, (Place[])value);
			if(edSvc.ShowDialog(form) == DialogResult.OK)
				return form.Places;

			// If OK was not pressed, return the original value
			return value;
		}

	}
}
