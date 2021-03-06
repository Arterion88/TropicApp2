﻿using System;
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

        public Event()
        {
            
        }

        public Event(XmlNode node)
        {
            Id = int.Parse(node.Attributes["Id"].Value);
            Name = node.Attributes["Name"].Value;
            To = DateTime.Parse(node.Attributes["To"].Value);
            foreach (XmlNode stand in node.ChildNodes)
                Stands.Add(new Stand(stand));
        }
    }
}
