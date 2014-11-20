using System;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for MetafileExporter.
	/// </summary>

#if !DEMO
	public class MetafileExporter
	{
		public MetafileExporter()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region public static Metafile SaveMetafile(Stream stream, PetriNetEditor pne)
		public static Metafile SaveMetafile(Stream stream, PetriNetEditor pne)
		{
			float fZoom = pne.Zoom;

			Graphics g = pne.CreateGraphics();
			IntPtr ip = g.GetHdc();
			Metafile mf = new Metafile(stream, ip);
			g.ReleaseHdc(ip);
			g.Dispose();

			Graphics gg = Graphics.FromImage(mf);

			if (PetriNetDocument.AntiAlias == true)
				gg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			//Draw all Connections
			foreach(Connection cn in pne.Connections)
			{
				cn.DrawConnection(gg, pne, Color.Black, Color.Red);
			}

			// Draw all Objects
			foreach(object o in pne.Objects)
			{
				if (o is IMetafileModel)
				{
					IMetafileModel imm = (IMetafileModel)o;
					imm.DrawModel(gg);
				}
			}


			gg.Dispose();

			return mf;
		}
		#endregion

	}
#endif

}
