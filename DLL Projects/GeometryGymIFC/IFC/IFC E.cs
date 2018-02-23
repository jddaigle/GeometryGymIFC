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
using System.Text;
using System.Reflection;
using GeometryGym.STEP;

namespace GeometryGym.Ifc
{
	public partial class IfcEdge : IfcTopologicalRepresentationItem //SUPERTYPE OF(ONEOF(IfcEdgeCurve, IfcOrientedEdge, IfcSubedge))
	{
		internal int mEdgeStart, mEdgeEnd;// : IfcVertex;
		public IfcVertex EdgeStart { get { return mDatabase[mEdgeStart] as IfcVertex; } set { mEdgeStart = value.mIndex; } }
		public IfcVertex EdgeEnd { get { return mDatabase[mEdgeEnd] as IfcVertex; } set { mEdgeEnd = value.mIndex; } }

		internal IfcEdge() : base() { }
		protected IfcEdge(DatabaseIfc db) : base(db) { }
		internal IfcEdge(DatabaseIfc db, IfcEdge e) : base(db, e)
		{
			if(e.mEdgeStart > 0)
				EdgeStart = db.Factory.Duplicate( e.EdgeStart) as IfcVertex;
			if(e.mEdgeEnd > 0)
				EdgeEnd = db.Factory.Duplicate( e.EdgeEnd) as IfcVertex;
		}
		internal IfcEdge(IfcVertex start, IfcVertex end) : base(start.mDatabase) { EdgeStart = start; EdgeEnd = end; }
		internal static IfcEdge Parse(string strDef) { IfcEdge e = new IfcEdge(); int ipos = 0; parseFields(e, ParserSTEP.SplitLineFields(strDef), ref ipos); return e; }
		internal static void parseFields(IfcEdge e, List<string> arrFields, ref int ipos) { e.mEdgeStart = ParserSTEP.ParseLink(arrFields[ipos++]); e.mEdgeEnd = ParserSTEP.ParseLink(arrFields[ipos++]); }
		protected override string BuildStringSTEP()
		{
			IfcOrientedEdge oe = this as IfcOrientedEdge;
			return base.BuildStringSTEP() + (oe == null ? "," + ParserSTEP.LinkToString(mEdgeStart) + "," + ParserSTEP.LinkToString(mEdgeEnd) : ",*,*");
		}
	}
	public partial class IfcEdgeCurve : IfcEdge, IfcCurveOrEdgeCurve
	{
		internal int mEdgeGeometry;// IfcCurve;
		internal bool mSameSense;// : BOOL;

		public IfcCurve EdgeGeometry { get { return mDatabase[mEdgeGeometry] as IfcCurve; } set { mEdgeGeometry = value.mIndex; } }
		public bool SameSense { get { return mSameSense; } }
		
		internal IfcEdgeCurve() : base() { }
		internal IfcEdgeCurve(DatabaseIfc db, IfcEdgeCurve e) : base(db,e) { EdgeGeometry = db.Factory.Duplicate(e.EdgeGeometry) as IfcCurve; mSameSense = e.mSameSense; }
		public IfcEdgeCurve(IfcVertexPoint start, IfcVertexPoint end, IfcCurve edge, bool sense) : base(start, end) { mEdgeGeometry = edge.mIndex; mSameSense = sense; }
	//	internal IfcEdgeCurve(IfcBoundedCurve ec, IfcVertexPoint s, IfcVertexPoint e,bool sense) : base(s, e) { mEdgeGeometry = ec.mIndex; mSameSense = true; }
		internal new static IfcEdgeCurve Parse(string strDef) { IfcEdgeCurve ec = new IfcEdgeCurve(); int ipos = 0; parseFields(ec, ParserSTEP.SplitLineFields(strDef), ref ipos); return ec; }
		internal static void parseFields(IfcEdgeCurve e, List<string> arrFields, ref int ipos) { IfcEdge.parseFields(e, arrFields, ref ipos); e.mEdgeGeometry = ParserSTEP.ParseLink(arrFields[ipos++]); e.mSameSense = ParserSTEP.ParseBool(arrFields[ipos++]); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mEdgeGeometry) + "," + ParserSTEP.BoolToString(mSameSense); }

		internal override void postParseRelate()
		{
			base.postParseRelate();
			EdgeGeometry.mEdge = this;
		}
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public abstract partial class IfcEdgeFeature : IfcFeatureElementSubtraction //  ABSTRACT SUPERTYPE OF (ONEOF (IfcChamferEdgeFeature , IfcRoundedEdgeFeature)) DEPRECEATED IFC4
	{
		internal double mFeatureLength;// OPTIONAL IfcPositiveLengthMeasure; 
		protected IfcEdgeFeature() : base() { }
		protected IfcEdgeFeature(DatabaseIfc db, IfcEdgeFeature f) : base(db, f) { mFeatureLength = f.mFeatureLength; }
		protected static void parseFields(IfcEdgeFeature f, List<string> arrFields, ref int ipos) { IfcFeatureElementSubtraction.parseFields(f, arrFields, ref ipos); f.mFeatureLength = ParserSTEP.ParseDouble(arrFields[ipos++]); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.DoubleOptionalToString(mFeatureLength); }
	}
	public partial class IfcEdgeLoop : IfcLoop
	{
		internal List<int> mEdgeList = new List<int>();// LIST [1:?] OF IfcOrientedEdge;
		public ReadOnlyCollection<IfcOrientedEdge> EdgeList { get { return new ReadOnlyCollection<IfcOrientedEdge>(mEdgeList.ConvertAll(x => mDatabase[x] as IfcOrientedEdge)); } }
		internal IfcEdgeLoop() : base() { }
		internal IfcEdgeLoop(DatabaseIfc db, IfcEdgeLoop l) : base(db,l) { l.EdgeList.ToList().ForEach(x => addEdge( db.Factory.Duplicate(x) as IfcOrientedEdge)); }
		public IfcEdgeLoop(IfcOrientedEdge edge) : base(edge.mDatabase) { mEdgeList.Add(edge.mIndex); }
		public IfcEdgeLoop(IfcOrientedEdge edge1, IfcOrientedEdge edge2) : base(edge1.mDatabase) { mEdgeList.Add(edge1.mIndex); mEdgeList.Add(edge2.mIndex); }
		public IfcEdgeLoop(List<IfcOrientedEdge> edges) : base(edges[0].mDatabase) { mEdgeList = edges.ConvertAll(x => x.mIndex); }
		internal IfcEdgeLoop(List<IfcVertexPoint> vertex)
			: base(vertex[0].mDatabase)
		{
			for (int icounter = 1; icounter < vertex.Count; icounter++)
				mEdgeList.Add(new IfcOrientedEdge(vertex[icounter - 1], vertex[icounter]).mIndex);
			mEdgeList.Add(new IfcOrientedEdge(vertex[vertex.Count - 1], vertex[0]).mIndex);
		}
		internal static IfcEdgeLoop Parse(string str) { IfcEdgeLoop l = new IfcEdgeLoop(); l.mEdgeList = ParserSTEP.SplitListLinks(str.Substring(1, str.Length - 2)); return l; }
		protected override string BuildStringSTEP()
		{
			string str = base.BuildStringSTEP() + ",(";
			if (mEdgeList.Count > 0)
				str += ParserSTEP.LinkToString(mEdgeList[0]);
			for (int icounter = 1; icounter < mEdgeList.Count; icounter++)
				str += "," + ParserSTEP.LinkToString(mEdgeList[icounter]);
			return str + ")";
		}

		internal void addEdge(IfcOrientedEdge edge) { mEdgeList.Add(mIndex); }
	}
	public partial class IfcElectricAppliance : IfcFlowTerminal //IFC4
	{
		internal IfcElectricApplianceTypeEnum mPredefinedType = IfcElectricApplianceTypeEnum.NOTDEFINED;// OPTIONAL : IfcElectricApplianceTypeEnum;
		public IfcElectricApplianceTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricAppliance() : base() { }
		internal IfcElectricAppliance(DatabaseIfc db, IfcElectricAppliance a) : base(db, a) { mPredefinedType = a.mPredefinedType; }
		public IfcElectricAppliance(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }
		internal static void parseFields(IfcElectricAppliance s, List<string> arrFields, ref int ipos)
		{
			IfcFlowTerminal.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcElectricApplianceTypeEnum)Enum.Parse(typeof(IfcElectricApplianceTypeEnum), str.Substring(1, str.Length - 2));
		}
		internal new static IfcElectricAppliance Parse(string strDef) { IfcElectricAppliance s = new IfcElectricAppliance(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcElectricApplianceTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + ".")); }
	}
	public partial class IfcElectricApplianceType : IfcFlowTerminalType
	{
		internal IfcElectricApplianceTypeEnum mPredefinedType = IfcElectricApplianceTypeEnum.NOTDEFINED;// : IfcDuctFittingTypeEnum;
		public IfcElectricApplianceTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricApplianceType() : base() { }
		internal IfcElectricApplianceType(DatabaseIfc db, IfcElectricApplianceType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal IfcElectricApplianceType(DatabaseIfc m, string name, IfcElectricApplianceTypeEnum t) : base(m) { Name = name; mPredefinedType = t; }
		internal static void parseFields(IfcElectricApplianceType t, List<string> arrFields, ref int ipos) { IfcFlowTerminalType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcElectricApplianceTypeEnum)Enum.Parse(typeof(IfcElectricApplianceTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcElectricApplianceType Parse(string strDef) { IfcElectricApplianceType t = new IfcElectricApplianceType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	public partial class IfcElectricDistributionBoard : IfcFlowController //IFC4
	{
		internal IfcElectricDistributionBoardTypeEnum mPredefinedType = IfcElectricDistributionBoardTypeEnum.NOTDEFINED;// OPTIONAL : IfcDamperTypeEnum;
		public IfcElectricDistributionBoardTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricDistributionBoard() : base() { }
		internal IfcElectricDistributionBoard(DatabaseIfc db, IfcElectricDistributionBoard b) : base(db,b) { mPredefinedType = b.mPredefinedType; }
		public IfcElectricDistributionBoard(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcElectricDistributionBoard s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcElectricDistributionBoardTypeEnum)Enum.Parse(typeof(IfcElectricDistributionBoardTypeEnum), str);
		}
		internal new static IfcElectricDistributionBoard Parse(string strDef) { IfcElectricDistributionBoard s = new IfcElectricDistributionBoard(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcElectricDistributionBoardTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcElectricDistributionBoardType : IfcFlowControllerType
	{
		internal IfcElectricDistributionBoardTypeEnum mPredefinedType = IfcElectricDistributionBoardTypeEnum.NOTDEFINED;// : IfcElectricDistributionBoardTypeEnum;
		public IfcElectricDistributionBoardTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricDistributionBoardType() : base() { }
		internal IfcElectricDistributionBoardType(DatabaseIfc db, IfcElectricDistributionBoardType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal IfcElectricDistributionBoardType(DatabaseIfc m, string name, IfcElectricDistributionBoardTypeEnum t) : base(m) { Name = name; mPredefinedType = t; }
		internal static void parseFields(IfcElectricDistributionBoardType t, List<string> arrFields, ref int ipos) { IfcFlowControllerType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcElectricDistributionBoardTypeEnum)Enum.Parse(typeof(IfcElectricDistributionBoardTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcElectricDistributionBoardType Parse(string strDef) { IfcElectricDistributionBoardType t = new IfcElectricDistributionBoardType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcElectricDistributionPoint : IfcFlowController // DEPRECEATED IFC4
	{
		internal IfcElectricDistributionPointFunctionEnum mDistributionPointFunction;// : IfcElectricDistributionPointFunctionEnum;
		internal string mUserDefinedFunction = "$";// : OPTIONAL IfcLabel;

		public IfcElectricDistributionPointFunctionEnum DistributionPointFunction { get { return mDistributionPointFunction; } set { mDistributionPointFunction = value; } }
		public string UserDefinedFunction { get { return mUserDefinedFunction == "$" ? "" : ParserIfc.Decode(mUserDefinedFunction); } set { mUserDefinedFunction = string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value); } }

		internal IfcElectricDistributionPoint() : base() { }
		internal IfcElectricDistributionPoint(DatabaseIfc db, IfcElectricDistributionPoint p) : base(db, p) { mDistributionPointFunction = p.mDistributionPointFunction; mUserDefinedFunction = p.mUserDefinedFunction; }
		public IfcElectricDistributionPoint(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcElectricDistributionPoint dp, List<string> arrFields, ref int ipos)
		{
			IfcFlowController.parseFields(dp, arrFields, ref ipos);
			dp.mDistributionPointFunction = (IfcElectricDistributionPointFunctionEnum)Enum.Parse(typeof(IfcElectricDistributionPointFunctionEnum), arrFields[ipos++].Replace(".", ""));
			dp.mUserDefinedFunction = arrFields[ipos++];
		}
		internal new static IfcElectricDistributionPoint Parse(string strDef) { IfcElectricDistributionPoint p = new IfcElectricDistributionPoint(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos); return p; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mDistributionPointFunction.ToString() + (mUserDefinedFunction == "$" ? ".,$" : ".,'" + mUserDefinedFunction + "'"); }
	}
	public partial class IfcElectricFlowStorageDevice : IfcFlowStorageDevice //IFC4
	{
		internal IfcElectricFlowStorageDeviceTypeEnum mPredefinedType = IfcElectricFlowStorageDeviceTypeEnum.NOTDEFINED;// OPTIONAL : IfcElectricFlowStorageDeviceTypeEnum;
		public IfcElectricFlowStorageDeviceTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricFlowStorageDevice() : base() { }
		internal IfcElectricFlowStorageDevice(DatabaseIfc db, IfcElectricFlowStorageDevice d) : base(db,d) { mPredefinedType = d.mPredefinedType; }
		public IfcElectricFlowStorageDevice(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcElectricFlowStorageDevice s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcElectricFlowStorageDeviceTypeEnum)Enum.Parse(typeof(IfcElectricFlowStorageDeviceTypeEnum), str);
		}
		internal new static IfcElectricFlowStorageDevice Parse(string strDef) { IfcElectricFlowStorageDevice s = new IfcElectricFlowStorageDevice(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcElectricFlowStorageDeviceTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcElectricFlowStorageDeviceType : IfcFlowStorageDeviceType
	{
		internal IfcElectricFlowStorageDeviceTypeEnum mPredefinedType = IfcElectricFlowStorageDeviceTypeEnum.NOTDEFINED;// : IfcElectricFlowStorageDeviceTypeEnum;
		public IfcElectricFlowStorageDeviceTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricFlowStorageDeviceType() : base() { }
		internal IfcElectricFlowStorageDeviceType(DatabaseIfc db, IfcElectricFlowStorageDeviceType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal static void parseFields(IfcElectricFlowStorageDeviceType t, List<string> arrFields, ref int ipos) { IfcFlowStorageDeviceType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcElectricFlowStorageDeviceTypeEnum)Enum.Parse(typeof(IfcElectricFlowStorageDeviceTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcElectricFlowStorageDeviceType Parse(string strDef) { IfcElectricFlowStorageDeviceType t = new IfcElectricFlowStorageDeviceType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	public partial class IfcElectricGenerator : IfcEnergyConversionDevice //IFC4
	{
		internal IfcElectricGeneratorTypeEnum mPredefinedType = IfcElectricGeneratorTypeEnum.NOTDEFINED;// OPTIONAL : IfcElectricGeneratorTypeEnum;
		public IfcElectricGeneratorTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricGenerator() : base() { }
		internal IfcElectricGenerator(DatabaseIfc db, IfcElectricGenerator g) : base(db, g) { mPredefinedType = g.mPredefinedType; }
		public IfcElectricGenerator(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcElectricGenerator s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcElectricGeneratorTypeEnum)Enum.Parse(typeof(IfcElectricGeneratorTypeEnum), str);
		}
		internal new static IfcElectricGenerator Parse(string strDef) { IfcElectricGenerator s = new IfcElectricGenerator(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcElectricGeneratorTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcElectricGeneratorType : IfcEnergyConversionDeviceType
	{
		internal IfcElectricGeneratorTypeEnum mPredefinedType = IfcElectricGeneratorTypeEnum.NOTDEFINED;// : IfcElectricGeneratorTypeEnum;
		public IfcElectricGeneratorTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricGeneratorType() : base() { }
		internal IfcElectricGeneratorType(DatabaseIfc db, IfcElectricGeneratorType t) : base(db,t) { mPredefinedType = t.mPredefinedType; }
		internal static void parseFields(IfcElectricGeneratorType t, List<string> arrFields, ref int ipos) { IfcEnergyConversionDeviceType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcElectricGeneratorTypeEnum)Enum.Parse(typeof(IfcElectricGeneratorTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcElectricGeneratorType Parse(string strDef) { IfcElectricGeneratorType t = new IfcElectricGeneratorType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcElectricHeaterType : IfcFlowTerminalType // DEPRECEATED IFC4
	{
		internal IfcElectricHeaterTypeEnum mPredefinedType = IfcElectricHeaterTypeEnum.NOTDEFINED;// : IfcElectricHeaterTypeEnum
		public IfcElectricHeaterTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricHeaterType() : base() { }
		internal IfcElectricHeaterType(DatabaseIfc db, IfcElectricHeaterType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal static void parseFields(IfcElectricHeaterType t, List<string> arrFields, ref int ipos) { IfcFlowTerminalType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcElectricHeaterTypeEnum)Enum.Parse(typeof(IfcElectricHeaterTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcElectricHeaterType Parse(string strDef) { IfcElectricHeaterType t = new IfcElectricHeaterType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	public partial class IfcElectricMotor : IfcEnergyConversionDevice //IFC4
	{
		internal IfcElectricMotorTypeEnum mPredefinedType = IfcElectricMotorTypeEnum.NOTDEFINED;// OPTIONAL : IfcElectricMotorTypeEnum;
		public IfcElectricMotorTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricMotor() : base() { }
		internal IfcElectricMotor(DatabaseIfc db, IfcElectricMotor m) : base(db, m) { mPredefinedType = m.mPredefinedType; }
		public IfcElectricMotor(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcElectricMotor s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcElectricMotorTypeEnum)Enum.Parse(typeof(IfcElectricMotorTypeEnum), str);
		}
		internal new static IfcElectricMotor Parse(string strDef) { IfcElectricMotor s = new IfcElectricMotor(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcElectricMotorTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcElectricMotorType : IfcEnergyConversionDeviceType
	{
		internal IfcElectricMotorTypeEnum mPredefinedType = IfcElectricMotorTypeEnum.NOTDEFINED;// : IfcElectricMotorTypeEnum;
		public IfcElectricMotorTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricMotorType() : base() { }
		internal IfcElectricMotorType(DatabaseIfc db, IfcElectricMotorType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal static void parseFields(IfcElectricMotorType t, List<string> arrFields, ref int ipos) { IfcEnergyConversionDeviceType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcElectricMotorTypeEnum)Enum.Parse(typeof(IfcElectricMotorTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcElectricMotorType Parse(string strDef) { IfcElectricMotorType t = new IfcElectricMotorType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	public partial class IfcElectricTimeControl : IfcFlowController //IFC4
	{
		internal IfcElectricTimeControlTypeEnum mPredefinedType = IfcElectricTimeControlTypeEnum.NOTDEFINED;// OPTIONAL : IfcElectricTimeControlTypeEnum;
		public IfcElectricTimeControlTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricTimeControl() : base() { }
		internal IfcElectricTimeControl(DatabaseIfc db, IfcElectricTimeControl c) : base(db, c) { mPredefinedType = c.mPredefinedType; }
		public IfcElectricTimeControl(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcElectricTimeControl s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcElectricTimeControlTypeEnum)Enum.Parse(typeof(IfcElectricTimeControlTypeEnum), str);
		}
		internal new static IfcElectricTimeControl Parse(string strDef) { IfcElectricTimeControl s = new IfcElectricTimeControl(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcElectricTimeControlTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcElectricTimeControlType : IfcFlowControllerType
	{
		internal IfcElectricTimeControlTypeEnum mPredefinedType = IfcElectricTimeControlTypeEnum.NOTDEFINED;// : IfcElectricTimeControlTypeEnum;
		public IfcElectricTimeControlTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElectricTimeControlType() : base() { }
		internal IfcElectricTimeControlType(DatabaseIfc db, IfcElectricTimeControlType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal IfcElectricTimeControlType(DatabaseIfc m, string name, IfcElectricTimeControlTypeEnum type) : base(m) { Name = name; mPredefinedType = type; }
		internal static void parseFields(IfcElectricTimeControlType t, List<string> arrFields, ref int ipos) { IfcFlowControllerType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcElectricTimeControlTypeEnum)Enum.Parse(typeof(IfcElectricTimeControlTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcElectricTimeControlType Parse(string strDef) { IfcElectricTimeControlType t = new IfcElectricTimeControlType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	//[Obsolete("DEPRECEATED IFC4", false)]
	//ENTITY IfcElectricalBaseProperties // DEPRECEATED IFC4
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcElectricalCircuit : IfcSystem // DEPRECEATED IFC4
	{
		internal IfcElectricalCircuit() : base() { }
		internal IfcElectricalCircuit(DatabaseIfc db, IfcElectricalCircuit c, bool downStream) : base(db,c,downStream) { }
		internal new static IfcElectricalCircuit Parse(string strDef) { IfcElectricalCircuit c = new IfcElectricalCircuit(); int ipos = 0; parseFields(c, ParserSTEP.SplitLineFields(strDef), ref ipos); return c; }
		internal static void parseFields(IfcElectricalCircuit c, List<string> arrFields, ref int ipos) { IfcSystem.parseFields(c, arrFields, ref ipos); }
	}
	[Obsolete("DEPRECEATED IFC2x2", false)]
	public partial class IfcElectricalElement : IfcElement  /* DEPRECEATED IFC2x2*/ {  	}
	public abstract partial class IfcElement : IfcProduct, IfcStructuralActivityAssignmentSelect //ABSTRACT SUPERTYPE OF (ONEOF(IfcBuildingElement,IfcCivilElement
	{ //,IfcDistributionElement,IfcElementAssembly,IfcElementComponent,IfcFeatureElement,IfcFurnishingElement,IfcGeographicElement,IfcTransportElement ,IfcVirtualElement,IfcElectricalElement SS,IfcEquipmentElement SS)) 
		private string mTag = "$";// : OPTIONAL IfcIdentifier;

		//INVERSE  
		internal List<IfcRelFillsElement> mFillsVoids = new List<IfcRelFillsElement>();// : SET [0:1] OF IfcRelFillsElement FOR RelatedBuildingElement;
		internal List<IfcRelConnectsElements> mConnectedTo = new List<IfcRelConnectsElements>();// : SET OF IfcRelConnectsElements FOR RelatingElement;
		internal List<IfcRelInterferesElements> mIsInterferedByElements = new List<IfcRelInterferesElements>();//	 :	SET OF IfcRelInterferesElements FOR RelatedElement;
		internal List<IfcRelInterferesElements> mInterferesElements = new List<IfcRelInterferesElements>();// :	SET OF IfcRelInterferesElements FOR RelatingElement;
		internal List<IfcRelProjectsElement> mHasProjections = new List<IfcRelProjectsElement>();// : SET OF IfcRelProjectsElement FOR RelatingElement;
		internal List<IfcRelReferencedInSpatialStructure> mReferencedInStructures = new List<IfcRelReferencedInSpatialStructure>();//  : 	SET OF IfcRelReferencedInSpatialStructure FOR RelatedElements;
		internal List<IfcRelVoidsElement> mHasOpenings = new List<IfcRelVoidsElement>(); //: SET [0:?] OF IfcRelVoidsElement FOR RelatingBuildingElement;
		internal List<IfcRelConnectsWithRealizingElements> mIsConnectionRealization = new List<IfcRelConnectsWithRealizingElements>();//	 : 	SET OF IfcRelConnectsWithRealizingElements FOR RealizingElements;
		internal List<IfcRelSpaceBoundary> mProvidesBoundaries = new List<IfcRelSpaceBoundary>();//	 : 	SET OF IfcRelSpaceBoundary FOR RelatedBuildingElement;
		internal List<IfcRelConnectsElements> mConnectedFrom = new List<IfcRelConnectsElements>();//	 : 	SET OF IfcRelConnectsElements FOR RelatedElement;
		internal IfcRelContainedInSpatialStructure mContainedInStructure = null;
		internal List<IfcRelConnectsStructuralActivity> mAssignedStructuralActivity = new List<IfcRelConnectsStructuralActivity>();//: 	SET OF IfcRelConnectsStructuralActivity FOR RelatingElement;

		internal List<IfcRelCoversBldgElements> mHasCoverings = new List<IfcRelCoversBldgElements>();// : SET OF IfcRelCoversBldgElements FOR RelatingBuildingElement; DEL IFC4
		internal List<IfcRelConnectsPortToElement> mHasPorts = new List<IfcRelConnectsPortToElement>();// :	SET OF IfcRelConnectsPortToElement FOR RelatedElement; DEL IFC4

		internal List<IfcRelConnectsStructuralElement> mHasStructuralMember = new List<IfcRelConnectsStructuralElement>();// DEL IFC4	 : 	SET OF IfcRelConnectsStructuralElement FOR RelatingElement;

		public string Tag { get { return mTag == "$" ? "" : ParserIfc.Decode(mTag); } set { mTag = string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value); } }
		public ReadOnlyCollection<IfcRelFillsElement> FillsVoids { get { return new ReadOnlyCollection<IfcRelFillsElement>(mFillsVoids); } }
		public ReadOnlyCollection<IfcRelConnectsElements> ConnectedTo { get { return new ReadOnlyCollection<IfcRelConnectsElements>(mConnectedTo); } }
		public ReadOnlyCollection<IfcRelInterferesElements> IsInterferedByElements { get { return new ReadOnlyCollection<IfcRelInterferesElements>(mIsInterferedByElements); } }
		public ReadOnlyCollection<IfcRelInterferesElements> InterferesElements { get { return new ReadOnlyCollection<IfcRelInterferesElements>(mInterferesElements); } }
		public ReadOnlyCollection<IfcRelProjectsElement> HasProjections { get { return new ReadOnlyCollection<IfcRelProjectsElement>(mHasProjections); } }
		public ReadOnlyCollection<IfcRelReferencedInSpatialStructure> ReferencedInStructures { get { return new ReadOnlyCollection<IfcRelReferencedInSpatialStructure>(mReferencedInStructures); } }
		public ReadOnlyCollection<IfcRelVoidsElement> HasOpenings { get { return new ReadOnlyCollection<IfcRelVoidsElement>(mHasOpenings); } }
		public ReadOnlyCollection<IfcRelConnectsWithRealizingElements> IsConnectionRealization { get { return new ReadOnlyCollection<IfcRelConnectsWithRealizingElements>(mIsConnectionRealization); } }
		public ReadOnlyCollection<IfcRelSpaceBoundary> ProvidesBoundaries { get { return new ReadOnlyCollection<IfcRelSpaceBoundary>(mProvidesBoundaries); } }
		public ReadOnlyCollection<IfcRelConnectsElements> ConnectedFrom { get { return new ReadOnlyCollection<IfcRelConnectsElements>(mConnectedFrom); } }
		public IfcRelContainedInSpatialStructure ContainedinStructure { get { return mContainedInStructure; } }
		public ReadOnlyCollection<IfcRelConnectsStructuralActivity> AssignedStructuralActivity { get { return new ReadOnlyCollection<IfcRelConnectsStructuralActivity>(mAssignedStructuralActivity); } }

		public ReadOnlyCollection<IfcRelCoversBldgElements> HasCoverings { get { return new ReadOnlyCollection<IfcRelCoversBldgElements>(mHasCoverings); } }
		//GEOMGYM
		//List<IfcRelConnectsStructuralActivity> mAssignedStructuralActivity = new List<IfcRelConnectsStructuralActivity>();//: 	SET OF IfcRelConnectsStructuralActivity FOR RelatingElement;

		protected IfcElement() : base() { }
		protected IfcElement(IfcElement basis) : base(basis)
		{
			mTag = basis.mTag;
#warning todo finish inverse
		}
		protected IfcElement(DatabaseIfc db) : base(db) { }
		protected IfcElement(DatabaseIfc db, IfcElement e, bool downStream) : base(db, e, downStream)
		{
			mTag = e.mTag;
#warning todo finish inverse

			foreach (IfcRelVoidsElement ve in e.mHasOpenings)
			{
				IfcRelVoidsElement dve = db.Factory.Duplicate(ve) as IfcRelVoidsElement;
				dve.RelatingBuildingElement = this;
			}
			List<IfcRelConnectsElements> rces = e.ConnectedTo.ToList();
			rces.AddRange(e.ConnectedFrom);
			foreach (IfcRelConnectsElements ce in rces)
			{
				IfcElement relating = ce.RelatingElement, related = ce.RelatedElement;
				if (relating.mIndex != e.mIndex && db.Factory.mDuplicateMapping.ContainsKey(relating.mIndex))
				{
					IfcRelConnectsElements rce = db.Factory.Duplicate(ce,false) as IfcRelConnectsElements;
					rce.RelatedElement = this;
					rce.RelatedElement = db[db.Factory.mDuplicateMapping[relating.mIndex]] as IfcElement;
				}
				if (related.mIndex != e.mIndex && db.Factory.mDuplicateMapping.ContainsKey(related.mIndex))
				{
					IfcRelConnectsElements rce = db.Factory.Duplicate(ce,false) as IfcRelConnectsElements;
					rce.RelatingElement = this;
					rce.RelatingElement = db[db.Factory.mDuplicateMapping[related.mIndex]] as IfcElement;
				}
			}
			foreach (IfcRelVoidsElement ve in e.mHasOpenings)
			{
				IfcRelVoidsElement rv = db.Factory.Duplicate(ve) as IfcRelVoidsElement;
				rv.RelatingBuildingElement = this;
			}
			if (e.mContainedInStructure != null)
				(db.Factory.Duplicate(e.mContainedInStructure, false) as IfcRelContainedInSpatialStructure).addRelated(this);
			foreach (IfcRelConnectsStructuralActivity rcss in e.mAssignedStructuralActivity)
			{
				IfcRelConnectsStructuralActivity rc = db.Factory.Duplicate(rcss) as IfcRelConnectsStructuralActivity;
				rc.RelatingElement = this;
			}

			foreach (IfcRelConnectsStructuralElement rcse in e.mHasStructuralMember)
			{
				IfcRelConnectsStructuralElement rc = db.Factory.Duplicate(rcse) as IfcRelConnectsStructuralElement;
				rc.RelatingElement = this;
			}
		}
		protected IfcElement(IfcObjectDefinition host, IfcObjectPlacement p, IfcProductRepresentation r) : base(host, p, r) { }
		protected IfcElement(IfcProduct host, IfcMaterialProfileSetUsage profile, IfcAxis2Placement3D placement, double length) : base(host, new IfcLocalPlacement(host.Placement, placement), null)
		{
			List<IfcShapeModel> reps = new List<IfcShapeModel>();
			IfcCartesianPoint cp = new IfcCartesianPoint(mDatabase, 0, 0, length);
			IfcPolyline ipl = new IfcPolyline(mDatabase.Factory.Origin, cp);
			reps.Add(IfcShapeRepresentation.GetAxisRep(ipl));

			profile.Associates.addRelated(this);

			IfcMaterialProfileSet ps = profile.ForProfileSet;
			IfcMaterialProfileSetUsageTapering psut = profile as IfcMaterialProfileSetUsageTapering;
			if (psut != null)
				throw new Exception("Tapering Elements not implemented yet!");
			IfcProfileDef pd = null;
			if (ps.mCompositeProfile > 0)
				pd = ps.CompositeProfile;
			else
			{
				if (ps.mMaterialProfiles.Count > 0)
					pd = ps.MaterialProfiles[0].Profile;
				else
					throw new Exception("Profile not provided");
			}
			if (pd != null)
				reps.Add(new IfcShapeRepresentation(new IfcExtrudedAreaSolid(pd, pd.CalculateTransform(profile.CardinalPoint), length)));

			Representation = new IfcProductDefinitionShape(reps);

		}
		protected IfcElement(IfcProduct host, IfcMaterialProfileSetUsage profile, IfcAxis2Placement3D placement, Tuple<double, double> arcOrigin, double arcAngle) : base(host, new IfcLocalPlacement(host.Placement, placement), null)
		{
			IfcMaterialProfileSet ps = profile.ForProfileSet;
			profile.Associates.addRelated(this);
			IfcMaterialProfile mp = ps.MaterialProfiles[0];
			IfcProfileDef pd = mp.Profile;
			DatabaseIfc db = host.mDatabase;
			List<IfcShapeModel> reps = new List<IfcShapeModel>();
			double length = Math.Sqrt(Math.Pow(arcOrigin.Item1, 2) + Math.Pow(arcOrigin.Item2, 2)), angMultipler = 1 / mDatabase.mContext.UnitsInContext.getScaleSI(IfcUnitEnum.PLANEANGLEUNIT);
			Tuple<double, double> normal = new Tuple<double, double>(-arcOrigin.Item2 / length, arcOrigin.Item1 / length);
			reps.Add(IfcShapeRepresentation.GetAxisRep(new IfcTrimmedCurve(new IfcCircle(new IfcAxis2Placement3D(new IfcCartesianPoint(db, arcOrigin.Item1, arcOrigin.Item2, 0), new IfcDirection(db, normal.Item1, normal.Item2, 0), new IfcDirection(db, -arcOrigin.Item1, -arcOrigin.Item2, 0)), length), new IfcTrimmingSelect(0), new IfcTrimmingSelect(arcAngle * angMultipler), true, IfcTrimmingPreference.PARAMETER)));
			IfcAxis2Placement3D translation = pd.CalculateTransform(profile.CardinalPoint);
			Tuple<double, double, double> pt = translation.Location.Coordinates;
			IfcAxis1Placement axis = new IfcAxis1Placement(new IfcCartesianPoint(db, arcOrigin.Item1 - pt.Item1, arcOrigin.Item2 - pt.Item2), new IfcDirection(db, normal.Item1, normal.Item2));
			reps.Add(new IfcShapeRepresentation(new IfcRevolvedAreaSolid(pd, translation, axis, arcAngle * angMultipler)));
			Representation = new IfcProductDefinitionShape(reps);
		}
		protected static void parseFields(IfcElement e, List<string> arrFields, ref int ipos) { IfcProduct.parseFields(e, arrFields, ref ipos); e.mTag = arrFields[ipos++].Replace("'", ""); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + (mTag == "$" ? "$" : "'" + mTag + "'"); }

		protected override void Parse(string str, ref int pos, int len)
		{
			base.Parse(str, ref pos, len);
			mTag = ParserSTEP.StripString(str, ref pos, len);
		}
		public IfcMaterialSelect MaterialSelect
		{
			get { return GetMaterialSelect(); }
			set { this.setMaterial(value); }
		}
		protected override IfcMaterialSelect GetMaterialSelect()
		{
			IfcMaterialSelect m = base.GetMaterialSelect();
			if (m != null)
				return m;
			if (IsTypedBy != null)
			{
				IfcElementType t = RelatingType as IfcElementType;
				if (t != null)
					return t.MaterialSelect;
			}
			return null;
		}

		internal IfcProduct getContainer()
		{
			if (mDecomposes != null)
				return mDecomposes.RelatingObject as IfcProduct;
			return (mContainedInStructure != null ? mContainedInStructure.RelatingStructure : null);
		}
		internal override void detachFromHost()
		{
			base.detachFromHost();
			if (mContainedInStructure != null)
				mContainedInStructure.removeObject(this);
		}
		internal static IfcElement constructElement(string className, IfcObjectDefinition container, IfcObjectPlacement pl, IfcProductRepresentation r) { return constructElement(className, container, pl, r, null); }
		internal static IfcElement constructElement(string className, IfcObjectDefinition host, IfcObjectPlacement pl, IfcProductRepresentation r, IfcDistributionSystem system)
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
			IfcElement element = null;
			Type type = Type.GetType("GeometryGym.Ifc." + str);
			if (type != null)
			{
				ConstructorInfo ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
null, new[] { typeof(IfcObjectDefinition), typeof(IfcObjectPlacement), typeof(IfcProductRepresentation) }, null);
				if (ctor == null)
				{
					ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
null, new[] { typeof(IfcObjectDefinition), typeof(IfcObjectPlacement), typeof(IfcProductRepresentation), typeof(IfcDistributionSystem) }, null);
					if (ctor == null)
						throw new Exception("XXX Unrecognized Ifc Constructor for " + className);
					else
						element = ctor.Invoke(new object[] { host, pl, r, system }) as IfcElement;
				}
				else
					element = ctor.Invoke(new object[] { host, pl, r }) as IfcElement;

			}
			if (element == null)
				element = new IfcBuildingElementProxy(host, pl, r);

			if (!string.IsNullOrEmpty(definedType))
			{
				if (host.mDatabase.mRelease == ReleaseVersion.IFC2x3)
					element.ObjectType = definedType;
				else
				{
					type = element.GetType();
					PropertyInfo pi = type.GetProperty("PredefinedType");
					if (pi != null)
					{
						Type enumType = Type.GetType("GeometryGym.Ifc." + type.Name + "TypeEnum");
						if (enumType != null)
						{
							FieldInfo fi = enumType.GetField(definedType);
							if (fi == null)
							{
								element.ObjectType = definedType;
								fi = enumType.GetField("NOTDEFINED");
							}
							if (fi != null)
							{
								int i = (int)fi.GetValue(enumType);
								object newEnumValue = Enum.ToObject(enumType, i);
								pi.SetValue(element, newEnumValue, null);
							}
							else
								element.ObjectType = definedType;
						}
						else
							element.ObjectType = definedType;
					}
					else
						element.ObjectType = definedType;
				}
			}
			return element;
		}
		public void AddMember(IfcStructuralMember m)
		{
			if (m == null)
				return;
			if (mDatabase.mRelease == ReleaseVersion.IFC2x3)
			{
				mHasStructuralMember.Add(new IfcRelConnectsStructuralElement(this, m));
			}
			else
			{
				string s = "Analytic Elements";
				foreach (IfcRelAssignsToProduct ra in mReferencedBy)
				{
					if (string.Compare(ra.Name, s, true) == 0)
					{
						if (!ra.mRelatedObjects.Contains(m.mIndex))
							ra.mRelatedObjects.Add(m.mIndex);
						return;
					}
				}
				IfcRelAssignsToProduct rap = new IfcRelAssignsToProduct(m, this) { Name = s };
			}
		}
		public void AssignStructuralActivity(IfcRelConnectsStructuralActivity connects) { mAssignedStructuralActivity.Add(connects); }
	}
	public abstract partial class IfcElementarySurface : IfcSurface //	ABSTRACT SUPERTYPE OF(ONEOF(IfcCylindricalSurface, IfcPlane))
	{
		private int mPosition;// : IfcAxis2Placement3D; 
		public IfcAxis2Placement3D Position { get { return mDatabase[mPosition] as IfcAxis2Placement3D; } set { mPosition = value.mIndex; } }

		protected IfcElementarySurface() : base() { }
		protected IfcElementarySurface(DatabaseIfc db, IfcElementarySurface s) : base(db,s) { Position = db.Factory.Duplicate(s.Position) as IfcAxis2Placement3D; }
		protected IfcElementarySurface(IfcAxis2Placement3D placement) : base(placement.mDatabase) { mPosition = placement.mIndex; }
		protected virtual void Parse(string str, ref int pos, int len) { mPosition = ParserSTEP.StripLink(str, ref pos, len); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mPosition); }
	}
	public partial class IfcElementAssembly : IfcElement
	{
		//GG
		private IfcObjectDefinition mHost = null;

		internal IfcAssemblyPlaceEnum mAssemblyPlace = IfcAssemblyPlaceEnum.NOTDEFINED;//: OPTIONAL IfcAssemblyPlaceEnum;
		internal IfcElementAssemblyTypeEnum mPredefinedType = IfcElementAssemblyTypeEnum.NOTDEFINED;//: OPTIONAL IfcElementAssemblyTypeEnum;
		public IfcAssemblyPlaceEnum AssemblyPlace { get { return mAssemblyPlace; } set { mAssemblyPlace = value; } }
		public IfcElementAssemblyTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElementAssembly() : base() { }
		internal IfcElementAssembly(DatabaseIfc db, IfcElementAssembly a, bool downStream) : base(db,a,downStream) { mPredefinedType = a.mPredefinedType; }
		public IfcElementAssembly(IfcObjectDefinition host, IfcAssemblyPlaceEnum place, IfcElementAssemblyTypeEnum type) : base(host.mDatabase) { mHost = host; AssemblyPlace = place; PredefinedType = type; }
		 
		internal static IfcElementAssembly Parse(string strDef) { IfcElementAssembly a = new IfcElementAssembly(); int ipos = 0; parseFields(a, ParserSTEP.SplitLineFields(strDef), ref ipos); return a; }
		internal static void parseFields(IfcElementAssembly a, List<string> arrFields, ref int ipos)
		{
			IfcElement.parseFields(a, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str.StartsWith("."))
				a.mAssemblyPlace = (IfcAssemblyPlaceEnum)Enum.Parse(typeof(IfcAssemblyPlaceEnum), str.Replace(".", ""));
			str = arrFields[ipos++];
			if (str.StartsWith("."))
				a.mPredefinedType = (IfcElementAssemblyTypeEnum)Enum.Parse(typeof(IfcElementAssemblyTypeEnum), str.Replace(".", ""));
		}
		protected override string BuildStringSTEP()
		{
			bool empty = Representation == null;
			if (empty)
			{
				for (int icounter = 0; icounter < mIsDecomposedBy.Count; icounter++)
				{
					if (mIsDecomposedBy[icounter].mRelatedObjects.Count > 0)
					{
						empty = false;
						break;
					}
				}
			}
			return (empty ? "" : base.BuildStringSTEP() + ",." + mAssemblyPlace.ToString() + ".,." + mPredefinedType.ToString() + ".");
		}
		protected override bool addProduct(IfcProduct product)
		{
			if (mIsDecomposedBy.Count == 0 || mIsDecomposedBy[0].mRelatedObjects.Count == 0)
			{
				IfcProduct hostproduct = mHost as IfcProduct;
				if(hostproduct != null)
					hostproduct.AddElement(this);
				else
				{
					new IfcRelAggregates(mHost, this);
				}
			}
			return base.addProduct(product);
		}
	}
	public partial class IfcElementAssemblyType : IfcElementType //IFC4
	{
		internal IfcElementAssemblyTypeEnum mPredefinedType = IfcElementAssemblyTypeEnum.NOTDEFINED;// IfcElementAssemblyTypeEnum; 
		public IfcElementAssemblyTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcElementAssemblyType() : base() { }
		internal IfcElementAssemblyType(DatabaseIfc db, IfcElementAssemblyType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		public IfcElementAssemblyType(DatabaseIfc m, string name, IfcElementAssemblyTypeEnum type) : base(m) { Name = name; mPredefinedType = type; if (m.mRelease == ReleaseVersion.IFC2x3) throw new Exception(KeyWord + " only supported in IFC4!"); }
		internal new static IfcElementAssemblyType Parse(string strDef) { IfcElementAssemblyType t = new IfcElementAssemblyType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		internal static void parseFields(IfcElementAssemblyType t, List<string> arrFields, ref int ipos)
		{
			IfcElementType.parseFields(t, arrFields, ref ipos);
			string s = arrFields[ipos++];
			if (s.StartsWith("."))
				t.mPredefinedType = (IfcElementAssemblyTypeEnum)Enum.Parse(typeof(IfcElementAssemblyTypeEnum), s.Replace(".", ""));
		}
		protected override string BuildStringSTEP() { return (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."); }
	}
	public abstract partial class IfcElementComponent : IfcElement //	ABSTRACT SUPERTYPE OF(ONEOF(IfcBuildingElementPart, IfcDiscreteAccessory, IfcFastener, IfcMechanicalFastener, IfcReinforcingElement, IfcVibrationIsolator))
	{
		protected IfcElementComponent() : base() { }
		protected IfcElementComponent(DatabaseIfc db, IfcElementComponent c) : base(db, c,false) { }
		protected IfcElementComponent(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation) : base(host,placement,representation) { }
		
		protected static void parseFields(IfcElementComponent c, List<string> arrFields, ref int ipos) { IfcElement.parseFields(c, arrFields, ref ipos); }
	}
	public abstract partial class IfcElementComponentType : IfcElementType // ABSTRACT SUPERTYPE OF (ONEOF	((IfcBuildingElementPartType, IfcDiscreteAccessoryType, IfcFastenerType, IfcMechanicalFastenerType, IfcReinforcingElementType, IfcVibrationIsolatorType)))
	{
		protected IfcElementComponentType() : base() { }
		protected IfcElementComponentType(DatabaseIfc db, IfcElementComponentType t) : base(db, t) { }
		protected IfcElementComponentType(DatabaseIfc db) : base(db) { }
		protected static void parseFields(IfcElementComponentType t, List<string> arrFields, ref int ipos) { IfcElementType.parseFields(t, arrFields, ref ipos); }
	}
	public partial class IfcElementQuantity : IfcQuantitySet
	{
		public override string KeyWord { get { return "IfcElementQuantity"; } }
		internal string mMethodOfMeasurement = "$";// : OPTIONAL IfcLabel;
		internal List<int> mQuantities = new List<int>();// : SET [1:?] OF IfcPhysicalQuantity; 

		public string MethodOfMeasurement { get { return (mMethodOfMeasurement == "$" ? "" : ParserIfc.Decode(mMethodOfMeasurement)); } set { mMethodOfMeasurement = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public ReadOnlyCollection<IfcPhysicalQuantity> Quantities { get { return new ReadOnlyCollection<IfcPhysicalQuantity>(mQuantities.ConvertAll(x => mDatabase[x] as IfcPhysicalQuantity)); } }

		internal IfcElementQuantity() : base() { }
		internal IfcElementQuantity(DatabaseIfc db, IfcElementQuantity q) : base(db, q) { mMethodOfMeasurement = q.mMethodOfMeasurement;  q.Quantities.ToList().ForEach(x => addQuantity( db.Factory.Duplicate(x) as IfcPhysicalQuantity)); }
		protected IfcElementQuantity(IfcObjectDefinition obj) : base(obj.mDatabase,"") { Name = this.GetType().Name; DefinesOccurrence.AddRelated(obj); }
		protected IfcElementQuantity(IfcTypeObject type) : base(type.mDatabase,"") { Name = this.GetType().Name; type.AddPropertySet(this); }
		public IfcElementQuantity(DatabaseIfc db, string name) : base(db, name) { }
		public IfcElementQuantity(string name, IfcPhysicalQuantity quantity) : base(quantity.mDatabase, name) { addQuantity(quantity); }
		public IfcElementQuantity(string name, IEnumerable<IfcPhysicalQuantity> quantities) : base(quantities.First().mDatabase, name) { foreach(IfcPhysicalQuantity q in quantities) addQuantity(q); }
		internal static IfcElementQuantity Parse(string strDef) { IfcElementQuantity q = new IfcElementQuantity(); int ipos = 0; parseFields(q, ParserSTEP.SplitLineFields(strDef), ref ipos); return q; }
		internal static void parseFields(IfcElementQuantity q, List<string> arrFields, ref int ipos) { IfcPropertySetDefinition.parseFields(q, arrFields, ref ipos); q.mMethodOfMeasurement = arrFields[ipos++].Replace("'", ""); q.mQuantities = ParserSTEP.SplitListLinks(arrFields[ipos++]); }
		protected override string BuildStringSTEP()
		{
			string str = base.BuildStringSTEP() + (mMethodOfMeasurement == "$" ? ",$,(" : ",'" + mMethodOfMeasurement + "',(") + (mQuantities.Count > 0 ? "#" + mQuantities[0] : "");
			for (int icounter = 1; icounter < mQuantities.Count; icounter++)
				str += ",#" + mQuantities[icounter];
			return str + ")";
		}
		internal override List<IBaseClassIfc> retrieveReference(IfcReference reference)
		{
			IfcReference ir = reference.InnerReference;
			List<IBaseClassIfc> result = new List<IBaseClassIfc>();
			if (ir == null)
			{
				if (string.Compare(reference.mAttributeIdentifier, "Quantities", true) == 0)
				{
					ReadOnlyCollection<IfcPhysicalQuantity> quants = Quantities;
					if (reference.mListPositions.Count == 0)
					{
						string name = reference.InstanceName;
						if (!string.IsNullOrEmpty(name))
						{
							foreach (IfcPhysicalQuantity q in quants)
							{
								if (string.Compare(q.Name, name) == 0)
									result.Add(q);
							}
						}
						else
							result.AddRange(quants);
					}
					else
					{
						foreach (int i in reference.mListPositions)
						{
							result.Add(quants[i - 1]);
						}
					}
					return result;
				}
			}
			if (string.Compare(reference.mAttributeIdentifier, "Quantities", true) == 0)
			{
				ReadOnlyCollection<IfcPhysicalQuantity> quants = Quantities;
				if (reference.mListPositions.Count == 0)
				{
					string name = reference.InstanceName;

					if (string.IsNullOrEmpty(name))
					{
						foreach (IfcPhysicalQuantity q in quants)
							result.AddRange(q.retrieveReference(reference.InnerReference));
					}
					else
					{
						foreach (IfcPhysicalQuantity q in quants)
						{
							if (string.Compare(name, q.Name) == 0)
								result.AddRange(q.retrieveReference(reference.InnerReference));
						}
					}
				}
				else
				{
					foreach (int i in reference.mListPositions)
						result.AddRange(quants[i - 1].retrieveReference(ir));
				}
				return result;
			}
			return base.retrieveReference(reference);
		}
		internal override bool isEmpty { get { return mQuantities.Count == 0; } }

		internal void addQuantity(IfcPhysicalQuantity quantity) { mQuantities.Add(quantity.mIndex); }
	}
	public abstract partial class IfcElementType : IfcTypeProduct //ABSTRACT SUPERTYPE OF(ONEOF(IfcBuildingElementType, IfcDistributionElementType, IfcElementAssemblyType, IfcElementComponentType, IfcFurnishingElementType, IfcGeographicElementType, IfcTransportElementType))
	{
		private string mElementType = "$";// : OPTIONAL IfcLabel
		public string ElementType { get { return mElementType == "$" ? "" : ParserIfc.Decode( mElementType); } set { mElementType = string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode( value); } }

		protected IfcElementType() : base() { }
		protected IfcElementType(IfcElementType basis) : base(basis) { mElementType = basis.mElementType; }
		protected IfcElementType(DatabaseIfc db, IfcElementType t) : base(db,t) { mElementType = t.mElementType; }
		protected IfcElementType(DatabaseIfc db) : base(db) { }
		protected static void parseFields(IfcElementType t, List<string> arrFields, ref int ipos) { IfcTypeProduct.parseFields(t, arrFields, ref ipos); t.mElementType = arrFields[ipos++].Replace("'", ""); }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 && (this as IfcDoorType != null || this as IfcWindowType != null) ? "" : (mElementType == "$" ? ",$" : ",'" + mElementType + "'"));
		}

		public IfcMaterialSelect MaterialSelect
		{
			get { return GetMaterialSelect(); }
			set { base.setMaterial(value); }
		}

		public IfcElement GenerateMappedItemElement(IfcProduct container, IfcCartesianTransformationOperator transform)  
		{
			string typename = this.GetType().Name;
			typename = typename.Substring(0, typename.Length - 4);
			IfcProductDefinitionShape pds = new IfcProductDefinitionShape(new IfcShapeRepresentation(new IfcMappedItem(RepresentationMaps[0], transform)));
			IfcElement element = IfcElement.constructElement(typename, container,null, pds);
			element.RelatingType = this;
			return element;
		}

	}
	public partial class IfcEllipse : IfcConic
	{
		internal double mSemiAxis1;// : IfcPositiveLengthMeasure;
		internal double mSemiAxis2;// : IfcPositiveLengthMeasure;

		public double SemiAxis1 { get { return mSemiAxis1; } set { mSemiAxis1 = value; } }
		public double SemiAxis2 { get { return mSemiAxis2; } set { mSemiAxis2 = value; } }

		internal IfcEllipse() : base() { }
		internal IfcEllipse(DatabaseIfc db, IfcEllipse e) : base(db,e) { mSemiAxis1 = e.mSemiAxis1; mSemiAxis2 = e.mSemiAxis2; }
		public IfcEllipse(IfcAxis2Placement placement, double axis1, double axis2) : base(placement) { mSemiAxis1 = axis1; mSemiAxis2 = axis2; }
		internal static IfcEllipse Parse(string str) { IfcEllipse e = new IfcEllipse(); int pos = 0; e.Parse(str, ref pos, str.Length); return e; }
		internal override void Parse(string str, ref int pos, int len)
		{
			base.Parse(str, ref pos, len);
			mSemiAxis1 = ParserSTEP.StripDouble(str, ref pos, len);
			mSemiAxis2 = ParserSTEP.StripDouble(str, ref pos, len);
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.DoubleToString(mSemiAxis1) + "," + ParserSTEP.DoubleToString(mSemiAxis2); }
	}
	public partial class IfcEllipseProfileDef : IfcParameterizedProfileDef
	{
		internal double mSemiAxis1;// : IfcPositiveLengthMeasure;
		internal double mSemiAxis2;// : IfcPositiveLengthMeasure;

		internal IfcEllipseProfileDef() : base() { }
		internal IfcEllipseProfileDef(DatabaseIfc db, IfcEllipseProfileDef p) : base(db, p) { mSemiAxis1 = p.mSemiAxis1; mSemiAxis2 = p.mSemiAxis2; }
	
		internal static void parseFields(IfcEllipseProfileDef p, List<string> arrFields, ref int ipos) { IfcParameterizedProfileDef.parseFields(p, arrFields, ref ipos); p.mSemiAxis1 = ParserSTEP.ParseDouble(arrFields[ipos++]); p.mSemiAxis2 = ParserSTEP.ParseDouble(arrFields[ipos++]); }
		internal new static IfcEllipseProfileDef Parse(string strDef) { IfcEllipseProfileDef p = new IfcEllipseProfileDef(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos); return p; }
	}
	public partial class IfcEnergyConversionDevice : IfcDistributionFlowElement //IFC4 Abstract
	{  //	SUPERTYPE OF(ONEOF(IfcAirToAirHeatRecovery, IfcBoiler, IfcBurner, IfcChiller, IfcCoil, IfcCondenser, IfcCooledBeam, 
		//IfcCoolingTower, IfcElectricGenerator, IfcElectricMotor, IfcEngine, IfcEvaporativeCooler, IfcEvaporator, IfcHeatExchanger,
		//IfcHumidifier, IfcMotorConnection, IfcSolarDevice, IfcTransformer, IfcTubeBundle, IfcUnitaryEquipment))
		public override string KeyWord { get { return mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "IfcEnergyConversionDevice" : base.KeyWord; } }

		internal IfcEnergyConversionDevice() : base() { }
		internal IfcEnergyConversionDevice(DatabaseIfc db, IfcEnergyConversionDevice d) : base(db, d) { }
		public IfcEnergyConversionDevice(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcEnergyConversionDevice d, List<string> arrFields, ref int ipos) { IfcDistributionFlowElement.parseFields(d, arrFields, ref ipos); }
		internal new static IfcEnergyConversionDevice Parse(string strDef) { IfcEnergyConversionDevice d = new IfcEnergyConversionDevice(); int ipos = 0; parseFields(d, ParserSTEP.SplitLineFields(strDef), ref ipos); return d; }
	}
	public abstract partial class IfcEnergyConversionDeviceType : IfcDistributionFlowElementType
	{ //ABSTRACT SUPERTYPE OF (ONEOF (IfcAirToAirHeatRecoveryType ,IfcBoilerType, Ifctype ,IfcChillerType ,IfcCoilType ,IfcCondenserType ,IfcCooledBeamType
		//,IfcCoolingTowerType ,IfcElectricGeneratorType ,IfcElectricMotorType ,IfcEvaporativeCoolerType ,IfcEvaporatorType ,IfcHeatExchangerType
		//,IfcHumidifierType ,IfcMotorConnectionType ,IfcSpaceHeaterType ,IfcTransformerType ,IfcTubeBundleType ,IfcUnitaryEquipmentType))
		protected IfcEnergyConversionDeviceType() : base() { }
		protected IfcEnergyConversionDeviceType(DatabaseIfc db) : base(db) { }
		protected IfcEnergyConversionDeviceType(DatabaseIfc db, IfcEnergyConversionDeviceType t) : base(db, t) { }
		protected static void parseFields(IfcEnergyConversionDeviceType t, List<string> arrFields, ref int ipos) { IfcDistributionFlowElementType.parseFields(t, arrFields, ref ipos); }
	}
	//[Obsolete("DEPRECEATED IFC4", false)]
	//IfcEnergyProperties // DEPRECEATED IFC4
	public partial class IfcEngine : IfcEnergyConversionDevice //IFC4
	{
		internal IfcEngineTypeEnum mPredefinedType = IfcEngineTypeEnum.NOTDEFINED;// OPTIONAL : IfcEngineTypeEnum;
		public IfcEngineTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcEngine() : base() { }
		internal IfcEngine(DatabaseIfc db, IfcEngine e) : base(db, e) { mPredefinedType = e.mPredefinedType; }
		public IfcEngine(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcEngine s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcEngineTypeEnum)Enum.Parse(typeof(IfcEngineTypeEnum), str);
		}
		internal new static IfcEngine Parse(string strDef) { IfcEngine s = new IfcEngine(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcEngineTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcEngineType : IfcEnergyConversionDeviceType
	{
		internal IfcEngineTypeEnum mPredefinedType = IfcEngineTypeEnum.NOTDEFINED;// : IfcEngineTypeEnum;
		public IfcEngineTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcEngineType() : base() { }
		internal IfcEngineType(DatabaseIfc db, IfcEngineType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal static void parseFields(IfcEngineType t, List<string> arrFields, ref int ipos) { IfcEnergyConversionDeviceType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcEngineTypeEnum)Enum.Parse(typeof(IfcEngineTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcEngineType Parse(string strDef) { IfcEngineType t = new IfcEngineType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcEnvironmentalImpactValue : IfcAppliedValue //DEPRECEATED
	{
		internal string mImpactType;// : IfcLabel;
		internal IfcEnvironmentalImpactCategoryEnum mEnvCategory = IfcEnvironmentalImpactCategoryEnum.NOTDEFINED;// IfcEnvironmentalImpactCategoryEnum
		internal string mUserDefinedCategory = "$";//  : OPTIONAL IfcLabel;
		internal IfcEnvironmentalImpactValue() : base() { }
		internal IfcEnvironmentalImpactValue(DatabaseIfc db, IfcEnvironmentalImpactValue v) : base(db,v) { mImpactType = v.mImpactType; mEnvCategory = v.mEnvCategory; mUserDefinedCategory = v.mUserDefinedCategory; }
		internal new static IfcEnvironmentalImpactValue Parse(string strDef, ReleaseVersion schema) { IfcEnvironmentalImpactValue v = new IfcEnvironmentalImpactValue(); int ipos = 0; parseFields(v, ParserSTEP.SplitLineFields(strDef), ref ipos, schema); return v; }
		internal static void parseFields(IfcEnvironmentalImpactValue v, List<string> arrFields, ref int ipos, ReleaseVersion schema) { IfcAppliedValue.parseFields(v, arrFields, ref ipos, schema); v.mImpactType = arrFields[ipos++]; v.mEnvCategory = (IfcEnvironmentalImpactCategoryEnum)Enum.Parse(typeof(IfcEnvironmentalImpactCategoryEnum), arrFields[ipos++].Replace(".", "")); v.mUserDefinedCategory = arrFields[ipos++]; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + mImpactType + ",." + mEnvCategory.ToString() + ".," + mUserDefinedCategory; }
	}
	
	[Obsolete("DEPRECEATED IFC2x2", false)]
	public partial class IfcEquipmentElement : IfcElement  
	{
		internal IfcEquipmentElement() : base() { }
		internal IfcEquipmentElement(DatabaseIfc db, IfcEquipmentElement e) : base(db,e,false) { }
		internal static IfcEquipmentElement Parse(string strDef) { IfcEquipmentElement e = new IfcEquipmentElement(); int ipos = 0; parseFields(e, ParserSTEP.SplitLineFields(strDef), ref ipos); return e; }
		internal static void parseFields(IfcCivilElement e, List<string> arrFields, ref int ipos) { IfcElement.parseFields(e, arrFields, ref ipos); }
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcEquipmentStandard : IfcControl 
	{
		internal IfcEquipmentStandard() : base() { }
		internal IfcEquipmentStandard(DatabaseIfc db, IfcEquipmentStandard s) : base(db,s) { }
		internal static IfcEquipmentStandard Parse(string strDef, ReleaseVersion schema) { IfcEquipmentStandard s = new IfcEquipmentStandard(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos,schema); return s; }
	}
	public partial class IfcEvaporativeCooler : IfcEnergyConversionDevice //IFC4
	{
		internal IfcEvaporativeCoolerTypeEnum mPredefinedType = IfcEvaporativeCoolerTypeEnum.NOTDEFINED;// OPTIONAL : IfcEvaporativeCoolerTypeEnum;
		public IfcEvaporativeCoolerTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcEvaporativeCooler() : base() { }
		internal IfcEvaporativeCooler(DatabaseIfc db, IfcEvaporativeCooler c) : base(db, c) { mPredefinedType = c.mPredefinedType; }
		public IfcEvaporativeCooler(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcEvaporativeCooler s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcEvaporativeCoolerTypeEnum)Enum.Parse(typeof(IfcEvaporativeCoolerTypeEnum), str);
		}
		internal new static IfcEvaporativeCooler Parse(string strDef) { IfcEvaporativeCooler s = new IfcEvaporativeCooler(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcEvaporativeCoolerTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcEvaporativeCoolerType : IfcEnergyConversionDeviceType
	{
		internal IfcEvaporativeCoolerTypeEnum mPredefinedType = IfcEvaporativeCoolerTypeEnum.NOTDEFINED;// : IfcEvaporativeCoolerTypeEnum;
		public IfcEvaporativeCoolerTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcEvaporativeCoolerType() : base() { }
		internal IfcEvaporativeCoolerType(DatabaseIfc db, IfcEvaporativeCoolerType t) : base(db,t) { mPredefinedType = t.mPredefinedType; }
		internal static void parseFields(IfcEvaporativeCoolerType t, List<string> arrFields, ref int ipos) { IfcEnergyConversionDeviceType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcEvaporativeCoolerTypeEnum)Enum.Parse(typeof(IfcEvaporativeCoolerTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcEvaporativeCoolerType Parse(string strDef) { IfcEvaporativeCoolerType t = new IfcEvaporativeCoolerType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	public partial class IfcEvaporator : IfcEnergyConversionDevice //IFC4
	{
		internal IfcEvaporatorTypeEnum mPredefinedType = IfcEvaporatorTypeEnum.NOTDEFINED;// OPTIONAL : IfcEvaporatorTypeEnum;
		public IfcEvaporatorTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcEvaporator() : base() { }
		internal IfcEvaporator(DatabaseIfc db, IfcEvaporator e) : base(db, e) { mPredefinedType = e.mPredefinedType; }
		public IfcEvaporator(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation, IfcDistributionSystem system) : base(host, placement, representation, system) { }

		internal static void parseFields(IfcEvaporator s, List<string> arrFields, ref int ipos)
		{
			IfcEnergyConversionDevice.parseFields(s, arrFields, ref ipos);
			string str = arrFields[ipos++];
			if (str[0] == '.')
				s.mPredefinedType = (IfcEvaporatorTypeEnum)Enum.Parse(typeof(IfcEvaporatorTypeEnum), str);
		}
		internal new static IfcEvaporator Parse(string strDef) { IfcEvaporator s = new IfcEvaporator(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }
		protected override string BuildStringSTEP()
		{
			return base.BuildStringSTEP() + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcEvaporatorTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}
	}
	public partial class IfcEvaporatorType : IfcEnergyConversionDeviceType
	{
		internal IfcEvaporatorTypeEnum mPredefinedType = IfcEvaporatorTypeEnum.NOTDEFINED;// : IfcEvaporatorTypeEnum;
		public IfcEvaporatorTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcEvaporatorType() : base() { }
		internal IfcEvaporatorType(DatabaseIfc db, IfcEvaporatorType t) : base(db,t) { mPredefinedType = t.mPredefinedType; }
		internal static void parseFields(IfcEvaporatorType t, List<string> arrFields, ref int ipos) { IfcEnergyConversionDeviceType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcEvaporatorTypeEnum)Enum.Parse(typeof(IfcEvaporatorTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcEvaporatorType Parse(string strDef) { IfcEvaporatorType t = new IfcEvaporatorType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	public partial class IfcEvent : IfcProcess //IFC4
	{
		internal IfcEventTypeEnum mPredefinedType = IfcEventTypeEnum.NOTDEFINED;// : IfcEventTypeEnum; 
		internal IfcEventTriggerTypeEnum mEventTriggerType = IfcEventTriggerTypeEnum.NOTDEFINED;// : IfcEventTypeEnum; 
		internal string mUserDefinedEventTriggerType = "$";//	:	OPTIONAL IfcLabel;
		internal int mEventOccurenceTime;//	:	OPTIONAL IfcEventTime;
		internal IfcEvent() : base() { }
		internal IfcEvent(DatabaseIfc db, IfcEvent e) : base(db,e) { mPredefinedType = e.mPredefinedType; mEventTriggerType = e.mEventTriggerType; mUserDefinedEventTriggerType = e.mUserDefinedEventTriggerType; }
		internal static void parseFields(IfcEvent e, List<string> arrFields, ref int ipos)
		{
			IfcProcess.parseFields(e, arrFields, ref ipos);
			e.mPredefinedType = (IfcEventTypeEnum)Enum.Parse(typeof(IfcEventTypeEnum), arrFields[ipos++].Replace(".", ""));
			e.mEventTriggerType = (IfcEventTriggerTypeEnum)Enum.Parse(typeof(IfcEventTriggerTypeEnum), arrFields[ipos++].Replace(".", ""));
			e.mUserDefinedEventTriggerType = arrFields[ipos++].Replace("'", "");
			e.mEventOccurenceTime = ParserSTEP.ParseLink(arrFields[ipos++]);
		}
		internal static IfcEvent Parse(string strDef) { IfcEvent t = new IfcEvent(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + ".,." + mEventTriggerType.ToString() + (mUserDefinedEventTriggerType == "$" ? ".,$" : (".,'" + mUserDefinedEventTriggerType + "'")) + "," + ParserSTEP.LinkToString(mEventOccurenceTime)); }
	}
	public partial class IfcEventType : IfcTypeProcess //IFC4
	{
		internal IfcEventTypeEnum mPredefinedType = IfcEventTypeEnum.NOTDEFINED;// : IfcEventTypeEnum; 
		internal IfcEventTriggerTypeEnum mEventTriggerType = IfcEventTriggerTypeEnum.NOTDEFINED;// : IfcEventTypeEnum; 
		internal string mUserDefinedEventTriggerType = "$";//	:	OPTIONAL IfcLabel;

		public IfcEventTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }
		public IfcEventTriggerTypeEnum EventTriggerType { get { return mEventTriggerType; } set { mEventTriggerType = value; } }
		public string UserDefinedEventTriggerType { get { return (mUserDefinedEventTriggerType == "$" ? "" : ParserIfc.Decode(mUserDefinedEventTriggerType)); } set { mUserDefinedEventTriggerType = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }

		internal IfcEventType() : base() { }
		internal IfcEventType(DatabaseIfc db, IfcEventType t) : base(db, t) { mPredefinedType = t.mPredefinedType; mEventTriggerType = t.mEventTriggerType; mUserDefinedEventTriggerType = t.mUserDefinedEventTriggerType; }
		internal IfcEventType(DatabaseIfc m, string name, IfcEventTypeEnum t, IfcEventTriggerTypeEnum trigger)
			: base(m) { Name = name; mPredefinedType = t; mEventTriggerType = trigger; }
		internal static void parseFields(IfcEventType t, List<string> arrFields, ref int ipos)
		{
			IfcTypeProcess.parseFields(t, arrFields, ref ipos);
			t.mPredefinedType = (IfcEventTypeEnum)Enum.Parse(typeof(IfcEventTypeEnum), arrFields[ipos++].Replace(".", ""));
			t.mEventTriggerType = (IfcEventTriggerTypeEnum)Enum.Parse(typeof(IfcEventTriggerTypeEnum), arrFields[ipos++].Replace(".", ""));
			t.mUserDefinedEventTriggerType = arrFields[ipos++].Replace("'", "");
		}
		internal new static IfcEventType Parse(string strDef) { IfcEventType t = new IfcEventType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + ".,." + mEventTriggerType.ToString() + (mUserDefinedEventTriggerType == "$" ? ".,$" : (".,'" + mUserDefinedEventTriggerType + "'"))); }
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcExtendedMaterialProperties : IfcMaterialPropertiesSuperSeded  // DEPRECEATED IFC4
	{
		internal List<int> mExtendedProperties = new List<int>(); //: SET [1:?] OF IfcProperty
		internal string mDescription = "$"; //: OPTIONAL IfcText;
		internal string mName; //: IfcLabel;

		public ReadOnlyCollection<IfcProperty> ExtendedProperties { get { return new ReadOnlyCollection<IfcProperty>( mExtendedProperties.ConvertAll(x => mDatabase[x] as IfcProperty)); } }
		public string Description { get { return (mDescription == "$" ? "" : ParserIfc.Decode(mDescription)); } set { mDescription = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public override string Name { get { return (mName == "$" ? "" : ParserIfc.Decode(mName)); } set { mName = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } } 

		internal IfcExtendedMaterialProperties() : base() { }
		internal IfcExtendedMaterialProperties(DatabaseIfc db, IfcExtendedMaterialProperties p) : base(db,p) { p.ExtendedProperties.ToList().ForEach(x=>addProperty( db.Factory.Duplicate(x) as IfcProperty)); mDescription = p.mDescription; mName = p.mName; }
		internal static IfcExtendedMaterialProperties Parse(string strDef) { IfcExtendedMaterialProperties p = new IfcExtendedMaterialProperties(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos); return p; }
		internal static void parseFields(IfcExtendedMaterialProperties p, List<string> arrFields, ref int ipos) { IfcMaterialPropertiesSuperSeded.parseFields(p, arrFields, ref ipos); p.mExtendedProperties = ParserSTEP.SplitListLinks(arrFields[ipos++]); p.mDescription = arrFields[ipos++]; p.mName = arrFields[ipos++]; }
		protected override string BuildStringSTEP()
		{
			string str = base.BuildStringSTEP() + ",(" + ParserSTEP.LinkToString(mExtendedProperties[0]);
			for (int icounter = 1; icounter < mExtendedProperties.Count; icounter++)
				str += "," + ParserSTEP.LinkToString(mExtendedProperties[icounter]);
			return str + ")" + "," + mDescription + "," + mName;
		}

		internal void addProperty(IfcProperty property) { mExtendedProperties.Add(property.mIndex); }
	}
	public abstract partial class IfcExtendedProperties : IfcPropertyAbstraction //IFC4 ABSTRACT SUPERTYPE OF (ONEOF (IfcMaterialProperties,IfcProfileProperties))
	{
		protected string mName = "$"; //: OPTIONAL IfcLabel;
		private string mDescription = "$"; //: OPTIONAL IfcText;
		internal Dictionary<string, IfcProperty> mProperties = new Dictionary<string, IfcProperty>();//: SET [1:?] OF IfcProperty 
		private List<int> mPropertyIndices = new List<int>();

		public override string Name { get { return (mName == "$" ? "" : ParserIfc.Decode(mName)); } set { mName = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public string Description { get { return (mDescription == "$" ? "" : ParserIfc.Decode(mDescription)); } set { mDescription = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public ReadOnlyDictionary<string, IfcProperty> Properties { get { return new ReadOnlyDictionary<string, IfcProperty>( mProperties); } }

		internal override void Initialize()
		{
			mProperties = new Dictionary<string, IfcProperty>();
			mPropertyIndices = new List<int>();
		}

		protected IfcExtendedProperties() : base() { }
		protected IfcExtendedProperties(DatabaseIfc db) : base(db) {  }
		protected IfcExtendedProperties(DatabaseIfc db, IfcExtendedProperties p) : base(db, p) { mName = p.mName; mDescription = p.mDescription; p.Properties.Values.ToList().ForEach(x => AddProperty( db.Factory.Duplicate(x) as IfcProperty));   }
		internal IfcExtendedProperties(string name, List<IfcProperty> props) : base(props[0].mDatabase)
		{
			Name = name;
			if (props != null)
				props.ForEach(x => AddProperty(x));
		}
		internal static void parseFields(IfcExtendedProperties p, List<string> arrFields, ref int ipos, ReleaseVersion schema)
		{
			IfcPropertyAbstraction.parseFields(p, arrFields, ref ipos);
			if (schema != ReleaseVersion.IFC2x3)
			{
				p.mName = arrFields[ipos++].Replace("'", "");
				p.mDescription = arrFields[ipos++].Replace("'", "");
				p.mPropertyIndices = ParserSTEP.SplitListLinks(arrFields[ipos++]);
			}
		}
		protected override string BuildStringSTEP()
		{
			if (mProperties.Count == 0)
				return "";
			if (mDatabase.mRelease == ReleaseVersion.IFC2x3)
				return base.BuildStringSTEP();
			string str = base.BuildStringSTEP() + (mName == "$" ? ",$," : ",'" + mName + "',") + (mDescription == "$" ? "$,(#" : "'" + mDescription + "',(#") + mPropertyIndices[0];
			for (int icounter = 1; icounter < mPropertyIndices.Count; icounter++)
				str += ",#" + mPropertyIndices[icounter];
			return str + ")";
		}

		internal override void postParseRelate()
		{
			base.postParseRelate();
			foreach (int i in mPropertyIndices)
			{
				IfcProperty p = mDatabase[i] as IfcProperty;
				if (p != null)
					mProperties.Add(p.Name, p);
			}
		}
		public void AddProperty(IfcProperty property)
		{
			if (property != null && !mProperties.ContainsKey(property.Name))
			{
				mProperties.Add(property.Name, property);
				mPropertyIndices.Add(property.Index);
			}
		}
		public void RemoveProperty(IfcProperty property)
		{
			if (property != null)
			{
				mProperties.Remove(property.Name);
				mPropertyIndices.Remove(property.Index);
			}
		}
		public IfcProperty FindProperty(string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;
			return (mProperties.ContainsKey(name) ? mProperties[name] : null);
		}
	}
	//ENTITY IfcExternallyDefinedHatchStyle
	public partial class IfcExternallyDefinedSurfaceStyle : IfcExternalReference, IfcSurfaceStyleElementSelect
	{
		internal IfcExternallyDefinedSurfaceStyle() : base() { }
		internal IfcExternallyDefinedSurfaceStyle(DatabaseIfc db, IfcExternallyDefinedSurfaceStyle s) : base(db, s) { }
		internal IfcExternallyDefinedSurfaceStyle(DatabaseIfc db) : base(db) { }
		internal static IfcExternallyDefinedSurfaceStyle Parse(string strDef) { IfcExternallyDefinedSurfaceStyle f = new IfcExternallyDefinedSurfaceStyle(); int ipos = 0; parseFields(f, ParserSTEP.SplitLineFields(strDef), ref ipos); return f; }
		internal static void parseFields(IfcExternallyDefinedSurfaceStyle f, List<string> arrFields, ref int ipos) { IfcExternalReference.parseFields(f, arrFields, ref ipos); }
	}
	//[Obsolete("DEPRECEATED IFC4", false)]
	//ENTITY IfcExternallyDefinedSymbol // DEPRECEATED IFC4
	internal class IfcExternallyDefinedTextFont : IfcExternalReference
	{
		internal IfcExternallyDefinedTextFont() : base() { }
		internal IfcExternallyDefinedTextFont(DatabaseIfc db, IfcExternallyDefinedTextFont f) : base(db, f) { }
		internal IfcExternallyDefinedTextFont(DatabaseIfc db) : base(db) { }
		internal static IfcExternallyDefinedTextFont Parse(string strDef) { IfcExternallyDefinedTextFont f = new IfcExternallyDefinedTextFont(); int ipos = 0; parseFields(f, ParserSTEP.SplitLineFields(strDef), ref ipos); return f; }
		internal static void parseFields(IfcExternallyDefinedTextFont f, List<string> arrFields, ref int ipos) { IfcExternalReference.parseFields(f, arrFields, ref ipos); }
	}
	public abstract partial class IfcExternalInformation : BaseClassIfc, IfcResourceObjectSelect // NEW IFC4	ABSTRACT SUPERTYPE OF(ONEOF(IfcClassification, IfcDocumentInformation, IfcLibraryInformation));
	{ //INVERSE
		internal List<IfcExternalReferenceRelationship> mHasExternalReferences = new List<IfcExternalReferenceRelationship>(); //IFC4
		internal List<IfcResourceConstraintRelationship> mHasConstraintRelationships = new List<IfcResourceConstraintRelationship>(); //gg

		public ReadOnlyCollection<IfcExternalReferenceRelationship> HasExternalReferences { get { return new ReadOnlyCollection<IfcExternalReferenceRelationship>( mHasExternalReferences); } }
		public ReadOnlyCollection<IfcResourceConstraintRelationship> HasConstraintRelationships { get { return new ReadOnlyCollection<IfcResourceConstraintRelationship>( mHasConstraintRelationships); } }

		protected IfcExternalInformation() : base() { }
		protected IfcExternalInformation(DatabaseIfc db) : base(db) { }
		protected IfcExternalInformation(DatabaseIfc db, IfcExternalInformation i) : base(db,i) { }
		protected static void parseFields(IfcExternalInformation r, List<string> arrFields, ref int ipos) { }

		public void AddExternalReferenceRelationship(IfcExternalReferenceRelationship referenceRelationship) { mHasExternalReferences.Add(referenceRelationship); }
		public void AddConstraintRelationShip(IfcResourceConstraintRelationship constraintRelationship) { mHasConstraintRelationships.Add(constraintRelationship); }
	}
	public abstract partial class IfcExternalReference : BaseClassIfc, IfcLightDistributionDataSourceSelect, IfcObjectReferenceSelect, IfcResourceObjectSelect//ABSTRACT SUPERTYPE OF (ONEOF (IfcClassificationReference ,IfcDocumentReference ,IfcExternallyDefinedHatchStyle
	{ //,IfcExternallyDefinedSurfaceStyle ,IfcExternallyDefinedSymbol ,IfcExternallyDefinedTextFont ,IfcLibraryReference)); 
		private string mLocation = "$";//  :	OPTIONAL IfcURIReference; ifc2x3 ifclabel
		private string mIdentification = "$";// : OPTIONAL IfcIdentifier; ifc2x3 ItemReference
		private string mName = "$";//  : OPTIONAL IfcLabel;
		//INVERSE  
		internal List<IfcExternalReferenceRelationship> mHasExternalReferences = new List<IfcExternalReferenceRelationship>(); //IFC4	public override string Name { get { return (mName == "$" ? "" : mName); } set { if (!string.IsNullOrEmpty(value)) mName = value; } } 
		internal List<IfcResourceConstraintRelationship> mHasConstraintRelationships = new List<IfcResourceConstraintRelationship>(); //gg
		internal List<IfcExternalReferenceRelationship> mExternalReferenceForResources = new List<IfcExternalReferenceRelationship>();//	:	SET [0:?] OF IfcExternalReferenceRelationship FOR RelatingReference;

		public string Location { get { return (mLocation == "$" ? "" : ParserIfc.Decode(mLocation)); } set { mLocation = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public string Identification { get { return (mIdentification == "$" ? "" : ParserIfc.Decode(mIdentification)); } set { mIdentification = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public override string Name { get { return (mName == "$" ? "" : ParserIfc.Decode(mName)); } set { mName = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public ReadOnlyCollection<IfcExternalReferenceRelationship> HasExternalReferences { get { return new ReadOnlyCollection<IfcExternalReferenceRelationship>( mHasExternalReferences); } }
		public ReadOnlyCollection<IfcResourceConstraintRelationship> HasConstraintRelationships { get { return new ReadOnlyCollection<IfcResourceConstraintRelationship>( mHasConstraintRelationships); } }
		public ReadOnlyCollection<IfcExternalReferenceRelationship> ExternalReferenceForResources { get { return new ReadOnlyCollection<IfcExternalReferenceRelationship>( mExternalReferenceForResources); } }

		protected IfcExternalReference() : base() { }
		protected IfcExternalReference(DatabaseIfc db, IfcExternalReference r) : base(db,r) { mLocation = r.mLocation; mIdentification = r.mIdentification; mName = r.mName; }
		protected IfcExternalReference(DatabaseIfc db) : base(db) { }
		protected static void parseFields(IfcExternalReference r, List<string> arrFields, ref int ipos)
		{
			r.mLocation = arrFields[ipos++].Replace("'", "");
			r.mIdentification = arrFields[ipos++].Replace("'", "");
			r.mName = arrFields[ipos++].Replace("'", "");
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + (mLocation == "$" ? ",$," : ",'" + mLocation + "',") + (mIdentification == "$" ? "$" : "'" + mIdentification + "'") + (mName == "$" ? ",$" : ",'" + mName + "'"); }

		public void AddExternalReferenceRelationship(IfcExternalReferenceRelationship referenceRelationship) { mHasExternalReferences.Add(referenceRelationship); }
		public void AddConstraintRelationShip(IfcResourceConstraintRelationship constraintRelationship) { mHasConstraintRelationships.Add(constraintRelationship); }
	}
	public partial class IfcExternalReferenceRelationship : IfcResourceLevelRelationship //IFC4
	{
		private int mRelatingReference;// :	IfcExternalReference;
		private List<int> mRelatedResourceObjects = new List<int>(); //	:	SET [1:?] OF IfcResourceObjectSelect;

		public IfcExternalReference RelatingReference { get { return mDatabase[mRelatingReference] as IfcExternalReference; } set { mRelatingReference = value.mIndex; value.mExternalReferenceForResources.Add(this); } }
		public ReadOnlyCollection<IfcResourceObjectSelect> RelatedResourceObjects { get { return new ReadOnlyCollection<IfcResourceObjectSelect>( mRelatedResourceObjects.ConvertAll(x => mDatabase[x] as IfcResourceObjectSelect)); } }

		//INVERSE
		public List<IfcExternalReferenceRelationship> HasExternalReferences { get { return mHasExternalReferences; } }
		internal List<IfcExternalReferenceRelationship> mHasExternalReferences = new List<IfcExternalReferenceRelationship>(); //IFC4
		internal IfcExternalReferenceRelationship() : base() { }
		internal IfcExternalReferenceRelationship(DatabaseIfc db, IfcExternalReferenceRelationship r) : base(db,r) { RelatingReference = db.Factory.Duplicate(r.RelatingReference) as IfcExternalReference; r.mRelatedResourceObjects.ToList().ForEach(x=>addRelated( db.Factory.Duplicate(r.mDatabase[x]) as IfcResourceObjectSelect)); }
		public IfcExternalReferenceRelationship(IfcExternalReference reference, IfcResourceObjectSelect related) : this(reference, new List<IfcResourceObjectSelect>() { related }) { }
		public IfcExternalReferenceRelationship(IfcExternalReference reference, List<IfcResourceObjectSelect> related)
			: base(reference.mDatabase) { mRelatingReference = reference.mIndex; related.ForEach(x=>addRelated(x)); }
		internal static IfcExternalReferenceRelationship Parse(string strDef, ReleaseVersion schema) { IfcExternalReferenceRelationship m = new IfcExternalReferenceRelationship(); int ipos = 0; parseFields(m, ParserSTEP.SplitLineFields(strDef), ref ipos,schema); return m; }
		internal static void parseFields(IfcExternalReferenceRelationship m, List<string> arrFields, ref int ipos, ReleaseVersion schema)
		{
			IfcResourceLevelRelationship.parseFields(m, arrFields, ref ipos,schema);
			m.mRelatingReference = ParserSTEP.ParseLink(arrFields[ipos++]);
			m.mRelatedResourceObjects = ParserSTEP.SplitListLinks(arrFields[ipos++]);
		}
		protected override string BuildStringSTEP()
		{
			if (mDatabase.mRelease == ReleaseVersion.IFC2x3)
				return "";
			string result = base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mRelatingReference) + ",(" + ParserSTEP.LinkToString(mRelatedResourceObjects[0]);
			for (int icounter = 1; icounter < mRelatedResourceObjects.Count; icounter++)
				result += "," + ParserSTEP.LinkToString(mRelatedResourceObjects[icounter]);
			return result + ")";

		}
		internal void addRelated(IfcResourceObjectSelect r)
		{
			mRelatedResourceObjects.Add(r.Index);
			r.AddExternalReferenceRelationship(this);
		}
		internal override void postParseRelate()
		{
			base.postParseRelate();
			foreach (IfcResourceObjectSelect ro in RelatedResourceObjects)
				ro.AddExternalReferenceRelationship(this);
			RelatingReference.mExternalReferenceForResources.Add(this);
		}
	}
	public partial class IfcExternalSpatialElement : IfcExternalSpatialStructureElement, IfcSpaceBoundarySelect //NEW IFC4
	{
		internal IfcExternalSpatialElementTypeEnum mPredefinedType = IfcExternalSpatialElementTypeEnum.NOTDEFINED;
		//INVERSE
		internal List<IfcRelSpaceBoundary> mBoundedBy = new List<IfcRelSpaceBoundary>();  //	BoundedBy : SET [0:?] OF IfcRelExternalSpatialElementBoundary FOR RelatingExternalSpatialElement;

		public IfcExternalSpatialElementTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }
		public ReadOnlyCollection<IfcRelSpaceBoundary> BoundedBy { get { return new ReadOnlyCollection<IfcRelSpaceBoundary>( mBoundedBy); } }

		internal IfcExternalSpatialElement() : base() { }
		internal IfcExternalSpatialElement(DatabaseIfc db, IfcExternalSpatialElement e, bool downStream) : base(db, e,downStream) { mPredefinedType = e.mPredefinedType; }
		internal IfcExternalSpatialElement(IfcSite host, string name, IfcExternalSpatialElementTypeEnum te)
			: base(host, name) { mPredefinedType = te; }
		internal static void parseFields(IfcExternalSpatialElement gp, List<string> arrFields, ref int ipos)
		{
			IfcSpatialStructureElement.parseFields(gp, arrFields, ref ipos);
			string s = arrFields[ipos++];
			if (s[0] == '.')
				gp.mPredefinedType = (IfcExternalSpatialElementTypeEnum)Enum.Parse(typeof(IfcExternalSpatialElementTypeEnum), s.Replace(".", ""));
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + (mPredefinedType == IfcExternalSpatialElementTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."); }
		internal static IfcExternalSpatialElement Parse(string strDef) { IfcExternalSpatialElement s = new IfcExternalSpatialElement(); int ipos = 0; parseFields(s, ParserSTEP.SplitLineFields(strDef), ref ipos); return s; }

		public void AddBoundary(IfcRelSpaceBoundary boundary) { mBoundedBy.Add(boundary); }
	}
	public abstract partial class IfcExternalSpatialStructureElement : IfcSpatialElement //	ABSTRACT SUPERTYPE OF(IfcExternalSpatialElement)
	{
		protected IfcExternalSpatialStructureElement() : base() { }
		protected IfcExternalSpatialStructureElement(IfcObjectPlacement pl) : base(pl) { }
		protected IfcExternalSpatialStructureElement(IfcSite host, string name) : base(host, name) { }
		protected IfcExternalSpatialStructureElement(DatabaseIfc db, IfcExternalSpatialStructureElement e, bool downStream) : base(db, e, downStream) { }
		protected static void parseFields(IfcExternalSpatialStructureElement s, List<string> arrFields, ref int ipos) { IfcSpatialElement.parseFields(s, arrFields, ref ipos); }
	}
	public partial class IfcExtrudedAreaSolid : IfcSweptAreaSolid // SUPERTYPE OF(IfcExtrudedAreaSolidTapered)
	{
		private int mExtrudedDirection;//: IfcDirection;
		private double mDepth;// : IfcPositiveLengthMeasure;

		public IfcDirection ExtrudedDirection { get { return mDatabase[mExtrudedDirection] as IfcDirection; } set { mExtrudedDirection = value.mIndex; } }
		public double Depth { get { return mDepth; } set { mDepth = value; } }

		internal IfcExtrudedAreaSolid() : base() { }
		internal IfcExtrudedAreaSolid(DatabaseIfc db, IfcExtrudedAreaSolid e) : base(db, e) { ExtrudedDirection = db.Factory.Duplicate(e.ExtrudedDirection) as IfcDirection; mDepth = e.mDepth; }
		public IfcExtrudedAreaSolid(IfcProfileDef prof, double depth) : base(prof) { ExtrudedDirection = mDatabase.Factory.ZAxis; mDepth = depth; }
		public IfcExtrudedAreaSolid(IfcProfileDef prof, IfcDirection dir, double depth) : base(prof) { mExtrudedDirection = dir.mIndex; mDepth = depth; }
		public IfcExtrudedAreaSolid(IfcProfileDef prof, IfcAxis2Placement3D position, double depth) : base(prof, position) { ExtrudedDirection = mDatabase.Factory.ZAxis; mDepth = depth; }
		public IfcExtrudedAreaSolid(IfcProfileDef prof, IfcAxis2Placement3D position, IfcDirection dir, double depth) : base(prof, position) { if(dir != null) mExtrudedDirection = dir.mIndex; mDepth = depth; }
		internal static IfcExtrudedAreaSolid Parse(string str) { IfcExtrudedAreaSolid e = new IfcExtrudedAreaSolid(); int pos = 0; e.Parse(str, ref pos, str.Length); return e; }
		protected override void Parse(string str, ref int pos, int len)
		{
			base.Parse(str, ref pos, len);
			mExtrudedDirection = ParserSTEP.StripLink(str, ref pos, len);
			mDepth = ParserSTEP.StripDouble(str, ref pos, len);
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mExtrudedDirection) + "," + ParserSTEP.DoubleToString(Math.Round(mDepth, mDatabase.mLengthDigits)); }
	}
	public partial class IfcExtrudedAreaSolidTapered : IfcExtrudedAreaSolid
	{
		private int mEndSweptArea;//: IfcProfileDef 
		public IfcProfileDef EndSweptArea { get { return mDatabase[mEndSweptArea] as IfcProfileDef; } set { mEndSweptArea = value.mIndex; } }

		internal IfcExtrudedAreaSolidTapered() : base() { }
		internal IfcExtrudedAreaSolidTapered(DatabaseIfc db, IfcExtrudedAreaSolidTapered e) : base(db,e) { EndSweptArea = db.Factory.Duplicate(e.EndSweptArea) as IfcProfileDef; }
		public IfcExtrudedAreaSolidTapered(IfcParameterizedProfileDef start, IfcAxis2Placement3D placement, double depth, IfcParameterizedProfileDef end) : base(start, placement, new IfcDirection(start.mDatabase,0,0,1), depth) { EndSweptArea = end; }
		public IfcExtrudedAreaSolidTapered(IfcDerivedProfileDef start, IfcAxis2Placement3D placement, double depth, IfcDerivedProfileDef end) : base(start, placement,new IfcDirection(start.mDatabase,0,0,1), depth ) { EndSweptArea = end; }
		internal new static IfcExtrudedAreaSolidTapered Parse(string str) { IfcExtrudedAreaSolidTapered e = new IfcExtrudedAreaSolidTapered(); int pos = 0; e.Parse(str, ref pos, str.Length); return e; }
		protected override void Parse(string str, ref int pos, int len)
		{
			base.Parse(str, ref pos, len);
			mEndSweptArea = ParserSTEP.StripLink(str, ref pos, len);
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mEndSweptArea); }
	}
}
