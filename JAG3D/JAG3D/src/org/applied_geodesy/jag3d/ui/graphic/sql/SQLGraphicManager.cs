using System;
using System.Collections.Generic;

/// <summary>
///*********************************************************************
/// Copyright by Michael Loesler, https://software.applied-geodesy.org   *
///                                                                      *
/// This program is free software; you can redistribute it and/or modify *
/// it under the terms of the GNU General Public License as published by *
/// the Free Software Foundation; either version 3 of the License, or    *
/// at your option any later version.                                    *
///                                                                      *
/// This program is distributed in the hope that it will be useful,      *
/// but WITHOUT ANY WARRANTY; without even the implied warranty of       *
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
/// GNU General Public License for more details.                         *
///                                                                      *
/// You should have received a copy of the GNU General Public License    *
/// along with this program; if not, see <http://www.gnu.org/licenses/>  *
/// or write to the                                                      *
/// Free Software Foundation, Inc.,                                      *
/// 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.            *
///                                                                      *
/// **********************************************************************
/// </summary>

namespace org.applied_geodesy.jag3d.ui.graphic.sql
{

	using ObservationType = org.applied_geodesy.adjustment.network.ObservationType;
	using PointType = org.applied_geodesy.adjustment.network.PointType;
	using GraphicExtent = org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent;
	using TableRowHighlightType = org.applied_geodesy.jag3d.ui.table.rowhighlight.TableRowHighlightType;
	using ArrowLayer = org.applied_geodesy.jag3d.ui.graphic.layer.ArrowLayer;
	using org.applied_geodesy.jag3d.ui.graphic.layer;
	using FontLayer = org.applied_geodesy.jag3d.ui.graphic.layer.FontLayer;
	using HighlightableLayer = org.applied_geodesy.jag3d.ui.graphic.layer.HighlightableLayer;
	using Layer = org.applied_geodesy.jag3d.ui.graphic.layer.Layer;
	using LayerManager = org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager;
	using LayerType = org.applied_geodesy.jag3d.ui.graphic.layer.LayerType;
	using LegendLayer = org.applied_geodesy.jag3d.ui.graphic.layer.LegendLayer;
	using LegendPositionType = org.applied_geodesy.jag3d.ui.graphic.layer.LegendPositionType;
	using ObservationLayer = org.applied_geodesy.jag3d.ui.graphic.layer.ObservationLayer;
	using ObservationSymbolProperties = org.applied_geodesy.jag3d.ui.graphic.layer.ObservationSymbolProperties;
	using PointLayer = org.applied_geodesy.jag3d.ui.graphic.layer.PointLayer;
	using PointShiftArrowLayer = org.applied_geodesy.jag3d.ui.graphic.layer.PointShiftArrowLayer;
	using ArrowSymbolType = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.ArrowSymbolType;
	using PointSymbolType = org.applied_geodesy.jag3d.ui.graphic.layer.symbol.PointSymbolType;
	using DataBase = org.applied_geodesy.util.sql.DataBase;

	using FXCollections = javafx.collections.FXCollections;
	using Color = javafx.scene.paint.Color;

	public class SQLGraphicManager
	{
		private readonly DataBase dataBase;

		public SQLGraphicManager(DataBase dataBase)
		{
			if (dataBase == null || !dataBase.Open)
			{
				throw new System.ArgumentException(this.GetType().Name + " : Error, database must be open! " + dataBase);
			}
			this.dataBase = dataBase;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void initLayer(org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager layerManager) throws java.sql.SQLException
		public virtual void initLayer(LayerManager layerManager)
		{
			string sqlExists = "SELECT TRUE AS \"exists\" FROM \"Layer\" WHERE \"type\" = ?";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sqlExists);
			LayerType[] layerTyps = LayerType.values();
			int order = 0;
			foreach (LayerType layerType in layerTyps)
			{
				Layer layer = layerManager.getLayer(layerType);
				if (layer == null)
				{
					continue;
				}

				int idx = 1;
				stmt.setInt(idx++, layerType.getId());
				ResultSet rs = stmt.executeQuery();
				bool exists = rs.next() && rs.getBoolean("exists") == true;

				switch (layerType.innerEnumValue)
				{
				case LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
				case LayerType.InnerEnum.DATUM_POINT_APRIORI:
				case LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
				case LayerType.InnerEnum.NEW_POINT_APRIORI:
				case LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
				case LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
				case LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
				case LayerType.InnerEnum.STOCHASTIC_POINT_APRIORI:
					if (exists)
					{
						this.load((PointLayer)layer);
					}
					else
					{
						this.save((PointLayer)layer, order++);
					}
					break;

				case LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
				case LayerType.InnerEnum.OBSERVATION_APRIORI:
					if (exists)
					{
						this.load((ObservationLayer)layer);
					}
					else
					{
						this.save((ObservationLayer)layer, order++);
					}
					break;

				case LayerType.InnerEnum.POINT_SHIFT_HORIZONTAL:
				case LayerType.InnerEnum.POINT_SHIFT_VERTICAL:
				case LayerType.InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL:
				case LayerType.InnerEnum.PRINCIPAL_COMPONENT_VERTICAL:
				case LayerType.InnerEnum.POINT_RESIDUAL_HORIZONTAL:
				case LayerType.InnerEnum.POINT_RESIDUAL_VERTICAL:
					if (exists)
					{
						this.load((ArrowLayer)layer);
					}
					else
					{
						this.save((ArrowLayer)layer, order++);
					}
					break;

				case LayerType.InnerEnum.ABSOLUTE_CONFIDENCE:
				case LayerType.InnerEnum.RELATIVE_CONFIDENCE:
					if (exists)
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: this.load((org.applied_geodesy.jag3d.ui.graphic.layer.ConfidenceLayer<?>)layer);
						this.load((ConfidenceLayer<object>)layer);
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: this.save((org.applied_geodesy.jag3d.ui.graphic.layer.ConfidenceLayer<?>)layer, order++);
						this.save((ConfidenceLayer<object>)layer, order++);
					}
					break;

				case LayerType.InnerEnum.LEGEND:
					if (exists)
					{
						this.load((LegendLayer)layer);
					}
					else
					{
						this.save((LegendLayer)layer, order++);
					}
					break;
				}
			}

			IList<LayerType> layerOrder = new List<LayerType>();
			string sqlOrder = "SELECT \"type\" FROM \"Layer\" ORDER BY \"order\" ASC";
			stmt = this.dataBase.getPreparedStatement(sqlOrder);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{
				LayerType type = LayerType.getEnumByValue(rs.getInt("type"));
				if (type != null)
				{
					layerOrder.Add(type);
				}
			}

			IList<Layer> reorderedLayerList = FXCollections.observableArrayList();
			foreach (LayerType layerType in layerOrder)
			{
				reorderedLayerList.Add(layerManager.getLayer(layerType));
			}
			layerManager.reorderLayer(reorderedLayerList);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void loadEllipseScale(org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager layerManager) throws java.sql.SQLException
		public virtual void loadEllipseScale(LayerManager layerManager)
		{
			string sql = "SELECT \"value\" FROM \"LayerEllipseScale\" WHERE \"id\" = 1 LIMIT 1";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				layerManager.EllipseScale = rs.getDouble("value");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public boolean load(org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent graphicExtent) throws java.sql.SQLException
		public virtual bool load(GraphicExtent graphicExtent)
		{
			string sql = "SELECT \"min_x\", \"min_y\", \"max_x\", \"max_y\" FROM \"LayerExtent\" WHERE \"id\" = 1 LIMIT 1";
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				double xmin = rs.getDouble("min_x");
				double ymin = rs.getDouble("min_y");
				double xmax = rs.getDouble("max_x");
				double ymax = rs.getDouble("max_y");

				graphicExtent.set(xmin, ymin, xmax, ymax);
				return true;
			}
			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void load(org.applied_geodesy.jag3d.ui.graphic.layer.LayerManager layerManager) throws java.sql.SQLException
		public virtual void load(LayerManager layerManager)
		{
			this.initLayer(layerManager);

			IDictionary<string, GraphicPoint> completeAprioriPointMap = new Dictionary<string, GraphicPoint>();
			IDictionary<string, GraphicPoint> completeAposterioriPointMap = new Dictionary<string, GraphicPoint>();

			LayerType[] layerTypes = new LayerType[] {LayerType.DATUM_POINT_APOSTERIORI, LayerType.NEW_POINT_APOSTERIORI, LayerType.REFERENCE_POINT_APOSTERIORI, LayerType.STOCHASTIC_POINT_APOSTERIORI, LayerType.DATUM_POINT_APRIORI, LayerType.NEW_POINT_APRIORI, LayerType.REFERENCE_POINT_APRIORI, LayerType.STOCHASTIC_POINT_APRIORI};

			foreach (LayerType layerType in layerTypes)
			{
				switch (layerType.innerEnumValue)
				{
				case LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
				case LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
				case LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
				case LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
					PointLayer pointAposterioriLayer = (PointLayer) layerManager.getLayer(layerType);
					completeAposterioriPointMap.PutAll(this.loadPoints(pointAposterioriLayer));
					break;

				case LayerType.InnerEnum.DATUM_POINT_APRIORI:
				case LayerType.InnerEnum.NEW_POINT_APRIORI:
				case LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
				case LayerType.InnerEnum.STOCHASTIC_POINT_APRIORI:
					PointLayer pointAprioriLayer = (PointLayer) layerManager.getLayer(layerType);
					completeAprioriPointMap.PutAll(this.loadPoints(pointAprioriLayer));
					break;

				case LayerType.InnerEnum.POINT_SHIFT_HORIZONTAL:
				case LayerType.InnerEnum.POINT_SHIFT_VERTICAL:
				case LayerType.InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL:
				case LayerType.InnerEnum.PRINCIPAL_COMPONENT_VERTICAL:
				case LayerType.InnerEnum.POINT_RESIDUAL_HORIZONTAL:
				case LayerType.InnerEnum.POINT_RESIDUAL_VERTICAL:
				case LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
				case LayerType.InnerEnum.OBSERVATION_APRIORI:
				case LayerType.InnerEnum.ABSOLUTE_CONFIDENCE:
				case LayerType.InnerEnum.RELATIVE_CONFIDENCE:
				case LayerType.InnerEnum.LEGEND:
					break;
				}
			}

			layerTypes = new LayerType[] {LayerType.OBSERVATION_APOSTERIORI, LayerType.OBSERVATION_APRIORI, LayerType.POINT_SHIFT_HORIZONTAL, LayerType.POINT_SHIFT_VERTICAL};

			foreach (LayerType layerType in layerTypes)
			{
				switch (layerType.innerEnumValue)
				{
				case LayerType.InnerEnum.POINT_SHIFT_HORIZONTAL:
				//case POINT_SHIFT_VERTICAL: // load both layers simultaneous to use equal references
					PointShiftArrowLayer pointShiftHorizontalArrowLayer = (PointShiftArrowLayer)layerManager.getLayer(LayerType.POINT_SHIFT_HORIZONTAL);
					PointShiftArrowLayer pointShiftVerticalArrowLayer = (PointShiftArrowLayer)layerManager.getLayer(LayerType.POINT_SHIFT_VERTICAL);
					this.loadCongruenceAnalysisNexus(pointShiftHorizontalArrowLayer, pointShiftVerticalArrowLayer, completeAposterioriPointMap);
					break;

				case LayerType.InnerEnum.OBSERVATION_APOSTERIORI:
					ObservationLayer observationAposterioriLayer = (ObservationLayer) layerManager.getLayer(layerType);
					this.loadObservations(observationAposterioriLayer, completeAposterioriPointMap);
					break;

				case LayerType.InnerEnum.OBSERVATION_APRIORI:
					ObservationLayer observationAprioriLayer = (ObservationLayer) layerManager.getLayer(layerType);
					this.loadObservations(observationAprioriLayer, completeAprioriPointMap);
					break;

				case LayerType.InnerEnum.ABSOLUTE_CONFIDENCE:
				case LayerType.InnerEnum.RELATIVE_CONFIDENCE:
				case LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
				case LayerType.InnerEnum.DATUM_POINT_APRIORI:
				case LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
				case LayerType.InnerEnum.NEW_POINT_APRIORI:
				case LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
				case LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
				case LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
				case LayerType.InnerEnum.STOCHASTIC_POINT_APRIORI:
				case LayerType.InnerEnum.PRINCIPAL_COMPONENT_HORIZONTAL:
				case LayerType.InnerEnum.PRINCIPAL_COMPONENT_VERTICAL:
				case LayerType.InnerEnum.POINT_RESIDUAL_HORIZONTAL:
				case LayerType.InnerEnum.POINT_RESIDUAL_VERTICAL:
				case LayerType.InnerEnum.POINT_SHIFT_VERTICAL:
				case LayerType.InnerEnum.LEGEND:
					break;
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private java.util.Map<String, GraphicPoint> loadPoints(org.applied_geodesy.jag3d.ui.graphic.layer.PointLayer pointLayer) throws java.sql.SQLException
		private IDictionary<string, GraphicPoint> loadPoints(PointLayer pointLayer)
		{
			LayerType layerType = pointLayer.LayerType;
			IList<GraphicPoint> pointList = new List<GraphicPoint>();
			IDictionary<string, GraphicPoint> pointMap = new Dictionary<string, GraphicPoint>();
			PointType type = null;
			bool selectAprioriValues = true;
			switch (layerType.innerEnumValue)
			{
			case LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
				type = PointType.DATUM_POINT;
				selectAprioriValues = false;
				break;

			case LayerType.InnerEnum.DATUM_POINT_APRIORI:
				type = PointType.DATUM_POINT;
				break;

			case LayerType.InnerEnum.NEW_POINT_APOSTERIORI:
				type = PointType.NEW_POINT;
				selectAprioriValues = false;
				break;

			case LayerType.InnerEnum.NEW_POINT_APRIORI:
				type = PointType.NEW_POINT;
				break;

			case LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
				type = PointType.REFERENCE_POINT;
				selectAprioriValues = false;
				break;

			case LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
				type = PointType.REFERENCE_POINT;
				break;

			case LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
				type = PointType.STOCHASTIC_POINT;
				selectAprioriValues = false;
				break;

			case LayerType.InnerEnum.STOCHASTIC_POINT_APRIORI:
				type = PointType.STOCHASTIC_POINT;
				break;

			default: // load only point layers
				break;
			}

			if (type != null)
			{

				string sql = "SELECT " + "\"name\", " + "\"y0\", \"x0\", " + "\"y\",  \"x\", " + "\"helmert_major_axis\", \"helmert_minor_axis\", " + "0.5 * PI() + \"helmert_alpha\" AS \"helmert_alpha\", " + "\"y0\" - \"y\" AS \"residual_y\", \"x0\" - \"x\" AS \"residual_x\", \"z0\" - \"z\" AS \"residual_z\", " + "\"first_principal_component_y\", \"first_principal_component_x\", \"first_principal_component_z\", " + "(CASE " + "WHEN \"dimension\" = 3 THEN LEAST(\"redundancy_x\", \"redundancy_y\", \"redundancy_z\") " + "WHEN \"dimension\" = 2 THEN LEAST(\"redundancy_x\", \"redundancy_y\") " + "ELSE \"redundancy_z\" " + "END) AS \"redundancy\", " + "(CASE " + "WHEN \"dimension\" = 3 THEN GREATEST(ABS(\"influence_on_position_x\"), ABS(\"influence_on_position_y\"), ABS(\"influence_on_position_z\")) " + "WHEN \"dimension\" = 2 THEN GREATEST(ABS(\"influence_on_position_x\"), ABS(\"influence_on_position_y\")) " + "ELSE ABS(\"influence_on_position_z\") " + "END) AS \"influence_on_position\", " + "CASE WHEN (" + "ABS(\"gross_error_x\") > ABS(\"minimal_detectable_bias_x\") " + "OR ABS(\"gross_error_y\") > ABS(\"minimal_detectable_bias_y\") " + "OR ABS(\"gross_error_z\") > ABS(\"minimal_detectable_bias_z\") " + ") THEN TRUE ELSE FALSE END AS \"gross_error_exceeded\", " + "\"p_prio\", \"significant\", " + "\"dimension\" " + "FROM \"PointApriori\" " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" " + "LEFT JOIN \"PointAposteriori\" ON \"PointApriori\".\"id\" = \"PointAposteriori\".\"id\" " + "WHERE \"PointGroup\".\"type\" = ? AND \"PointApriori\".\"enable\" = TRUE AND \"PointGroup\".\"enable\" = TRUE " + "ORDER BY \"PointGroup\".\"id\" ASC, \"PointApriori\".\"id\" ASC";

				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				int idx = 1;
				stmt.setInt(idx++, type.getId());
				ResultSet rs = stmt.executeQuery();

				while (rs.next())
				{

					string name = rs.getString("name");
					int dimension = rs.getInt("dimension");

					if (selectAprioriValues)
					{
						double x0 = rs.getDouble("x0");
						double y0 = rs.getDouble("y0");

						GraphicPoint graphicPoint = new GraphicPoint(name, dimension, y0, x0);
						pointList.Add(graphicPoint);
						pointMap[name] = graphicPoint;
					}
					else
					{
						bool significant = rs.getBoolean("significant");
						if (rs.wasNull())
						{
							continue;
						}

						double x = rs.getDouble("x");
						if (rs.wasNull())
						{
							continue;
						}

						double y = rs.getDouble("y");
						if (rs.wasNull())
						{
							continue;
						}

						double majorAxis = rs.getDouble("helmert_major_axis");
						double minorAxis = rs.getDouble("helmert_minor_axis");
						double angle = rs.getDouble("helmert_alpha");
						double pPrio = rs.getDouble("p_prio");

						double principleComponentX = rs.getDouble("first_principal_component_x");
						double principleComponentY = rs.getDouble("first_principal_component_y");
						double principleComponentZ = rs.getDouble("first_principal_component_z");

						double residualX = rs.getDouble("residual_x");
						double residualY = rs.getDouble("residual_y");
						double residualZ = rs.getDouble("residual_z");

						double minRedundancy = rs.getDouble("redundancy");
						double maxInfluenceOnPosition = rs.getDouble("influence_on_position");
						bool grossErrorExceeded = rs.getBoolean("gross_error_exceeded");

						GraphicPoint graphicPoint = new GraphicPoint(name, dimension, y, x, majorAxis, minorAxis, angle, residualY, residualX, residualZ, principleComponentY, principleComponentX, principleComponentZ, minRedundancy, maxInfluenceOnPosition, pPrio, grossErrorExceeded, significant);
						pointList.Add(graphicPoint);
						pointMap[name] = graphicPoint;
					}
				}
			}
			pointLayer.Points = pointList;
			return pointMap;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadObservations(org.applied_geodesy.jag3d.ui.graphic.layer.ObservationLayer observationLayer, java.util.Map<String, GraphicPoint> completePointMap) throws java.sql.SQLException
		private void loadObservations(ObservationLayer observationLayer, IDictionary<string, GraphicPoint> completePointMap)
		{
			LayerType layerType = observationLayer.LayerType;
			IDictionary<PointPairKey, ObservableMeasurement> observationMap = new Dictionary<PointPairKey, ObservableMeasurement>();

			bool selectAprioriValues = layerType == LayerType.OBSERVATION_APRIORI;

			string sql = "SELECT " + "\"start_point_name\", \"end_point_name\", \"type\", " + "\"StartPointApriori\".\"x0\" AS \"xs0\", \"StartPointApriori\".\"y0\" AS \"ys0\", " + "\"StartPointAposteriori\".\"x\" AS \"xs\", \"StartPointAposteriori\".\"y\" AS \"ys\", " + "\"EndPointApriori\".\"x0\" AS \"xe0\", \"EndPointApriori\".\"y0\" AS \"ye0\", " + "\"EndPointAposteriori\".\"x\" AS \"xe\", \"EndPointAposteriori\".\"y\" AS \"ye\", " + "\"ObservationAposteriori\".\"significant\", " + "\"ObservationAposteriori\".\"redundancy\", " + "ABS(\"ObservationAposteriori\".\"influence_on_position\") AS \"influence_on_position\", " + "CASE WHEN (ABS(\"ObservationAposteriori\".\"gross_error\") > ABS(\"ObservationAposteriori\".\"minimal_detectable_bias\")) THEN TRUE ELSE FALSE END AS \"gross_error_exceeded\", " + "\"ObservationAposteriori\".\"p_prio\" " + "FROM \"ObservationApriori\" " + "JOIN \"ObservationGroup\" ON \"ObservationGroup\".\"id\" = \"ObservationApriori\".\"group_id\" " + "LEFT JOIN \"ObservationAposteriori\" ON \"ObservationAposteriori\".\"id\" = \"ObservationApriori\".\"id\" " + "JOIN \"PointApriori\" AS \"StartPointApriori\" ON  \"StartPointApriori\".\"name\" = \"start_point_name\" " + "JOIN \"PointApriori\" AS \"EndPointApriori\" ON \"EndPointApriori\".\"name\" = \"end_point_name\" " + "JOIN \"PointGroup\" AS \"StartPointGroup\" ON  \"StartPointGroup\".\"id\" = \"StartPointApriori\".\"group_id\" " + "JOIN \"PointGroup\" AS \"EndPointGroup\" ON  \"EndPointGroup\".\"id\" = \"EndPointApriori\".\"group_id\" " + "LEFT JOIN \"PointAposteriori\" AS \"StartPointAposteriori\" ON \"StartPointAposteriori\".\"id\" = \"StartPointApriori\".\"id\" " + "LEFT JOIN \"PointAposteriori\" AS \"EndPointAposteriori\" ON \"EndPointAposteriori\".\"id\" = \"EndPointApriori\".\"id\" " + "WHERE " + "\"ObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE " + "AND \"StartPointApriori\".\"enable\" = TRUE AND \"EndPointApriori\".\"enable\" = TRUE " + "AND \"StartPointGroup\".\"enable\" = TRUE AND \"EndPointGroup\".\"enable\" = TRUE " + "UNION ALL " + "SELECT " + "\"start_point_name\", \"end_point_name\", \"type\", " + "\"StartPointApriori\".\"x0\" AS \"xs0\", \"StartPointApriori\".\"y0\" AS \"ys0\", " + "\"StartPointAposteriori\".\"x\" AS \"xs\", \"StartPointAposteriori\".\"y\" AS \"ys\", " + "\"EndPointApriori\".\"x0\" AS \"xe0\", \"EndPointApriori\".\"y0\" AS \"ye0\", " + "\"EndPointAposteriori\".\"x\" AS \"xe\", \"EndPointAposteriori\".\"y\" AS \"ye\", " + "\"GNSSObservationAposteriori\".\"significant\", " + "(CASE " + "WHEN \"ObservationGroup\".\"type\" = ? THEN LEAST(\"GNSSObservationAposteriori\".\"redundancy_x\", \"GNSSObservationAposteriori\".\"redundancy_y\", \"GNSSObservationAposteriori\".\"redundancy_z\") " + "WHEN \"ObservationGroup\".\"type\" = ? THEN LEAST(\"GNSSObservationAposteriori\".\"redundancy_x\", \"GNSSObservationAposteriori\".\"redundancy_y\") " + "ELSE \"GNSSObservationAposteriori\".\"redundancy_z\" " + "END) AS \"redundancy\", " + "(CASE " + "WHEN \"ObservationGroup\".\"type\" = ? THEN GREATEST(ABS(\"GNSSObservationAposteriori\".\"influence_on_position_x\"), ABS(\"GNSSObservationAposteriori\".\"influence_on_position_y\"), ABS(\"GNSSObservationAposteriori\".\"influence_on_position_z\")) " + "WHEN \"ObservationGroup\".\"type\" = ? THEN GREATEST(ABS(\"GNSSObservationAposteriori\".\"influence_on_position_x\"), ABS(\"GNSSObservationAposteriori\".\"influence_on_position_y\")) " + "ELSE ABS(\"GNSSObservationAposteriori\".\"influence_on_position_z\") " + "END) AS \"influence_on_position\", " + "CASE WHEN (" + "ABS(\"GNSSObservationAposteriori\".\"gross_error_x\") > ABS(\"GNSSObservationAposteriori\".\"minimal_detectable_bias_x\") " + "OR ABS(\"GNSSObservationAposteriori\".\"gross_error_y\") > ABS(\"GNSSObservationAposteriori\".\"minimal_detectable_bias_y\") " + "OR ABS(\"GNSSObservationAposteriori\".\"gross_error_z\") > ABS(\"GNSSObservationAposteriori\".\"minimal_detectable_bias_z\") " + ") THEN TRUE ELSE FALSE END AS \"gross_error_exceeded\", " + "\"GNSSObservationAposteriori\".\"p_prio\" " + "FROM \"GNSSObservationApriori\" " + "JOIN \"ObservationGroup\" ON \"ObservationGroup\".\"id\" = \"GNSSObservationApriori\".\"group_id\" " + "LEFT JOIN \"GNSSObservationAposteriori\" ON \"GNSSObservationAposteriori\".\"id\" = \"GNSSObservationApriori\".\"id\" " + "JOIN \"PointApriori\" AS \"StartPointApriori\" ON  \"StartPointApriori\".\"name\" = \"start_point_name\" " + "JOIN \"PointApriori\" AS \"EndPointApriori\" ON \"EndPointApriori\".\"name\" = \"end_point_name\" " + "JOIN \"PointGroup\" AS \"StartPointGroup\" ON  \"StartPointGroup\".\"id\" = \"StartPointApriori\".\"group_id\" " + "JOIN \"PointGroup\" AS \"EndPointGroup\" ON  \"EndPointGroup\".\"id\" = \"EndPointApriori\".\"group_id\" " + "LEFT JOIN \"PointAposteriori\" AS \"StartPointAposteriori\" ON \"StartPointAposteriori\".\"id\" = \"StartPointApriori\".\"id\" " + "LEFT JOIN \"PointAposteriori\" AS \"EndPointAposteriori\" ON \"EndPointAposteriori\".\"id\" = \"EndPointApriori\".\"id\" " + "WHERE " + "\"GNSSObservationApriori\".\"enable\" = TRUE AND \"ObservationGroup\".\"enable\" = TRUE " + "AND \"StartPointApriori\".\"enable\" = TRUE AND \"EndPointApriori\".\"enable\" = TRUE " + "AND \"StartPointGroup\".\"enable\" = TRUE AND \"EndPointGroup\".\"enable\" = TRUE";

			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			int idx = 1;
			stmt.setInt(idx++, ObservationType.GNSS3D.getId()); // redundancy
			stmt.setInt(idx++, ObservationType.GNSS2D.getId());
			stmt.setInt(idx++, ObservationType.GNSS3D.getId()); // influence on position
			stmt.setInt(idx++, ObservationType.GNSS2D.getId());
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{

				string startPointName = rs.getString("start_point_name");
				string endPointName = rs.getString("end_point_name");

				if (!completePointMap.ContainsKey(startPointName) || !completePointMap.ContainsKey(endPointName))
				{
					continue;
				}

				PointPairKey key = new PointPairKey(startPointName, endPointName);
				ObservationType observationType = ObservationType.getEnumByValue(rs.getInt("type"));

				if (selectAprioriValues)
				{
					if (!observationMap.ContainsKey(key))
					{
						GraphicPoint startPoint = completePointMap[startPointName];
						GraphicPoint endPoint = completePointMap[endPointName];

						observationMap[key] = new ObservableMeasurement(startPoint, endPoint);
					}

					ObservableMeasurement observableLink = observationMap[key];

					switch (observationType.innerEnumValue)
					{
					case ObservationType.InnerEnum.LEVELING:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.LEVELING);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.LEVELING);
						}
						break;

					case ObservationType.InnerEnum.DIRECTION:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.DIRECTION);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.DIRECTION);
						}
						break;

					case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
					case ObservationType.InnerEnum.SLOPE_DISTANCE:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.DISTANCE);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.DISTANCE);
						}
						break;

					case ObservationType.InnerEnum.ZENITH_ANGLE:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.ZENITH_ANGLE);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.ZENITH_ANGLE);
						}
						break;

					case ObservationType.InnerEnum.GNSS1D:
					case ObservationType.InnerEnum.GNSS2D:
					case ObservationType.InnerEnum.GNSS3D:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.GNSS);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.GNSS);
						}
						break;
					}
				}
				else
				{
					bool significant = rs.getBoolean("significant");
					if (rs.wasNull())
					{
						continue;
					}

					double redundancy = rs.getDouble("redundancy");
					double influenceOnPosition = rs.getDouble("influence_on_position");
					double pPrio = rs.getDouble("p_prio");
					bool grossErrorExceeded = rs.getBoolean("gross_error_exceeded");

					if (!observationMap.ContainsKey(key))
					{
						GraphicPoint startPoint = completePointMap[startPointName];
						GraphicPoint endPoint = completePointMap[endPointName];

						observationMap[key] = new ObservableMeasurement(startPoint, endPoint);
					}

					ObservableMeasurement observableLink = observationMap[key];
					if (significant)
					{
						observableLink.Significant = significant;
					}
					observableLink.Redundancy = Math.Min(observableLink.Redundancy, redundancy);
					observableLink.InfluenceOnPosition = Math.Max(observableLink.InfluenceOnPosition, influenceOnPosition);
					observableLink.Pprio = Math.Min(observableLink.Pprio, pPrio);
					observableLink.GrossErrorExceeded = grossErrorExceeded;

					switch (observationType.innerEnumValue)
					{
					case ObservationType.InnerEnum.LEVELING:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.LEVELING);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.LEVELING);
						}
						break;

					case ObservationType.InnerEnum.DIRECTION:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.DIRECTION);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.DIRECTION);
						}
						break;

					case ObservationType.InnerEnum.HORIZONTAL_DISTANCE:
					case ObservationType.InnerEnum.SLOPE_DISTANCE:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.DISTANCE);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.DISTANCE);
						}
						break;

					case ObservationType.InnerEnum.ZENITH_ANGLE:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.ZENITH_ANGLE);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.ZENITH_ANGLE);
						}
						break;

					case ObservationType.InnerEnum.GNSS1D:
					case ObservationType.InnerEnum.GNSS2D:
					case ObservationType.InnerEnum.GNSS3D:
						if (startPointName.Equals(observableLink.StartPoint.Name))
						{
							observableLink.addStartPointObservationType(ObservationSymbolProperties.ObservationType.GNSS);
						}
						else
						{
							observableLink.addEndPointObservationType(ObservationSymbolProperties.ObservationType.GNSS);
						}
						break;
					}

				}
			}
			observationLayer.ObservableMeasurements = new List<ObservableMeasurement>(observationMap.Values);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadCongruenceAnalysisNexus(org.applied_geodesy.jag3d.ui.graphic.layer.PointShiftArrowLayer pointShiftHorizontalArrowLayer, org.applied_geodesy.jag3d.ui.graphic.layer.PointShiftArrowLayer pointShiftVerticalArrowLayer, java.util.Map<String, GraphicPoint> completePointMap) throws java.sql.SQLException
		private void loadCongruenceAnalysisNexus(PointShiftArrowLayer pointShiftHorizontalArrowLayer, PointShiftArrowLayer pointShiftVerticalArrowLayer, IDictionary<string, GraphicPoint> completePointMap)
		{
			IDictionary<PointPairKey, RelativeConfidence> relativeHorizontalConfidences = new Dictionary<PointPairKey, RelativeConfidence>();
			IDictionary<PointPairKey, RelativeConfidence> relativeVerticalConfidences = new Dictionary<PointPairKey, RelativeConfidence>();

			string sql = "SELECT " + "\"start_point_name\", \"end_point_name\", " + "\"CongruenceAnalysisGroup\".\"dimension\", " + "\"StartPointAposteriori\".\"x\" AS \"xs\", " + "\"StartPointAposteriori\".\"y\" AS \"ys\", " + "\"StartPointAposteriori\".\"z\" AS \"zs\", " + "\"EndPointAposteriori\".\"x\" AS \"xe\", " + "\"EndPointAposteriori\".\"y\" AS \"ye\", " + "\"EndPointAposteriori\".\"z\" AS \"ze\", " + "\"CongruenceAnalysisPointPairAposteriori\".\"confidence_major_axis_2d\", " + "\"CongruenceAnalysisPointPairAposteriori\".\"confidence_minor_axis_2d\", " + "0.5 * PI() + \"CongruenceAnalysisPointPairAposteriori\".\"confidence_alpha_2d\" AS \"confidence_alpha_2d\", " + "\"CongruenceAnalysisPointPairAposteriori\".\"significant\" " + "FROM \"CongruenceAnalysisPointPairApriori\" " + "JOIN \"CongruenceAnalysisPointPairAposteriori\" ON \"CongruenceAnalysisPointPairApriori\".\"id\" = \"CongruenceAnalysisPointPairAposteriori\".\"id\" " + "JOIN \"CongruenceAnalysisGroup\" ON \"CongruenceAnalysisPointPairApriori\".\"group_id\" = \"CongruenceAnalysisGroup\".\"id\" " + "JOIN \"PointApriori\" AS \"StartPointApriori\" ON \"CongruenceAnalysisPointPairApriori\".\"start_point_name\" = \"StartPointApriori\".\"name\" " + "JOIN \"PointApriori\" AS \"EndPointApriori\" ON \"CongruenceAnalysisPointPairApriori\".\"end_point_name\" = \"EndPointApriori\".\"name\" " + "JOIN \"PointAposteriori\" AS \"StartPointAposteriori\" ON \"StartPointApriori\".\"id\" = \"StartPointAposteriori\".\"id\" " + "JOIN \"PointAposteriori\" AS \"EndPointAposteriori\" ON \"EndPointApriori\".\"id\" = \"EndPointAposteriori\".\"id\" " + "JOIN \"PointGroup\" AS \"StartPointGroup\" ON \"StartPointApriori\".\"group_id\" = \"StartPointGroup\".\"id\" " + "JOIN \"PointGroup\" AS \"EndPointGroup\" ON \"EndPointApriori\".\"group_id\" = \"EndPointGroup\".\"id\" " + "WHERE " + "\"CongruenceAnalysisPointPairApriori\".\"enable\" = TRUE AND \"CongruenceAnalysisGroup\".\"enable\" = TRUE " + "AND \"StartPointApriori\".\"enable\" = TRUE AND \"EndPointApriori\".\"enable\" = TRUE " + "AND \"StartPointGroup\".\"enable\" = TRUE AND \"EndPointGroup\".\"enable\" = TRUE " + "UNION ALL " + "SELECT " + "\"name\" AS \"start_point_name\", \"name\" AS \"end_point_name\", " + "\"dimension\", " + "\"x\" - 0.5 * \"gross_error_x\" AS \"xs\", " + "\"y\" - 0.5 * \"gross_error_y\" AS \"ys\", " + "\"z\" - 0.5 * \"gross_error_z\" AS \"zs\", " + "\"x\" + 0.5 * \"gross_error_x\" AS \"xe\", " + "\"y\" + 0.5 * \"gross_error_y\" AS \"ye\", " + "\"z\" + 0.5 * \"gross_error_z\" AS \"ze\", " + "0 AS \"confidence_major_axis_2d\", " + "0 AS \"confidence_minor_axis_2d\", " + "0 AS \"confidence_alpha_2d\", " + "\"significant\" " + "FROM \"PointApriori\" " + "JOIN \"PointAposteriori\" ON \"PointApriori\".\"id\" =  \"PointAposteriori\".\"id\" " + "JOIN \"PointGroup\" ON \"PointApriori\".\"group_id\" = \"PointGroup\".\"id\" " + "WHERE " + "\"PointApriori\".\"enable\" = TRUE AND \"PointGroup\".\"enable\" = TRUE " + "AND \"PointAposteriori\".\"significant\" = TRUE";


			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			ResultSet rs = stmt.executeQuery();

			while (rs.next())
			{

				string startPointName = rs.getString("start_point_name");
				string endPointName = rs.getString("end_point_name");
				int dimension = rs.getInt("dimension");

				double majorAxis = rs.getDouble("confidence_major_axis_2d");
				double minorAxis = rs.getDouble("confidence_minor_axis_2d");
				double angle = rs.getDouble("confidence_alpha_2d");

				if (!completePointMap.ContainsKey(startPointName) || !completePointMap.ContainsKey(endPointName))
				{
					continue;
				}

				bool significant = rs.getBoolean("significant");
				if (rs.wasNull())
				{
					continue;
				}

				PointPairKey key = new PointPairKey(startPointName, endPointName);

				if (relativeHorizontalConfidences.ContainsKey(key) && relativeVerticalConfidences.ContainsKey(key))
				{
					continue;
				}

				GraphicPoint startPoint = completePointMap[startPointName];
				GraphicPoint endPoint = completePointMap[endPointName];

				double deltaHeight = 0.0;
				if (startPoint.Dimension != 2 && endPoint.Dimension != 2)
				{
					double zs = rs.getDouble("zs");
					double ze = rs.getDouble("ze");
					deltaHeight = ze - zs;
				}

				// shift values of reference points
				if (startPointName.Equals(endPointName))
				{
					double xs = rs.getDouble("xs");
					double ys = rs.getDouble("ys");
					double xe = rs.getDouble("xe");
					double ye = rs.getDouble("ye");

					startPoint = new GraphicPoint(startPointName, dimension, ys, xs);
					endPoint = new GraphicPoint(endPointName, dimension, ye, xe);

					startPoint.visibleProperty().bind(completePointMap[startPointName].visibleProperty());
					endPoint.visibleProperty().bind(completePointMap[endPointName].visibleProperty());
				}

				if (dimension != 1 && !relativeHorizontalConfidences.ContainsKey(key))
				{
					RelativeConfidence relativeHorizontalConfidence = new RelativeConfidence(startPoint, endPoint, deltaHeight, majorAxis, minorAxis, angle, significant);
					relativeHorizontalConfidences[key] = relativeHorizontalConfidence;
				}

				if (dimension != 2 && !relativeVerticalConfidences.ContainsKey(key))
				{
					RelativeConfidence relativeVerticalConfidence = new RelativeConfidence(startPoint, endPoint, deltaHeight, dimension == 1 ? majorAxis : 0, dimension == 1 ? minorAxis : 0, angle, significant);
					relativeVerticalConfidences[key] = relativeVerticalConfidence;
				}
			}

			pointShiftHorizontalArrowLayer.RelativeConfidences = new List<RelativeConfidence>(relativeHorizontalConfidences.Values);
			pointShiftVerticalArrowLayer.RelativeConfidences = new List<RelativeConfidence>(relativeVerticalConfidences.Values);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void saveEllipseScale(double scale) throws java.sql.SQLException
		public virtual void saveEllipseScale(double scale)
		{
			string sql = "MERGE INTO \"LayerEllipseScale\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS DOUBLE)) " + ") AS \"vals\" (\"id\", \"value\") ON \"LayerEllipseScale\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"LayerEllipseScale\".\"value\" = \"vals\".\"value\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"value\" ";
			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, 1); // default id
			stmt.setDouble(idx++, scale);
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.jag3d.ui.graphic.util.GraphicExtent extent) throws java.sql.SQLException
		public virtual void save(GraphicExtent extent)
		{
			string sql = "MERGE INTO \"LayerExtent\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE)) " + ") AS \"vals\" (\"id\", \"min_x\", \"min_y\", \"max_x\", \"max_y\") ON \"LayerExtent\".\"id\" = \"vals\".\"id\" " + "WHEN MATCHED THEN UPDATE SET " + "\"LayerExtent\".\"min_x\" = \"vals\".\"min_x\", " + "\"LayerExtent\".\"min_y\" = \"vals\".\"min_y\", " + "\"LayerExtent\".\"max_x\" = \"vals\".\"max_x\", " + "\"LayerExtent\".\"max_y\" = \"vals\".\"max_y\"  " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"id\", " + "\"vals\".\"min_x\", " + "\"vals\".\"min_y\", " + "\"vals\".\"max_x\", " + "\"vals\".\"max_y\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, 1); // default id
			stmt.setDouble(idx++, extent.MinX);
			stmt.setDouble(idx++, extent.MinY);
			stmt.setDouble(idx++, extent.MaxX);
			stmt.setDouble(idx++, extent.MaxY);
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.jag3d.ui.graphic.layer.PointLayer pointLayer, int order) throws java.sql.SQLException
		public virtual void save(PointLayer pointLayer, int order)
		{
			this.saveLayer(pointLayer);
			this.saveLayerOrder(pointLayer.LayerType, order);
			this.saveFont(pointLayer);
			this.saveSymbolAndPointVisibleProperies(pointLayer);

			switch (pointLayer.LayerType.innerEnumValue)
			{
			case LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
			case LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
			case LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
			case LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
				this.save(pointLayer);
				break;
			default:
				break;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.jag3d.ui.graphic.layer.ObservationLayer observationLayer, int order) throws java.sql.SQLException
		public virtual void save(ObservationLayer observationLayer, int order)
		{
			this.saveLayer(observationLayer);
			this.saveLayerOrder(observationLayer.LayerType, order);
			this.saveObservationLayerColors(observationLayer);

			if (observationLayer.LayerType == LayerType.OBSERVATION_APOSTERIORI)
			{
				this.save(observationLayer);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.jag3d.ui.graphic.layer.ArrowLayer arrowLayer, int order) throws java.sql.SQLException
		public virtual void save(ArrowLayer arrowLayer, int order)
		{
			this.saveLayer(arrowLayer);
			this.saveLayerOrder(arrowLayer.LayerType, order);
			this.saveSymbol(arrowLayer);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.jag3d.ui.graphic.layer.LegendLayer legendLayer, int order) throws java.sql.SQLException
		public virtual void save(LegendLayer legendLayer, int order)
		{
			this.saveLayer(legendLayer);
			this.saveLayerOrder(legendLayer.LayerType, order);
			this.saveFont(legendLayer);
			this.savePosition(legendLayer);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void save(org.applied_geodesy.jag3d.ui.graphic.layer.ConfidenceLayer<?> confidenceLayer, int order) throws java.sql.SQLException
		public virtual void save<T1>(ConfidenceLayer<T1> confidenceLayer, int order)
		{
			this.saveLayer(confidenceLayer);
			this.saveLayerOrder(confidenceLayer.LayerType, order);
			this.saveStrokeColor(confidenceLayer);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void save(org.applied_geodesy.jag3d.ui.graphic.layer.HighlightableLayer layer) throws java.sql.SQLException
		private void save(HighlightableLayer layer)
		{
			string sql = "MERGE INTO \"HighlightLayerProperty\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS INT)) " + ") AS \"vals\" (\"layer\", \"red\", \"green\", \"blue\", \"line_width\", \"type\") ON \"HighlightLayerProperty\".\"layer\" = \"vals\".\"layer\" " + "WHEN MATCHED THEN UPDATE SET " + "\"HighlightLayerProperty\".\"red\"        = \"vals\".\"red\", " + "\"HighlightLayerProperty\".\"green\"      = \"vals\".\"green\", " + "\"HighlightLayerProperty\".\"blue\"       = \"vals\".\"blue\", " + "\"HighlightLayerProperty\".\"line_width\" = \"vals\".\"line_width\", " + "\"HighlightLayerProperty\".\"type\"       = \"vals\".\"type\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"layer\", " + "\"vals\".\"red\", " + "\"vals\".\"green\", " + "\"vals\".\"blue\"," + "\"vals\".\"line_width\", " + "\"vals\".\"type\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, layer.LayerType.getId());
			stmt.setDouble(idx++, layer.HighlightColor.getRed());
			stmt.setDouble(idx++, layer.HighlightColor.getGreen());
			stmt.setDouble(idx++, layer.HighlightColor.getBlue());
			stmt.setDouble(idx++, layer.HighlightLineWidth);
			stmt.setInt(idx++, layer.HighlightType.getId());
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveLayerOrder(org.applied_geodesy.jag3d.ui.graphic.layer.LayerType type, int order) throws java.sql.SQLException
		private void saveLayerOrder(LayerType type, int order)
		{
			string sql = "UPDATE \"Layer\" SET \"order\" = ? WHERE \"type\" = ?";
			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, order);
			stmt.setInt(idx++, type.getId());
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveStrokeColor(org.applied_geodesy.jag3d.ui.graphic.layer.ConfidenceLayer<?> confidenceLayer) throws java.sql.SQLException
		private void saveStrokeColor<T1>(ConfidenceLayer<T1> confidenceLayer)
		{
			string sql = "MERGE INTO \"ConfidenceLayerProperty\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE)) " + ") AS \"vals\" (\"layer\", \"red\", \"green\", \"blue\") ON \"ConfidenceLayerProperty\".\"layer\" = \"vals\".\"layer\" " + "WHEN MATCHED THEN UPDATE SET " + "\"ConfidenceLayerProperty\".\"red\"   = \"vals\".\"red\", " + "\"ConfidenceLayerProperty\".\"green\" = \"vals\".\"green\", " + "\"ConfidenceLayerProperty\".\"blue\"  = \"vals\".\"blue\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"layer\", " + "\"vals\".\"red\", " + "\"vals\".\"green\", " + "\"vals\".\"blue\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, confidenceLayer.LayerType.getId());
			stmt.setDouble(idx++, confidenceLayer.StrokeColor.getRed());
			stmt.setDouble(idx++, confidenceLayer.StrokeColor.getGreen());
			stmt.setDouble(idx++, confidenceLayer.StrokeColor.getBlue());
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveObservationLayerColors(org.applied_geodesy.jag3d.ui.graphic.layer.ObservationLayer observationLayer) throws java.sql.SQLException
		private void saveObservationLayerColors(ObservationLayer observationLayer)
		{
			bool hasBatch = false;
			try
			{
				this.dataBase.AutoCommit = false;
				string sql = "MERGE INTO \"ObservationLayerProperty\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"layer\", \"observation_type\", \"red\", \"green\", \"blue\", \"visible\") " + "ON \"ObservationLayerProperty\".\"layer\" = \"vals\".\"layer\" AND \"ObservationLayerProperty\".\"observation_type\" = \"vals\".\"observation_type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"ObservationLayerProperty\".\"red\"     = \"vals\".\"red\", " + "\"ObservationLayerProperty\".\"green\"   = \"vals\".\"green\", " + "\"ObservationLayerProperty\".\"blue\"    = \"vals\".\"blue\", " + "\"ObservationLayerProperty\".\"visible\" = \"vals\".\"visible\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"layer\", " + "\"vals\".\"observation_type\", " + "\"vals\".\"red\", " + "\"vals\".\"green\", " + "\"vals\".\"blue\", " + "\"vals\".\"visible\" ";

				int idx = 1;
				PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
				stmt.setInt(idx++, observationLayer.LayerType.getId());

				foreach (ObservationSymbolProperties.ObservationType observationType in ObservationSymbolProperties.ObservationType.values())
				{
					idx = 2;
					ObservationSymbolProperties properties = observationLayer.getObservationSymbolProperties(observationType);
					if (properties == null)
					{
						continue;
					}
					stmt.setInt(idx++, observationType.getId());
					stmt.setDouble(idx++, properties.Color.getRed());
					stmt.setDouble(idx++, properties.Color.getGreen());
					stmt.setDouble(idx++, properties.Color.getBlue());
					stmt.setBoolean(idx++, properties.Visible);
					stmt.addBatch();
					hasBatch = true;
				}
				if (hasBatch)
				{
					stmt.executeLargeBatch();
				}
			}
			finally
			{
				this.dataBase.AutoCommit = true;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveLayer(org.applied_geodesy.jag3d.ui.graphic.layer.Layer layer) throws java.sql.SQLException
		private void saveLayer(Layer layer)
		{
			string sql = "MERGE INTO \"Layer\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS INT), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"type\", \"red\", \"green\", \"blue\", \"symbol_size\", \"line_width\", \"order\", \"visible\") ON \"Layer\".\"type\" = \"vals\".\"type\" " + "WHEN MATCHED THEN UPDATE SET " + "\"Layer\".\"red\"         = \"vals\".\"red\", " + "\"Layer\".\"green\"       = \"vals\".\"green\", " + "\"Layer\".\"blue\"        = \"vals\".\"blue\", " + "\"Layer\".\"symbol_size\" = \"vals\".\"symbol_size\", " + "\"Layer\".\"line_width\"  = \"vals\".\"line_width\", " + "\"Layer\".\"order\"       = \"vals\".\"order\", " + "\"Layer\".\"visible\"     = \"vals\".\"visible\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"type\", " + "\"vals\".\"red\", " + "\"vals\".\"green\", " + "\"vals\".\"blue\", " + "\"vals\".\"symbol_size\", " + "\"vals\".\"line_width\", " + "\"vals\".\"order\", " + "\"vals\".\"visible\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, layer.LayerType.getId());
			stmt.setDouble(idx++, layer.Color.getRed());
			stmt.setDouble(idx++, layer.Color.getGreen());
			stmt.setDouble(idx++, layer.Color.getBlue());
			stmt.setDouble(idx++, layer.SymbolSize);
			stmt.setDouble(idx++, layer.LineWidth);
			stmt.setInt(idx++, -1);
			stmt.setBoolean(idx++, layer.Visible);
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveSymbol(org.applied_geodesy.jag3d.ui.graphic.layer.ArrowLayer arrowLayer) throws java.sql.SQLException
		private void saveSymbol(ArrowLayer arrowLayer)
		{
			string sql = "MERGE INTO \"ArrowLayerProperty\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT)) " + ") AS \"vals\" (\"layer\", \"type\") ON \"ArrowLayerProperty\".\"layer\" = \"vals\".\"layer\" " + "WHEN MATCHED THEN UPDATE SET " + "\"ArrowLayerProperty\".\"type\" = \"vals\".\"type\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"layer\", " + "\"vals\".\"type\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, arrowLayer.LayerType.getId());
			stmt.setInt(idx++, arrowLayer.SymbolType.getId());
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void savePosition(org.applied_geodesy.jag3d.ui.graphic.layer.LegendLayer legendLayer) throws java.sql.SQLException
		private void savePosition(LegendLayer legendLayer)
		{
			string sql = "MERGE INTO \"LegendLayerProperty\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT)) " + ") AS \"vals\" (\"layer\", \"type\") ON \"LegendLayerProperty\".\"layer\" = \"vals\".\"layer\" " + "WHEN MATCHED THEN UPDATE SET " + "\"LegendLayerProperty\".\"type\" = \"vals\".\"type\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"layer\", " + "\"vals\".\"type\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, legendLayer.LayerType.getId());
			stmt.setInt(idx++, legendLayer.LegendPositionType.getId());
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveSymbolAndPointVisibleProperies(org.applied_geodesy.jag3d.ui.graphic.layer.PointLayer pointLayer) throws java.sql.SQLException
		private void saveSymbolAndPointVisibleProperies(PointLayer pointLayer)
		{
			string sql = "MERGE INTO \"PointLayerProperty\" USING (VALUES " + "(CAST(? AS INT), CAST(? AS INT), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN), CAST(? AS BOOLEAN)) " + ") AS \"vals\" (\"layer\", \"type\", \"point_1d_visible\", \"point_2d_visible\", \"point_3d_visible\") ON \"PointLayerProperty\".\"layer\" = \"vals\".\"layer\" " + "WHEN MATCHED THEN UPDATE SET " + "\"PointLayerProperty\".\"type\" = \"vals\".\"type\", " + "\"PointLayerProperty\".\"point_1d_visible\" = \"vals\".\"point_1d_visible\", " + "\"PointLayerProperty\".\"point_2d_visible\" = \"vals\".\"point_2d_visible\", " + "\"PointLayerProperty\".\"point_3d_visible\" = \"vals\".\"point_3d_visible\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"layer\", " + "\"vals\".\"type\", " + "\"vals\".\"point_1d_visible\", " + "\"vals\".\"point_2d_visible\", " + "\"vals\".\"point_3d_visible\" ";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, pointLayer.LayerType.getId());
			stmt.setInt(idx++, pointLayer.PointSymbolType.getId());
			stmt.setBoolean(idx++, pointLayer.Point1DVisible);
			stmt.setBoolean(idx++, pointLayer.Point2DVisible);
			stmt.setBoolean(idx++, pointLayer.Point3DVisible);
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void saveFont(org.applied_geodesy.jag3d.ui.graphic.layer.FontLayer fontLayer) throws java.sql.SQLException
		private void saveFont(FontLayer fontLayer)
		{
			string sql = "MERGE INTO \"LayerFont\" USING (VALUES " + "(CAST(? AS INT), ?, CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE), CAST(? AS DOUBLE)) " + ") AS \"vals\" (\"layer\", \"family\", \"size\", \"red\", \"green\", \"blue\") ON \"LayerFont\".\"layer\" = \"vals\".\"layer\" " + "WHEN MATCHED THEN UPDATE SET " + "\"LayerFont\".\"family\" = \"vals\".\"family\", " + "\"LayerFont\".\"size\"   = \"vals\".\"size\", " + "\"LayerFont\".\"red\"    = \"vals\".\"red\", " + "\"LayerFont\".\"green\"  = \"vals\".\"green\", " + "\"LayerFont\".\"blue\"   = \"vals\".\"blue\" " + "WHEN NOT MATCHED THEN INSERT VALUES " + "\"vals\".\"layer\", " + "\"vals\".\"family\", " + "\"vals\".\"size\", " + "\"vals\".\"red\", " + "\"vals\".\"green\", " + "\"vals\".\"blue\" ";
			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, fontLayer.LayerType.getId());
			stmt.setString(idx++, fontLayer.FontFamily);
			stmt.setDouble(idx++, fontLayer.FontSize);
			stmt.setDouble(idx++, fontLayer.FontColor.getRed());
			stmt.setDouble(idx++, fontLayer.FontColor.getGreen());
			stmt.setDouble(idx++, fontLayer.FontColor.getBlue());
			stmt.execute();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void load(org.applied_geodesy.jag3d.ui.graphic.layer.ObservationLayer observationLayer) throws java.sql.SQLException
		private void load(ObservationLayer observationLayer)
		{
			this.loadLayer(observationLayer);
			this.loadObservationColors(observationLayer);

			if (observationLayer.LayerType == LayerType.OBSERVATION_APOSTERIORI)
			{
				this.loadHighlightProperties(observationLayer);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void load(org.applied_geodesy.jag3d.ui.graphic.layer.PointLayer pointLayer) throws java.sql.SQLException
		private void load(PointLayer pointLayer)
		{
			this.loadLayer(pointLayer);
			this.loadFont(pointLayer);
			this.loadSymbolAndPointVisibleProperties(pointLayer);

			switch (pointLayer.LayerType.innerEnumValue)
			{
			case LayerType.InnerEnum.DATUM_POINT_APOSTERIORI:
			case LayerType.InnerEnum.REFERENCE_POINT_APOSTERIORI:
			case LayerType.InnerEnum.REFERENCE_POINT_APRIORI:
			case LayerType.InnerEnum.STOCHASTIC_POINT_APOSTERIORI:
				this.loadHighlightProperties(pointLayer);
				break;
			default:
				break;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadHighlightProperties(org.applied_geodesy.jag3d.ui.graphic.layer.HighlightableLayer layer) throws java.sql.SQLException
		private void loadHighlightProperties(HighlightableLayer layer)
		{
			string sql = "SELECT " + "\"red\", \"green\", \"blue\", \"line_width\", \"type\" " + "FROM \"HighlightLayerProperty\" " + "WHERE \"layer\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, layer.LayerType.getId());
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				int typeId = rs.getInt("type");
				TableRowHighlightType type = TableRowHighlightType.getEnumByValue(typeId);
				if (!rs.wasNull() && type != null)
				{
					layer.HighlightType = type;
				}
				else
				{
					layer.HighlightType = TableRowHighlightType.NONE;
				}

				double opacity = 1.0;

				double red = rs.getDouble("red");
				red = Math.Min(Math.Max(0, red), 1);

				double green = rs.getDouble("green");
				green = Math.Min(Math.Max(0, green), 1);

				double blue = rs.getDouble("blue");
				blue = Math.Min(Math.Max(0, blue), 1);

				double lineWidth = rs.getDouble("line_width");

				layer.HighlightColor = new Color(red, green, blue, opacity);
				layer.HighlightLineWidth = lineWidth >= 0 ? lineWidth : 0;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void load(org.applied_geodesy.jag3d.ui.graphic.layer.ArrowLayer arrowLayer) throws java.sql.SQLException
		private void load(ArrowLayer arrowLayer)
		{
			this.loadLayer(arrowLayer);
			this.loadSymbol(arrowLayer);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void load(org.applied_geodesy.jag3d.ui.graphic.layer.LegendLayer legendLayer) throws java.sql.SQLException
		private void load(LegendLayer legendLayer)
		{
			this.loadLayer(legendLayer);
			this.loadFont(legendLayer);
			this.loadPosition(legendLayer);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void load(org.applied_geodesy.jag3d.ui.graphic.layer.ConfidenceLayer<?> confidenceLayer) throws java.sql.SQLException
		private void load<T1>(ConfidenceLayer<T1> confidenceLayer)
		{
			this.loadLayer(confidenceLayer);
			this.loadStrokeColor(confidenceLayer);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadSymbolAndPointVisibleProperties(org.applied_geodesy.jag3d.ui.graphic.layer.PointLayer pointLayer) throws java.sql.SQLException
		private void loadSymbolAndPointVisibleProperties(PointLayer pointLayer)
		{
			string sql = "SELECT " + "\"type\", \"point_1d_visible\", \"point_2d_visible\", \"point_3d_visible\" " + "FROM \"PointLayerProperty\" " + "WHERE \"layer\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, pointLayer.LayerType.getId());
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				PointSymbolType pointSymbolType = PointSymbolType.getEnumByValue(rs.getInt("type"));
				if (pointSymbolType == null)
				{
					return;
				}

				pointLayer.SymbolType = pointSymbolType;
				pointLayer.Point1DVisible = rs.getBoolean("point_1d_visible");
				pointLayer.Point2DVisible = rs.getBoolean("point_2d_visible");
				pointLayer.Point3DVisible = rs.getBoolean("point_3d_visible");
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadStrokeColor(org.applied_geodesy.jag3d.ui.graphic.layer.ConfidenceLayer<?> confidenceLayer) throws java.sql.SQLException
		private void loadStrokeColor<T1>(ConfidenceLayer<T1> confidenceLayer)
		{
			string sql = "SELECT " + "\"red\", \"green\", \"blue\" " + "FROM \"ConfidenceLayerProperty\" " + "WHERE \"layer\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, confidenceLayer.LayerType.getId());
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				double opacity = 1.0;

				double red = rs.getDouble("red");
				red = Math.Min(Math.Max(0, red), 1);

				double green = rs.getDouble("green");
				green = Math.Min(Math.Max(0, green), 1);

				double blue = rs.getDouble("blue");
				blue = Math.Min(Math.Max(0, blue), 1);

				confidenceLayer.StrokeColor = new Color(red, green, blue, opacity);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadSymbol(org.applied_geodesy.jag3d.ui.graphic.layer.ArrowLayer arrowLayer) throws java.sql.SQLException
		private void loadSymbol(ArrowLayer arrowLayer)
		{
			string sql = "SELECT " + "\"type\" " + "FROM \"ArrowLayerProperty\" " + "WHERE \"layer\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, arrowLayer.LayerType.getId());
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				ArrowSymbolType arrowSymbolType = ArrowSymbolType.getEnumByValue(rs.getInt("type"));
				if (arrowSymbolType == null)
				{
					return;
				}

				arrowLayer.SymbolType = arrowSymbolType;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadPosition(org.applied_geodesy.jag3d.ui.graphic.layer.LegendLayer legendLayer) throws java.sql.SQLException
		private void loadPosition(LegendLayer legendLayer)
		{
			string sql = "SELECT " + "\"type\" " + "FROM \"LegendLayerProperty\" " + "WHERE \"layer\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, legendLayer.LayerType.getId());
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				LegendPositionType legendPositionType = LegendPositionType.getEnumByValue(rs.getInt("type"));
				if (legendPositionType == null)
				{
					return;
				}

				legendLayer.LegendPositionType = legendPositionType;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadFont(org.applied_geodesy.jag3d.ui.graphic.layer.FontLayer layer) throws java.sql.SQLException
		private void loadFont(FontLayer layer)
		{
			string sql = " SELECT " + "\"family\", \"size\", " + "\"red\", \"green\", \"blue\" " + "FROM \"LayerFont\" " + "WHERE \"layer\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, layer.LayerType.getId());
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				string fontFamily = rs.getString("family");
				double fontSize = rs.getDouble("size");

				double opacity = 1.0;

				double red = rs.getDouble("red");
				red = Math.Min(Math.Max(0, red), 1);

				double green = rs.getDouble("green");
				green = Math.Min(Math.Max(0, green), 1);

				double blue = rs.getDouble("blue");
				blue = Math.Min(Math.Max(0, blue), 1);

				layer.FontColor = new Color(red, green, blue, opacity);
				layer.FontFamily = fontFamily;
				layer.FontSize = fontSize;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadLayer(org.applied_geodesy.jag3d.ui.graphic.layer.Layer layer) throws java.sql.SQLException
		private void loadLayer(Layer layer)
		{
			string sql = "SELECT " + "\"red\", \"green\", \"blue\", \"symbol_size\", \"line_width\", \"visible\" " + "FROM \"Layer\" " + "WHERE \"type\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, layer.LayerType.getId());
			ResultSet rs = stmt.executeQuery();

			if (rs.next())
			{
				double opacity = 1.0;

				double red = rs.getDouble("red");
				red = Math.Min(Math.Max(0, red), 1);

				double green = rs.getDouble("green");
				green = Math.Min(Math.Max(0, green), 1);

				double blue = rs.getDouble("blue");
				blue = Math.Min(Math.Max(0, blue), 1);

				double symbolSize = rs.getDouble("symbol_size");
				symbolSize = Math.Max(0, symbolSize);

				double lineWidth = rs.getDouble("line_width");
				lineWidth = Math.Max(0, lineWidth);

				bool visible = rs.getBoolean("visible");

				layer.Color = new Color(red, green, blue, opacity);
				layer.SymbolSize = symbolSize;
				layer.LineWidth = lineWidth;
				layer.Visible = visible;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void loadObservationColors(org.applied_geodesy.jag3d.ui.graphic.layer.ObservationLayer observationLayer) throws java.sql.SQLException
		private void loadObservationColors(ObservationLayer observationLayer)
		{
			string sql = "SELECT " + "\"observation_type\", " + "\"red\", \"green\", \"blue\", \"visible\" " + "FROM \"ObservationLayerProperty\" " + "WHERE \"layer\" = ? AND \"observation_type\" = ? LIMIT 1";

			int idx = 1;
			PreparedStatement stmt = this.dataBase.getPreparedStatement(sql);
			stmt.setInt(idx++, observationLayer.LayerType.getId());

			foreach (ObservationSymbolProperties.ObservationType observationType in ObservationSymbolProperties.ObservationType.values())
			{
				stmt.setInt(idx, observationType.getId());
				ResultSet rs = stmt.executeQuery();
				ObservationSymbolProperties properties = observationLayer.getObservationSymbolProperties(observationType);

				if (rs.next() && properties != null)
				{
					double opacity = 1.0;

					double red = rs.getDouble("red");
					red = Math.Min(Math.Max(0, red), 1);

					double green = rs.getDouble("green");
					green = Math.Min(Math.Max(0, green), 1);

					double blue = rs.getDouble("blue");
					blue = Math.Min(Math.Max(0, blue), 1);

					bool visible = rs.getBoolean("visible");

					properties.Color = new Color(red, green, blue, opacity);
					properties.Visible = visible;
				}
			}
		}
	}

}