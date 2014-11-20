using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	public class ResizableControl :  UserControl, ISerializable
	{
		#region public bool AllowResizing
		public bool AllowResizing
		{
			get
			{
				return this.bAllowResizing;
			}
			set
			{
				this.bAllowResizing = value;
			}
		}
		#endregion

		#region public new Size DefaultSize
		public new Size DefaultSize
		{
			get
			{
				return this.szDefaultSize;
			}
			set
			{
				this.szDefaultSize = value;
			}
		}
		#endregion

		#region public Hashtable EventHandlers
		public Hashtable EventHandlers
		{
			get
			{
				return this.htEventHandlers;
			}
		}
		#endregion

		internal event ResizedEventHandler Resized;

		protected Size szDefaultSize;
		private Size szBeforeSizing;
		protected bool bResizing = false;
		private bool bAllowResizing = false;
		private Point ptMouseDownOffset;
		private AnchorStyles asResizingBorder = AnchorStyles.None;
		private Hashtable htEventHandlers = new Hashtable();

		private System.ComponentModel.IContainer components = null;

		#region public ResizableControl(Size szDefaultSize)
		public ResizableControl(Size szDefaultSize)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			this.szDefaultSize = szDefaultSize;

			this.MouseMove += new MouseEventHandler(ResizableControl_MouseMove);
			this.MouseDown += new MouseEventHandler(ResizableControl_MouseDown);
			this.MouseUp += new MouseEventHandler(ResizableControl_MouseUp);
		}
		#endregion

		#region protected ResizableControl(SerializationInfo info, StreamingContext context)
		protected ResizableControl(SerializationInfo info, StreamingContext context)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			this.MouseMove += new MouseEventHandler(ResizableControl_MouseMove);
			this.MouseDown += new MouseEventHandler(ResizableControl_MouseDown);
			this.MouseUp += new MouseEventHandler(ResizableControl_MouseUp);

			this.szDefaultSize = (Size)info.GetValue("defaultsize", typeof(Size));
		}
		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion


		#region public void GetObjectData(SerializationInfo info, StreamingContext context)
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("defaultsize", this.DefaultSize);
		}
		#endregion


		#region private void ResizableControl_MouseMove(object sender, MouseEventArgs e)
		private void ResizableControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.bAllowResizing == true)
			{
				#region if (this.Capture != true)
				if (this.Capture != true)
				{
					if ((e.X < 4) && (e.Y >= 4 && e.Y <= (this.Height - 5)))
					{
						this.Cursor = Cursors.SizeWE;
						asResizingBorder = AnchorStyles.Left;
					}
					else if ((e.X > (this.Width - 5)) && (e.Y >= 4 && e.Y <= (this.Height - 5)))
					{
						this.Cursor = Cursors.SizeWE;
						asResizingBorder = AnchorStyles.Right;
					}
					else if ((e.X >= 4 && e.X <= (this.Width - 5)) && (e.Y < 4))
					{
						this.Cursor = Cursors.SizeNS;
						asResizingBorder = AnchorStyles.Top;
					}
					else if ((e.X >= 4 && e.X <= (this.Width - 5)) && (e.Y > (this.Height - 5)))
					{
						this.Cursor = Cursors.SizeNS;
						asResizingBorder = AnchorStyles.Bottom;
					}
					else if (((e.X < 4) && (e.Y < 4)))
					{
						this.Cursor = Cursors.SizeNWSE;
						asResizingBorder = AnchorStyles.Top | AnchorStyles.Left;
					}
					else if (((e.X > (this.Width - 5)) && (e.Y > (this.Height - 5))))
					{
						this.Cursor = Cursors.SizeNWSE;
						asResizingBorder = AnchorStyles.Bottom | AnchorStyles.Right;
					}
					else if ((e.X > (this.Width - 5)) && (e.Y < 4))
					{
						this.Cursor = Cursors.SizeNESW;
						asResizingBorder = AnchorStyles.Top | AnchorStyles.Right;
					}
					else if ((e.X < 4) && (e.Y > (this.Height - 5)))
					{
						this.Cursor = Cursors.SizeNESW;
						asResizingBorder = AnchorStyles.Bottom | AnchorStyles.Left;
					}
					else
					{
						this.Cursor = Cursors.SizeAll;
						asResizingBorder = AnchorStyles.None;
					}
				}
				#endregion
				
				#region else
				else
				{
					if (this.asResizingBorder != AnchorStyles.None)
					{
						if (this.asResizingBorder == AnchorStyles.Left)
						{
							this.Location = new Point(this.Location.X + e.X - this.ptMouseDownOffset.X, this.Location.Y);
							this.Width = this.Width - e.X + this.ptMouseDownOffset.X;
						}
						else if (this.asResizingBorder == AnchorStyles.Right)
						{
							this.Width = e.X + (this.szBeforeSizing.Width - ptMouseDownOffset.X);
						}
						else if (this.asResizingBorder == AnchorStyles.Top)
						{
							this.Location = new Point(this.Location.X, this.Location.Y + e.Y - this.ptMouseDownOffset.Y);
							this.Height = this.Height - e.Y + this.ptMouseDownOffset.Y;
						}
						else if (this.asResizingBorder == AnchorStyles.Bottom)
						{
							this.Height = e.Y + (this.szBeforeSizing.Height - ptMouseDownOffset.Y);
						}
						else if (this.asResizingBorder == (AnchorStyles.Top | AnchorStyles.Left))
						{
							this.Location = new Point(this.Location.X + e.X - this.ptMouseDownOffset.X, this.Location.Y + e.Y - this.ptMouseDownOffset.Y);
							this.Size = new Size(this.Width - e.X + this.ptMouseDownOffset.X, this.Height - e.Y + this.ptMouseDownOffset.Y);
						}
						else if (this.asResizingBorder == (AnchorStyles.Top | AnchorStyles.Right))
						{
							this.Location = new Point(this.Location.X, this.Location.Y + e.Y - this.ptMouseDownOffset.Y);
							this.Size = new Size(e.X + (this.szBeforeSizing.Width - this.ptMouseDownOffset.X), this.Height - e.Y + this.ptMouseDownOffset.Y);
						}
						else if (this.asResizingBorder == (AnchorStyles.Bottom | AnchorStyles.Right))
						{
							this.Size = new Size(e.X + (this.szBeforeSizing.Width - this.ptMouseDownOffset.X), e.Y + (this.szBeforeSizing.Height - ptMouseDownOffset.Y));
						}
						else if (this.asResizingBorder == (AnchorStyles.Bottom | AnchorStyles.Left))
						{
							this.Location = new Point(this.Location.X + e.X - this.ptMouseDownOffset.X, this.Location.Y);
							this.Size = new Size(this.Width - e.X + this.ptMouseDownOffset.X, e.Y + (this.szBeforeSizing.Height - ptMouseDownOffset.Y));
						}

						this.Refresh();
						this.Parent.Refresh();
					}
				}
				#endregion
			}
		}
		#endregion

		#region private void ResizableControl_MouseDown(object sender, MouseEventArgs e)
		private void ResizableControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (this.bAllowResizing == true)
			{
				// Remembers point where mouse was clicked in Place control
				this.ptMouseDownOffset = new Point(e.X, e.Y);

				if (this.Cursor != Cursors.SizeAll)
				{
					this.Capture = true;
					this.bResizing = true;

					this.szBeforeSizing = this.Size;
				}
			}
		}
		#endregion

		#region private void ResizableControl_MouseUp(object sender, MouseEventArgs e)
		private void ResizableControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.bAllowResizing == true)
			{
				if (this.szBeforeSizing != this.Size && this.bResizing == true)
				{
					// Change default size
					if (this.Parent != null && this.Parent is EditorSurface)
					{
						EditorSurface es = (EditorSurface)this.Parent;
						this.DefaultSize = new Size((int)(this.Width / es.Zoom), (int)(this.Height / es.Zoom));
					}

					if (this.Resized != null)
						this.Resized(this, new ResizedEventArgs(this.szBeforeSizing));
				}

				this.Capture = false;
				this.bResizing = false;
			}
		}
		#endregion

	}

	public class ResizedEventArgs
	{
		public Size OldValue
		{
			get
			{
				return this.szOldValue;
			}
		}

		private Size szOldValue;

		public ResizedEventArgs(Size szOldValue)
		{
			this.szOldValue = szOldValue;
		}

		public static ResizedEventArgs Empty = new ResizedEventArgs(new Size(0, 0));
	}

}

