using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace App2
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Stand> Stands = new List<Stand>();
        //public DateTime From;
        public DateTime To;

        public Event(XmlNode node)
        {
            Id = int.Parse(node.Attributes["Id"].Value);
            Name = node.Attributes["Name"].Value;

            //From = DateTime.Parse(node["From"].InnerText);
            //To = DateTime.Parse(node.Attributes["Name"].Value);
            foreach (XmlNode stand in node["Stands"].ChildNodes)
                Stands.Add(Stand.NewStand(stand));
        }
    }
}
