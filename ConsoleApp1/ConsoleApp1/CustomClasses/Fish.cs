using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FINdr
{
    //class that holds all the fish data
    public class Fish
    {
        //positional data with the index being the frame count
        public Vector[] Path;
        //public Behaviour[] Behaviours;

        //name of the fish for ease of categorizing
        public string FirstName;
        

        public Vector GetVelocity(int Frame)
        {
            if (Frame == 0)
                return new Vector(0, 0);

            return Path[Frame] - Path[Frame - 1];
        }

       public Fish(string name)
        {
            FirstName = name;
        }
    }

    public enum Behaviour
    {
        None = -1,
        Tail_Nose,
        Chase,
        Quiver
    }
}


