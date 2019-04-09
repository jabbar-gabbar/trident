using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trident
{
    public class Upload
    {
        private List<string> finalList;
        public Upload(List<string> finalList)
        {
            this.finalList = finalList;
        }

        public void start()
        {
            // iterate loop, call uploadCore to upload one file at a time, commit inventory/log progress at interval of 10 items
            foreach (var filePath in finalList)
            {

            }
        }
    }
}
