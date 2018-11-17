using System;

namespace FINdr
{
    //base node class with virtual operate function
    public class FilterNode
    {
        public virtual int Operate()
        {
            return 0;
        }
    }
    
    //Node that contains some sort of variable based on the fish
    public class VariableNode : FilterNode
    {
        //The variable to get from the fish (position, acceleration, etc.)
        public FilterData Value = FilterData.Dir;


        public override int Operate()
        {
            //get the two current fish and frame
            Fish a = PatternLearning.FishA;
            Fish b = PatternLearning.FishB;
            int frame = PatternLearning.CurrentFrame;

            int ToReturn = 0;

            //depending on the variable type, return the different values
            switch (Value)
            {
                case FilterData.Acc:
                    ToReturn =  (int)(a.GetVelocity(frame) - a.GetVelocity(frame - 1)).GetMagnitude();
                    break;
                case FilterData.Dir:
                    ToReturn = (int)a.GetVelocity(frame).GetMagnitude();
                    break;
                case FilterData.Pos:
                    ToReturn = (int)(a.Path[frame] - b.Path[frame]).GetMagnitude();
                    break;
                case FilterData.Vel:
                    ToReturn = (int)a.GetVelocity(frame).GetMagnitude();
                    break;
            }

            return ToReturn;
        }
    }

    //node for a constant
    public class ConstantNode : FilterNode
    {
        public int Var = 0;

        public override int Operate()
        {
            return Var;
        }
    }

    
    //node that takes input parameters and outputs a value
    public class FunctionNode : FilterNode
    {

        //the input parameters
        public FilterNode[] Params;
        //the operation to use
        public ArithmeticType op;

        public override int Operate()
        {

            //work backwords to get the values
            int Value_A = Params[0].Operate();
            int Value_B = Params[1].Operate();

            int ToReturn = 0;

            //depending on the operation, combine the values and return the result
            switch (op)
            {
                case ArithmeticType.add:
                    ToReturn = Value_A + Value_B;
                    break;
                case ArithmeticType.subtract:
                    ToReturn = Value_A - Value_B;
                    break;
                case ArithmeticType.multiply:
                    ToReturn = Value_A * Value_B;
                    break;
                case ArithmeticType.divide:
                    ToReturn = Value_A / Value_B;
                    break;
            }

            return ToReturn;

        }
    }

    
    public enum ArithmeticType
    {
        add,
        subtract,
        multiply,
        divide
    }

        
        
    
}
    