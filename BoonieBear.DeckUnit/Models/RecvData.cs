using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using De.TorstenMandelkow.MetroChart;

namespace BoonieBear.DeckUnit.Models
{
    class RecvData
    {
        public string node { get; set; }
        public UInt16 ID { get; set; }
        public DateTime RecvTime { get; set; }

        private string filepath;

        public byte[] GetFile()
        {
            if (File.Exists(filepath))
            {

                return File.ReadAllBytes(filepath);

            }
            return null;
        }
    }
}
