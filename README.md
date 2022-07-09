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
Read [article about develop that package](https://zen.yandex.ru/media/id/5d0dba97ecd5cf00afaf2938/dynamo-i-openstreetmap-62c9c27b6060bd7a75496c1a) (Russian only).
