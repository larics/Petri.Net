using System;
using System.ComponentModel;
using System.Collections;
using System.Runtime.Serialization;

namespace PetriNetSimulator2
{
	/// <summary>
	/// Summary description for ReleaseTime.
	/// </summary>
	
	[Serializable]
	[TypeConverter(typeof(ReleaseTimeConverter))]
	public class ReleaseTime : ISerializable
	{
		// Properties
		#region public int Time
		public int Time
		{
			get
			{
				return this.iReleaseTime;
			}
		}
		#endregion

		#region public Place OperationPlace
		public Place OperationPlace
		{
			get
			{
				return this.pOperation;
			}
		}
		#endregion

		// Fields
        private int iReleaseTime = 1;
		private Place pOperation;
		private string sOperation;

		public ReleaseTime(Place pOperation, int iReleaseTime)
		{
			this.pOperation = pOperation;
			this.iReleaseTime = iReleaseTime;
		}

		#region protected ReleaseTime(SerializationInfo info, StreamingContext context)
		protected ReleaseTime(SerializationInfo info, StreamingContext context)
		{
			this.sOperation = info.GetString("operation");
			this.iReleaseTime = info.GetInt32("releasetime");
		}
		#endregion

		#region void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("operation", "P" + this.pOperation.Index);
			info.AddValue("releasetime", this.iReleaseTime);
		}
		#endregion

		public void RestoreReference(Hashtable ht)
		{
			this.pOperation = (Place)ht[this.sOperation];
		}
	}


	public class ReleaseTimeConverter : TypeConverter
	{
		#region public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo (context, destinationType);
		}
		#endregion

		#region public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType == typeof(string) && value is ReleaseTime)
			{
				ReleaseTime rt = (ReleaseTime)value;

				return "P" + rt.OperationPlace.Index + "-" + rt.OperationPlace.NameID + " = " + rt.Time;
			}

			return base.ConvertTo (context, culture, value, destinationType);
		}
		#endregion

		#region public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom (context, sourceType);
		}
		#endregion

		#region public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
//			if(value is string)
//			{
//				string s = (string)value;
//				s.Replace(" ", "");
//				string[] saParts = ((string)value).Split(new char[]{'-','='});
//
//				string sIndex = saParts[0].Replace("P", "");
//				int iIndex = int.Parse(sIndex);
//				int iTime = int.Parse(saParts[2]);
//
//				// Find place
//				Place pFound = null;
//				foreach(Place p in Place.Instances)
//				{
//					if (p.Index == iIndex)
//					{
//						pFound = p;
//						break;
//					}
//				}
//
//				if (pFound != null)
//				{
//					ReleaseTime rt = new ReleaseTime(pFound, iTime);
//					return rt;
//				}
//			}


			return base.ConvertFrom (context, culture, value);
		}
		#endregion

	}

}
