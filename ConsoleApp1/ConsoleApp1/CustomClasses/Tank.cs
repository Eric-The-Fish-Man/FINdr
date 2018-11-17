using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FINdr
{
    //class for each tank. Each tank has two fish; a male and a female.
    public class Tank
    {
        // a list of all the fish in the tank. The reason this is not limited to two is that the sorting algorithm can sometimes
        //get confused and think there are more than two fish. This does get sorted out, and the combined fish are then
        //put into the male and female variables
        public List<Fish> FishInTank = new List<Fish>();
        public Fish Male;
        public Fish Female;
        //Last name to categorize the fish
        public string LastName;

        //A list of the behaviours exhibited in this tank (index is the frame count)
        public Behaviour[] Behaviours;


        public Tank(string name, int life)
        {
            LastName = name;
            Behaviours = new Behaviour[life];
        }

        public override string ToString()
        {
            return LastName;
        }
    }
}
