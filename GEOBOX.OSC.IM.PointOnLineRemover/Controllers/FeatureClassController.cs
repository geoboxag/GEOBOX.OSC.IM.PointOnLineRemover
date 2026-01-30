using Autodesk.Map.IM.Data;
using Autodesk.Map.IM.Forms;
using GEOBOX.OSC.IM.PointOnLineRemover.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEOBOX.OSC.IM.PointOnLineRemover.Controllers
{
    /// <summary>
    /// Read Feature Classes from Database (Document)
    /// </summary>
    internal class FeatureClassController
    {
        private Document document;

        public FeatureClassController(Document tbDocument)
        {
            document = tbDocument;
        }

        internal List<FeatureClassDetail> GetFeatureClasses()
        {
            return LandmanagementStaticBoundarys();
        }

        private List<FeatureClassDetail> LandmanagementStaticBoundarys()
        {
            List<FeatureClassDetail> classList = new List<FeatureClassDetail>();

            List<string> classNames = new List<string>()
            {
                "LM_AD_CANTON_BOUNDARY_L",
                "LM_AD_COUNTRY_BOUNDARY",
                "LM_AD_DISTRICT_BOUNDARY_L",
                "LM_AD_MUNICIP_BOUNDARY_L"
            };

            foreach (string className in classNames)
            {
                if (document.Connection.FeatureClasses.Contains(className))
                {
                    FeatureClass fc = document.Connection.FeatureClasses.Get(className);
                    string displayText = $"{fc.Caption} ({fc.Name})";
                    classList.Add(new FeatureClassDetail(displayText, fc));
                }
            }

            return classList;
        }
    }
}