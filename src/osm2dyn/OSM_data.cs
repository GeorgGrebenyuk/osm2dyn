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
    public class OSM_data
    {
        public string path_to_downloaded_file;
        //Dynamo data
        public Dictionary<string, List<double>> id2point = new Dictionary<string, List<double>>();

        public List<OSM_object> osm_points;
        public List<OSM_object> osm_polylines;
        public List<OSM_object> osm_polygons;

        /// <summary>
        /// Load from Web (https://www.openstreetmap.org) OSM file for terrtory or getting an error if operation unsuccessfully
        /// </summary>
        /// <param name="bb">BoundingBox to needing area (coordinates of corner points)</param>
        public OSM_data(double min_longitude, double min_latitude, double max_longitude, double max_latitude)
        {
            //https://wiki.openstreetmap.org/wiki/Downloading_data
            //Execution for large area. It's may to use overpass.openstreetmap.ru but we're not did it
            if (Math.Abs(max_longitude - min_longitude) >= 0.5 | Math.Abs(max_latitude - min_latitude) >= 0.5) return;
            //in this order: left, bottom, right, top (min longitude, min latitude, max longitude, max latitude)
            string request = $"https://api.openstreetmap.org/api/0.6/map?bbox=min_long,min_lat,max_long,max_lat";
            request = request.
                Replace("min_long", min_longitude.ToString()).
                Replace("min_lat", min_latitude.ToString()).
                Replace("max_long", max_longitude.ToString()).
                Replace("max_lat", max_latitude.ToString());

            WebClient wc = new WebClient();
            this.path_to_downloaded_file = Path.GetTempPath() + "\\" + Guid.NewGuid().ToString() + ".osm";
            wc.DownloadFile(request, this.path_to_downloaded_file);
            this.StartReading();
        }

        private void StartReading()
        {
            //X = lat; Y = long
            //https://wiki.openstreetmap.org/wiki/OSM_XML
            XDocument osm_file = XDocument.Load(this.path_to_downloaded_file);

            //https://wiki.openstreetmap.org/wiki/Node
            id2point = new Dictionary<string, List<double>>();
            osm_points = new List<OSM_object>();
            IEnumerable<XElement> osm_nodes = osm_file.Descendants().Where(a => a.Name.LocalName == "node");
            foreach (XElement el in osm_nodes)
            {
                string id = el.Attribute("id").Value;
                double latitude = Convert.ToDouble(el.Attribute("lat").Value);
                double longitude = Convert.ToDouble(el.Attribute("lon").Value);

                IEnumerable<XElement> tags = el.Descendants().Where(b => b.Name.LocalName == "tag");
                if (tags.Any())
                {
                    Dictionary<string, string> attrs = new Dictionary<string, string>();
                    foreach (XElement one_tag in tags)
                    {
                        attrs.Add(one_tag.Attribute("k").Value, one_tag.Attribute("v").Value);
                    }
                    osm_points.Add(new OSM_object(new List<List<double>>(1) { new List<double>(3) { latitude, longitude, 0d } }, attrs));

                }
                id2point.Add(id, new List<double>(3) { latitude, longitude, 0d });

            }
            //https://wiki.openstreetmap.org/wiki/Way lines and polygons
            osm_polylines = new List<OSM_object>();
            osm_polygons = new List<OSM_object>();
            IEnumerable<XElement> osm_ways = osm_file.Descendants().Where(a => a.Name.LocalName == "way");
            foreach (XElement el in osm_ways)
            {
                bool IsPlg = false;
                IEnumerable<XElement> nodes = el.Descendants().Where(b => b.Name.LocalName == "nd");
                
                if (nodes.Any() && nodes.Count() > 1)
                {
                    List<List<double>> points = new List<List<double>>();
                    foreach (XElement p_ref in nodes)
                    {
                        points.Add(id2point[p_ref.Attribute("ref").Value]);
                    }
                    if (points[0] == points[points.Count()-1]) IsPlg = true;

                    IEnumerable<XElement> tags = el.Descendants().Where(b => b.Name.LocalName == "tag");
                    if (tags.Any())
                    {
                        Dictionary<string, string> attrs = new Dictionary<string, string>();
                        foreach (XElement one_tag in tags)
                        {
                            attrs.Add(one_tag.Attribute("k").Value, one_tag.Attribute("v").Value);
                        }
                        if (!IsPlg) osm_polygons.Add(new OSM_object(points, attrs));
                        else osm_polylines.Add(new OSM_object(points, attrs));
                    }
                }
            }
            //Relations ?

            File.Delete(this.path_to_downloaded_file);
        }
        /// <summary>
        /// Getting OSM points with attributes
        /// </summary>
        /// <returns></returns>
        public List<OSM_object> get_OSM_points()
        {
            return this.osm_points;
        }
        /// <summary>
        /// Getting OSM polylines with attributes
        /// </summary>
        /// <returns></returns>
        public List<OSM_object> get_OSM_polygons()
        {
            return this.osm_polylines;
        }
        /// <summary>
        /// Getting OSM polygons with attributes
        /// </summary>
        /// <returns></returns>
        public List<OSM_object> get_OSM_polylines()
        {
            return this.osm_polygons;
        }
    }
}
