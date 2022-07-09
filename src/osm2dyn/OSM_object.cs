using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;

using dg = Autodesk.DesignScript.Geometry;
using dr = Autodesk.DesignScript.Runtime;

using System.Xml.Linq;

namespace osm2dyn
{
    public class OSM_object
    {
        /// <summary>
        /// 0 = point; 1 = polycurve; 2 = polygons
        /// </summary>
        public int geom_type;
        public List<List<double>> geometry;
        public Dictionary<string, string> attributes;
        //public OSM_object (dg.Point object_geometry, Dictionary<string, string> object_tags)
        //{
        //    this.attributes = object_tags;
        //    this.geometry = object_geometry;
        //}
        public OSM_object(List<List<double>> object_geometry, Dictionary<string, string> object_tags)
        {
            this.attributes = object_tags;
            this.geometry = object_geometry;
        }
        public List<List<double>> GetGeometry()
        {
            return this.geometry;
        }
        public Dictionary<string, string> GetAttributes()
        {
            return this.attributes;
        }
    }
}
