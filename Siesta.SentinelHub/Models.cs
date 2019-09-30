using System;
using System.Collections.Generic;

namespace Siesta.SentinelHub
{
    public class Output
    {
        public List<Data> C0 {get; set;}
    }

    public class Data
    {
        public DateTime Date { get; set; }

        public BasicStats BasicStats { get; set; }
    }

    public class BasicStats 
    {
        public decimal Min { get; set; }

        public decimal Max { get; set; }

        public decimal Mean { get; set; }

        public decimal StDev { get; set; }
    }

    public class SentinelLocation
    {
        public Info Info { get; set; }

        public List<Position> Positions { get; set;}
    }

    public class Info
    {
        public string Satname { get; set;}

        public string Satid { get; set; }

        public int Transactionscount { get; set; }
    }

    public class Position
    {
        public decimal Satlatitude { get; set; }

        public decimal Satlongitude { get; set; }
    }
}