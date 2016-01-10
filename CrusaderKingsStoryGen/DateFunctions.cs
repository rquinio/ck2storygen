using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen
{
    public static class DateFunctions
    {
        public static int MakeDOBAtLeastAdult(int inDOB)
        {
            int age = 769 - inDOB;

            if (age < 16)
                inDOB -= (16 - age);

            return inDOB;
        }

    }
}
