using Autodesk.Map.IM.Data;
using Autodesk.Map.IM.Data.Provider;
using Autodesk.Map.IM.Graphic;
using GEOBOX.OSC.Common.Logging;
using GEOBOX.OSC.IM.PointOnLineRemover.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Windows.Documents;
using GEOBOX.OSC.IM.PointOnLineRemover.Properties;

namespace GEOBOX.OSC.IM.PointOnLineRemover.Controllers
{
    internal class LinePointsController
    {
        private readonly ILogger logger;
        private readonly Dictionary<string, PointCoordinateDetail> pointsToRemove;
        private readonly string pointsToRemoveJson;

        /// <summary>
        /// Contruct with the points to remove and logger
        /// </summary>
        /// <param name="pointsToRemove">Points to remove</param>
        /// <param name="logger">Logger</param>
        /// <exception cref="ArgumentException">Dublicated points</exception>
        public LinePointsController(List<PointCoordinateDetail> pointsToRemove, ILogger logger)
        {
            this.logger = logger;
            pointsToRemoveJson = JsonSerializer.Serialize(pointsToRemove);

            // pointsToRemove to dictionary with new key
            this.pointsToRemove = pointsToRemove.ToDictionary(elem => elem.UUID, elem => elem);
        }

        /// <summary>
        /// Remove points from feature class (line).
        /// </summary>
        /// <param name="featureClass">Feature class</param>
        internal void RemovePointsFromLines(FeatureClass featureClass)
        {
            logger.WriteInformation(String.Format(Resources.LogMessage_StartFeatureClass, featureClass.Caption, featureClass.Name));

            var tolerance = featureClass.Tolerance;

            // find the lines (FIDs) matching the points
            var featuresWithPointsToRemove = FindPointsOnLines(featureClass);

            // get the features
            var features = featureClass.GetFeatures(true, featuresWithPointsToRemove.Keys);

            int changeCounter = 0;

            // loop the affected features
            foreach (var feature in features)
            {
                var geometry = feature.Geometry;
                if (geometry != null && geometry is LineString lineString)
                {
                    // loop the poins matching this feature
                    foreach (var point in featuresWithPointsToRemove[feature.FID])
                    {
                        // loop the line points, skip start and endpoint
                        for (int linePointIndex = 0; linePointIndex < lineString.Count; linePointIndex++)
                        {
                            var linePoint = lineString[linePointIndex];
                            if (Math.Abs(linePoint.X - point.East) < tolerance && Math.Abs(linePoint.Y - point.North) < tolerance)
                            {
                                if (linePointIndex == 0) // startpoint
                                {
                                    logger.WriteWarning(String.Format(Resources.LinePoint_StartPoint_Message, point.East, point.North, feature.FID));
                                }
                                else if (linePointIndex == (lineString.Count - 1)) // endpoint
                                {
                                    logger.WriteWarning(String.Format(Resources.LinePoint_EndPoint_Message, point.East, point.North, feature.FID));
                                }
                                else
                                {
                                    lineString.RemoveAt(linePointIndex);
                                    logger.WriteInformation(String.Format(Resources.LinePoint_SuccesRemoved_Message, point.East, point.North, feature.FID));
                                    changeCounter++;
                                }

                                // Set the changed geometry on the feature again, otherwise change tracker seems not to be triggered
                                feature.Geometry = geometry;

                                // only one line point at a position expected
                                break;
                            }
                        }
                    }
                }
                else if (geometry != null)
                {
                    logger.WriteWarning($"Geometry is not a LineString - Feature: {feature.FID}");
                }
            }

            try
            {
                featureClass.UpdateFeatures(features, true);
            }
            catch (Exception ex)
            {
                logger.WriteError(String.Format(Resources.LogMessage_UpdateFeature_Error, featureClass.Caption, featureClass.Name, ex.Message));
            }

            logger.WriteInformation(String.Format(Resources.LogMessage_EndFeatureClass, featureClass.Caption, featureClass.Name, features.Count(), changeCounter));
        }

        /// <summary>
        /// Find the features with a line geom matching the points.
        /// </summary>
        /// <param name="featureClass">Feature class to find points on.</param>
        /// <returns>The features and the matching points (CoordinateCollection).</returns>
        private Dictionary<long, List<PointCoordinateDetail>> FindPointsOnLines(FeatureClass featureClass)
        {
            var connection = featureClass.Connection;
            var query = LinesByPointsQuery(featureClass);

            var featuresWithPoints = new Dictionary<long /* FID */, List<PointCoordinateDetail> /* Points to remove */>();

            using (Command command = new Command(query, connection))
            {
                command.Parameters.Add("json_input", pointsToRemoveJson);

                using (DataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var fid = reader.GetInt64(0);
                        var idPoint = reader.GetString(1);

                        // Add feature to the dictionary
                        if (!featuresWithPoints.ContainsKey(fid))
                            featuresWithPoints.Add(fid, new List<PointCoordinateDetail>());

                        // Add point to the dictionary
                        featuresWithPoints[fid].Add(pointsToRemove[idPoint]);
                    }
                }
            }

            return featuresWithPoints;
        }

        /// <summary>
        /// Get an SQL to find the features containing the linepoints in the JSON.
        /// The lookup is performed in two steps:
        /// 1. Spacial mask (faster)
        /// 2. Exact points (slow)
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        private string LinesByPointsQuery(FeatureClass featureClass)
        {
            var srid = featureClass.CoordinateSystem.Id;
            var tolerance = featureClass.Tolerance;

            return @$"
/* 1. Load the points from a json into POINTS_TO_FIND. */
with POINTS_TO_FIND as
(
    select ID_POINT,
        sdo_geometry
        (
            2001, /* 2D Point*/
            {srid},
            sdo_point_type(x, y, NULL),
            NULL,
            NULL
        ) AS geom
    from json_table
        (
            :json_input,
            '$[*]'
            columns
            (
                ID_POINT varchar(36) path '$.UUID',
                x        number path '$.East',
                y        number path '$.North'
            )
        )
)
/* 2. Join the POINTS_TO_FIND with the specified FEATURECLASS. */
select distinct
        L.FID        AS FID,
        P.ID_POINT   AS ID_POINT
from    {featureClass.Name} L
join    POINTS_TO_FIND P
/* 2.1 Filter POINTS_TO_FIND are on the line geometry of FEATURECLASS.GEOM (ignores if it matches a linepoint or not). */
on      SDO_RELATE
        (
            L.GEOM,
            P.GEOM,
            'mask=ANYINTERACT'
        ) = 'TRUE'
/* 2.2 Filter if POINTS_TO_FIND match explicit linepoints of FEATURECLASS.GEOM. */
        and exists
        (
            select 1
            from   TABLE(SDO_UTIL.GETVERTICES(L.GEOM)) V
            where  
                ABS(V.X - P.GEOM.SDO_POINT.X) < {tolerance}
                and ABS(V.Y - P.GEOM.SDO_POINT.Y) < {tolerance}
        )";
        }
    }
}
