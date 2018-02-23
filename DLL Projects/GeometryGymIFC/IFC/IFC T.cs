// MIT License
// Copyright (c) 2016 Geometry Gym Pty Ltd

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
// LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GeometryGym.STEP;

namespace GeometryGym.Ifc
{
	public partial class IfcTable : BaseClassIfc, IfcMetricValueSelect, IfcObjectReferenceSelect
	{
		internal string mName = "$"; //:	OPTIONAL IfcLabel;
		private List<int> mRows = new List<int>();// OPTIONAL LIST [1:?] OF IfcTableRow;
		private List<int> mColumns = new List<int>();// :	OPTIONAL LIST [1:?] OF IfcTableColumn;

		public override string Name { get { return (mName == "$" ? "" : ParserIfc.Decode(mName)); } set { mName = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } } 
		public ReadOnlyCollection<IfcTableRow> Rows { get { return new ReadOnlyCollection<IfcTableRow>( mRows.ConvertAll(x => mDatabase[x] as IfcTableRow)); } }
		public ReadOnlyCollection<IfcTableColumn> Columns { get { return new ReadOnlyCollection<IfcTableColumn>( mColumns.ConvertAll(x => mDatabase[x] as IfcTableColumn)); } }

		internal IfcTable() : base() { }
		public IfcTable(DatabaseIfc db) : base(db) { }
		internal IfcTable(DatabaseIfc db, IfcTable t) : base(db) { mName = t.mName; t.Rows.ToList().ForEach(x=>addRow( db.Factory.Duplicate(t) as IfcTableRow)); t.Columns.ToList().ForEach(x=>addColumn( db.Factory.Duplicate(x) as IfcTableColumn)); }
		public IfcTable(string name, List<IfcTableRow> rows, List<IfcTableColumn> cols) : base(rows == null || rows.Count == 0 ? cols[0].mDatabase : rows[0].mDatabase)
		{
			Name = name.Replace("'", "");
			rows.ForEach(x=>addRow(x));
			cols.ForEach(x=>addColumn(x));
		}
		internal static void parseFields(IfcTable t, List<string> arrFields, ref int ipos) { t.mName = arrFields[ipos++]; t.mRows = ParserSTEP.SplitListLinks(arrFields[ipos++]); t.mColumns = ParserSTEP.SplitListLinks(arrFields[ipos++]); }
		protected override string BuildStringSTEP()
		{
			string s = "";
			if (mRows.Count == 0)
				s = "$";
			else
			{
				s = "(" + ParserSTEP.LinkToString(mRows[0]);
				for (int icounter = 1; icounter < mRows.Count; icounter++)
					s += "," + ParserSTEP.LinkToString(mRows[icounter]);
				s += ")";
			}
			if (mDatabase.mRelease != ReleaseVersion.IFC2x3)
			{
				if (mColumns.Count == 0)
					s += ",$";
				else
				{
					s += ",(" + ParserSTEP.LinkToString(mColumns[0]);
					for (int icounter = 1; icounter < mColumns.Count; icounter++)
						s += "," + ParserSTEP.LinkToString(mColumns[icounter]);
					s += ")";
				}
			}
			return base.BuildStringSTEP() + (mName == "$" ? ",$," : ",'" + mName + "',") + s;
		}
		internal static IfcTable Parse(string strDef) { IfcTable t = new IfcTable(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		internal void addRow(IfcTableRow row) { mRows.Add(row.mIndex);  }
		internal void addColumn(IfcTableColumn column) { mColumns.Add(column.mIndex); }
	}
	public partial class IfcTableColumn : BaseClassIfc
	{
		internal string mIdentifier = "$";//	 :	OPTIONAL IfcIdentifier;
		internal string mName = "$";//	 :	OPTIONAL IfcLabel;
		internal string mDescription = "$";//	 :	OPTIONAL IfcText;
		internal int mUnit;//	 :	OPTIONAL IfcUnit;
		private int mReferencePath;//	 :	OPTIONAL IfcReference;

		public string Identifier { get { return (mIdentifier == "$" ? "" : ParserIfc.Decode(mIdentifier)); } set { mIdentifier = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public override string Name { get { return (mName == "$" ? "" : ParserIfc.Decode(mName)); } set { mName = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public string Description { get { return (mDescription == "$" ? "" : ParserIfc.Decode(mDescription)); } set { mDescription = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public IfcUnit Unit { get { return mDatabase[mUnit] as IfcUnit; } set { mUnit = (value == null ? 0 : value.Index); } }
		public IfcReference ReferencePath { get { return mDatabase[mReferencePath] as IfcReference; } set { mReferencePath = (value == null ? 0 : value.mIndex); } }

		internal IfcTableColumn() : base() { }
		public IfcTableColumn(DatabaseIfc db) : base(db) { }
		internal IfcTableColumn(DatabaseIfc db, IfcTableColumn c) : base(db,c) { mIdentifier = c.mIdentifier; mName = c.mName; mDescription = c.mDescription; if(c.mUnit >0) Unit = db.Factory.Duplicate(c.mDatabase[ c.mUnit]) as IfcUnit; if(c.mReferencePath > 0) ReferencePath = db.Factory.Duplicate(c.ReferencePath) as IfcReference; }
		 
		internal static void parseFields(IfcTableColumn t, List<string> arrFields, ref int ipos)
		{
			t.mIdentifier = arrFields[ipos++];
			t.mName = arrFields[ipos++].Replace("'", "");
			t.mDescription = arrFields[ipos++];
			t.mUnit = ParserSTEP.ParseLink(arrFields[ipos++]);
			t.mReferencePath = ParserSTEP.ParseLink(arrFields[ipos++]);
		}
		protected override string BuildStringSTEP() { return (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : base.BuildStringSTEP() + (mIdentifier == "$" ? ",$," : ",'" + mIdentifier + "',") + (mName == "$" ? "$," : "'" + mName + "',") + (mDescription == "$" ? "$," : "'" + mDescription + "',") + ParserSTEP.LinkToString(mUnit) + "," + ParserSTEP.LinkToString(mReferencePath)); }
		internal static IfcTableColumn Parse(string strDef) { IfcTableColumn t = new IfcTableColumn(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
	}
	public partial class IfcTableRow : BaseClassIfc
	{
		internal List<IfcValue> mRowCells = new List<IfcValue>();// :	OPTIONAL LIST [1:?] OF IfcValue;
		internal bool mIsHeading = false; //:	:	OPTIONAL BOOLEAN;

		public ReadOnlyCollection<IfcValue> RowCells { get { return new ReadOnlyCollection<IfcValue>( mRowCells); } }
		public bool IsHeading { get { return mIsHeading; } set { mIsHeading = value; } }

		internal IfcTableRow() : base() { }
		internal IfcTableRow(DatabaseIfc db, IfcTableRow r) : base(db,r) { mRowCells = r.mRowCells; mIsHeading = r.mIsHeading; }
		public IfcTableRow(DatabaseIfc db, IfcValue val) : this(db, new List<IfcValue>() { val }, false) { }
		public IfcTableRow(DatabaseIfc db, List<IfcValue> vals, bool isHeading) : base(db)
		{
			mRowCells.AddRange(vals);
			mIsHeading = isHeading;
		}
		internal static void parseFields(IfcTableRow t, List<string> arrFields, ref int ipos)
		{
			string s = arrFields[ipos++];
			if (s != "$")
			{
				List<string> ss = ParserSTEP.SplitLineFields(s.Substring(1, s.Length - 2));
				for (int icounter = 0; icounter < ss.Count; icounter++)
				{
					IfcValue v = ParserIfc.parseValue(ss[icounter]);
					if (v != null)
						t.mRowCells.Add(v);
				}
			}
			t.mIsHeading = ParserSTEP.ParseBool(arrFields[ipos++]);
		}
		protected override string BuildStringSTEP()
		{
			string s = "";
			if (mRowCells.Count == 0)
				s = ",$,";
			else
			{
				s = ",(" + mRowCells[0].ToString();
				for (int icounter = 1; icounter < mRowCells.Count; icounter++)
					s += "," + mRowCells[icounter].ToString();
				s += "),";
			}
			return base.BuildStringSTEP() + s + ParserSTEP.BoolToString(mIsHeading);
		}
		internal static IfcTableRow Parse(string strDef) { IfcTableRow t = new IfcTableRow(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
	}
	public partial class IfcTank : IfcFlowStorageDevice //IFC4
	{
		internal IfcTankTypeEnum mPredefinedType = IfcTankTypeEnum.NOTDEFINED;// OPTIONAL : IfcTankTypeEnum;
		public IfcTankTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcTank() : base() { }
		internal IfcTank(DatabaseIfc db, IfcTank t) : base(db,t) { mPredefinedType = t.mPredefinedType; }
		public IfcTank(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcTank s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcTankTypeEnum)Enum.Parse(typeof(IfcTankTypeEnum), str);
		}
		internal new static IfcTank Parse(string strDef) { IfcTank s = new IfcTank(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcTankTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcTankType : IfcFlowStorageDeviceType
	{
		internal IfcTankTypeEnum mPredefinedType = IfcTankTypeEnum.NOTDEFINED;// : IfcDuctFittingTypeEnum; 
		public IfcTankTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcTankType() : base() { }
		internal IfcTankType(DatabaseIfc db, IfcTankType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal static void parseFields(IfcTankType t, List<string> arrFields, ref int ipos) { IfcFlowStorageDeviceType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcTankTypeEnum)Enum.Parse(typeof(IfcTankTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcTankType Parse(string strDef) { IfcTankType t = new IfcTankType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	public partial class IfcTask : IfcProcess //SUPERTYPE OF (ONEOF(IfcMove,IfcOrderAction) both depreceated IFC4) 
	{
		//internal string mTaskId; //  : 	IfcIdentifier; IFC4 midentification
		private string mStatus = "$";// : OPTIONAL IfcLabel;
		internal string mWorkMethod = "$";// : OPTIONAL IfcLabel;
		internal bool mIsMilestone;// : BOOLEAN
		internal int mPriority;// : OPTIONAL INTEGER IFC4
		internal int mTaskTime;// : OPTIONAL IfcTaskTime; IFC4
		internal IfcTaskTypeEnum mPredefinedType = IfcTaskTypeEnum.NOTDEFINED;// : OPTIONAL IfcTaskTypeEnum

		internal string Status { get { return mStatus; } }
		internal IfcTaskTime TaskTime { get { return mDatabase[mTaskTime] as IfcTaskTime; } set { mTaskTime = value == null ? 0 : value.mIndex; } }

		internal IfcTask() : base() { }
		internal IfcTask(DatabaseIfc db, IfcTask t) : base(db,t) { mStatus = t.mStatus; mWorkMethod = t.mWorkMethod; mIsMilestone = t.mIsMilestone; mPriority = t.mPriority; if(t.mTaskTime > 0) TaskTime = db.Factory.Duplicate(t.TaskTime) as IfcTaskTime; mPredefinedType = t.mPredefinedType; }
		
		internal static IfcTask Parse(string strDef, ReleaseVersion schema) { IfcTask t = new IfcTask(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos, schema); return t; }
		internal static void parseFields(IfcTask t, List<string> arrFields, ref int ipos, ReleaseVersion schema)
		{
			IfcProcess.parseFields(t, arrFields, ref ipos,schema);
			if (schema == ReleaseVersion.IFC2x3)
				t.mIdentification = arrFields[ipos++];
			t.mStatus = arrFields[ipos++];
			t.mWorkMethod = arrFields[ipos++];
			t.mIsMilestone = ParserSTEP.ParseBool(arrFields[ipos++]);
			t.mPriority = ParserSTEP.ParseInt(arrFields[ipos++]);
			if (schema != ReleaseVersion.IFC2x3)
			{
				t.mTaskTime = ParserSTEP.ParseLink(arrFields[ipos++]);
				string s = arrFields[ipos++];
				if (s.StartsWith("."))
					t.mPredefinedType = (IfcTaskTypeEnum)Enum.Parse(typeof(IfcTaskTypeEnum), s.Replace(".", ""));
			}
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? ",'" + mIdentification + "'" : "") + "," + mStatus + "," + mWorkMethod + "," + ParserSTEP.BoolToString(mIsMilestone) + "," + mPriority.ToString() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : "," + ParserSTEP.LinkToString(mTaskTime) + ",." + mPredefinedType.ToString() + "."); }
	}
	public partial class IfcTaskTime : IfcSchedulingTime //IFC4
	{
		internal IfcTaskDurationEnum mDurationType = IfcTaskDurationEnum.NOTDEFINED;	// :	OPTIONAL IfcTaskDurationEnum;
		internal string mScheduleDuration = "$";//	 :	OPTIONAL IfcDuration;
		internal DateTime mScheduleStart = DateTime.MinValue, mScheduleFinish = DateTime.MinValue, mEarlyStart = DateTime.MinValue, mEarlyFinish = DateTime.MinValue, mLateStart = DateTime.MinValue, mLateFinish = DateTime.MinValue; //:	OPTIONAL IfcDateTime;
		internal string mFreeFloat = "$", mTotalFloat = "$";//	 :	OPTIONAL IfcDuration;
		internal bool mIsCritical;//	 :	OPTIONAL BOOLEAN;
		internal DateTime mStatusTime = DateTime.MinValue;//	 :	OPTIONAL IfcDateTime;
		internal string mActualDuration = "$";//	 :	OPTIONAL IfcDuration;
		internal DateTime mActualStart = DateTime.MinValue, mActualFinish = DateTime.MinValue;//	 :	OPTIONAL IfcDateTime;
		internal string mRemainingTime = "$";//	 :	OPTIONAL IfcDuration;
		internal double mCompletion = double.NaN;//	 :	OPTIONAL IfcPositiveRatioMeasure; 

		public IfcTaskDurationEnum DurationType { get { return mDurationType; } set { mDurationType = value; } }
		public IfcDuration ScheduleDuration { get { return IfcDuration.Convert(mScheduleDuration); } set { mScheduleDuration = IfcDuration.Convert(value); } }
		public DateTime ScheduleStart { get { return mScheduleStart; } set { mScheduleStart = value; } }
		public DateTime ScheduleFinish { get { return mScheduleFinish; } set { mScheduleFinish = value; } }
		public DateTime EarlyStart { get { return mEarlyStart; } set { mEarlyStart = value; } }
		public DateTime EarlyFinish { get { return mEarlyFinish; } set { mEarlyFinish = value; } }
		public DateTime LateStart { get { return mLateStart; } set { mLateStart = value; } }
		public DateTime LateFinish { get { return mLateFinish; } set { mLateFinish = value; } }
		public IfcDuration FreeFloat { get { return IfcDuration.Convert(mFreeFloat); } set { mFreeFloat = IfcDuration.Convert(value); } }
		public IfcDuration TotalFloat { get { return IfcDuration.Convert(mTotalFloat); } set { mTotalFloat = IfcDuration.Convert(value); } }
		public bool IsCritical { get { return mIsCritical; } set { mIsCritical = value; } }
		public DateTime StatusTime { get { return mStatusTime; } set { mStatusTime = value; } }
		public IfcDuration ActualDuration { get { return IfcDuration.Convert(mActualDuration); } set { mActualDuration = IfcDuration.Convert(value); } }
		public DateTime ActualStart { get { return mActualStart; } set { mActualStart = value; } }
		public DateTime ActualFinish { get { return mActualFinish; } set { mActualFinish = value; } }
		public IfcDuration RemainingTime { get { return IfcDuration.Convert(mRemainingTime); } set { mRemainingTime = IfcDuration.Convert(value); } }
		public double Completion { get { return mCompletion; } set { mCompletion = value; } }

		internal IfcTaskTime() : base() { }
		internal IfcTaskTime(DatabaseIfc db, IfcTaskTime t) : base(db,t)
		{
			mDurationType = t.mDurationType; mScheduleDuration = t.mScheduleDuration; mScheduleStart = t.mScheduleStart; mScheduleFinish = t.mScheduleFinish;
			mEarlyStart = t.mEarlyStart; mEarlyFinish = t.mEarlyFinish; mLateStart = t.mLateStart; mLateFinish = t.mLateFinish; mFreeFloat = t.mFreeFloat; mTotalFloat = t.mTotalFloat;
			mIsCritical = t.mIsCritical; mStatusTime = t.mStatusTime; mActualDuration = t.mActualDuration; mActualStart = t.mActualStart; mActualFinish = t.mActualFinish;
			mRemainingTime = t.mRemainingTime; mCompletion = t.mCompletion;
		}
		internal IfcTaskTime(DatabaseIfc db) : base(db) { }
		
		internal static IfcTaskTime Parse(string strDef) { IfcTaskTime s = new IfcTaskTime(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		internal static void parseFields(IfcTaskTime s, List<string> arrFields, ref int ipos)
		{
			IfcSchedulingTime.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str.StartsWith("."))
				s.mDurationType = (IfcTaskDurationEnum)Enum.Parse(typeof(IfcTaskDurationEnum), str.Replace(".", ""));
			s.mScheduleDuration = arrFields[ipos++].Replace("'", "");
			s.mScheduleStart = IfcDateTime.parseSTEP(arrFields[ipos++]);
			s.mScheduleFinish = IfcDateTime.parseSTEP(arrFields[ipos++]);
			s.mEarlyStart = IfcDateTime.parseSTEP(arrFields[ipos++]);
			s.mEarlyFinish = IfcDateTime.parseSTEP(arrFields[ipos++]);
			s.mLateStart = IfcDateTime.parseSTEP(arrFields[ipos++]);
			s.mLateFinish = IfcDateTime.parseSTEP(arrFields[ipos++]);
			s.mFreeFloat = arrFields[ipos++].Replace("'", "");
			s.mTotalFloat = arrFields[ipos++].Replace("'", "");
			s.mIsCritical = ParserSTEP.ParseBool(arrFields[ipos++]);
			s.mStatusTime = IfcDateTime.parseSTEP(arrFields[ipos++]);
			s.mActualDuration = arrFields[ipos++].Replace("'", "");
			s.mActualStart = IfcDateTime.parseSTEP(arrFields[ipos++]);
			s.mActualFinish = IfcDateTime.parseSTEP(arrFields[ipos++]);
			s.mRemainingTime = arrFields[ipos++].Replace("'", "");
			s.mCompletion = ParserSTEP.ParseDouble(arrFields[ipos++]);
		}
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + ",." + mDurationType + (mScheduleDuration == "$" ? ".,$," : ".,'" + mScheduleDuration + "',") + IfcDateTime.formatSTEP(mScheduleStart) + "," +
				IfcDateTime.formatSTEP(mScheduleFinish) + "," + IfcDateTime.formatSTEP(mEarlyStart) + "," + IfcDateTime.formatSTEP(mEarlyFinish) + "," + IfcDateTime.formatSTEP(mLateStart) + "," +
				IfcDateTime.formatSTEP(mLateFinish) + (mFreeFloat == "$" ? ",$," : ",'" + mFreeFloat + "',") + (mTotalFloat == "$" ? "$," : "'" + mTotalFloat + "',") + ParserSTEP.BoolToString(mIsCritical) + "," +
				IfcDateTime.formatSTEP(mStatusTime) + "," + (mActualDuration == "$" ? "$," : "'" + mActualDuration + "',") + IfcDateTime.formatSTEP(mActualStart) + "," + IfcDateTime.formatSTEP(mActualFinish) + "," +
				(mRemainingTime == "$" ? "$," : "'" + mRemainingTime + "',") + ParserSTEP.DoubleOptionalToString(mCompletion);
		}
	}
	public partial class IfcTaskType : IfcTypeProcess //IFC4
	{
		internal IfcTaskTypeEnum mPredefinedType = IfcTaskTypeEnum.NOTDEFINED;// : IfcTaskTypeEnum; 
		private string mWorkMethod = "$";// : OPTIONAL IfcLabel;

		public IfcTaskTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }
		public string WorkMethod { get { return (mWorkMethod == "$" ? "" : ParserIfc.Decode(mWorkMethod)); } set { mWorkMethod = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }

		internal IfcTaskType() : base() { }
		internal IfcTaskType(DatabaseIfc db, IfcTaskType t) : base(db, t) { mPredefinedType = t.mPredefinedType; mWorkMethod = t.mWorkMethod; }
		internal IfcTaskType(DatabaseIfc m, string name, IfcTaskTypeEnum t) : base(m) { Name = name; mPredefinedType = t; }
		internal static void parseFields(IfcTaskType t, List<string> arrFields, ref int ipos) { IfcTypeProcess.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcTaskTypeEnum)Enum.Parse(typeof(IfcTaskTypeEnum), arrFields[ipos++].Replace(".", "")); t.mWorkMethod = arrFields[ipos++].Replace("'", ""); }
		internal new static IfcTaskType Parse(string strDef) { IfcTaskType t = new IfcTaskType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + (mWorkMethod == "$" ? ".,$" : (".,'" + mWorkMethod + "'"))); }
	}
	public partial class IfcTelecomAddress : IfcAddress
	{
		internal List<string> mTelephoneNumbers = new List<string>();// : OPTIONAL LIST [1:?] OF IfcLabel;
		internal List<string> mFacsimileNumbers = new List<string>();// : OPTIONAL LIST [1:?] OF IfcLabel;
		internal string mPagerNumber = "$";// :OPTIONAL IfcLabel;
		internal List<string> mElectronicMailAddresses = new List<string>();// : OPTIONAL LIST [1:?] OF IfcLabel;
		internal string mWWWHomePageURL = "$";// : OPTIONAL IfcLabel;
		internal List<string> mMessagingIDs = new List<string>();// : OPTIONAL LIST [1:?] OF IfcURIReference //IFC4

		public ReadOnlyCollection<string> TelephoneNumbers { get { return new ReadOnlyCollection<string>( mTelephoneNumbers.ConvertAll(x=>ParserIfc.Decode(x))); } }
		public ReadOnlyCollection<string> FacsimileNumbers { get { return new ReadOnlyCollection<string>( mFacsimileNumbers.ConvertAll(x=>ParserIfc.Decode(x))); } }
		public string PagerNumber { get { return ParserIfc.Decode(mPagerNumber); } set { mPagerNumber = (value == null ? "$" : ParserIfc.Encode(value)); } }
		public ReadOnlyCollection<string> ElectronicMailAddresses { get { return new ReadOnlyCollection<string>( mElectronicMailAddresses.ConvertAll(x=>ParserIfc.Decode(x))); } }
		public string WWWHomePageURL { get { return ParserIfc.Decode(mWWWHomePageURL); } set { mWWWHomePageURL = (value == null ? "$" : ParserIfc.Encode(value)); } }
		public ReadOnlyCollection<string> MessagingIDs { get { return new ReadOnlyCollection<string>( mMessagingIDs.ConvertAll(x=>ParserIfc.Decode(x))); } }

		internal IfcTelecomAddress() : base() { }
		public IfcTelecomAddress(DatabaseIfc db) : base(db) { }
		internal IfcTelecomAddress(DatabaseIfc db, IfcTelecomAddress a) : base(db, a) { mTelephoneNumbers = new List<string>(a.mTelephoneNumbers.ToArray()); mFacsimileNumbers = new List<string>(a.mFacsimileNumbers.ToArray()); mPagerNumber = a.mPagerNumber; mElectronicMailAddresses = new List<string>(a.mElectronicMailAddresses.ToArray()); mWWWHomePageURL = a.mWWWHomePageURL; mMessagingIDs.AddRange(a.mMessagingIDs); }
		internal static void parseFields(IfcTelecomAddress a, List<string> arrFields, ref int ipos,ReleaseVersion schema)
		{
			IfcAddress.parseFields(a, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str != "$")
			{
				List<string> lst = ParserSTEP.SplitLineFields(str.Substring(1, str.Length - 2));
				for (int icounter = 0; icounter < lst.Count; icounter++)
					a.mTelephoneNumbers.Add(lst[icounter]);
			}
			str = arrFields[ipos++];
			if (str != "$")
			{
				List<string> lst = ParserSTEP.SplitLineFields(str.Substring(1, str.Length - 2));
				for (int icounter = 0; icounter < lst.Count; icounter++)
					a.mFacsimileNumbers.Add(lst[icounter]);
			}
			a.mPagerNumber = arrFields[ipos++];
			str = arrFields[ipos++];
			if (str != "$")
			{
				List<string> lst = ParserSTEP.SplitLineFields(str.Substring(1, str.Length - 2));
				for (int icounter = 0; icounter < lst.Count; icounter++)
					a.mElectronicMailAddresses.Add(lst[icounter]);
			}
			a.mWWWHomePageURL = arrFields[ipos++];
			if (schema != ReleaseVersion.IFC2x3)
			{
				str = arrFields[ipos++];
				if (!str.StartsWith("$"))
					a.mMessagingIDs = ParserSTEP.SplitListStrings(str.Substring(1, str.Length - 2));
			}
		}
		protected override string BuildStringSTEP()
		{
			string str = base.BuildStringSTEP();
			if (mTelephoneNumbers.Count == 0)
				str += ",$,";
			else
			{
				str += ",(" + mTelephoneNumbers[0];
				for (int icounter = 1; icounter < mTelephoneNumbers.Count; icounter++)
					str += "," + mTelephoneNumbers[icounter];
				str += "),";
			}
			if (mFacsimileNumbers.Count == 0)
				str += "$,";
			else
			{
				str += "(" + mFacsimileNumbers[0];
				for (int icounter = 1; icounter < mFacsimileNumbers.Count; icounter++)
					str += "," + mFacsimileNumbers[icounter];
				str += "),";
			}

			str += mPagerNumber;
			if (mElectronicMailAddresses.Count == 0)
				str += ",$,";
			else
			{
				str += ",(" + mElectronicMailAddresses[0];
				for (int icounter = 1; icounter < mElectronicMailAddresses.Count; icounter++)
					str += "," + mElectronicMailAddresses[icounter];
				str += "),";
			}
			str += mWWWHomePageURL;
			if (mDatabase.mRelease != ReleaseVersion.IFC2x3)
			{
				if (mMessagingIDs.Count == 0)
					str += ",$";
				else
				{
					str += ",('" + mMessagingIDs[0];
					for (int icounter = 1; icounter < mMessagingIDs.Count; icounter++)
						str += "','" + mMessagingIDs[icounter];
					str += "')";
				}
			}
			return str;
		}
		internal static IfcTelecomAddress Parse(string strDef,ReleaseVersion schema) { IfcTelecomAddress a = new IfcTelecomAddress(); int ipos = 0; parseFields(a, ParserSTEP.SplitLineFields(strDef), ref ipos,schema); return a; }

		public void AddTelephoneNumber(string number) { if(!string.IsNullOrEmpty(number)) mTelephoneNumbers.Add(ParserIfc.Encode(number)); }
		public void AddFacsimileNumber(string number) { if(!string.IsNullOrEmpty(number)) mFacsimileNumbers.Add(ParserIfc.Encode(number)); }
		public void AddElectronicMailAddress(string address) { if(!string.IsNullOrEmpty(address))  mElectronicMailAddresses.Add(ParserIfc.Encode(address)); }
		public void AddMessagingID(string id) { if(!string.IsNullOrEmpty(id)) mMessagingIDs.Add(ParserIfc.Encode(id)); }
	}
	public partial class IfcTendon : IfcReinforcingElement
	{
		internal IfcTendonTypeEnum mPredefinedType;// : IfcTendonTypeEnum;//
		internal double mNominalDiameter;// : IfcPositiveLengthMeasure;
		internal double mCrossSectionArea;// : IfcAreaMeasure;
		internal double mTensionForce;// : OPTIONAL IfcForceMeasure;
		internal double mPreStress;// : OPTIONAL IfcPressureMeasure;
		internal double mFrictionCoefficient;// //: OPTIONAL IfcNormalisedRatioMeasure;
		internal double mAnchorageSlip;// : OPTIONAL IfcPositiveLengthMeasure;
		internal double mMinCurvatureRadius;// : OPTIONAL IfcPositiveLengthMeasure; 
		public IfcTendonTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }
		internal IfcTendon() : base() { }
		internal IfcTendon(DatabaseIfc db, IfcTendon t) : base(db, t)
		{
			mPredefinedType = t.mPredefinedType;
			mNominalDiameter = t.mNominalDiameter;
			mCrossSectionArea = t.mCrossSectionArea;
			mTensionForce = t.mTensionForce;
			mPreStress = t.mPreStress;
			mFrictionCoefficient = t.mFrictionCoefficient;
			mAnchorageSlip = t.mAnchorageSlip;
			mMinCurvatureRadius = t.mMinCurvatureRadius;
		}
		public IfcTendon(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, double diam, double area, double forceMeasure, double pretress, double fricCoeff, double anchorSlip, double minCurveRadius)
			: base(host, placement,representation)
		{
			mNominalDiameter = diam;
			mCrossSectionArea = area;
			mTensionForce = forceMeasure;
			mPreStress = pretress;
			mFrictionCoefficient = fricCoeff;
			mAnchorageSlip = anchorSlip;
			mMinCurvatureRadius = minCurveRadius;
		}
		internal static IfcTendon Parse(string strDef) { IfcTendon t = new IfcTendon(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		internal static void parseFields(IfcTendon c, List<string> arrFields, ref int ipos)
		{
			IfcReinforcingElement.parseFields(c, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				c.mPredefinedType = (IfcTendonTypeEnum)Enum.Parse(typeof(IfcTendonTypeEnum), str.Replace(".", ""));
			c.mNominalDiameter = ParserSTEP.ParseDouble(arrFields[ipos++]);
			c.mCrossSectionArea = ParserSTEP.ParseDouble(arrFields[ipos++]);
			c.mTensionForce = ParserSTEP.ParseDouble(arrFields[ipos++]);
			c.mPreStress = ParserSTEP.ParseDouble(arrFields[ipos++]);
			c.mFrictionCoefficient = ParserSTEP.ParseDouble(arrFields[ipos++]);
			c.mAnchorageSlip = ParserSTEP.ParseDouble(arrFields[ipos++]);
			c.mMinCurvatureRadius = ParserSTEP.ParseDouble(arrFields[ipos++]);

		}
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease != ReleaseVersion.IFC2x3 && mPredefinedType == IfcTendonTypeEnum.NOTDEFINED ? ",$," : ",." + mPredefinedType.ToString() + ".,") + ParserSTEP.DoubleToString(mNominalDiameter) + "," +
				ParserSTEP.DoubleToString(mCrossSectionArea) + "," + ParserSTEP.DoubleToString(mTensionForce) + "," +
				ParserSTEP.DoubleToString(mPreStress) + "," + ParserSTEP.DoubleToString(mFrictionCoefficient) + "," +
				ParserSTEP.DoubleToString(mAnchorageSlip) + "," + ParserSTEP.DoubleToString(mMinCurvatureRadius);
		}
	}
	public partial class IfcTendonAnchor : IfcReinforcingElement
	{
		internal IfcTendonAnchorTypeEnum mPredefinedType = IfcTendonAnchorTypeEnum.NOTDEFINED;// :	OPTIONAL IfcTendonAnchorTypeEnum;
		public IfcTendonAnchorTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcTendonAnchor() : base() { }
		internal IfcTendonAnchor(DatabaseIfc db, IfcTendonAnchor a) : base(db, a) { mPredefinedType = a.mPredefinedType; }
		public IfcTendonAnchor(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation) : base(host, placement, representation) { }
		internal static IfcTendonAnchor Parse(string strDef) { IfcTendonAnchor t = new IfcTendonAnchor(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		internal static void parseFields(IfcTendonAnchor a, List<string> arrFields, ref int ipos)
		{
			IfcReinforcingElement.parseFields(a, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				a.mPredefinedType = (IfcTendonAnchorTypeEnum)Enum.Parse(typeof(IfcTendonAnchorTypeEnum), str.Replace(".", ""));
		}
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcTendonAnchorTypeEnum.NOTDEFINED ? ",$," : ",." + mPredefinedType.ToString() + "."));
		}
	}
	//IfcTendonAnchorType
	public partial class IfcTendonType : IfcReinforcingElementType  //IFC4
	{
		internal IfcTendonTypeEnum mPredefinedType = IfcTendonTypeEnum.NOTDEFINED;// : IfcTendonType; //IFC4
		private double mNominalDiameter;// : IfcPositiveLengthMeasure; 	IFC4 OPTIONAL
		internal double mCrossSectionArea;// : IfcAreaMeasure; IFC4 OPTIONAL
		internal double mSheathDiameter;// : OPTIONAL IfcPositiveLengthMeasure;

		public IfcTendonTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }
		public double NominalDiameter { get { return mNominalDiameter; } set { mNominalDiameter = value; } }

		internal IfcTendonType() : base() { }
		internal IfcTendonType(DatabaseIfc db, IfcTendonType t) : base(db, t)
		{
			mPredefinedType = t.mPredefinedType;
			mNominalDiameter = t.mNominalDiameter;
			mCrossSectionArea = t.mCrossSectionArea;
			mSheathDiameter = t.mSheathDiameter;
		}

		public IfcTendonType(DatabaseIfc m, string name, IfcTendonTypeEnum type, double diameter, double area, double sheathDiameter)
			: base(m)
		{
			Name = name;
			mPredefinedType = type;
			mNominalDiameter = diameter;
			mCrossSectionArea = area;
			mSheathDiameter = sheathDiameter;
		}
		internal new static IfcTendonType Parse(string strDef) { int ipos = 0; IfcTendonType t = new IfcTendonType(); parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		internal static void parseFields(IfcTendonType t, List<string> arrFields, ref int ipos)
		{
			IfcReinforcingElementType.parseFields(t, arrFields, ref ipos);
			t.mPredefinedType = (IfcTendonTypeEnum)Enum.Parse(typeof(IfcTendonTypeEnum), arrFields[ipos++].Replace(".", ""));
			t.mNominalDiameter = ParserSTEP.ParseDouble(arrFields[ipos++]);
			t.mCrossSectionArea = ParserSTEP.ParseDouble(arrFields[ipos++]);
			t.mSheathDiameter = ParserSTEP.ParseDouble(arrFields[ipos++]);
		}
		protected override string BuildStringSTEP()
		{
			string result = base.BuildStringSTEP();
			result += ",." + mPredefinedType + ".," + ParserSTEP.DoubleOptionalToString(mNominalDiameter) + ",";
			result += ParserSTEP.DoubleOptionalToString(mCrossSectionArea) + "," + ParserSTEP.DoubleOptionalToString(mSheathDiameter);
			return result;
		}
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcTerminatorSymbol : IfcAnnotationSymbolOccurrence // DEPRECEATED IFC4
	{
		internal int mAnnotatedCurve;// : IfcAnnotationCurveOccurrence; 
		internal IfcTerminatorSymbol() : base() { }
		//internal IfcTerminatorSymbol(IfcTerminatorSymbol i) : base(i) { mAnnotatedCurve = i.mAnnotatedCurve; }
		internal new static IfcTerminatorSymbol Parse(string str) { IfcTerminatorSymbol s = new IfcTerminatorSymbol(); int pos = 0; s.Parse(str,ref pos, str.Length); return s; }
		protected override void Parse(string str, ref int pos, int len)
		{
			base.Parse(str, ref pos, len);
			mAnnotatedCurve = ParserSTEP.StripLink(str, ref pos, len);
		}
	}
	public abstract partial class IfcTessellatedFaceSet : IfcTessellatedItem, IfcBooleanOperand //ABSTRACT SUPERTYPE OF(IfcTriangulatedFaceSet, IfcPolygonalFaceSet )
	{
		internal int mCoordinates;// : 	IfcCartesianPointList;
		
		// INVERSE
		internal IfcIndexedColourMap mHasColours = null;// : SET [0:1] OF IfcIndexedColourMap FOR MappedTo;
		internal List<IfcIndexedTextureMap> mHasTextures = new List<IfcIndexedTextureMap>();// : SET [0:?] OF IfcIndexedTextureMap FOR MappedTo;

		public IfcCartesianPointList Coordinates { get { return mDatabase[mCoordinates] as IfcCartesianPointList; } set { mCoordinates = value.mIndex; } }
		public IfcIndexedColourMap HasColours { get { return mHasColours; } }
		public ReadOnlyCollection<IfcIndexedTextureMap> HasTextures { get { return new ReadOnlyCollection<IfcIndexedTextureMap>( mHasTextures); } }

		protected IfcTessellatedFaceSet() : base() { }
		protected IfcTessellatedFaceSet(DatabaseIfc db, IfcTessellatedFaceSet s) : base(db,s) { Coordinates = db.Factory.Duplicate( s.Coordinates) as IfcCartesianPointList; }
		protected IfcTessellatedFaceSet(IfcCartesianPointList3D pl) : base(pl.mDatabase) { mCoordinates = pl.mIndex; }
		protected override string BuildStringSTEP() { return  base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mCoordinates); }
		protected virtual void Parse(string str, ref int pos, int len)
		{
			mCoordinates = ParserSTEP.StripLink(str, ref pos, len);
		}
	}
	public abstract partial class IfcTessellatedItem : IfcGeometricRepresentationItem //IFC4
	{
		protected IfcTessellatedItem() : base() { }
		protected IfcTessellatedItem(DatabaseIfc db, IfcTessellatedItem i) : base(db,i) { }
		protected IfcTessellatedItem(DatabaseIfc db) : base(db) { }
	}
	public partial class IfcTextLiteral : IfcGeometricRepresentationItem //SUPERTYPE OF	(IfcTextLiteralWithExtent)
	{
		internal string mLiteral;// : IfcPresentableText;
		internal int mPlacement;// : IfcAxis2Placement;
		internal IfcTextPath mPath;// : IfcTextPath;
		 
		public string Description { get { return ParserIfc.Decode(mLiteral); } set { mLiteral = ParserIfc.Encode(value); } }
		public IfcAxis2Placement Placement { get { return mDatabase[mPlacement] as IfcAxis2Placement; } }
		public IfcTextPath Path { get { return mPath; } set { mPath = value; } }

		internal IfcTextLiteral() : base() { }
		internal IfcTextLiteral(DatabaseIfc db, IfcTextLiteral l) : base(db,l) { mLiteral = l.mLiteral; mPlacement = db.Factory.Duplicate(l.mDatabase[l.mPlacement]).mIndex; mPath = l.mPath; }
		internal static IfcTextLiteral Parse(string str) { IfcTextLiteral l = new IfcTextLiteral(); int pos = 0; l.Parse(str, ref pos, str.Length); return l; }
		protected virtual void Parse(string str, ref int pos, int len)
		{ 
			mLiteral = ParserSTEP.StripField(str, ref pos, len);
			mPlacement = ParserSTEP.StripLink(str, ref pos, len);
			mPath = (IfcTextPath)Enum.Parse(typeof(IfcTextPath), ParserSTEP.StripField(str, ref pos, len).Replace(".", ""));
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",'" + mLiteral + "'," + ParserSTEP.LinkToString(mPlacement) + ",." + mPath.ToString() + "."; }
	}
	public partial class IfcTextLiteralWithExtent : IfcTextLiteral
	{
		internal int mExtent;// : IfcPlanarExtent;
		internal string mBoxAlignment;// : IfcBoxAlignment; 
		internal IfcTextLiteralWithExtent() : base() { }
		//internal IfcTextLiteralWithExtent(IfcTextLiteralWithExtent o) : base(o) { mExtent = o.mExtent; mBoxAlignment = o.mBoxAlignment; }

		internal new static IfcTextLiteralWithExtent Parse(string str) { IfcTextLiteralWithExtent l = new IfcTextLiteralWithExtent(); int pos = 0; l.Parse(str,ref pos, str.Length); return l; }
		protected override void Parse(string str, ref int pos, int len)
		{
			base.Parse(str, ref pos, len);
			mExtent = ParserSTEP.StripLink(str, ref pos, len);
			mBoxAlignment = ParserSTEP.StripString(str,ref pos, len);
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mExtent) + ",'" + mBoxAlignment + "'"; }
	}
	public partial class IfcTextStyle : IfcPresentationStyle, IfcPresentationStyleSelect
	{
		internal int mTextCharacterAppearance;// : OPTIONAL IfcCharacterStyleSelect;
		internal int mTextStyle;// : OPTIONAL IfcTextStyleSelect;
		internal int mTextFontStyle;// : IfcTextFontSelect; 
		internal bool mModelOrDraughting = true;//	:	OPTIONAL BOOLEAN; IFC4CHANGE
		internal IfcTextStyle() : base() { }
	//	internal IfcTextStyle(IfcTextStyle v) : base(v) { mTextCharacterAppearance = v.mTextCharacterAppearance; mTextStyle = v.mTextStyle; mTextFontStyle = v.mTextFontStyle; mModelOrDraughting = v.mModelOrDraughting; }
		internal static void parseFields(IfcTextStyle s, List<string> arrFields, ref int ipos,ReleaseVersion schema)
		{
			IfcPresentationStyle.parseFields(s, arrFields, ref ipos);
			s.mTextCharacterAppearance = ParserSTEP.ParseLink(arrFields[ipos++]);
			s.mTextStyle = ParserSTEP.ParseLink(arrFields[ipos++]);
			s.mTextFontStyle = ParserSTEP.ParseLink(arrFields[ipos++]);
			if (schema != ReleaseVersion.IFC2x3)
				s.mModelOrDraughting = ParserSTEP.ParseBool(arrFields[ipos++]);
		}
		internal static IfcTextStyle Parse(string strDef,ReleaseVersion schema) { IfcTextStyle s = new IfcTextStyle(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos,schema); return s; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mTextCharacterAppearance) + "," + ParserSTEP.LinkToString(mTextStyle) + "," + ParserSTEP.LinkToString(mTextFontStyle) + (mDatabase.mRelease != ReleaseVersion.IFC2x3 ? "," + ParserSTEP.BoolToString(mModelOrDraughting) : ""); }
	}
	public partial class IfcTextStyleFontModel : IfcPreDefinedTextFont
	{
		internal List<string> mFontFamily = new List<string>(1);// : OPTIONAL LIST [1:?] OF IfcTextFontName;
		internal string mFontStyle = "$";// : OPTIONAL IfcFontStyle; ['normal','italic','oblique'];
		internal string mFontVariant = "$";// : OPTIONAL IfcFontVariant; ['normal','small-caps'];
		internal string mFontWeight = "$";// : OPTIONAL IfcFontWeight; // ['normal','small-caps','100','200','300','400','500','600','700','800','900'];
		internal string mFontSize;// : IfcSizeSelect; IfcSizeSelect = SELECT (IfcRatioMeasure ,IfcLengthMeasure ,IfcDescriptiveMeasure ,IfcPositiveLengthMeasure ,IfcNormalisedRatioMeasure ,IfcPositiveRatioMeasure);
		internal IfcTextStyleFontModel() : base() { }
		internal IfcTextStyleFontModel(DatabaseIfc db, IfcTextStyleFontModel m) : base(db,m)
		{
	//		mFontFamily = new List<string>(i.mFontFamily.ToArray());
			mFontStyle = m.mFontStyle;
			mFontVariant = m.mFontVariant;
			mFontWeight = m.mFontWeight;
			mFontSize = m.mFontSize;
		}
		internal static IfcTextStyleFontModel Parse(string strDef) { IfcTextStyleFontModel f = new IfcTextStyleFontModel(); int ipos = 0; parseFields(f, ParserSTEP.SplitLineFields(strDef), ref ipos); return f; }
		internal static void parseFields(IfcTextStyleFontModel f, List<string> arrFields, ref int ipos)
		{
			IfcPreDefinedTextFont.parseFields(f, arrFields, ref ipos);
			string s = arrFields[ipos++];
			if (s != "$")
			{
				List<string> lst = ParserSTEP.SplitLineFields(s.Substring(1, s.Length - 2));
				for (int icounter = 0; icounter < lst.Count; icounter++)
					f.mFontFamily.Add(lst[icounter]);
			}
			f.mFontStyle = arrFields[ipos++];
			f.mFontVariant = arrFields[ipos++];
			f.mFontWeight = arrFields[ipos++];
			f.mFontSize = arrFields[ipos++];
		}
		protected override string BuildStringSTEP()
		{
			string str = base.BuildStringSTEP();
			if (mFontFamily.Count > 0)
			{
				str += ",(" + mFontFamily[0];
				for (int icounter = 1; icounter < mFontFamily.Count; icounter++)
					str += "," + mFontFamily[icounter];
				str += "),";
			}
			else
				str += ",$,";
			return str + mFontStyle + "," + mFontVariant + "," + mFontWeight + "," + mFontSize;
		}
	}
	public partial class IfcTextStyleForDefinedFont : BaseClassIfc
	{
		internal int mColour;// : IfcColour;
		internal int mBackgroundColour;// : OPTIONAL IfcColour;
		internal IfcTextStyleForDefinedFont() : base() { }
	//	internal IfcTextStyleForDefinedFont(IfcTextStyleForDefinedFont o) : base() { mColour = o.mColour; mBackgroundColour = o.mBackgroundColour; }
		internal static void parseFields(IfcTextStyleForDefinedFont f, List<string> arrFields, ref int ipos) { f.mColour = ParserSTEP.ParseLink(arrFields[ipos++]); f.mBackgroundColour = ParserSTEP.ParseLink(arrFields[ipos++]); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mColour) + "," + ParserSTEP.LinkToString(mBackgroundColour); }
		internal static IfcTextStyleForDefinedFont Parse(string strDef) { IfcTextStyleForDefinedFont f = new IfcTextStyleForDefinedFont(); int ipos = 0; parseFields(f, ParserSTEP.SplitLineFields(strDef), ref ipos); return f; }
	}
	public partial class IfcTextStyleTextModel : IfcPresentationItem
	{
		//internal int mDiffuseTransmissionColour, mDiffuseReflectionColour, mTransmissionColour, mReflectanceColour;//	 :	IfcColourRgb;
		internal IfcTextStyleTextModel() : base() { }
		internal IfcTextStyleTextModel(DatabaseIfc db, IfcTextStyleTextModel m) : base(db,m) { }
	 
		protected override void parseFields(List<string> arrFields, ref int ipos)
		{
			base.parseFields(arrFields, ref ipos);
			//s.mDiffuseTransmissionColour = IFCModel.mSTP.parseSTPLink(arrFields[ipos++]);
			//s.mDiffuseReflectionColour = IFCModel.mSTP.parseSTPLink(arrFields[ipos++]);
			//s.mTransmissionColour = IFCModel.mSTP.parseSTPLink(arrFields[ipos++]);
			//s.mReflectanceColour = IFCModel.mSTP.parseSTPLink(arrFields[ipos++]);
		}
		internal static IfcTextStyleTextModel Parse(string strDef) { IfcTextStyleTextModel s = new IfcTextStyleTextModel(); int ipos = 0; s.parseFields(ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		//protected override string BuildString() { return (mModel.mOutputEssential ? "" : base.BuildString() + "," + IFCModel.mSTP.STPLinkToString(mDiffuseTransmissionColour) + "," + IFCModel.mSTP.STPLinkToString(mDiffuseReflectionColour) + "," + IFCModel.mSTP.STPLinkToString(mTransmissionColour) + "," + IFCModel.mSTP.STPLinkToString(mReflectanceColour)); }
	}
	//[Obsolete("DEPRECEATED IFC4", false)]
	//ENTITY IfcTextStyleWithBoxCharacteristics; // DEPRECEATED IFC4
	public abstract partial class IfcTextureCoordinate : IfcPresentationItem  //ABSTRACT SUPERTYPE OF(ONEOF(IfcIndexedTextureMap, IfcTextureCoordinateGenerator, IfcTextureMap))
	{
		internal List<int> mMaps = new List<int>();// : LIST [1:?] OF IfcSurfaceTexture
		public ReadOnlyCollection<IfcSurfaceTexture> Maps { get { return new ReadOnlyCollection<IfcSurfaceTexture>( mMaps.ConvertAll(x => mDatabase[x] as IfcSurfaceTexture)); } }

		internal IfcTextureCoordinate() : base() { }
		internal IfcTextureCoordinate(DatabaseIfc db, IfcTextureCoordinate c) : base(db, c) { c.Maps.ToList().ForEach(x=>addMap( db.Factory.Duplicate(x) as IfcSurfaceTexture)); }
		public IfcTextureCoordinate(DatabaseIfc m, List<IfcSurfaceTexture> maps) : base(m) { mMaps = maps.ConvertAll(x => x.mIndex); }

		protected override void parseFields(List<string> arrFields, ref int pos)
		{
			mMaps = ParserSTEP.SplitListLinks(arrFields[pos++]);
		}
		protected override string BuildStringSTEP()
		{
			string result = base.BuildStringSTEP() + ",(#" + mMaps[0];
			for (int icounter = 1; icounter < mMaps.Count; icounter++)
				result += ",#" + mMaps[icounter];
			return result + ")";
		}
		internal void addMap(IfcSurfaceTexture map) { mMaps.Add(map.mIndex); }
	}
	//ENTITY IfcTextureCoordinateGenerator
	//ENTITY IfcTextureMap
	//ENTITY IfcTextureVertex;
	public partial class IfcTextureVertexList : IfcPresentationItem
	{
		internal Tuple<double, double>[] mTexCoordsList = new Tuple<double, double>[0];// : LIST [1:?] OF IfcSurfaceTexture

		internal IfcTextureVertexList() : base() { }
		internal IfcTextureVertexList(DatabaseIfc db, IfcTextureVertexList l) : base(db,l) { mTexCoordsList = l.mTexCoordsList; }
		public IfcTextureVertexList(DatabaseIfc m, IEnumerable<Tuple<double, double>> coords) : base(m) { mTexCoordsList = coords.ToArray(); }

		internal static IfcTextureVertexList Parse(string strDef) { IfcTextureVertexList l = new IfcTextureVertexList(); int pos = 0; l.parseFields(ParserSTEP.SplitLineFields(strDef), ref pos); return l; }
		protected override void parseFields(List<string> arrFields, ref int ipos) { base.parseFields(arrFields, ref ipos); mTexCoordsList = ParserSTEP.SplitListDoubleTuple(arrFields[ipos++]); }
		protected override string BuildStringSTEP()
		{
			Tuple<double, double> pair = mTexCoordsList[0];
			string result = base.BuildStringSTEP() + ",((" + ParserSTEP.DoubleToString(pair.Item1) + "," + ParserSTEP.DoubleToString(pair.Item2);
			for (int icounter = 1; icounter < mTexCoordsList.Length; icounter++)
			{
				pair = mTexCoordsList[icounter];
				result += "),(" + ParserSTEP.DoubleToString(pair.Item1) + "," + ParserSTEP.DoubleToString(pair.Item2);
			}

			return result + "))";
		}
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcThermalMaterialProperties : IfcMaterialPropertiesSuperSeded // DEPRECEATED IFC4
	{
		internal double mSpecificHeatCapacity = double.NaN;// : OPTIONAL IfcSpecificHeatCapacityMeasure;
		internal double mBoilingPoint = double.NaN;// : OPTIONAL IfcThermodynamicTemperatureMeasure;
		internal double mFreezingPoint = double.NaN;// : OPTIONAL IfcThermodynamicTemperatureMeasure;
		internal double mThermalConductivity = double.NaN;// : OPTIONAL IfcThermalConductivityMeasure; 
		internal IfcThermalMaterialProperties() : base() { }
		internal IfcThermalMaterialProperties(DatabaseIfc db, IfcThermalMaterialProperties p) : base(db,p) { mSpecificHeatCapacity = p.mSpecificHeatCapacity; mBoilingPoint = p.mBoilingPoint; mFreezingPoint = p.mFreezingPoint; mThermalConductivity = p.mThermalConductivity; }
		internal static IfcThermalMaterialProperties Parse(string strDef) { IfcThermalMaterialProperties p = new IfcThermalMaterialProperties(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos); return p; }
		internal static void parseFields(IfcThermalMaterialProperties p, List<string> arrFields, ref int ipos)
		{
			IfcMaterialPropertiesSuperSeded.parseFields(p, arrFields, ref ipos);
			p.mSpecificHeatCapacity = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mBoilingPoint = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mFreezingPoint = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mThermalConductivity = ParserSTEP.ParseDouble(arrFields[ipos++]);
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.DoubleOptionalToString(mSpecificHeatCapacity) + "," + ParserSTEP.DoubleOptionalToString(mBoilingPoint) + "," + ParserSTEP.DoubleOptionalToString(mFreezingPoint) + "," + ParserSTEP.DoubleOptionalToString(mThermalConductivity); }
	}
	public interface IfcTimeOrRatioSelect { string String { get; } } // IFC4 	IfcRatioMeasure, IfcDuration	
	public partial class IfcTimePeriod : BaseClassIfc // IFC4
	{
		internal string mStart; //:	IfcTime;
		internal string mFinish; //:	IfcTime;
		internal IfcTimePeriod() : base() { }
		internal IfcTimePeriod(IfcTimePeriod m) : base() { mStart = m.mStart; mFinish = m.mFinish; }
		internal IfcTimePeriod(DatabaseIfc m, DateTime start, DateTime finish) : base(m) { mStart = IfcTime.convert(start); mFinish = IfcTime.convert(finish);}
		internal static IfcTimePeriod Parse(string strDef) { IfcTimePeriod m = new IfcTimePeriod(); int ipos = 0; parseFields(m, ParserSTEP.SplitLineFields(strDef), ref ipos); return m; }
		internal static void parseFields(IfcTimePeriod m, List<string> arrFields, ref int ipos) { m.mStart = arrFields[ipos++]; m.mFinish = arrFields[ipos++]; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",'" + mStart + "','" + mFinish + "'"; }
	}
	public abstract partial class IfcTimeSeries : BaseClassIfc, IfcMetricValueSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect //ABSTRACT SUPERTYPE OF (ONEOF(IfcIrregularTimeSeries,IfcRegularTimeSeries));
	{
		internal string mName = "$";// : OPTIONAL IfcLabel;		
		internal string mDescription;// : OPTIONAL IfcText;
		internal int mStartTime;// : IfcDateTimeSelect;
		internal int mEndTime;// : IfcDateTimeSelect;
		internal IfcTimeSeriesDataTypeEnum mTimeSeriesDataType = IfcTimeSeriesDataTypeEnum.NOTDEFINED;// : IfcTimeSeriesDataTypeEnum;
		internal IfcDataOriginEnum mDataOrigin = IfcDataOriginEnum.NOTDEFINED;// : IfcDataOriginEnum;
		internal string mUserDefinedDataOrigin = "$";// : OPTIONAL IfcLabel;
		internal int mUnit;// : OPTIONAL IfcUnit; 
		//INVERSE
		internal List<IfcExternalReferenceRelationship> mHasExternalReferences = new List<IfcExternalReferenceRelationship>(); //IFC4
		internal List<IfcResourceConstraintRelationship> mHasConstraintRelationships = new List<IfcResourceConstraintRelationship>(); //gg

		public override string Name { get { return (mName == "$" ? "" : ParserIfc.Decode(mName)); } set { mName = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public ReadOnlyCollection<IfcExternalReferenceRelationship> HasExternalReferences { get { return new ReadOnlyCollection<IfcExternalReferenceRelationship>( mHasExternalReferences); } }
		public ReadOnlyCollection<IfcResourceConstraintRelationship> HasConstraintRelationships { get { return new ReadOnlyCollection<IfcResourceConstraintRelationship>( mHasConstraintRelationships); } }

		protected IfcTimeSeries() : base() { }
		//protected IfcTimeSeries(DatabaseIfc db, IfcTimeSeries i)
		//	: base(db,i)
		//{
		//	mName = i.mName;
		//	mDescription = i.mDescription;
		//	mStartTime = i.mStartTime;
		//	mEndTime = i.mEndTime;
		//	mTimeSeriesDataType = i.mTimeSeriesDataType;
		//	mDataOrigin = i.mDataOrigin;
		//	mUserDefinedDataOrigin = i.mUserDefinedDataOrigin;
		//	mUnit = i.mUnit;
		//}
		protected IfcTimeSeries(DatabaseIfc db) : base(db) { }
		internal static void parseFields(IfcTimeSeries s, List<string> arrFields, ref int ipos)
		{
			s.mName = arrFields[ipos++].Replace("'", "");
			s.mDescription = arrFields[ipos++].Replace("'", "");
			s.mStartTime = ParserSTEP.ParseLink(arrFields[ipos++]);
			s.mEndTime = ParserSTEP.ParseLink(arrFields[ipos++]);
			s.mTimeSeriesDataType = (IfcTimeSeriesDataTypeEnum)Enum.Parse(typeof(IfcTimeSeriesDataTypeEnum), arrFields[ipos++].Replace(".", ""));
			string str = arrFields[ipos++];
			if (str.StartsWith("."))
				s.mDataOrigin = (IfcDataOriginEnum)Enum.Parse(typeof(IfcDataOriginEnum), str.Replace(".", ""));
			s.mUserDefinedDataOrigin = arrFields[ipos++];
			s.mUnit = ParserSTEP.ParseLink(arrFields[ipos++]);
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",'" + mName + "','" + mDescription + "'," + ParserSTEP.LinkToString(mStartTime) + "," + ParserSTEP.LinkToString(mEndTime) + ",." + mTimeSeriesDataType.ToString() + ".,." + mDataOrigin.ToString() + ".," + mUserDefinedDataOrigin + "," + ParserSTEP.LinkToString(mUnit); }

		public void AddExternalReferenceRelationship(IfcExternalReferenceRelationship referenceRelationship) { mHasExternalReferences.Add(referenceRelationship); }
		public void AddConstraintRelationShip(IfcResourceConstraintRelationship constraintRelationship) { mHasConstraintRelationships.Add(constraintRelationship); }
	}
	//[Obsolete("DEPRECEATED IFC4", false)]
	//ENTITY IfcTimeSeriesReferenceRelationship; // DEPRECEATED IFC4
	//[Obsolete("DEPRECEATED IFC4", false)]
	//ENTITY IfcTimeSeriesSchedule // DEPRECEATED IFC4
	//ENTITY IfcTimeSeriesValue;  
	public abstract partial class IfcTopologicalRepresentationItem : IfcRepresentationItem  /*(IfcConnectedFaceSet,IfcEdge,IfcFace,IfcFaceBound,IfcLoop,IfcPath,IfcVertex))*/
	{
		protected IfcTopologicalRepresentationItem() : base() { }
		protected IfcTopologicalRepresentationItem(DatabaseIfc db) : base(db) { }
		protected IfcTopologicalRepresentationItem(DatabaseIfc db, IfcTopologicalRepresentationItem i) : base(db,i) { }
	}
	public partial class IfcTopologyRepresentation : IfcShapeModel
	{
		internal IfcTopologyRepresentation() : base() { }
		internal IfcTopologyRepresentation(DatabaseIfc db, IfcTopologyRepresentation r) : base(db, r) { }
		internal IfcTopologyRepresentation(IfcConnectedFaceSet fs, string identifier) : base(fs, identifier, "FaceSet") { }
		internal IfcTopologyRepresentation(IfcEdge e, string identifier) : base(e, identifier, "Edge") { }
		internal IfcTopologyRepresentation(IfcFace fs, string identifier) : base(fs, identifier, "Face") { }
		internal IfcTopologyRepresentation(IfcVertex v, string identifier) : base(v, identifier, "Vertex") { }
		internal new static IfcTopologyRepresentation Parse(string strDef) { IfcTopologyRepresentation r = new IfcTopologyRepresentation(); int pos = 0; r.parseString(strDef, ref pos, strDef.Length); return r; }
		internal static IfcTopologyRepresentation getRepresentation(IfcTopologicalRepresentationItem ri)
		{
			IfcConnectedFaceSet cfs = ri as IfcConnectedFaceSet;
			if (cfs != null)
				return new IfcTopologyRepresentation(cfs, "");
			IfcEdge e = ri as IfcEdge;
			if (e != null)
				return new IfcTopologyRepresentation(e, "");
			IfcFace f = ri as IfcFace;
			if (f != null)
				return new IfcTopologyRepresentation(f, "");
			IfcVertex v = ri as IfcVertex;
			if (v != null)
				return new IfcTopologyRepresentation(v, "");
			return null;
		}
		
	}
	public partial class IfcToroidalSurface : IfcElementarySurface //IFC4.2
	{
		internal double mMajorRadius;// : IfcPositiveLengthMeasure; 
		internal double mMinorRadius;// : IfcPositiveLengthMeasure; 
		public double MajorRadius { get { return mMajorRadius; } set { mMajorRadius = value; } }
		public double MinorRadius { get { return mMinorRadius; } set { mMinorRadius = value; } }
		internal IfcToroidalSurface() : base() { }
		internal IfcToroidalSurface(DatabaseIfc db, IfcToroidalSurface s) : base(db, s) { mMajorRadius = s.mMajorRadius; mMinorRadius = s.mMinorRadius; }

		internal static IfcToroidalSurface Parse(string str) { IfcToroidalSurface s = new IfcToroidalSurface(); int pos = 0; s.Parse(str, ref pos, str.Length); return s; }
		protected override void Parse(string str, ref int pos, int len) { base.Parse(str, ref pos, len); mMajorRadius = ParserSTEP.StripDouble(str, ref pos, len); mMinorRadius = ParserSTEP.StripDouble(str, ref pos, len); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.DoubleToString(mMajorRadius) + "," + ParserSTEP.DoubleToString(mMinorRadius); }
	}
	public partial class IfcTransformer : IfcEnergyConversionDevice //IFC4
	{
		internal IfcTransformerTypeEnum mPredefinedType = IfcTransformerTypeEnum.NOTDEFINED;// OPTIONAL : IfcTransformerTypeEnum;
		public IfcTransformerTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcTransformer() : base() { }
		internal IfcTransformer(DatabaseIfc db, IfcTransformer t) : base(db,t) { mPredefinedType = t.mPredefinedType; }
		public IfcTransformer(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcTransformer s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcTransformerTypeEnum)Enum.Parse(typeof(IfcTransformerTypeEnum), str);
		}
		internal new static IfcTransformer Parse(string strDef) { IfcTransformer s = new IfcTransformer(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcTransformerTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcTransformerType : IfcEnergyConversionDeviceType
	{
		internal IfcTransformerTypeEnum mPredefinedType = IfcTransformerTypeEnum.NOTDEFINED;// : IfcTransformerEnum; 
		public IfcTransformerTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcTransformerType() : base() { }
		internal IfcTransformerType(DatabaseIfc db, IfcTransformerType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal IfcTransformerType(DatabaseIfc m, string name, IfcTransformerTypeEnum type) : base(m) { Name = name; mPredefinedType = type; }
		internal static void parseFields(IfcTransformerType t, List<string> arrFields, ref int ipos) { IfcEnergyConversionDeviceType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcTransformerTypeEnum)Enum.Parse(typeof(IfcTransformerTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcTransformerType Parse(string strDef) { IfcTransformerType t = new IfcTransformerType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	public partial class IfcTranslationalStiffnessSelect
	{
		internal bool mRigid = false;
		internal IfcLinearStiffnessMeasure mStiffness = null;

		public bool Rigid { get { return mRigid; } }
		public IfcLinearStiffnessMeasure Stiffness { get { return mStiffness; } }

		public IfcTranslationalStiffnessSelect(bool fix) { mRigid = fix; }
		public IfcTranslationalStiffnessSelect(double stiff) { mStiffness = new IfcLinearStiffnessMeasure(stiff); }
		public IfcTranslationalStiffnessSelect(IfcLinearStiffnessMeasure stiff) { mStiffness = stiff; }
		internal static IfcTranslationalStiffnessSelect Parse(string str,ReleaseVersion version)
		{
			if (str == "$")
				return null;
			if (str.StartsWith("IFCBOOL"))
				return new IfcTranslationalStiffnessSelect(((IfcBoolean)ParserIfc.parseSimpleValue(str)).mValue);
			if (str.StartsWith("IFCLIN"))
				return new IfcTranslationalStiffnessSelect((IfcLinearStiffnessMeasure)ParserIfc.parseDerivedMeasureValue(str));
			if (str.StartsWith("."))
				return new IfcTranslationalStiffnessSelect(ParserSTEP.ParseBool(str));
			double d = ParserSTEP.ParseDouble(str), tol = 1e-9;
			if (version == ReleaseVersion.IFC2x3)
			{
				if (Math.Abs(d + 1) < tol)
					return new IfcTranslationalStiffnessSelect(true) { mStiffness = new IfcLinearStiffnessMeasure(-1) };
				if (Math.Abs(d) < tol)
					return new IfcTranslationalStiffnessSelect(false) { mStiffness = new IfcLinearStiffnessMeasure(0) };
			}
			return new IfcTranslationalStiffnessSelect(new IfcLinearStiffnessMeasure(d));
		}
		public override string ToString() { return (mStiffness == null ? "IFCBOOLEAN(" + ParserSTEP.BoolToString(mRigid) + ")" : mStiffness.ToString()); }
	}
	public partial class IfcTransportElement : IfcElement
	{
		internal IfcTransportElementTypeEnum mPredefinedType = IfcTransportElementTypeEnum.NOTDEFINED;// : 	OPTIONAL IfcTransportElementTypeEnum;
		internal double mCapacityByWeight = double.NaN;// : 	OPTIONAL IfcMassMeasure;
		internal double mCapacityByNumber = double.NaN;//	 : 	OPTIONAL IfcCountMeasure;

		public IfcTransportElementTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }
		//public double CapacityByWeight { get { return mCapacityByWeight; } set { mCapacityByWeight = value; } }
		//public double CapacityByNumber { get { return CapacityByNumber; } set { mCapacityByNumber = value; } }

		internal IfcTransportElement() : base() { }
		internal IfcTransportElement(DatabaseIfc db, IfcTransportElement e) : base(db, e,false) { }
		public IfcTransportElement(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation) : base(host, placement, representation) { }
		internal static void parseFields(IfcTransportElement e, List<string> arrFields, ref int ipos, ReleaseVersion schema)
		{
			IfcElement.parseFields(e, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str != "$")
				Enum.TryParse<IfcTransportElementTypeEnum>(str.Substring(1, str.Length - 2), out e.mPredefinedType);
			if(schema == ReleaseVersion.IFC2x3)
			{
				e.mCapacityByWeight = ParserSTEP.ParseDouble(arrFields[ipos++]);
				e.mCapacityByNumber = ParserSTEP.ParseDouble(arrFields[ipos++]);
			}
		}
		internal static IfcTransportElement Parse(string strDef, ReleaseVersion schema) { IfcTransportElement e = new IfcTransportElement(); int ipos = 0; parseFields(e, ParserSTEP.SplitLineFields(strDef), ref ipos,schema); return e; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + (mPredefinedType == IfcTransportElementTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + ".") + (mDatabase.Release == ReleaseVersion.IFC2x3 ? "," + ParserSTEP.DoubleOptionalToString(mCapacityByWeight) + "," + ParserSTEP.DoubleOptionalToString(mCapacityByNumber) : ""); }
	}
	public partial class IfcTransportElementType : IfcElementType
	{
		internal IfcTransportElementTypeEnum mPredefinedType;// IfcTransportElementTypeEnum; 
		public IfcTransportElementTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcTransportElementType() : base() { }
		internal IfcTransportElementType(DatabaseIfc db, IfcTransportElementType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		public IfcTransportElementType(DatabaseIfc m, string name, IfcTransportElementTypeEnum type) : base(m) { Name = name; mPredefinedType = type; }
		internal new static IfcTransportElementType Parse(string strDef) { IfcTransportElementType t = new IfcTransportElementType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		internal static void parseFields(IfcTransportElementType t, List<string> arrFields, ref int ipos) { IfcElementType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcTransportElementTypeEnum)Enum.Parse(typeof(IfcTransportElementTypeEnum), arrFields[ipos++].Replace(".", "")); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	public partial class IfcTrapeziumProfileDef : IfcParameterizedProfileDef
	{
		internal double mBottomXDim;// : IfcPositiveLengthMeasure;
		internal double mTopXDim;// : IfcPositiveLengthMeasure;
		internal double mYDim;// : IfcPositiveLengthMeasure;
		internal double mTopXOffset;// : IfcPositiveLengthMeasure; 
		internal IfcTrapeziumProfileDef() : base() { }
		internal IfcTrapeziumProfileDef(DatabaseIfc db, IfcTrapeziumProfileDef p) : base(db, p) { mBottomXDim = p.mBottomXDim; mTopXDim = p.mTopXDim; mYDim = p.mYDim; mTopXOffset = p.mTopXOffset; }
		internal IfcTrapeziumProfileDef(DatabaseIfc db, string name,double bottomXDim, double topXDim,double yDim,double topXOffset) : base(db,name)
		{
			if (mDatabase.mModelView != ModelView.Ifc4NotAssigned && mDatabase.mModelView != ModelView.If2x3NotAssigned)
				throw new Exception("Invalid Model View for IfcTrapeziumProfileDef : " + db.ModelView.ToString());
			mBottomXDim = bottomXDim;
			mTopXDim = topXDim;
			mYDim = yDim;
			mTopXOffset = topXOffset;
		}
		internal static void parseFields(IfcTrapeziumProfileDef p, List<string> arrFields, ref int ipos)
		{
			IfcParameterizedProfileDef.parseFields(p, arrFields, ref ipos);
			p.mBottomXDim = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mTopXDim = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mYDim = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mTopXOffset = ParserSTEP.ParseDouble(arrFields[ipos++]);
		}
		internal new static IfcTrapeziumProfileDef Parse(string strDef) { IfcTrapeziumProfileDef p = new IfcTrapeziumProfileDef(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos); return p; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.DoubleToString(mBottomXDim) + "," + ParserSTEP.DoubleToString(mTopXDim) + "," + ParserSTEP.DoubleToString(mYDim) + "," + ParserSTEP.DoubleToString(mTopXOffset); }
	}
	public partial class IfcTriangulatedFaceSet : IfcTessellatedFaceSet
	{
		internal Tuple<double, double, double>[] mNormals = new Tuple<double, double, double>[0];// : OPTIONAL LIST [1:?] OF LIST [3:3] OF IfcParameterValue; 
		internal IfcLogicalEnum mClosed = IfcLogicalEnum.UNKNOWN; // 	OPTIONAL BOOLEAN;
		internal Tuple<int, int, int>[] mCoordIndex = new Tuple<int, int, int>[0];// : 	LIST [1:?] OF LIST [3:3] OF INTEGER;
		internal Tuple<int, int, int>[] mNormalIndex = new Tuple<int, int, int>[0];// :	OPTIONAL LIST [1:?] OF LIST [3:3] OF INTEGER;  
		internal List<int> mPnIndex = new List<int>(); // : OPTIONAL LIST [1:?] OF IfcPositiveInteger;

		public ReadOnlyCollection< Tuple<double, double,double>> Normals { get { return new ReadOnlyCollection<Tuple<double, double, double>>( mNormals); } }
		public bool Closed { get { return mClosed == IfcLogicalEnum.TRUE; } set { mClosed = value ? IfcLogicalEnum.TRUE : IfcLogicalEnum.FALSE; } }
		public ReadOnlyCollection<Tuple<int, int, int>> CoordIndex { get { return new ReadOnlyCollection<Tuple<int, int, int>>(mCoordIndex); } }
		public ReadOnlyCollection<Tuple<int, int, int>> NormalIndex { get { return new ReadOnlyCollection<Tuple<int, int, int>>( mNormalIndex); } }
		public ReadOnlyCollection<int> PnIndex { get { return new ReadOnlyCollection<int>( mPnIndex); } }

		internal IfcTriangulatedFaceSet() : base() { }
		internal IfcTriangulatedFaceSet(DatabaseIfc db, IfcTriangulatedFaceSet s) : base(db,s)
		{
			if (s.mNormals.Length > 0)
				mNormals = s.mNormals.ToArray();
			mClosed = s.mClosed;
			mCoordIndex = s.mCoordIndex.ToArray();
			if(s.mNormalIndex.Length > 0)
			mNormalIndex = s.mNormalIndex.ToArray();
		}
		public IfcTriangulatedFaceSet(IfcCartesianPointList3D pl, bool closed, IEnumerable<Tuple<int, int, int>> coords)
			: base(pl) { setCoordIndex(coords); Closed = closed; }
		internal static IfcTriangulatedFaceSet Parse(string str)
		{
			IfcTriangulatedFaceSet t = new IfcTriangulatedFaceSet();
			int pos = 0;
			t.Parse(str, ref pos, str.Length);
			return t;
		}
		protected override void Parse(string str, ref int pos, int len)
		{
			base.Parse(str, ref pos, len);
			string field = ParserSTEP.StripField(str, ref pos, len);
			if (field.StartsWith("("))
				mNormals = ParserSTEP.SplitListDoubleTriple(field);
			mClosed = ParserIfc.StripLogical(str, ref pos, len);
			field = ParserSTEP.StripField(str, ref pos, len);
			mCoordIndex = ParserSTEP.SplitListSTPIntTriple(field);
			field = ParserSTEP.StripField(str, ref pos, len);
			if (field.StartsWith("("))
				mNormalIndex = ParserSTEP.SplitListSTPIntTriple(field);
			try
			{
				mPnIndex = ParserSTEP.StripListInt(str, ref pos, len);
			}
			catch(Exception) { }
		}
		protected override string BuildStringSTEP()
		{
			StringBuilder sb = new StringBuilder();
			if (mNormals.Length == 0)
				sb.Append( ",$,");
			else
			{
				Tuple<double, double, double> normal = mNormals[0];
				sb.Append( ",((" + ParserSTEP.DoubleToString(normal.Item1) + "," + ParserSTEP.DoubleToString(normal.Item2) + "," + ParserSTEP.DoubleToString(normal.Item3) + ")");
				for (int icounter = 1; icounter < mNormals.Length; icounter++)
				{
					normal = mNormals[icounter];
					sb.Append( ",(" + ParserSTEP.DoubleToString(normal.Item1) + "," + ParserSTEP.DoubleToString(normal.Item2) + "," + ParserSTEP.DoubleToString(normal.Item3) + ")");
				}
				sb.Append("),");
			}
			sb.Append( mClosed == IfcLogicalEnum.UNKNOWN ? "$" : ParserSTEP.BoolToString(Closed));
			Tuple<int, int, int> p = mCoordIndex[0];
			sb.Append(",((" + p.Item1 + "," + p.Item2 + "," + p.Item3);
			for (int icounter = 1; icounter < mCoordIndex.Length; icounter++)
			{
				p = mCoordIndex[icounter];
				sb.Append("),(" + p.Item1 + "," + p.Item2 + "," + p.Item3);
			}
			if (mNormalIndex.Length == 0)
				sb.Append(")),$");
			else
			{
				p = mNormalIndex[0];
				sb.Append(")),((" + p.Item1 + "," + p.Item2 + "," + p.Item3);
				for (int icounter = 1; icounter < mNormalIndex.Length; icounter++)
				{
					p = mNormalIndex[icounter];
					sb.Append("),(" + p.Item1 + "," + p.Item2 + "," + p.Item3);
				}
				sb.Append("))");
			}
			if (mPnIndex.Count == 0)
				sb.Append(",$");
			else
			{
				sb.Append(",(" + mPnIndex[0]);
				for (int icounter = 1; icounter < mPnIndex.Count; icounter++)
					sb.Append("," + mPnIndex[icounter]);
				sb.Append(")");
			}
			return base.BuildStringSTEP() + sb.ToString();
		}

		internal void setCoordIndex(IEnumerable<Tuple<int,int,int>> coords) { mCoordIndex = coords.ToArray(); }
	}
	public partial class IfcTrimmedCurve : IfcBoundedCurve
	{
		private int mBasisCurve;//: IfcCurve;
		internal IfcTrimmingSelect mTrim1;// : SET [1:2] OF IfcTrimmingSelect;
		internal IfcTrimmingSelect mTrim2;//: SET [1:2] OF IfcTrimmingSelect;
		private bool mSenseAgreement = false;// : BOOLEAN;
		internal IfcTrimmingPreference mMasterRepresentation = IfcTrimmingPreference.UNSPECIFIED;// : IfcTrimmingPreference; 

		public IfcCurve BasisCurve { get { return mDatabase[mBasisCurve] as IfcCurve; } set { mBasisCurve = value.mIndex; } }
		public IfcTrimmingSelect Trim1 { get { return mTrim1; } set { mTrim1 = value; } }
		public IfcTrimmingSelect Trim2 { get { return mTrim2; } set { mTrim2 = value; } }
		public bool SenseAgreement { get { return mSenseAgreement; } set { mSenseAgreement = value; } }
		public IfcTrimmingPreference MasterRepresentation { get { return mMasterRepresentation; } set { mMasterRepresentation = value; } }

		internal IfcTrimmedCurve() : base() { }
		internal IfcTrimmedCurve(DatabaseIfc db, IfcTrimmedCurve c) : base(db,c)
		{
			BasisCurve = db.Factory.Duplicate(c.BasisCurve) as IfcCurve;
			mTrim1 = new IfcTrimmingSelect(c.mTrim1.mIfcParameterValue);
			mTrim2 = new IfcTrimmingSelect(c.mTrim2.mIfcParameterValue);
			if (c.mTrim1.mIfcCartesianPoint > 0)
				mTrim1.mIfcCartesianPoint = db.Factory.Duplicate(c.mDatabase[c.mTrim1.mIfcCartesianPoint]).mIndex;
			if (c.mTrim2.mIfcCartesianPoint > 0)
				mTrim2.mIfcCartesianPoint = db.Factory.Duplicate(c.mDatabase[c.mTrim2.mIfcCartesianPoint]).mIndex;
			mSenseAgreement = c.mSenseAgreement;
			mMasterRepresentation = c.mMasterRepresentation;
		}
		public IfcTrimmedCurve(IfcConic basis, IfcTrimmingSelect start, IfcTrimmingSelect end, bool senseAgreement, IfcTrimmingPreference tp) 
			: this(basis.Database, start,end, senseAgreement,tp) { BasisCurve = basis; }
		public IfcTrimmedCurve(IfcLine basis, IfcTrimmingSelect start, IfcTrimmingSelect end, bool senseAgreement, IfcTrimmingPreference tp)
			: this(basis.Database, start, end, senseAgreement, tp) { BasisCurve = basis; }
		//public IfcTrimmedCurve(IfcClothoid basis, IfcTrimmingSelect start, IfcTrimmingSelect end, bool senseAgreement, IfcTrimmingPreference tp)
		//	: this(basis.Database, start, end, senseAgreement, tp) { BasisCurve = basis; }
		private IfcTrimmedCurve(DatabaseIfc db, IfcTrimmingSelect start, IfcTrimmingSelect end, bool senseAgreement, IfcTrimmingPreference tp) : base(db)
		{
			mTrim1 = start;
			mTrim2 = end;
			mSenseAgreement = senseAgreement;
			mMasterRepresentation = tp;
		}
		internal IfcTrimmedCurve(IfcCartesianPoint start, Tuple<double, double> arcInteriorPoint, IfcCartesianPoint end) : base(start.mDatabase)
		{
			Tuple<double, double, double> pt1 = start.Coordinates, pt3 = end.Coordinates;
			DatabaseIfc db = start.Database;
			double xDelta_a = arcInteriorPoint.Item1 - pt1.Item1;
			double yDelta_a = arcInteriorPoint.Item2 - pt1.Item2;
			double xDelta_b = pt3.Item1 - arcInteriorPoint.Item1;
			double yDelta_b = pt3.Item2 - arcInteriorPoint.Item2;
			double x = 0, y = 0;
			double tol = 1e-9;
			if (Math.Abs(xDelta_a) < tol && Math.Abs(yDelta_b) < tol)
			{
				x = (arcInteriorPoint.Item1 + pt3.Item1) / 2;
				y = (pt1.Item2 + arcInteriorPoint.Item2) / 2;
			}
			else
			{
				double aSlope = yDelta_a / xDelta_a; // 
				double bSlope = yDelta_b / xDelta_b;
				if (Math.Abs(aSlope - bSlope) < tol)
				{   // points are colinear
					// line curve
					BasisCurve = new IfcPolyline(start, end);
					mTrim1 = new IfcTrimmingSelect(0);
					mTrim2 = new IfcTrimmingSelect(1);
					MasterRepresentation = IfcTrimmingPreference.PARAMETER;
					return;
				}

				// calc center
				x = (aSlope * bSlope * (pt1.Item2 - pt3.Item2) + bSlope * (pt1.Item1 + arcInteriorPoint.Item1)
					- aSlope * (arcInteriorPoint.Item1 + pt3.Item1)) / (2 * (bSlope - aSlope));
				y = -1 * (x - (pt1.Item1 + arcInteriorPoint.Item1) / 2) / aSlope + (pt1.Item2 + arcInteriorPoint.Item2) / 2;
			}

			double radius = Math.Sqrt(Math.Pow(pt1.Item1 - x, 2) + Math.Pow(pt1.Item2 - y, 2));
			BasisCurve = new IfcCircle(new IfcAxis2Placement2D(new IfcCartesianPoint(db, x, y)) { RefDirection = new IfcDirection(db, pt1.Item1-x, pt1.Item2-y) }, radius);
			mTrim1 = new IfcTrimmingSelect(0,start);
			mSenseAgreement = (((arcInteriorPoint.Item1 - pt1.Item1) * (pt3.Item2 - arcInteriorPoint.Item2)) - ((arcInteriorPoint.Item2 - pt1.Item2) * (pt3.Item1 - arcInteriorPoint.Item1))) > 0;
			double t3 = Math.Atan2(pt3.Item2 - y, pt3.Item1 - x), t1 = Math.Atan2(pt1.Item2 - y, pt1.Item1 - x);
			if (t3 < 0)
				t3 = 2 * Math.PI + t3;
			mTrim2 = new IfcTrimmingSelect((t3 - t1 ) / db.mContext.UnitsInContext.getScaleSI(IfcUnitEnum.PLANEANGLEUNIT), end );
			mMasterRepresentation = IfcTrimmingPreference.PARAMETER;
		}	
		internal static IfcTrimmedCurve Parse(string str)
		{
			IfcTrimmedCurve c = new IfcTrimmedCurve();
			int pos = 0, len = str.Length;
			c.mBasisCurve = ParserSTEP.StripLink(str, ref pos, len);
			c.mTrim1 = IfcTrimmingSelect.Parse(ParserSTEP.StripField(str, ref pos, len));
			c.mTrim2 = IfcTrimmingSelect.Parse(ParserSTEP.StripField(str, ref pos, len));
			c.mSenseAgreement = ParserSTEP.StripBool(str, ref pos, len);
			c.mMasterRepresentation = (IfcTrimmingPreference)Enum.Parse(typeof(IfcTrimmingPreference), ParserSTEP.StripField(str, ref pos, len).Replace(".", ""));
			return c;
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mBasisCurve) + "," + mTrim1.ToString() + "," + mTrim2.ToString() + "," + ParserSTEP.BoolToString(mSenseAgreement) + ",." + mMasterRepresentation.ToString() + "."; }
	}
	public partial class IfcTrimmingSelect
	{
		public IfcTrimmingSelect() { }
		public IfcTrimmingSelect(IfcCartesianPoint cp)
		{
			mIfcParameterValue = double.NaN;
			mIfcCartesianPoint = (cp != null ? cp.mIndex : 0);
		}
		public IfcTrimmingSelect(double param) { mIfcParameterValue = param; }
		public IfcTrimmingSelect(double param, IfcCartesianPoint cp) : this(cp) { mIfcParameterValue = param; }
		
		internal double mIfcParameterValue = double.NaN;
		public double IfcParameterValue { get { return mIfcParameterValue; } }
		internal int mIfcCartesianPoint;
		public int IfcCartesianPoint { get { return mIfcCartesianPoint; } }
		internal static IfcTrimmingSelect Parse(string str)
		{
			IfcTrimmingSelect ts = new IfcTrimmingSelect();
			ts.mIfcParameterValue = double.NaN;
			int i = 0;
			if (str[i] == '(')
				i++;
			char c = str[i];
			if (c == '#')
			{
				string ls = "#";
				i++;
				while (i < str.Length)
				{
					c = str[i];
					if (c == ',' || c == ')')
						break;
					ls += c;
					i++;
				}
				ts.mIfcCartesianPoint = ParserSTEP.ParseLink(ls);
				if (c == ',')
				{
					if (str.Substring(i + 1).StartsWith("IFCPARAMETERVALUE(", true, System.Globalization.CultureInfo.CurrentCulture))
					{
						i += 19;
						string pv = "";
						while (str[i] != ')')
						{
							pv += str[i++];
						}
						ts.mIfcParameterValue = ParserSTEP.ParseDouble(pv);
					}
				}
			}
			else
			{
				if (str.Substring(i).StartsWith("IFCPARAMETERVALUE(", true, System.Globalization.CultureInfo.CurrentCulture))
				{
					i += 18;
					string pv = "";
					while (str[i] != ')')
					{
						pv += str[i++];
					}
					ts.mIfcParameterValue = ParserSTEP.ParseDouble(pv);
				}
				if (++i < str.Length)
				{
					if (str[i++] == ',')
					{
						ts.mIfcCartesianPoint = ParserSTEP.ParseLink(str.Substring(i, str.Length - i - 1));
					}
				}
			}
			return ts;
		}
		public override string ToString()
		{
			string str = "(";
			if (!double.IsNaN(mIfcParameterValue))
			{
				str += "IFCPARAMETERVALUE(" + ParserSTEP.DoubleToString(mIfcParameterValue) + ")";
				if (mIfcCartesianPoint > 0)
					str += "," + ParserSTEP.LinkToString(mIfcCartesianPoint);
				return str + ")";
			}
			else
				return str + ParserSTEP.LinkToString(mIfcCartesianPoint) + ")";
		}
	}
	public partial class IfcTShapeProfileDef : IfcParameterizedProfileDef
	{
		internal double mDepth, mFlangeWidth, mWebThickness, mFlangeThickness;// : IfcPositiveLengthMeasure;
		internal double mFilletRadius = double.NaN, mFlangeEdgeRadius = double.NaN, mWebEdgeRadius = double.NaN;// : OPTIONAL IfcPositiveLengthMeasure;
		internal double mWebSlope = double.NaN, mFlangeSlope = double.NaN;// : OPTIONAL IfcPlaneAngleMeasure;
		internal double mCentreOfGravityInX = double.NaN;// : OPTIONAL IfcPositiveLengthMeasure 

		public double Depth { get { return mDepth; } set { mDepth = value; } }
		public double FlangeWidth { get { return mFlangeWidth; } set { mFlangeWidth = value; } }
		public double WebThickness { get { return mWebThickness; } set { mWebThickness = value; } }
		public double FlangeThickness { get { return mFlangeThickness; } set { mFlangeThickness = value; } }
		public double FilletRadius { get { return mFilletRadius; } set { mFilletRadius = value; } }
		public double FlangeEdgeRadius { get { return mFlangeEdgeRadius; } set { mFlangeEdgeRadius = value; } }
		public double WebEdgeRadius { get { return mWebEdgeRadius; } set { mWebEdgeRadius = value; } }
		public double WebSlope { get { return mWebSlope; } set { mWebSlope = value; } }
		public double FlangeSlope { get { return mFlangeSlope; } set { mFlangeSlope = value; } }

		internal IfcTShapeProfileDef() : base() { }
		internal IfcTShapeProfileDef(DatabaseIfc db, IfcTShapeProfileDef p) : base(db, p)
		{
			mDepth = p.mDepth;
			mFlangeWidth = p.mFlangeWidth;
			mWebThickness = p.mWebThickness;
			mFlangeThickness = p.mFlangeThickness;
			mFilletRadius = p.mFilletRadius;
			mFlangeEdgeRadius = p.mFlangeEdgeRadius;
			mWebEdgeRadius = p.mWebEdgeRadius;
			mWebSlope = p.mWebSlope;
			mFlangeSlope = p.mFlangeSlope;
		}
		public IfcTShapeProfileDef(DatabaseIfc db, string name, double depth, double flangeWidth, double webThickness, double flangeThickness)
			: base(db,name)
		{
			mDepth = depth;
			mFlangeWidth = flangeWidth;
			mWebThickness = webThickness;
			mFlangeThickness = flangeThickness;
		}


		internal static void parseFields(IfcTShapeProfileDef p, List<string> arrFields, ref int ipos,ReleaseVersion schema)
		{
			IfcParameterizedProfileDef.parseFields(p, arrFields, ref ipos);
			p.mDepth = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mFlangeWidth = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mWebThickness = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mFlangeThickness = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mFilletRadius = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mFlangeEdgeRadius = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mWebEdgeRadius = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mWebSlope = ParserSTEP.ParseDouble(arrFields[ipos++]);
			p.mFlangeSlope = ParserSTEP.ParseDouble(arrFields[ipos++]);
			if (schema == ReleaseVersion.IFC2x3)
				p.mCentreOfGravityInX = ParserSTEP.ParseDouble(arrFields[ipos++]);	
		}
		internal static IfcTShapeProfileDef Parse(string strDef,ReleaseVersion schema) { IfcTShapeProfileDef p = new IfcTShapeProfileDef(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos,schema); return p; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + "," + ParserSTEP.DoubleToString(mDepth) + "," + ParserSTEP.DoubleToString(mFlangeWidth) + "," +
				ParserSTEP.DoubleToString(mWebThickness) + "," + ParserSTEP.DoubleToString(mFlangeThickness) + "," +
				ParserSTEP.DoubleOptionalToString(mFilletRadius) + "," + ParserSTEP.DoubleOptionalToString(mFlangeEdgeRadius) + "," +
				ParserSTEP.DoubleOptionalToString(mWebEdgeRadius) + "," + ParserSTEP.DoubleOptionalToString(mWebSlope) + "," +
				ParserSTEP.DoubleOptionalToString(mFlangeSlope) + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "," + ParserSTEP.DoubleOptionalToString(mCentreOfGravityInX) : "");
		}
	}
	public partial class IfcTubeBundle : IfcEnergyConversionDevice //IFC4
	{
		internal IfcTubeBundleTypeEnum mPredefinedType = IfcTubeBundleTypeEnum.NOTDEFINED;// OPTIONAL : IfcTubeBundleTypeEnum;
		public IfcTubeBundleTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcTubeBundle() : base() { }
		internal IfcTubeBundle(DatabaseIfc db, IfcTubeBundle b) : base(db, b) { mPredefinedType = b.mPredefinedType; }
		public IfcTubeBundle(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcTubeBundle s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcTubeBundleTypeEnum)Enum.Parse(typeof(IfcTubeBundleTypeEnum), str);
		}
		internal new static IfcTubeBundle Parse(string strDef) { IfcTubeBundle s = new IfcTubeBundle(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcTubeBundleTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcTubeBundleType : IfcEnergyConversionDeviceType
	{
		internal IfcTubeBundleTypeEnum mPredefinedType = IfcTubeBundleTypeEnum.NOTDEFINED;// : IfcTubeBundleEnum; 
		public IfcTubeBundleTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcTubeBundleType() : base() { }
		internal IfcTubeBundleType(DatabaseIfc db, IfcTubeBundleType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal IfcTubeBundleType(DatabaseIfc m, string name, IfcTubeBundleTypeEnum t) : base(m) { Name = name; PredefinedType = t; }
		internal static void parseFields(IfcTubeBundleType t, List<string> arrFields, ref int ipos) { IfcEnergyConversionDeviceType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcTubeBundleTypeEnum)Enum.Parse(typeof(IfcTubeBundleTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcTubeBundleType Parse(string strDef) { IfcTubeBundleType t = new IfcTubeBundleType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcTwoDirectionRepeatFactor : IfcOneDirectionRepeatFactor // DEPRECEATED IFC4
	{
		internal int mSecondRepeatFactor;//  : IfcVector 
		public IfcVector SecondRepeatFactor { get { return mDatabase[mSecondRepeatFactor] as IfcVector; } set { mSecondRepeatFactor = value.mIndex; } }

		internal IfcTwoDirectionRepeatFactor() : base() { }
		internal IfcTwoDirectionRepeatFactor(DatabaseIfc db, IfcTwoDirectionRepeatFactor f) : base(db,f) { SecondRepeatFactor = db.Factory.Duplicate(f.SecondRepeatFactor) as IfcVector; }
		internal new static IfcTwoDirectionRepeatFactor Parse(string str) { IfcTwoDirectionRepeatFactor f = new IfcTwoDirectionRepeatFactor(); int pos = 0; f.Parse(str, ref pos, str.Length); return f; }
		protected override void Parse(string str, ref int pos, int len)
		{
			base.Parse(str, ref pos, len);
			mSecondRepeatFactor = ParserSTEP.StripLink(str, ref pos, len);
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mSecondRepeatFactor); }
	}
	public partial class IfcTypeObject : IfcObjectDefinition //(IfcTypeProcess, IfcTypeProduct, IfcTypeResource) IFC4 ABSTRACT 
	{
		internal string mApplicableOccurrence = "$";// : OPTIONAL IfcLabel;
		internal List<int> mHasPropertySets = new List<int>();// : OPTIONAL SET [1:?] OF IfcPropertySetDefinition 
		//INVERSE 
		internal IfcRelDefinesByType mObjectTypeOf = null;

		public string ApplicableOccurrence { get { return (mApplicableOccurrence == "$" ? "" : ParserIfc.Decode(mApplicableOccurrence)); } set { mApplicableOccurrence = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public ReadOnlyCollection<IfcPropertySetDefinition> HasPropertySets { get { return new ReadOnlyCollection<IfcPropertySetDefinition>(mHasPropertySets.ConvertAll(x => mDatabase[x] as IfcPropertySetDefinition)); } }
		public IfcRelDefinesByType ObjectTypeOf { get { return mObjectTypeOf; } }
		//GeomGym
		internal IfcMaterialProfileSet mTapering = null;

		public override string Name { set { base.Name = string.IsNullOrEmpty( value) ? "NameNotAssigned" : value; } }

		protected IfcTypeObject() : base() { Name = "NameNotAssigned"; }
		protected IfcTypeObject(IfcTypeObject basis) : base(basis) { mApplicableOccurrence = basis.mApplicableOccurrence; mHasPropertySets = basis.mHasPropertySets; mObjectTypeOf = basis.mObjectTypeOf; }
		protected IfcTypeObject(DatabaseIfc db, IfcTypeObject t) : base(db,t,false) { mApplicableOccurrence = t.mApplicableOccurrence; t.HasPropertySets.ToList().ForEach(x=>AddPropertySet(db.Factory.Duplicate(x) as IfcPropertySetDefinition)); }
		internal IfcTypeObject(DatabaseIfc db) : base(db) { Name = "NameNotAssigned"; IfcRelDefinesByType rdt = new IfcRelDefinesByType(this) { Name = Name }; }
		
		internal static void parseFields(IfcTypeObject t, List<string> arrFields, ref int ipos)
		{
			IfcObjectDefinition.parseFields(t, arrFields, ref ipos);
			t.mApplicableOccurrence = arrFields[ipos++];
			string str = arrFields[ipos++];
			if (str != "$")
				t.mHasPropertySets = ParserSTEP.SplitListLinks(str);
		}
		protected override string BuildStringSTEP()
		{
			string psetlist = "";
			if (mHasPropertySets.Count > 0)
			{
				int icounter = 0;
				ReadOnlyCollection<IfcPropertySetDefinition> psets = HasPropertySets;
				for(icounter = 0; icounter < psets.Count; icounter++ )
				{
					if (psets[icounter].isEmpty)
						continue;
					psetlist = "#" + psets[icounter].mIndex;
					break;
				}
				for (icounter++; icounter < psets.Count; icounter++)
				{
					if (!psets[icounter].isEmpty)
						psetlist += ",#" + psets[icounter].mIndex;
				}
			}
			return base.BuildStringSTEP() + "," + mApplicableOccurrence + (string.IsNullOrEmpty(psetlist) ? ",$" : ",(" + psetlist + ")");
		}
		internal static IfcTypeObject Parse(string strDef) { IfcTypeObject o = new IfcTypeObject(); int ipos = 0; parseFields(o, ParserSTEP.SplitLineFields(strDef), ref ipos); return o; }

		public void AddPropertySet(IfcPropertySetDefinition psd) { mHasPropertySets.Add(psd.mIndex); psd.mDefinesType.Add(this); }
		internal override void postParseRelate()
		{
			base.postParseRelate();
			ReadOnlyCollection<IfcPropertySetDefinition> psets = HasPropertySets;
			foreach(IfcPropertySetDefinition pset in psets)
				pset.mDefinesType.Add(this);
		}
		protected override List<T> Extract<T>(Type type)
		{
			List<T> result = base.Extract<T>(type);
			foreach (IfcPropertySetDefinition psd in HasPropertySets)
				result.AddRange(psd.Extract<T>());
			return result;
		}
		internal IfcPropertySet findPropertySet(string name)
		{
			foreach(IfcPropertySet pset in HasPropertySets)
			{
				if (pset != null && string.Compare(pset.Name, name) == 0)
					return pset;
			}
			return null;
		}
		internal override List<IBaseClassIfc> retrieveReference(IfcReference r)
		{
			IfcReference ir = r.InnerReference;
			List<IBaseClassIfc> result = new List<IBaseClassIfc>();
			if (ir == null)
			{
				return null;
			}
			if (string.Compare(r.mAttributeIdentifier, "HasPropertySets", true) == 0)
			{

				ReadOnlyCollection<IfcPropertySetDefinition> psets = HasPropertySets;
				if (r.mListPositions.Count == 0)
				{
					string name = r.InstanceName;

					if (string.IsNullOrEmpty(name))
					{
						foreach (IfcPropertySetDefinition pset in psets)
							result.AddRange(pset.retrieveReference(r.InnerReference));
					}
					else
					{
						foreach (IfcPropertySetDefinition pset in psets)
						{
							if (string.Compare(name, pset.Name) == 0)
								result.AddRange(pset.retrieveReference(r.InnerReference));
						}
					}
				}
				else
				{
					foreach (int i in r.mListPositions)
						result.AddRange(psets[i - 1].retrieveReference(ir));
				}
				return result;
			}
			return base.retrieveReference(r);
		}
		internal override void changeSchema(ReleaseVersion schema)
		{
			base.changeSchema(schema);
			if (mObjectTypeOf != null)
				mObjectTypeOf.changeSchema(schema);
		}
		internal void IsolateObject(string filename)
		{
			DatabaseIfc db = new DatabaseIfc(mDatabase);
			db.Factory.Duplicate(this,true);
			if (mObjectTypeOf != null)
			{
				foreach (IfcObject o in mObjectTypeOf.RelatedObjects)
					db.Factory.Duplicate(o);
			}
			IfcSite site = db.Project.RootElement as IfcSite;
			if (site != null)
			{
				IfcProductRepresentation pr = site.Representation;
				if (pr != null)
				{
					site.Representation = null;
					pr.Destruct(true);
				}
			}
			db.WriteFile(filename);
		}
	}
	public abstract partial class IfcTypeProcess : IfcTypeObject //ABSTRACT SUPERTYPE OF(ONEOF(IfcEventType, IfcProcedureType, IfcTaskType))
	{
		private string mIdentification = "$";// :	OPTIONAL IfcIdentifier;
		private string mLongDescription = "$";//	 :	OPTIONAL IfcText;
		private string mProcessType = "$";//	 :	OPTIONAL IfcLabel;

		public string Identification { get { return (mIdentification == "$" ? "" : ParserIfc.Decode(mIdentification)); } set { mIdentification = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public string LongDescription { get { return (mLongDescription == "$" ? "" : ParserIfc.Decode(mLongDescription)); } set { mLongDescription = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public string ProcessType { get { return (mProcessType == "$" ? "" : ParserIfc.Decode(mProcessType)); } set { mProcessType = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }

		protected IfcTypeProcess() : base() { }
		protected IfcTypeProcess(DatabaseIfc db, IfcTypeProcess t) : base(db, t) { mIdentification = t.mIdentification; mLongDescription = t.mLongDescription; mProcessType = t.mProcessType; }
		protected IfcTypeProcess(DatabaseIfc db) : base(db) { }
		protected static void parseFields(IfcTypeProcess p, List<string> arrFields, ref int ipos) { IfcTypeObject.parseFields(p, arrFields, ref ipos); p.mIdentification = arrFields[ipos++].Replace("'", ""); p.mLongDescription = arrFields[ipos++].Replace("'", ""); p.mProcessType = arrFields[ipos++].Replace("'", ""); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + (mIdentification == "$" ? ",$," : ",'" + mIdentification + "',") + (mLongDescription == "$" ? "$," : "'" + mLongDescription + "',") + (mProcessType == "$" ? "$" : "'" + mProcessType + "'"); }
	}
	public partial class IfcTypeProduct : IfcTypeObject, IfcProductSelect //ABSTRACT SUPERTYPE OF (ONEOF (IfcDoorStyle ,IfcElementType ,IfcSpatialElementType ,IfcWindowStyle)) 
	{ 
		internal List<int> mRepresentationMaps = new List<int>();// : OPTIONAL LIST [1:?] OF UNIQUE IfcRepresentationMap;
		private string mTag = "$";// : OPTIONAL IfcLabel 
		//INVERSE
		internal List<IfcRelAssignsToProduct> mReferencedBy = new List<IfcRelAssignsToProduct>();//	 :	SET OF IfcRelAssignsToProduct FOR RelatingProduct;
		
		public ReadOnlyCollection<IfcRepresentationMap> RepresentationMaps { get { return new ReadOnlyCollection<IfcRepresentationMap>(mRepresentationMaps.ConvertAll(x => mDatabase[x] as IfcRepresentationMap)); } }
		public string Tag { get { return (mTag == "$" ? "" : mTag); } set { mTag = (string.IsNullOrEmpty(value) ? "$" : value); } }
		public ReadOnlyCollection<IfcRelAssignsToProduct> ReferencedBy { get { return new ReadOnlyCollection<IfcRelAssignsToProduct>(mReferencedBy); } }

		protected IfcTypeProduct() : base() { }
		protected IfcTypeProduct(IfcTypeProduct basis) : base(basis)
		{
			mRepresentationMaps = basis.mRepresentationMaps;
			mTag = basis.mTag;
		}
		protected IfcTypeProduct(DatabaseIfc db, IfcTypeProduct t) : base(db,t) { t.RepresentationMaps.ToList().ForEach(x=>AddRepresentationMap( db.Factory.Duplicate(x) as IfcRepresentationMap)); mTag = t.mTag; }
		protected IfcTypeProduct(DatabaseIfc db) : base(db) {  }

		internal new static IfcTypeProduct Parse(string strDef) { IfcTypeProduct p = new IfcTypeProduct(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos); return p; }
		internal static void parseFields(IfcTypeProduct p, List<string> arrFields, ref int ipos) { IfcTypeObject.parseFields(p, arrFields, ref ipos); p.mRepresentationMaps = ParserSTEP.SplitListLinks(arrFields[ipos++]); p.mTag = arrFields[ipos++].Replace("'", ""); }
		protected override string BuildStringSTEP()
		{
			string str = base.BuildStringSTEP() + ",";
			if (mRepresentationMaps.Count > 0)
			{
				str += "(" + ParserSTEP.LinkToString(mRepresentationMaps[0]);
				for (int icounter = 1; icounter < mRepresentationMaps.Count; icounter++)
					str += "," + ParserSTEP.LinkToString(mRepresentationMaps[icounter]);
				str += ")";
			}
			else
				str += "$";
			return str + (mTag == "$" ? ",$" : ",'" + mTag + "'");
		}

		internal override void postParseRelate()
		{
			base.postParseRelate();
			ReadOnlyCollection<IfcRepresentationMap> repMaps = RepresentationMaps;
			foreach(IfcRepresentationMap repMap in repMaps)
				repMap.mRepresents.Add(this);
		}

		public void Assign(IfcRelAssignsToProduct assigns) { mReferencedBy.Add(assigns); }
		public void Remove(IfcRelAssignsToProduct assigns) { mReferencedBy.Remove(assigns); }
		public void AddRepresentationMap(IfcRepresentationMap representationMap)
		{
			mRepresentationMaps.Add(representationMap.mIndex);
			representationMap.mRepresents.Add(this);
		}
		internal override void changeSchema(ReleaseVersion schema)
		{
			ReadOnlyCollection<IfcRepresentationMap> repMaps = RepresentationMaps;
			foreach(IfcRepresentationMap repMap in repMaps)
				repMap.changeSchema(schema);
			ReadOnlyCollection<IfcPropertySetDefinition> psets = HasPropertySets;
			foreach(IfcPropertySetDefinition pset in psets)
				pset.changeSchema(schema);
			base.changeSchema(schema);
		}

		internal IfcElement genMappedItemElement(IfcProduct container, IfcCartesianTransformationOperator3D t)
		{
			string typename = this.GetType().Name;
			typename = typename.Substring(0, typename.Length - 4);
			IfcShapeRepresentation sr = new IfcShapeRepresentation(new IfcMappedItem(RepresentationMaps[0], t));
			IfcProductDefinitionShape pds = new IfcProductDefinitionShape(sr);
			IfcElement element = IfcElement.constructElement(typename, container, null, pds);
			element.RelatingType = this;
			foreach (IfcRelNests nests in mIsNestedBy)
			{
				foreach (IfcObjectDefinition od in nests.RelatedObjects)
				{
					IfcDistributionPort port = od as IfcDistributionPort;
					if (port != null)
					{
						IfcDistributionPort newPort = new IfcDistributionPort(element) { FlowDirection = port.FlowDirection, PredefinedType = port.PredefinedType, SystemType = port.SystemType };
						newPort.Placement = new IfcLocalPlacement(element.Placement, t.generate());
						IfcLocalPlacement placement = port.Placement as IfcLocalPlacement;
						if (placement != null)
							newPort.Placement = new IfcLocalPlacement(newPort.Placement, placement.RelativePlacement);
						for (int dcounter = 0; dcounter < port.mIsDefinedBy.Count; dcounter++)
							port.mIsDefinedBy[dcounter].AddRelated(newPort);
					}
				}
			}
			ReadOnlyCollection<IfcPropertySetDefinition> psets = HasPropertySets;
			foreach(IfcPropertySetDefinition pset in psets)
			{
				if (pset.IsInstancePropertySet)
					pset.AssignObjectDefinition(element);
			}
			return element;
		}

		internal static IfcTypeProduct constructType(DatabaseIfc db, string className, string name)
		{
			string str = className, definedType = "";
			if (!string.IsNullOrEmpty(str))
			{
				string[] fields = str.Split(".".ToCharArray());
				if (fields.Length > 1)
				{
					str = fields[0];
					definedType = fields[1];
				}
			}
			IfcTypeProduct result = null;
			Type type = Type.GetType("GeometryGym.Ifc." + str);
			if (type != null)
			{
				Type enumType = Type.GetType("GeometryGym.Ifc." + type.Name + "Enum");
				ConstructorInfo ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
null, new[] { typeof(DatabaseIfc), typeof(string) }, null);
				if (ctor == null)
				{
					ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
null, new[] { typeof(DatabaseIfc), typeof(string), enumType }, null);
					if (ctor == null)
						throw new Exception("XXX Unrecognized Ifc Constructor for " + className);
					else
					{
						object predefined = Enum.Parse(enumType, "NOTDEFINED");
						result = ctor.Invoke(new object[] { db, name,predefined  }) as IfcTypeProduct;
					}
				}
				else
					result = ctor.Invoke(new object[] { db, name }) as IfcTypeProduct;

			
				if (result != null && !string.IsNullOrEmpty(definedType))
				{
					IfcElementType et = result as IfcElementType;
					type = result.GetType();
					PropertyInfo pi = type.GetProperty("PredefinedType");
					if (pi != null)
					{
						if (enumType != null)
						{
							FieldInfo fi = enumType.GetField(definedType);
							if (fi == null)
							{
								if (et != null)
								{
									et.ElementType = definedType;
									fi = enumType.GetField("NOTDEFINED");
								}
							}
							if (fi != null)
							{
								int i = (int)fi.GetValue(enumType);
								object newEnumValue = Enum.ToObject(enumType, i);
								pi.SetValue(result, newEnumValue, null);
							}
							else if (et != null)
								et.ElementType = definedType;
						}
						else if (et != null)
							et.ElementType = definedType;
					}
				}
			}
			return result;
		}
	}
	public abstract partial class IfcTypeResource : IfcTypeObject //ABSTRACT SUPERTYPE OF(IfcConstructionResourceType)
	{
		internal string mIdentification = "$";// :	OPTIONAL IfcIdentifier;
		internal string mLongDescription = "$";//	 :	OPTIONAL IfcText;
		internal string mResourceType = "$";//	 :	OPTIONAL IfcLabel;

		public string Identification { get { return (mIdentification == "$" ? "" : ParserIfc.Decode(mIdentification)); } set { mIdentification = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public string LongDescription { get { return (mLongDescription == "$" ? "" : ParserIfc.Decode(mLongDescription)); } set { mLongDescription = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public string ResourceType { get { return (mResourceType == "$" ? "" : ParserIfc.Decode(mResourceType)); } set { mResourceType = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }

		protected IfcTypeResource() : base() { }
		protected IfcTypeResource(DatabaseIfc db, IfcTypeResource t) : base(db,t) { mIdentification = t.mIdentification; mLongDescription = t.mLongDescription; mResourceType = t.mResourceType; }
		protected IfcTypeResource(DatabaseIfc db) : base(db) { }
		protected static void parseFields(IfcTypeResource p, List<string> arrFields, ref int ipos) { IfcTypeObject.parseFields(p, arrFields, ref ipos); p.mIdentification = arrFields[ipos++].Replace("'", ""); p.mLongDescription = arrFields[ipos++].Replace("'", ""); p.mResourceType = arrFields[ipos++].Replace("'", ""); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + (mIdentification == "$" ? ",$," : ",'" + mIdentification + "',") + (mLongDescription == "$" ? "$," : "'" + mLongDescription + "',") + (mResourceType == "$" ? "$" : "'" + mResourceType + "'"); }
	}
}
