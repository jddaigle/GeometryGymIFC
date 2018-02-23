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
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using GeometryGym.STEP;

namespace GeometryGym.Ifc
{
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcGasTerminalType : IfcFlowTerminalType // DEPRECEATED IFC4
	{
		internal IfcGasTerminalTypeEnum mPredefinedType = IfcGasTerminalTypeEnum.NOTDEFINED;// : IfcGasTerminalBoxTypeEnum;
		public IfcGasTerminalTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcGasTerminalType() : base() { }
		internal IfcGasTerminalType(DatabaseIfc db, IfcGasTerminalType t) : base(db, t) { mPredefinedType = t.mPredefinedType; }
		internal IfcGasTerminalType(DatabaseIfc m, string name, IfcGasTerminalTypeEnum type) : base(m) { Name = name; mPredefinedType = type; }
		internal static void parseFields(IfcGasTerminalType t, List<string> arrFields, ref int ipos) { IfcFlowControllerType.parseFields(t, arrFields, ref ipos); t.mPredefinedType = (IfcGasTerminalTypeEnum)Enum.Parse(typeof(IfcGasTerminalTypeEnum), arrFields[ipos++].Replace(".", "")); }
		internal new static IfcGasTerminalType Parse(string strDef) { IfcGasTerminalType t = new IfcGasTerminalType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."; }
	}
	[Obsolete("DEPRECEATED IFC4", false)]
	public partial class IfcGeneralMaterialProperties : IfcMaterialPropertiesSuperSeded // DEPRECEATED IFC4
	{
		internal double mMolecularWeight = double.NaN; //: OPTIONAL IfcMolecularWeightMeasure;
		internal double mPorosity = double.NaN; //: OPTIONAL IfcNormalisedRatioMeasure;
		internal double mMassDensity = double.NaN;//OPTIONAL IfcMassDensityMeasure

		public double MolecularWeight { get { return mMolecularWeight; } set { mMolecularWeight = value; } }
		public double Porosity { get { return mPorosity; } set { mPorosity = value; } }
		public double MassDensity { get { return mMassDensity; } set { mMassDensity = value; } } 

		internal IfcGeneralMaterialProperties() : base() { }
		internal IfcGeneralMaterialProperties(DatabaseIfc db, IfcGeneralMaterialProperties p) : base(db,p) { mMolecularWeight = p.mMolecularWeight; mPorosity = p.mPorosity; mMassDensity = p.mMassDensity; }
		public IfcGeneralMaterialProperties(IfcMaterial material) : base(material) { }
		internal IfcGeneralMaterialProperties(IfcMaterial mat, double molecularWeight, double porosity, double massDensity) : base(mat)
		{
			mMolecularWeight = molecularWeight;
			mPorosity = porosity;
			mMassDensity = massDensity;
		}
		internal static IfcGeneralMaterialProperties Parse(string strDef) { IfcGeneralMaterialProperties p = new IfcGeneralMaterialProperties(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos); return p; }
		internal static void parseFields(IfcGeneralMaterialProperties p, List<string> arrFields, ref int ipos) { IfcMaterialPropertiesSuperSeded.parseFields(p, arrFields, ref ipos); p.mMolecularWeight = ParserSTEP.ParseDouble(arrFields[ipos++]); p.mPorosity = ParserSTEP.ParseDouble(arrFields[ipos++]); p.mMassDensity = ParserSTEP.ParseDouble(arrFields[ipos++]); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.DoubleOptionalToString(mMolecularWeight) + "," + ParserSTEP.DoubleOptionalToString(mPorosity) + "," + ParserSTEP.DoubleOptionalToString(mMassDensity); }
	}
	public partial class IfcGeneralProfileProperties : IfcProfileProperties //DELETED IFC4  SUPERTYPE OF	(IfcStructuralProfileProperties)
	{ 
		internal double mPhysicalWeight = double.NaN;// : OPTIONAL IfcMassPerLengthMeasure;
		internal double mPerimeter = double.NaN;// : OPTIONAL IfcPositiveLengthMeasure;
		internal double mMinimumPlateThickness = double.NaN;// : OPTIONAL IfcPositiveLengthMeasure;
		internal double mMaximumPlateThickness = double.NaN;// : OPTIONAL IfcPositiveLengthMeasure;
		internal double mCrossSectionArea = double.NaN;// : OPTIONAL IfcAreaMeasure;

		public double PhysicalWeight { get { return mPhysicalWeight; } set { mPhysicalWeight = value; } }
		public double Perimeter { get { return mPerimeter; } set { mPerimeter = value; } }
		public double MinimumPlateThickness { get { return mMinimumPlateThickness; } set { mMinimumPlateThickness = value; } }
		public double MaximumPlateThickness { get { return mMaximumPlateThickness; } set { mMaximumPlateThickness = value; } }
		public double CrossSectionArea { get { return mCrossSectionArea; } set { mCrossSectionArea = value; } }

		internal IfcGeneralProfileProperties() : base() { }
		internal IfcGeneralProfileProperties(DatabaseIfc db, IfcGeneralProfileProperties p) : base(db, p) { mPhysicalWeight = p.mPhysicalWeight; mPerimeter = p.mPerimeter; mMinimumPlateThickness = p.mMinimumPlateThickness; mMaximumPlateThickness = p.mMaximumPlateThickness; mCrossSectionArea = p.mCrossSectionArea; }
		public IfcGeneralProfileProperties(string name, IfcProfileDef p) : base(name, p) { }
		internal IfcGeneralProfileProperties(string name, List<IfcProperty> props, IfcProfileDef p) : base(name, props, p) { }

		internal new static IfcGeneralProfileProperties Parse(string strDef,ReleaseVersion schema) { IfcGeneralProfileProperties p = new IfcGeneralProfileProperties(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos,schema); return p; }
		internal static void parseFields(IfcGeneralProfileProperties gp, List<string> arrFields, ref int ipos,ReleaseVersion schema)
		{
			IfcProfileProperties.parseFields(gp, arrFields, ref ipos,schema);
			gp.mPhysicalWeight = ParserSTEP.ParseDouble(arrFields[ipos++]);
			gp.mPerimeter = ParserSTEP.ParseDouble(arrFields[ipos++]);
			gp.mMinimumPlateThickness = ParserSTEP.ParseDouble(arrFields[ipos++]);
			gp.mMaximumPlateThickness = ParserSTEP.ParseDouble(arrFields[ipos++]);
			gp.mCrossSectionArea = ParserSTEP.ParseDouble(arrFields[ipos++]);
		}
		protected override string BuildStringSTEP() { string str = base.BuildStringSTEP(); return (string.IsNullOrEmpty(str) || mDatabase.mRelease != ReleaseVersion.IFC2x3 ? "" : str + "," + ParserSTEP.DoubleOptionalToString(mPhysicalWeight) + "," + ParserSTEP.DoubleOptionalToString(mPerimeter) + "," + ParserSTEP.DoubleOptionalToString(mMinimumPlateThickness) + "," + ParserSTEP.DoubleOptionalToString(mMaximumPlateThickness) + "," + ParserSTEP.DoubleOptionalToString(mCrossSectionArea)); }
	}
	public partial class IfcGeographicElement : IfcElement  //IFC4
	{
		internal IfcGeographicElementTypeEnum mPredefinedType = IfcGeographicElementTypeEnum.NOTDEFINED;// OPTIONAL IfcGeographicElementTypeEnum; 
		public IfcGeographicElementTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcGeographicElement() : base() { }
		internal IfcGeographicElement(DatabaseIfc db, IfcGeographicElement e) : base(db, e,false) { mPredefinedType = e.mPredefinedType; }
		public IfcGeographicElement(IfcObjectDefinition host, IfcObjectPlacement placement, IfcProductRepresentation representation) : base(host, placement, representation) { if (mDatabase.mRelease == ReleaseVersion.IFC2x3) throw new Exception(KeyWord + " only supported in IFC4!"); }
		
		internal static IfcGeographicElement Parse(string strDef) { IfcGeographicElement e = new IfcGeographicElement(); int ipos = 0; parseFields(e, ParserSTEP.SplitLineFields(strDef), ref ipos); return e; }
		internal static void parseFields(IfcGeographicElement e, List<string> arrFields, ref int ipos)
		{
			IfcElement.parseFields(e, arrFields, ref ipos);
			string s = arrFields[ipos++];
			if (s.StartsWith("."))
				e.mPredefinedType = (IfcGeographicElementTypeEnum)Enum.Parse(typeof(IfcGeographicElementTypeEnum), s.Replace(".", ""));
		}
		protected override string BuildStringSTEP() { return (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : base.BuildStringSTEP() + ",." + mPredefinedType + "."); }
	}
	public partial class IfcGeographicElementType : IfcElementType //IFC4
	{
		internal IfcGeographicElementTypeEnum mPredefinedType = IfcGeographicElementTypeEnum.NOTDEFINED;// IfcGeographicElementTypeEnum; 
		public IfcGeographicElementTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		internal IfcGeographicElementType() : base() { }
		internal IfcGeographicElementType(DatabaseIfc db, IfcGeographicElementType t) : base(db,t) { mPredefinedType = t.mPredefinedType; }
		internal IfcGeographicElementType(DatabaseIfc m, string name, IfcGeographicElementTypeEnum type) : base(m) { Name = name; mPredefinedType = type; if (m.mRelease == ReleaseVersion.IFC2x3) throw new Exception(KeyWord + " only supported in IFC4!"); }
		internal new static IfcGeographicElementType Parse(string strDef) { IfcGeographicElementType t = new IfcGeographicElementType(); int ipos = 0; parseFields(t, ParserSTEP.SplitLineFields(strDef), ref ipos); return t; }
		internal static void parseFields(IfcGeographicElementType t, List<string> arrFields, ref int ipos)
		{
			IfcElementType.parseFields(t, arrFields, ref ipos);
			string s = arrFields[ipos++];
			if (s.StartsWith("."))
				t.mPredefinedType = (IfcGeographicElementTypeEnum)Enum.Parse(typeof(IfcGeographicElementTypeEnum), s.Replace(".", ""));
		}
		protected override string BuildStringSTEP() { return (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : base.BuildStringSTEP() + ",." + mPredefinedType.ToString() + "."); }
	}
	public partial class IfcGeometricCurveSet : IfcGeometricSet
	{
		internal IfcGeometricCurveSet() : base() { }
		internal IfcGeometricCurveSet(DatabaseIfc db, IfcGeometricCurveSet s) : base(db,s) { }
		public IfcGeometricCurveSet(List<IfcGeometricSetSelect> set) : base(set)
		{
			for (int icounter = 0; icounter < set.Count; icounter++)
			{
				IfcSurface s = set[icounter] as IfcSurface;
				if (s != null)
					throw new Exception("XXX Error, IfcSurface cannot be added to IfcGeometricCurveSet " + mIndex);
			}
		}
		internal new static IfcGeometricCurveSet Parse(string str) { IfcGeometricCurveSet s = new IfcGeometricCurveSet(); s.parse(str); return s; }
	}
	public partial class IfcGeometricRepresentationContext : IfcRepresentationContext, IfcCoordinateReferenceSystemSelect
	{
		internal int mCoordinateSpaceDimension;// : IfcDimensionCount;
		internal double mPrecision = 1e-8;// : OPTIONAL REAL;
		internal int mWorldCoordinateSystem;// : IfcAxis2Placement;
		internal int mTrueNorth;// : OPTIONAL IfcDirection; 
		//INVERSE
		internal List<IfcGeometricRepresentationSubContext> mHasSubContexts = new List<IfcGeometricRepresentationSubContext>();//	 :	SET OF IfcGeometricRepresentationSubContext FOR ParentContext;
		private IfcCoordinateOperation mHasCoordinateOperation = null; //IFC4

		public int CoordinateSpaceDimension { get { return mCoordinateSpaceDimension; } set { mCoordinateSpaceDimension = value; } }
		public double Precision { get { return mPrecision; } set { mPrecision = value; } }
		public IfcAxis2Placement WorldCoordinateSystem { get { return mDatabase[mWorldCoordinateSystem] as IfcAxis2Placement; } set { mWorldCoordinateSystem = (value == null ? 0 : value.Index); } }
		public IfcDirection TrueNorth 
		{ 
			get { return mDatabase[mTrueNorth] as IfcDirection; }  
			set 
			{
				if (value == null)
					mTrueNorth = 0;
				else
				{
					if (Math.Abs(value.DirectionRatioZ) > mDatabase.Tolerance)
						throw new Exception("True North direction must be 2 dimensional direction");
					mTrueNorth = value.mIndex; 
				}
			} 
		}
		public List<IfcGeometricRepresentationSubContext> HasSubContexts { get { return mHasSubContexts; } }
		public IfcCoordinateOperation HasCoordinateOperation { get { return mHasCoordinateOperation; } set { mHasCoordinateOperation = value; if(value.mSourceCRS != mIndex) value.SourceCRS = this; } }
		
		internal IfcGeometricRepresentationContext() : base() { }
		protected IfcGeometricRepresentationContext(DatabaseIfc db) : base(db) { }
		internal IfcGeometricRepresentationContext(DatabaseIfc db, IfcGeometricRepresentationContext c) : base(db, c)
		{
			mCoordinateSpaceDimension = c.mCoordinateSpaceDimension;
			mPrecision = c.mPrecision;
			if(c.mWorldCoordinateSystem > 0)
				WorldCoordinateSystem = db.Factory.Duplicate(c.mDatabase[ c.mWorldCoordinateSystem]) as IfcAxis2Placement;
			if (c.mTrueNorth > 0)
				TrueNorth = db.Factory.Duplicate(c.TrueNorth) as IfcDirection;

			foreach (IfcGeometricRepresentationSubContext sc in mHasSubContexts)
				db.Factory.Duplicate(sc);
		}
		internal IfcGeometricRepresentationContext(DatabaseIfc db, int SpaceDimension, double precision) : base(db)
		{
			if (db.Context != null)
				db.Context.addRepresentationContext(this);
			mCoordinateSpaceDimension = SpaceDimension;
			mPrecision = Math.Max(1e-8, precision);
			WorldCoordinateSystem = new IfcAxis2Placement3D(new IfcCartesianPoint(db,0,0,0));
			TrueNorth = new IfcDirection(mDatabase, 0, 1);
		}
		public IfcGeometricRepresentationContext(int coordinateSpaceDimension, IfcAxis2Placement worldCoordinateSystem) : base(worldCoordinateSystem.Database) { mCoordinateSpaceDimension = coordinateSpaceDimension; WorldCoordinateSystem = worldCoordinateSystem; }

		internal static void parseFields(IfcGeometricRepresentationContext c, List<string> arrFields, ref int ipos)
		{
			IfcRepresentationContext.parseFields(c, arrFields, ref ipos);
			c.mCoordinateSpaceDimension = ParserSTEP.ParseInt(arrFields[ipos++]);
			c.mPrecision = ParserSTEP.ParseDouble(arrFields[ipos++]);
			c.mWorldCoordinateSystem = ParserSTEP.ParseLink(arrFields[ipos++]);
			c.mTrueNorth = ParserSTEP.ParseLink(arrFields[ipos++]);
		}
		protected override string BuildStringSTEP()
		{
			if (this as IfcGeometricRepresentationSubContext != null)
				return base.BuildStringSTEP() + ",*,*,*,*";
			
			return base.BuildStringSTEP() + "," + (mCoordinateSpaceDimension == 0 ? "*" : mCoordinateSpaceDimension.ToString()) + "," + (mPrecision == 0 ? "*" : ParserSTEP.DoubleOptionalToString(mPrecision)) + "," + ParserSTEP.LinkToString(mWorldCoordinateSystem) + "," + ParserSTEP.LinkToString(mTrueNorth);
		}
		internal static IfcGeometricRepresentationContext Parse(string strDef) { IfcGeometricRepresentationContext c = new IfcGeometricRepresentationContext(); int ipos = 0; parseFields(c, ParserSTEP.SplitLineFields(strDef), ref ipos); return c; }
	}
	public abstract partial class IfcGeometricRepresentationItem : IfcRepresentationItem 
	{
		//ABSTRACT SUPERTYPE OF (ONEOF (IfcAnnotationFillArea, IfcBooleanResult, IfcBoundingBox, IfcCartesianTransformationOperator, 
		//		IfcCompositeCurveSegment, IfcCsgPrimitive3D, IfcCurve, IfcDirection,IfcFaceBasedSurfaceModel, IfcFillAreaStyleHatching, 
		//		IfcFillAreaStyleTiles, IfcGeometricSet, IfcHalfSpaceSolid, IfcLightSource, IfcPlacement, IfcPlanarExtent, IfcPoint,
		//		IfcSectionedSpine, IfcShellBasedSurfaceModel, IfcSolidModel, IfcSurface, IfcTextLiteral, IfcVector))
		// IFC2x3 IfcAnnotationSurface, IfcDefinedSymbol, IfcDraughtingCallout, IfcFillAreaStyleTileSymbolWithStyle, IfcOneDirectionRepeatFactor
		// IFC4 IfcCartesianPointList, IfcTessellatedItem
		protected IfcGeometricRepresentationItem() : base() { }
		protected IfcGeometricRepresentationItem(DatabaseIfc db) : base(db) { }
		protected IfcGeometricRepresentationItem(DatabaseIfc db, IfcGeometricRepresentationItem i) : base(db,i) { }
	}
	public partial class IfcGeometricRepresentationSubContext : IfcGeometricRepresentationContext
	{
		internal int mContainerContext;// : IfcGeometricRepresentationContext;
		internal double mTargetScale = double.NaN;// : OPTIONAL IfcPositiveRatioMeasure;
		private IfcGeometricProjectionEnum mTargetView;// : IfcGeometricProjectionEnum;
		internal string mUserDefinedTargetView = "$";// : OPTIONAL IfcLabel;

		public IfcGeometricRepresentationContext ContainerContext { get { return mDatabase[mContainerContext] as IfcGeometricRepresentationContext; } set { mContainerContext = value.mIndex; value.mHasSubContexts.Add(this); } }
		public double TargetScale { get { return mTargetScale; } set { mTargetScale = value; } }
		public IfcGeometricProjectionEnum TargetView { get { return mTargetView; } set { mTargetView = value; } }
		public string UserDefinedTargetView { get { return (mUserDefinedTargetView == "$" ? "" : ParserIfc.Decode(mUserDefinedTargetView)); } set { mUserDefinedTargetView = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }

		internal IfcGeometricRepresentationSubContext() : base() { }
		internal IfcGeometricRepresentationSubContext(DatabaseIfc db, IfcGeometricRepresentationSubContext s) : base(db, s)
		{
			ContainerContext = db.Factory.Duplicate(s.ContainerContext) as IfcGeometricRepresentationContext;

			mTargetScale = s.mTargetScale;
			mTargetView = s.mTargetView;
			mUserDefinedTargetView = s.mUserDefinedTargetView;
		}
		public IfcGeometricRepresentationSubContext(IfcGeometricRepresentationContext container, IfcGeometricProjectionEnum view)
			: base(container.mDatabase)
		{
			ContainerContext = container;
			mContextType = container.mContextType;
			mTargetView = view;
		}
		internal static void parseFields(IfcGeometricRepresentationSubContext c, List<string> arrFields, ref int ipos)
		{
			IfcGeometricRepresentationContext.parseFields(c, arrFields, ref ipos);
			c.mContainerContext = ParserSTEP.ParseLink(arrFields[ipos++]);
			c.mTargetScale = ParserSTEP.ParseDouble(arrFields[ipos++]);
			c.mTargetView = (IfcGeometricProjectionEnum)Enum.Parse(typeof(IfcGeometricProjectionEnum), arrFields[ipos++].Replace(".", ""));
			c.mUserDefinedTargetView = arrFields[ipos++];
		}
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mContainerContext) + (double.IsNaN(mTargetScale) || mTargetScale <=0 ? ",$,." : "," + ParserSTEP.DoubleOptionalToString(mTargetScale) + ",.") + mTargetView.ToString() + (mUserDefinedTargetView == "$" ?  ".,$" : ".,'" + mUserDefinedTargetView + "'"); }
		internal new static IfcGeometricRepresentationSubContext Parse(string strDef) { IfcGeometricRepresentationSubContext c = new IfcGeometricRepresentationSubContext(); int ipos = 0; parseFields(c, ParserSTEP.SplitLineFields(strDef), ref ipos); return c; }
		internal override void postParseRelate()
		{
			base.postParseRelate();
			ContainerContext.HasSubContexts.Add(this);
		}
	}
	public partial class IfcGeometricSet : IfcGeometricRepresentationItem //SUPERTYPE OF(IfcGeometricCurveSet)
	{
		private List<int> mElements = new List<int>(); //SET [1:?] OF IfcGeometricSetSelect; 
		public ReadOnlyCollection<IfcGeometricSetSelect> Elements { get { return new ReadOnlyCollection<IfcGeometricSetSelect>( mElements.ConvertAll(x => mDatabase[x] as IfcGeometricSetSelect)); } }

		internal IfcGeometricSet() : base() { }
		internal IfcGeometricSet(DatabaseIfc db, IfcGeometricSet s) : base(db,s) { s.mElements.ToList().ForEach(x=>addElement( db.Factory.Duplicate(s.mDatabase[x]) as IfcGeometricSetSelect)); }
		public IfcGeometricSet(List<IfcGeometricSetSelect> set) : base(set[0].Database) { mElements = set.ConvertAll(x => x.Index); }
		protected override string BuildStringSTEP()
		{
			if (mElements.Count == 0)
				return "";
			string str = base.BuildStringSTEP() + ",(" + ParserSTEP.LinkToString(mElements[0]);
			for (int icounter = 1; icounter < mElements.Count; icounter++)
				str += "," + ParserSTEP.LinkToString(mElements[icounter]);
			return str + ")";
		}
		internal static IfcGeometricSet Parse(string str) { IfcGeometricSet s = new IfcGeometricSet(); s.parse(str); return s; }
		protected void parse(string str) { mElements = ParserSTEP.SplitListLinks(str.Substring(1, str.Length - 2)); }

		internal void addElement(IfcGeometricSetSelect element)
		{
			if (!mElements.Contains(element.Index))
				mElements.Add(element.Index);
		}
		internal void removeElement(IfcGeometricSetSelect element)
		{
			if(element != null)
				mElements.Remove(element.Index);
		}
	}
	public partial interface IfcGeometricSetSelect : IBaseClassIfc { } //SELECT ( IfcPoint, IfcCurve,  IfcSurface);
	public partial class IfcGrid : IfcProduct
	{
		private List<int> mUAxes = new List<int>();// : LIST [1:?] OF UNIQUE IfcGridAxis;
		private List<int> mVAxes = new List<int>();// : LIST [1:?] OF UNIQUE IfcGridAxis;
		private List<int> mWAxes = new List<int>();// : OPTIONAL LIST [1:?] OF UNIQUE IfcGridAxis;
		internal IfcGridTypeEnum mPredefinedType = IfcGridTypeEnum.NOTDEFINED;// :OPTIONAL IfcGridTypeEnum; //IFC4 CHANGE  New attribute.
																			  //INVERSE
		internal IfcRelContainedInSpatialStructure mContainedInStructure = null;

		internal IfcGridTypeEnum PredefinedType { get { return mPredefinedType; } set { mPredefinedType = value; } }

		public ReadOnlyCollection<IfcGridAxis> UAxes { get { return new ReadOnlyCollection<IfcGridAxis>( mUAxes.ConvertAll(x => mDatabase[x] as IfcGridAxis)); } }
		public ReadOnlyCollection<IfcGridAxis> VAxes { get { return new ReadOnlyCollection<IfcGridAxis>( mVAxes.ConvertAll(x => mDatabase[x] as IfcGridAxis)); } }
		public ReadOnlyCollection<IfcGridAxis> WAxes { get { return new ReadOnlyCollection<IfcGridAxis>( mWAxes.ConvertAll(x => mDatabase[x] as IfcGridAxis)); } }

		internal IfcGrid() : base() { }
		internal IfcGrid(DatabaseIfc db, IfcGrid g) : base(db, g,false)
		{
			g.UAxes.ToList().ForEach(x => AddUAxis( db.Factory.Duplicate(x) as IfcGridAxis));
			g.VAxes.ToList().ForEach(x => AddVAxis( db.Factory.Duplicate(x) as IfcGridAxis));
			g.WAxes.ToList().ForEach(x => AddWAxis( db.Factory.Duplicate(x) as IfcGridAxis));
			mPredefinedType = g.mPredefinedType;
		}
		public IfcGrid(IfcSpatialElement host, IfcAxis2Placement3D placement, List<IfcGridAxis> uAxes, List<IfcGridAxis> vAxes) 
			: base(new IfcLocalPlacement(host.Placement, placement), getRepresentation(uAxes,vAxes, null))
		{
			host.addGrid(this);
			if(uAxes != null)
				mUAxes = uAxes.ConvertAll(x=>x.mIndex);
			if (vAxes != null)
				mVAxes = vAxes.ConvertAll(x => x.mIndex);
			
		}
		public IfcGrid(IfcSpatialElement host, IfcAxis2Placement3D placement, List<IfcGridAxis> uAxes, List<IfcGridAxis> vAxes, List<IfcGridAxis> wAxes) 
			:this(host,placement, uAxes,vAxes) { wAxes.ForEach(x=>AddWAxis(x)); }

		internal static IfcGrid Parse(string strDef, ReleaseVersion schema) { IfcGrid g = new IfcGrid(); int ipos = 0; parseFields(g, ParserSTEP.SplitLineFields(strDef), ref ipos,schema); return g; }
		internal static void parseFields(IfcGrid g, List<string> arrFields, ref int ipos, ReleaseVersion schema)
		{
			IfcProduct.parseFields(g, arrFields, ref ipos);
			g.mUAxes = ParserSTEP.SplitListLinks(arrFields[ipos++]);
			g.mVAxes = ParserSTEP.SplitListLinks(arrFields[ipos++]);
			string s = arrFields[ipos++];
			if (s != "$")
				g.mWAxes = ParserSTEP.SplitListLinks(s);
			if (schema != ReleaseVersion.IFC2x3)
			{
				s = arrFields[ipos++];
				if (s[0] == '.')
					g.mPredefinedType = (IfcGridTypeEnum)Enum.Parse(typeof(IfcGridTypeEnum), s.Replace(".", ""));
			}
		}
		protected override string BuildStringSTEP()
		{
			string str = base.BuildStringSTEP() + ",(";
			if (mUAxes.Count > 0)
			{
				str += "#" + mUAxes[0];
				for (int icounter = 1; icounter < mUAxes.Count; icounter++)
					str += ",#" + mUAxes[icounter];
			}
			str += "),(";
			if (mVAxes.Count > 0)
			{
				str += "#" + mVAxes[0];
				for (int icounter = 1; icounter < mVAxes.Count; icounter++)
					str += ",#" + mVAxes[icounter];
			}
			str += "),";
			if (mWAxes.Count > 0)
			{
				str += "(#" + mWAxes[0];
				for (int icounter = 1; icounter < mWAxes.Count; icounter++)
					str += ",#" + mWAxes[icounter];
				str += ")";
			}
			else
				str += "$";
			return str + (mDatabase.mRelease == ReleaseVersion.IFC2x3 ? "" : (mPredefinedType == IfcGridTypeEnum.NOTDEFINED ? ",$" : ",." + mPredefinedType.ToString() + "."));
		}

		internal void AddUAxis(IfcGridAxis a) { mUAxes.Add(a.mIndex); a.mPartOfU = this; setShapeRep(a); }
		internal void AddVAxis(IfcGridAxis a) { mVAxes.Add(a.mIndex); a.mPartOfV = this; setShapeRep(a); }
		internal void AddWAxis(IfcGridAxis a) { mWAxes.Add(a.mIndex); a.mPartOfW = this; setShapeRep(a); }
		internal void RemoveUAxis(IfcGridAxis a) { mUAxes.Remove(a.mIndex); a.mPartOfU = null; removeExistingFromShapeRep(a);  }
		internal void RemoveVAxis(IfcGridAxis a) { mVAxes.Remove(a.mIndex); a.mPartOfV = null; removeExistingFromShapeRep(a); }
		internal void RemoveWAxis(IfcGridAxis a) { mWAxes.Remove(a.mIndex); a.mPartOfW = null; removeExistingFromShapeRep(a); }

		private void setShapeRep(IfcGridAxis a) { setShapeRep(new List<IfcGridAxis>() { a }); }
		private void setShapeRep(List<IfcGridAxis> axis)
		{
			if (axis == null || axis.Count == 0)
				return;
			IfcProductDefinitionShape pds = Representation as IfcProductDefinitionShape;
			if (pds == null)
				Representation = new IfcProductDefinitionShape(new IfcShapeRepresentation(new IfcGeometricCurveSet(axis.ConvertAll(x => (IfcGeometricSetSelect)x.AxisCurve))));
			else
			{
				foreach (IfcShapeModel sm in pds.Representations)
				{
					IfcShapeRepresentation sr = sm as IfcShapeRepresentation;
					if (sr != null)
					{
						foreach (IfcRepresentationItem gri in sr.Items)
						{
							IfcGeometricCurveSet curveSet = gri as IfcGeometricCurveSet;
							if (curveSet != null)
							{
								axis.ForEach(x => curveSet.addElement( (IfcGeometricSetSelect)x.AxisCurve));
								return;
							}
						}
					}
				}
			}
			
		}

		private static IfcProductRepresentation getRepresentation(List<IfcGridAxis> uAxes, List<IfcGridAxis> vAxes, List<IfcGridAxis> wAxes)
		{
			List<IfcGeometricSetSelect> set = new List<IfcGeometricSetSelect>();
			if (uAxes != null)
			{
				foreach (IfcGridAxis axis in uAxes)
					set.Add(axis.AxisCurve);
			}
			if (vAxes != null)
			{
				foreach (IfcGridAxis axis in vAxes)
					set.Add(axis.AxisCurve);
			}
			if (wAxes != null)
			{
				foreach (IfcGridAxis axis in wAxes)
					set.Add(axis.AxisCurve);
			}
			return new IfcProductDefinitionShape(new IfcShapeRepresentation(new IfcGeometricCurveSet(set)));
		}
		private void removeExistingFromShapeRep(IfcGridAxis a) { removeExistingFromShapeRep(new List<IfcGridAxis>() { a }); }
		private void removeExistingFromShapeRep(List<IfcGridAxis> axis)
		{
			if (axis == null || axis.Count == 0)
				return;
			IfcProductDefinitionShape pds = Representation as IfcProductDefinitionShape;
			if (pds != null)
			{
				foreach (IfcShapeModel sm in pds.Representations)
				{
					IfcShapeRepresentation sr = sm as IfcShapeRepresentation;
					if (sr != null)
					{
						foreach (IfcRepresentationItem gri in sr.Items)
						{
							IfcGeometricCurveSet curveSet = gri as IfcGeometricCurveSet;
							if (curveSet != null)
								axis.ForEach(x=>curveSet.removeElement((IfcGeometricSetSelect)x.AxisCurve));
						}
					}
				}
			}
		}

		//removeExistingFromShapeRep(UAxes);
		//mUAxes = value == null ? new List<int>() : value.ConvertAll(x => x.mIndex);
		//		setShapeRep(value);

		internal override void detachFromHost()
		{
			base.detachFromHost();
			if (mContainedInStructure != null)
			{
				mContainedInStructure.mRelatedElements.Remove(mIndex);
				mContainedInStructure = null;
			}
		}

		internal override void postParseRelate()
		{
			base.postParseRelate();
			foreach (IfcGridAxis a in UAxes)
				a.mPartOfU = this;
			foreach (IfcGridAxis a in VAxes)
				a.mPartOfV = this;
			foreach (IfcGridAxis a in WAxes)
				a.mPartOfW = this;
		}
	}
	public partial class IfcGridAxis : BaseClassIfc
	{
		private string mAxisTag = "$";// : OPTIONAL IfcLabel;
		private int mAxisCurve;// : IfcCurve;
		internal bool mSameSense;// : IfcBoolean;

		//INVERSE
		internal IfcGrid mPartOfW = null;//	:	SET [0:1] OF IfcGrid FOR WAxes;
		internal IfcGrid mPartOfV = null;//	:	SET [0:1] OF IfcGrid FOR VAxes;
		internal IfcGrid mPartOfU = null;//	:	SET [0:1] OF IfcGrid FOR UAxes;
		internal List<IfcVirtualGridIntersection> mHasIntersections = new List<IfcVirtualGridIntersection>();//:	SET OF IfcVirtualGridIntersection FOR IntersectingAxes;

		public override string Name { get { return AxisTag; } set { AxisTag = value; } }
		public string AxisTag { get { return mAxisTag == "$" ? "" : ParserIfc.Decode(mAxisTag); } set { mAxisTag = (string.IsNullOrEmpty(value) ? "$" : ParserIfc.Encode(value)); } }
		public IfcCurve AxisCurve { get { return mDatabase[mAxisCurve] as IfcCurve; } set { mAxisCurve = value.mIndex; } }

		internal IfcGridAxis() : base() { }
		internal IfcGridAxis(DatabaseIfc db, IfcGridAxis a) : base(db) { mAxisTag = a.mAxisTag; AxisCurve = db.Factory.Duplicate(a.AxisCurve) as IfcCurve; mSameSense = a.mSameSense; }
		public IfcGridAxis(string tag, IfcCurve axis, bool sameSense) : base(axis.Database) { if (!string.IsNullOrEmpty(tag)) mAxisTag = tag.Replace("'", ""); mAxisCurve = axis.mIndex; mSameSense = sameSense; }
		internal static IfcGridAxis Parse(string strDef) { IfcGridAxis a = new IfcGridAxis(); int ipos = 0; parseFields(a, ParserSTEP.SplitLineFields(strDef), ref ipos); return a; }
		internal static void parseFields(IfcGridAxis a, List<string> arrFields, ref int ipos) { a.mAxisTag = arrFields[ipos++].Replace("'", ""); a.mAxisCurve = ParserSTEP.ParseLink(arrFields[ipos++]); a.mSameSense = ParserSTEP.ParseBool(arrFields[ipos++]); }
		protected override string BuildStringSTEP() { return base.BuildStringSTEP() + (mAxisTag == "$" ? ",$," : ",'" + mAxisTag + "',") + ParserSTEP.LinkToString(mAxisCurve) + "," + ParserSTEP.BoolToString(mSameSense); }
	}
	public partial class IfcGridPlacement : IfcObjectPlacement
	{
		internal int mPlacementLocation;// : IfcVirtualGridIntersection ;
		internal int mPlacementRefDirection;// : OPTIONAL IfcVirtualGridIntersection;

		public IfcVirtualGridIntersection PlacementLocation { get { return mDatabase[mPlacementLocation] as IfcVirtualGridIntersection; } set { mPlacementLocation = value.mIndex; } }
		public IfcVirtualGridIntersection PlacementRefDirection { get { return mDatabase[mPlacementRefDirection] as IfcVirtualGridIntersection; } set { mPlacementRefDirection = (value == null ? 0 : value.mIndex); } }

		internal IfcGridPlacement() : base() { }
		internal IfcGridPlacement(DatabaseIfc db, IfcGridPlacement p) : base(db, p)
		{
			PlacementLocation = db.Factory.Duplicate(p.PlacementLocation) as IfcVirtualGridIntersection;
			if (p.mPlacementRefDirection > 0)
				PlacementRefDirection = db.Factory.Duplicate(p.PlacementRefDirection) as IfcVirtualGridIntersection;
		}
		internal static IfcGridPlacement Parse(string strDef) { IfcGridPlacement p = new IfcGridPlacement(); int ipos = 0; parseFields(p, ParserSTEP.SplitLineFields(strDef), ref ipos); return p; }
		internal static void parseFields(IfcGridPlacement p, List<string> arrFields, ref int ipos) { IfcObjectPlacement.parseFields(p, arrFields, ref ipos); p.mPlacementLocation = ParserSTEP.ParseLink(arrFields[ipos++]); p.mPlacementRefDirection = ParserSTEP.ParseLink(arrFields[ipos++]); }
		protected override string BuildStringSTEP() { return (mPlacesObject.Count == 0 ? "" : base.BuildStringSTEP() + "," + ParserSTEP.LinkToString(mPlacementLocation) + "," + ParserSTEP.LinkToString(mPlacementRefDirection)); }
	}
	public partial class IfcGroup : IfcObject //SUPERTYPE OF (ONEOF (IfcAsset ,IfcCondition ,IfcInventory ,IfcStructuralLoadGroup ,IfcStructuralResultGroup ,IfcSystem ,IfcZone))
	{
		//INVERSE
		internal List<IfcRelAssignsToGroup> mIsGroupedBy = new List<IfcRelAssignsToGroup>();// IFC4 SET : IfcRelAssignsToGroup FOR RelatingGroup;
		public ReadOnlyCollection<IfcRelAssignsToGroup> IsGroupedBy { get { return new ReadOnlyCollection<IfcRelAssignsToGroup>( mIsGroupedBy); } }

		internal IfcGroup() : base() { }
		internal IfcGroup(DatabaseIfc db, IfcGroup g, bool downStream) : base(db,g,downStream) { }
		public IfcGroup(DatabaseIfc m, string name) : base(m) { Name = name; new IfcRelAssignsToGroup(this); }
		internal IfcGroup(List<IfcObjectDefinition> ods) : base(ods[0].mDatabase) { mIsGroupedBy.Add(new IfcRelAssignsToGroup(this, ods)); }

		internal static IfcGroup Parse(string strDef) { IfcGroup g = new IfcGroup(); int ipos = 0; parseFields(g, ParserSTEP.SplitLineFields(strDef), ref ipos); return g; }
		internal static void parseFields(IfcGroup g, List<string> arrFields, ref int ipos) { IfcObject.parseFields(g, arrFields, ref ipos); }
		protected override string BuildStringSTEP() { return (mDatabase.ModelView == ModelView.Ifc2x3Coordination ? "" : base.BuildStringSTEP()); }
		internal void assign(IfcObjectDefinition o) { mIsGroupedBy[0].AddRelated(o); }
	}
}
