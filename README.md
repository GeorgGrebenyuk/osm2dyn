# osm2dyn
Source code for Autodesk Dynamo's package "osm2dyn" to load OSM (Open Street Map) data and convert it to Dynamo data types
# About package
This package contains methods to download local OSM file by your interested area via ```api.openstreetmap.org```. Result of file is avaliable as three Lists (for point-data, linear-tata and polygons-data with attributes).

OSM data | Gynamo data
-- | --
Point-data | Point
Linear-data, no closed | Polycurve
Linear-data, is closed | Polygons

# Sample materials
Look script ```loading_Rotterdam.dyn``` in Releases's materials.
