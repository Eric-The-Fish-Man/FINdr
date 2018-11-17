using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FINdr
{
    //the class that holds all the processes for determining patterns
     public class PatternLearning
    {
        //Each behaviour has its own filter, which uses its own neural network to determine the pattern.
        List<BehaviourFilter> Filters = new List<BehaviourFilter>();


        //for the curernt tank under observation, the current fish and frame
        public static Fish FishA;
        public static Fish FishB;
        public static int CurrentFrame;

        //the process run to teach the neural network
        public void Learn(Tank[] tanks)
        {
            //the three base filters we are looking for
            Filters.Add(new BehaviourFilter("Chase"));
            Filters.Add(new BehaviourFilter("TailNose"));
            Filters.Add(new BehaviourFilter("Quiver"));

            // loop through each of the tanks
            foreach(Tank t in tanks)
            {
                //loop through each filter on each tank
                foreach (BehaviourFilter filter in Filters)
                {
                    //loop through all the frames of this tank
                    for (int i = 0; i < 1; i++)
                    {
                        //set the fish and frames data
                        FishA = t.FishInTank[0];
                        FishB = t.FishInTank[1];
                        CurrentFrame = i;

                        //keep trying to form a pattern based off of the data given
                        while((filter.Name == t.Behaviours[i].ToString()) != filter.Compare())
                        {

                        }

                    }
                }
                break;
            }

        }

        
    }

    //the filter for the data
    class BehaviourFilter
    {
        public string Name;
        //the neural network 
        List<FilterNode> Nodes = new List<FilterNode>();

        FilterOperation CompareType = FilterOperation.ET;

        public BehaviourFilter(string name)
        {
            Name = name;
        }

        //the function called to calculate the final values
        public bool Compare()
        {
            int Value_A = Nodes[Nodes.Count - 1].Operate();
            int Value_B = Nodes[Nodes.Count - 2].Operate();

            bool ToReturn = false;

            switch (CompareType)
            {
                case FilterOperation.ET:
                    ToReturn = Value_A == Value_B;
                    break;
                case FilterOperation.GT:
                    ToReturn = Value_A > Value_B;
                    break;
                case FilterOperation.LT:
                    ToReturn = Value_A < Value_B;
                    break;
            }

            return ToReturn;

        }

    }



    public enum FilterData
    {
        Pos,
        Vel,
        Acc,
        Dir
    }


    public enum FilterOperation
    {
        GT,
        LT,
        ET
    }
}
