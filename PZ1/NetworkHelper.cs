using PZ1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace PZ1
{
    public class NetworkHelper
    {
        private static List<Point> allPoints = new List<Point>();
        private static List<Point> busyPoints = new List<Point>();
        private static List<Entity> groupElement = new List<Entity>();

        public static Network ParseXML(string path)
        {
            Network retNetwork = new Network();

            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlNodeList substations = document.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList switches = document.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodes = document.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            XmlNodeList lines = document.SelectNodes("/NetworkModel/Lines/LineEntity");

            retNetwork.Substations = GetSubstations(substations);
            retNetwork.Switches = GetSwitches(switches);
            retNetwork.Nodes = GetNodes(nodes);
            retNetwork.Lines = GetLines(lines);

            return retNetwork;
        }

        private static List<SubstationEntity> GetSubstations(XmlNodeList xmlSubstations)
        {
            List<SubstationEntity> substationEntities = new List<SubstationEntity>();

            for (int i = 0; i < xmlSubstations.Count; i++)
            {
                SubstationEntity substationEntity = new SubstationEntity();

                substationEntity.Id = long.Parse(xmlSubstations[i].SelectSingleNode("Id").InnerText);
                substationEntity.Name = xmlSubstations[i].SelectSingleNode("Name").InnerText;
                substationEntity.X = double.Parse(xmlSubstations[i].SelectSingleNode("X").InnerText);
                substationEntity.Y = double.Parse(xmlSubstations[i].SelectSingleNode("Y").InnerText);

                ToLatLon(substationEntity.X, substationEntity.Y, 34, out double x, out double y);
                substationEntity.X = x;
                substationEntity.Y = y;

                substationEntities.Add(substationEntity);
            }

            return substationEntities;
        }

        private static List<SwitchEntity> GetSwitches(XmlNodeList xmlSwitches)
        {
            List<SwitchEntity> sw = new List<SwitchEntity>();

            for(int i = 0; i < xmlSwitches.Count; i++)
            {
                SwitchEntity switchEntity = new SwitchEntity();

                switchEntity.Id = long.Parse(xmlSwitches[i].SelectSingleNode("Id").InnerText);
                switchEntity.Name = xmlSwitches[i].SelectSingleNode("Name").InnerText;
                switchEntity.X = double.Parse(xmlSwitches[i].SelectSingleNode("X").InnerText);
                switchEntity.Y = double.Parse(xmlSwitches[i].SelectSingleNode("Y").InnerText);
                switchEntity.Status = xmlSwitches[i].SelectSingleNode("Status").InnerText;

                ToLatLon(switchEntity.X, switchEntity.Y, 34, out double x, out double y);
                switchEntity.X = x;
                switchEntity.Y = y;

                sw.Add(switchEntity);
            }

            return sw;
        }

        private static List<NodeEntity> GetNodes(XmlNodeList xmlNodes)
        {
            List<NodeEntity> nodes = new List<NodeEntity>();

            for (int i = 0; i < xmlNodes.Count; i++)
            {
                NodeEntity node = new NodeEntity();

                node.Id = long.Parse(xmlNodes[i].SelectSingleNode("Id").InnerText);
                node.Name = xmlNodes[i].SelectSingleNode("Name").InnerText;
                node.X = double.Parse(xmlNodes[i].SelectSingleNode("X").InnerText);
                node.Y = double.Parse(xmlNodes[i].SelectSingleNode("Y").InnerText);

                ToLatLon(node.X, node.Y, 34, out double x, out double y);
                node.X = x;
                node.Y = y;

                nodes.Add(node);
            }

            return nodes;
        }

        private static List<LineEntity> GetLines(XmlNodeList xmlLines)
        {
            List<LineEntity> lines = new List<LineEntity>();

            for (int i = 0; i < xmlLines.Count; i++)
            {
                LineEntity line = new LineEntity();

                line.ConductorMaterial = xmlLines[i].SelectSingleNode("ConductorMaterial").InnerText;
                line.FirstEnd = long.Parse(xmlLines[i].SelectSingleNode("FirstEnd").InnerText);
                line.Id = long.Parse(xmlLines[i].SelectSingleNode("Id").InnerText);
                line.IsUnderground = bool.Parse(xmlLines[i].SelectSingleNode("IsUnderground").InnerText);
                line.LineType = xmlLines[i].SelectSingleNode("LineType").InnerText;
                line.Name = xmlLines[i].SelectSingleNode("Name").InnerText;
                line.R = float.Parse(xmlLines[i].SelectSingleNode("R").InnerText);
                line.SecondEnd = long.Parse(xmlLines[i].SelectSingleNode("SecondEnd").InnerText);
                line.ThermalConstantHeat = long.Parse(xmlLines[i].SelectSingleNode("ThermalConstantHeat").InnerText);

                lines.Add(line);
            }

            return lines;
        }

        public static void ScalePositions(Network network)
        {
            double minX = network.Substations[0].X;
            double minY = network.Substations[0].Y;
            double maxX = network.Substations[0].X;
            double maxY = network.Substations[0].Y;

            foreach (SubstationEntity item in network.Substations)
            {
                if (item.X < minX)
                    minX = item.X;
                if (item.Y < minY)
                    minY = item.Y;
                if (item.X > maxX)
                    maxX = item.X;
                if (item.Y > maxY)
                    maxY = item.Y;
            }

            foreach (SwitchEntity item in network.Switches)
            {
                if (item.X < minX)
                    minX = item.X;
                if (item.Y < minY)
                    minY = item.Y;
                if (item.X > maxX)
                    maxX = item.X;
                if (item.Y > maxY)
                    maxY = item.Y;
            }

            foreach (NodeEntity item in network.Nodes)
            {
                if (item.X < minX)
                    minX = item.X;
                if (item.Y < minY)
                    minY = item.Y;
                if (item.X > maxX)
                    maxX = item.X;
                if (item.Y > maxY)
                    maxY = item.Y;
            }

            double stepX = (maxX - minX) / 100;
            double stepY = (maxY - minY) / 100;

            ScaleSubstations(network, minX, minY, stepX, stepY);
            ScaleSwitches(network, minX, minY, stepX, stepY);
            ScaleNodes(network, minX, minY, stepX, stepY);

        }

        private static void ScaleSubstations(Network network, double minX, double minY, double stepX, double stepY)
        {
            bool found;
            foreach (SubstationEntity se in network.Substations)
            {
                found = false;

                se.XPosition = (int)Math.Ceiling((se.X - minX) / stepX);
                se.YPosition = (int)(Math.Ceiling((se.Y - minY) / stepY));

                foreach (Point p in allPoints)
                {
                    if (p.X == se.XPosition && p.Y == se.YPosition)
                    {
                        busyPoints.Add(new Point(se.XPosition, se.YPosition));
                        found = true;
                        break;
                    }
                }

                if (!found)
                    allPoints.Add(new Point(se.XPosition, se.YPosition));
            }
        }

        private static void ScaleSwitches(Network network, double minX, double minY, double stepX, double stepY)
        {
            bool found;
            foreach (SwitchEntity se in network.Switches)
            {
                found = false;

                se.XPosition = (int)Math.Ceiling((se.X - minX) / stepX);
                se.YPosition = (int)(Math.Ceiling((se.Y - minY) / stepY));

                foreach (Point p in allPoints)
                {
                    if (p.X == se.XPosition && p.Y == se.YPosition)
                    {
                        busyPoints.Add(new Point(se.XPosition, se.YPosition));
                        found = true;
                        break;
                    }
                }

                if (!found)
                    allPoints.Add(new Point(se.XPosition, se.YPosition));
            }
        }


        private static void ScaleNodes(Network network, double minX, double minY, double stepX, double stepY)
        {
            bool found;
            foreach (NodeEntity se in network.Nodes)
            {
                found = false;

                se.XPosition = (int)Math.Ceiling((se.X - minX) / stepX);
                se.YPosition = (int)(Math.Ceiling((se.Y - minY) / stepY));

                foreach (Point p in allPoints)
                {
                    if (p.X == se.XPosition && p.Y == se.YPosition)
                    {
                        busyPoints.Add(new Point(se.XPosition, se.YPosition));
                        found = true;
                        break;
                    }
                }

                if (!found)
                    allPoints.Add(new Point(se.XPosition, se.YPosition));
            }
        }

        public static void DrawEntities(Network network, Grid grid, Canvas canvas)
        {
            DrawSubstations(network, grid);
            DrawSwitches(network, grid);
            DrawNodes(network, grid);
            //DrawLines(network, grid, canvas);

            bool[] array = new bool[groupElement.Count];
            for (int i = 0; i < groupElement.Count; i++)
                array[i] = false;

            if (groupElement.Count != 0)
            {            
                for (int i = 0; i < groupElement.Count - 1; i++)
                {
                    if (array[i] == true)
                        continue;
                    else
                    {
                        array[i] = true;

                        Rectangle shape = new Rectangle()
                        {
                            Height = 6,
                            Width = 6,
                            Stroke = new SolidColorBrush(Colors.Black),
                            Fill = new SolidColorBrush(Colors.Blue),
                        };

                        string toolTip = "";
                        List<Entity> tempList = new List<Entity>();
                        tempList.Add(groupElement[i]);
                        for (int j = i + 1; j < groupElement.Count; j++)
                        {
                            if (groupElement[i].XPosition == groupElement[j].XPosition &&
                                groupElement[i].YPosition == groupElement[j].YPosition)
                            {
                                tempList.Add(groupElement[j]);
                                array[j] = true;
                            }
                        }

                        foreach (Entity item in tempList)
                        {
                            if (item is SubstationEntity)
                                toolTip += "Substation: \n" + "ID:" + ((SubstationEntity)item).Id + "\nName: " + ((SubstationEntity)item).Name + "\n";
                            else if (item is SwitchEntity)
                                toolTip += "Switch: \n" + "ID: " + ((SwitchEntity)item).Id + "\nName: " + ((SwitchEntity)item).Name + "\nStatus: " + ((SwitchEntity)item).Status + "\n";
                            else if (item is NodeEntity)
                                toolTip += "Node: \n" + "ID:" + ((NodeEntity)item).Id + "\nName: " + ((NodeEntity)item).Name + "\n";
                        }

                        shape.ToolTip = toolTip;
                        Grid.SetRow(shape, tempList[0].YPosition);
                        Grid.SetColumn(shape, tempList[0].XPosition);

                        grid.Children.Add(shape);
                    }
                }
            }        
        }

        private static void DrawSubstations(Network network, Grid grid)
        {
            bool found;
            foreach (SubstationEntity s in network.Substations)
            {
                found = false;
                foreach (Point p in busyPoints)
                {
                    if (s.XPosition == p.X && s.YPosition == p.Y)
                    {
                        groupElement.Add(s);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Ellipse shape = new Ellipse()
                    {
                        Height = 6,
                        Width = 6,
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = new SolidColorBrush(Colors.Red),
                    };

                    shape.ToolTip = "Substation: \n" + "ID:" + s.Id + "\nName: " + s.Name;

                    Grid.SetRow(shape, s.YPosition);
                    Grid.SetColumn(shape, s.XPosition);

                    grid.Children.Add(shape);
                }
            }
        }

        private static void DrawSwitches(Network network, Grid grid)
        {
            bool found;
            foreach (SwitchEntity s in network.Switches)
            {
                found = false;
                foreach (Point p in busyPoints)
                {
                    if (s.XPosition == p.X && s.YPosition == p.Y)
                    {
                        groupElement.Add(s);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Ellipse shape = new Ellipse()
                    {
                        Height = 6,
                        Width = 6,
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = new SolidColorBrush(Colors.Green),
                    };

                    shape.ToolTip = "Switch: \n" + "ID:" + s.Id + "\nName: " + s.Name + "\nStatus: " + s.Status;

                    Grid.SetRow(shape, s.YPosition);
                    Grid.SetColumn(shape, s.XPosition);

                    grid.Children.Add(shape);
                }
            }
        }

        private static void DrawNodes(Network network, Grid grid)
        {
            bool found;
            foreach (NodeEntity s in network.Nodes)
            {
                found = false;
                foreach (Point p in busyPoints)
                {
                    if (s.XPosition == p.X && s.YPosition == p.Y)
                    {
                        groupElement.Add(s);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Ellipse shape = new Ellipse()
                    {
                        Height = 6,
                        Width = 6,
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = new SolidColorBrush(Colors.Yellow),
                    };

                    shape.ToolTip = "Node: \n" + "ID:" + s.Id + "\nName: " + s.Name;

                    Grid.SetRow(shape, s.YPosition);
                    Grid.SetColumn(shape, s.XPosition);

                    grid.Children.Add(shape);
                }
            }
        }

        private static void DrawLines(Network network, Grid grid, Canvas canvas)
        {
            List<Tuple<int, int, int, int>> connectedPoints = GetConnectedPoints(network);

            foreach (Tuple<int, int, int, int> item in connectedPoints)
            {
                List<System.Windows.Point> coordinates = ConvertToCoordinates(item, grid);

                Line line = new Line();
                line.X1 = coordinates[0].X;
                line.Y1 = coordinates[0].Y;
                line.X2 = coordinates[1].X;
                line.Y2 = coordinates[1].Y;
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 1;
                canvas.Children.Add(line);
            
            }

        }

        private static List<System.Windows.Point> ConvertToCoordinates(Tuple<int, int, int, int> blocks, Grid grid)
        {
            List<System.Windows.Point> coordinates = new List<System.Windows.Point>();

            double x1 = ((grid.Width / 100) * blocks.Item1) - 6.455;
            double y1 = ((grid.Height / 100) * blocks.Item2) -2.78;
            System.Windows.Point p = new System.Windows.Point(x1, y1);
            coordinates.Add(p);

            double x2 = ((grid.Width / 100) * blocks.Item3) - 6.455;
            double y2 = ((grid.Height / 100) * blocks.Item4) - 2.78;
            p = new System.Windows.Point(x2, y2);
            coordinates.Add(p);

            return coordinates;
        }

        private static List<Tuple<int, int, int, int>> GetConnectedPoints(Network network)
        {
            List<Tuple<int, int, int, int>> connectedPoints = new List<Tuple<int, int, int, int>>();

            foreach (LineEntity ent in network.Lines)
            {
                int x1 = -1, y1 = -1, x2 = -1, y2 = -1;

                for (int i = 0; i < network.Substations.Count; i++)
                {
                    if (network.Substations[i].Id == ent.FirstEnd)
                    {
                        x1 = network.Substations[i].XPosition;
                        y1 = network.Substations[i].YPosition;
                        
                    }
                    if (network.Substations[i].Id == ent.SecondEnd)
                    {
                        x2 = network.Substations[i].XPosition;
                        y2 = network.Substations[i].YPosition;
                        
                    }
                }

                for (int i = 0; i < network.Nodes.Count; i++)
                {
                    if (network.Nodes[i].Id == ent.FirstEnd)
                    {
                        x1 = network.Nodes[i].XPosition;
                        y1 = network.Nodes[i].YPosition;
                        
                    }
                    if (network.Nodes[i].Id == ent.SecondEnd)
                    {
                        x2 = network.Nodes[i].XPosition;
                        y2 = network.Nodes[i].YPosition;
                       
                    }
                }

                for (int i = 0; i < network.Switches.Count; i++)
                {
                    if (network.Switches[i].Id == ent.FirstEnd)
                    {
                        x1 = network.Switches[i].XPosition;
                        y1 = network.Switches[i].YPosition;
                        
                    }
                    if (network.Switches[i].Id == ent.SecondEnd)
                    {
                        x2 = network.Switches[i].XPosition;
                        y2 = network.Switches[i].YPosition;


                    }
                }

                if (x1 != -1 && y1 != -1 && x2 != -1 && y2 != -1)
                    connectedPoints.Add(new Tuple<int, int, int, int>(x1, y1, x2, y2));
            }

            return connectedPoints;
        }

        //From UTM to Latitude and longitude in decimal
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }
    }
}
