using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PetriNetSimulator2
{
	public delegate void SelectionHandler(object sender, SelectionEventArgs sea);
	public delegate void ResizedEventHandler(object sender, ResizedEventArgs rea);
	public delegate void LocationChangedEventHandler(object sender, LocationChangedEventArgs lcea);

	public class NoAvailableLayerException : System.Exception
	{
		public NoAvailableLayerException() : base()
		{

		}
	}

	public class InterfaceNotImplementedException : System.Exception
	{
		public InterfaceNotImplementedException() : base()
		{

		}
		public InterfaceNotImplementedException(string sMessage) : base(sMessage)
		{

		}
	}

	public class CommonPropertiesAttribute : System.Attribute
	{
		public CommonPropertiesAttribute() : base()
		{

		}
	}

	public class ControlPropertiesAttribute : System.Attribute
	{
		public ControlPropertiesAttribute() : base()
		{

		}
	}

	public class TimeInvariantAttribute : System.Attribute
	{
		public TimeInvariantAttribute() : base()
		{

		}
	}

	public class StohasticInputTypeAttribute : System.Attribute
	{
		public StohasticInputTypeAttribute() : base()
		{

		}
	}

	public class PeriodicInputTypeAttribute : System.Attribute
	{
		public PeriodicInputTypeAttribute() : base()
		{

		}
	}

	public class ConnectionPropertiesAttribute : System.Attribute
	{
		public ConnectionPropertiesAttribute() : base()
		{

		}
	}


	public enum PetriNetType
	{
		TimeInvariant,
		PTimed
	};

	public enum PlaceType
	{
		Input,
		Resource,
		Operation,
		Control,
		Output
	};

	public enum LineDirection
	{
		Up,
		Down,
		Left,
		Right,
		None
	};

	public enum ControlOrientation
	{
		Horizontal,
		Vertical
	};

	public enum DocumentView
	{
		Editor,
		Description,
		Response,
		None
	};

	public enum InputType
	{
		Fixed,
		Periodic,
		Stohastic
	};

	public enum UndoRedoAction
	{
		Created,
		Deleted,
		LocationChanged,
		Resized
	};

	public class ClipboardMetafileHelper
	{
		[DllImport("user32.dll")]
		static extern bool OpenClipboard(IntPtr hWndNewOwner);
		[DllImport("user32.dll")]
		static extern bool EmptyClipboard();
		[DllImport("user32.dll")]
		static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
		[DllImport("user32.dll")]
		static extern bool CloseClipboard();
		[DllImport("gdi32.dll")]
		static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
		[DllImport("gdi32.dll")]
		static extern bool DeleteEnhMetaFile(IntPtr hemf);
    
		#region static public bool PutEnhMetafileOnClipboard( IntPtr hWnd, Metafile mf )
		// Metafile mf is set to an invalid state inside this function
		static public bool PutEnhMetafileOnClipboard( IntPtr hWnd, Metafile mf )
		{
			bool bResult = false;
			IntPtr hEMF, hEMF2;
			hEMF = mf.GetHenhmetafile(); // invalidates mf
			if( ! hEMF.Equals( new IntPtr(0) ) )
			{
				hEMF2 = CopyEnhMetaFile( hEMF, new IntPtr(0) );
				if( ! hEMF2.Equals( new IntPtr(0) ) )
				{
					if( OpenClipboard( hWnd ) )
					{
						if( EmptyClipboard() )
						{
							IntPtr hRes = SetClipboardData( 14 /*CF_ENHMETAFILE*/, hEMF2 );
							bResult = hRes.Equals( hEMF2 );
							CloseClipboard();
						}
					}
				}
				DeleteEnhMetaFile( hEMF );
			}
			return bResult;
		}
		#endregion
	}

	public class API
	{
		[DllImport("Shell32.dll")]
		public static extern IntPtr ShellExecute(IntPtr handle, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
	}

	public class TreeViewHelper
	{
		[DllImport("TreeViewHelper.dll", EntryPoint="Initialize")] 
		public static extern void Initialize();

		[DllImport("TreeViewHelper.dll", EntryPoint="EnableHScroller")]
		public static extern void EnableHScroller(IntPtr TreeViewHandle);

		[DllImport("TreeViewHelper.dll", EntryPoint="DisableHScroller")]
        public static extern void DisableHScroller(IntPtr TreeViewHandle);

		[DllImport("TreeViewHelper.dll", EntryPoint="ScrollUp")]
		public static extern void ScrollUp(IntPtr TreeViewHandle);
	
//	    <DllImport("TreeViewHelper.dll", EntryPoint:="ScrollDown")> _
//		Friend Shared Sub ScrollDown(ByVal TreeViewHandle As IntPtr)
//		End Sub
//		<DllImport("TreeViewHelper.dll", EntryPoint:="ScrollLeft")> _
//		Friend Shared Sub ScrollLeft(ByVal TreeViewHandle As IntPtr)
//		End Sub
//		<DllImport("TreeViewHelper.dll", EntryPoint:="ScrollRight")> _
//		Friend Shared Sub ScrollRight(ByVal TreeViewHandle As IntPtr)
//		End Sub
//
//		<DllImport("TreeViewHelper.dll", EntryPoint:="CanScrollUp")> _
//		Friend Shared Function CanScrollUp(ByVal TreeViewHandle As IntPtr) As Integer
//		End Function
//		<DllImport("TreeViewHelper.dll", EntryPoint:="CanScrollDown")> _
//		Friend Shared Function CanScrollDown(ByVal TreeViewHandle As IntPtr) As Integer
//		End Function
//		<DllImport("TreeViewHelper.dll", EntryPoint:="CanScrollLeft")> _
//		Friend Shared Function CanScrollLeft(ByVal TreeViewHandle As IntPtr) As Integer
//		End Function
//		<DllImport("TreeViewHelper.dll", EntryPoint:="CanScrollRight")> _
//		Friend Shared Function CanScrollRight(ByVal TreeViewHandle As IntPtr) As Integer
//		End Function
//
//		<DllImport("TreeViewHelper.dll", EntryPoint:="SetLinesColor")> _
//		Friend Shared Sub SetLinesColor(ByVal TreeViewHandle As IntPtr, ByVal R As Integer, _
//										ByVal G As Integer, ByVal B As Integer)
//		End Sub
//		<DllImport("TreeViewHelper.dll", EntryPoint:="SetInsertMarkColor")> _
//		Friend Shared Sub SetInsertMarkColor(ByVal TreeViewHandle As IntPtr, ByVal R As Integer, _
//											ByVal G As Integer, ByVal B As Integer)
//		End Sub
//
//		<DllImport("TreeViewHelper.dll", EntryPoint:="GetItemAtPt")> _
//		Friend Shared Function GetItemAtPt(ByVal TreeViewHandle As IntPtr, ByVal X As Integer, _
//										ByVal Y As Integer) As IntPtr
//		End Function
//		<DllImport("TreeViewHelper.dll", EntryPoint:="GetItemRect")> _
//		Friend Shared Sub GetItemRect(ByVal TreeViewHandle As IntPtr, ByVal ItemHandle As IntPtr, _
//									ByRef Top As Integer, ByRef Bottom As Integer)
//		End Sub
//
//		<DllImport("TreeViewHelper.dll", EntryPoint:="SetInsertMarkBefore")> _
//		Friend Shared Sub SetInsertMarkBefore(ByVal TreeViewHandle As IntPtr, ByVal ItemHandle As IntPtr)
//		End Sub
//		<DllImport("TreeViewHelper.dll", EntryPoint:="SetInsertMarkAfter")> _
//		Friend Shared Sub SetInsertMarkAfter(ByVal TreeViewHandle As IntPtr, ByVal ItemHandle As IntPtr)
//		End Sub
//		<DllImport("TreeViewHelper.dll", EntryPoint:="RemoveInsertMark")> _
//		Friend Shared Sub RemoveInsertMark(ByVal TreeViewHandle As IntPtr)
//		End Sub

	}
}
