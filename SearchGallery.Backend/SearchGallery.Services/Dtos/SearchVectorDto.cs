using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchGallery.Services
{
    public class SearchVectorDto
    {
        public Guid Id { get; set; }
        public float[] Vector { get; set; }
        public string VectorString { get; set; }
        public void ConvertVectorString()
        {
            this.Vector = VectorString.Split(";").Select(x => float.Parse(x)).ToArray();
        }
    }
}
