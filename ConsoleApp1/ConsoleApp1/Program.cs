using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FINdr
{
    class Program : System.Windows.Forms.Form
    {

        //VIDEO DATA

        //The dimensions of the initial video
        public Vector Dimensions = new Vector(706, 730);
        //The number of tanks (width x height)
        public Vector TankCount = new Vector(2, 3);
        //Total number of frames in the video
        public int Frames = 301;

        //INTERNAL VARIABLES

        //list of all the fish
        public List<Fish> fish = new List<Fish>();
        //list of all the tanks
        public Tank[] Tanks;
        //AI
        public PatternLearning AI;
        //index of the current tank 
        public int CurrentTankIndex = 0;
        //index of the current fish
        public int CurrentFishIndex = 0;
        //Whether to display the male's info or the female's info
        public bool DisplayMale = true;
        //the current frame to display
        public int CurrentFrame;
        //The type of information to display
        public PlottingType CurrentPlottingType = PlottingType.Vector;
        
        //The Visual scrollbar
        private HScrollBar hScrollBar1;
        //Close button
        private Button OK_Button;

        //DESIGN ELEMENTS --- DATA SELECTION

        //Lists the different Tanks
        private ComboBox comboBox1;

        //Panel containing buttons for data type
        private Panel panel1;
        //Positional Data
        private RadioButton Vector;
        //Velocity data
        private RadioButton Velocity;
        //Magnitude data
        private RadioButton Magnitude;
        //Direction data
        private RadioButton Direction;

        //Contains the male/female radiobuttons
        private Panel panel2;
        //When selected, displays the female data
        private RadioButton Display_Female;
        //When selected, displays the male data
        private RadioButton Display_Male;


        //DESIGN ELEMENTS --- DATA OUTPUT

        //Displays Fish Name
        private TextBox textBox2;
        //Displays Fish Data
        private TextBox textBox1;
        //The visualization window
        private PictureBox DrawingWindow;
        //The graphics drawn on the window
        private Graphics Visuals;
        private TextBox textBox3;

        //size of the ellipse
        private int ellipseSize = 10;


        [STAThread]
        static void Main(string[] args)
        {
            //Adds visuals to the program
            Application.EnableVisualStyles();
            //Runs program
            Application.Run(new Program());

        }

        Program()
        {
            //Sets up the visual components
            InitializeComponent();

            //Indicates where the drawing will take place
            Visuals = DrawingWindow.CreateGraphics();

            ProcessData();

            AI = new PatternLearning();
            //AI.Learn(Tanks);
        }

        //function called to read the tracked data and format it as objects for simplified processing
        void ProcessData()
        {
            Tanks = new Tank[(int)(TankCount.x * TankCount.y)];

            //random variable for later
            Random r = new Random();

            //Random names are given to the fish to identify them
            //Each fish has its own first name, but the fish that share a tank have the same last name
            string[] MaleFirstNames = { "Charles", "Carl", "Thomas", "Dave","Fishy","Eddie", "Louie"};
            string[] FemaleFirstNames = { "Henrietta", "Camille", "Carla", "Patricia", "Ella", "Thelma", "Bea"};
            string[] LastNames = { "Smith", "Da Vinci", "Ramirez", "von Gelsing", "McFishFace", "Pantel", "Olding","Carlesburg"};
            

            //Read the tracking file
            //The format for the tracking data is as follows:
            // /FISH_1_X1/FISH_1_Y1/BEHAVIOUR_1(if applicable)/FISH_2_X1/FISH_2_Y2/---/...
            // /FISH_1_X2/FISH_1_Y2/BEHAVIOUR_2(if applicable)/FISH_2_X2/FISH_2_Y2/---/...
            //...
            string[] Lines = System.IO.File.ReadAllLines("Results.csv");

            //Loop through each line one at a time
            for (int i = 2; i < Lines.Length; i++)
            {
                //split each line into its cells (in a csv the cells are separated by commas
                string[] Cells = Lines[i].Split(',');

                //Go through each cell
                for (int j = 1; j < Cells.Length; j += 3)
                {
                    //If the cell is empty, skip it
                    if (Cells[j] == " ")
                    {
                        continue;
                    }

                    //Get the index of the current fish (since there are 3 cells per fish, it equals a third of the current cell index)
                    int CurrentFish = (int)Math.Ceiling((double)j / 3);
                    //If the current fish has no data yet, create a fish
                    if (CurrentFish > fish.Count)
                    {
                        //Creates a new fish, by default with a male first name. If the fish ends up being female, the name will be changed
                        //Also sets the path to a new array of vectors equal to the number of frames of the video
                        Fish f = new Fish(MaleFirstNames[r.Next(0, MaleFirstNames.Length)])
                        {
                            Path = new Vector[Frames]
                        };
                        //Add this fish to the list of fish
                        fish.Add(f);

                        //Gets the tank index based on the current position
                        int TankIndex = (int)(Math.Floor((float.Parse(Cells[j]) * TankCount.x / Dimensions.x)) * (TankCount.y) + Math.Floor((float.Parse(Cells[j + 1]) * TankCount.y / Dimensions.y)));
                       
                        //If the tank doesnt exist yet, add it
                        if (Tanks[TankIndex] == null)
                        {
                            Tanks[TankIndex] = new Tank(LastNames[TankIndex],Frames);
                        }
                        //add the fish to the tank
                        Tanks[TankIndex].FishInTank.Add(f);

                        //Set the behaviour type of the fish, if it exists
                        int Type = -1;
                        int.TryParse(Cells[j + 2], out Type);
                        Tanks[TankIndex].Behaviours[CurrentFrame] = (Behaviour)Type;
                        Console.WriteLine((Behaviour)Type);
                    }

                    //set the location of the fish
                    fish[CurrentFish - 1].Path[i - 2] = new Vector(float.Parse(Cells[j]), float.Parse(Cells[j + 1]));

                        
                }
            }
            



            //Go through each tank
            foreach (Tank t in Tanks)
            {

                if (t.FishInTank.Count >= 2 && t.FishInTank[0].Path[0] != null && t.FishInTank[1].Path[0] != null)
                {

                    //the male fish is always higher, so set them accordingly
                    if (t.FishInTank[0].Path[0].y < t.FishInTank[1].Path[0].y)
                    {
                        t.Male = t.FishInTank[0];

                        t.Female = t.FishInTank[1];
                    }
                    else
                    {


                        t.Male = t.FishInTank[1];

                        t.Female = t.FishInTank[0];
                    }
                } else
                {
                    t.Male = t.FishInTank[0];
                    t.Female = t.FishInTank[0];
                }
                //Sets the first name of the fish
                t.Female.FirstName = FemaleFirstNames[r.Next(0, FemaleFirstNames.Length)];

                //Removes teh fish in the current tank
                t.FishInTank.Remove(t.Male);
                
               t.FishInTank.Remove(t.Female);
                
                //make sure the fish have a position the first frame
                if(t.Male.Path[0] == null)
                {
                    t.Male.Path[0] = t.Female.Path[0];
                } else if(t.Female.Path[0] == null)
                {
                    t.Female.Path[0] = t.Male.Path[0];
                }

                //loop through all the frames
                for(int i = 1; i < Frames;i++)
                {
                    //if both fish dont have a current path
                    if(t.Male.Path[i] == null && t.Female.Path[i] == null)
                    {
                        //give them empty vectors
                        t.Male.Path[i] = new Vector(0, 0);
                        t.Female.Path[i] = new Vector(0, 0);

                        //get all fish currently existing
                        List<Fish> ActiveFish = new List<Fish>();
                        foreach (Fish f in t.FishInTank)
                        { 
                            if (f.Path[i] != null)
                            {
                                ActiveFish.Add(f);
                               
                            }
                        }

                        //if there is two fish active...
                        if(ActiveFish.Count == 2)
                        {
                            //merge the active fish with the initial male/female fish based on closeness to each other
                            if((t.Male.Path[i - 1] - ActiveFish[0].Path[i]).GetMagnitude() > (t.Female.Path[i - 1] - ActiveFish[0].Path[i]).GetMagnitude())
                            {
                                for (int j = i; j < Frames && ActiveFish[0].Path[j] != null; j++)
                                {
                                    t.Male.Path[j] = ActiveFish[0].Path[j];
                                }

                                for (int j = i; j < Frames && ActiveFish[1].Path[j] != null; j++)
                                {
                                    t.Female.Path[j] = ActiveFish[1].Path[j];
                                }
                            } else
                            {
                                for (int j = i; j < Frames && ActiveFish[1].Path[j] != null; j++)
                                {
                                    t.Male.Path[j] = ActiveFish[1].Path[j];
                                }

                                for (int j = i; j < Frames && ActiveFish[0].Path[j] != null; j++)
                                {
                                    t.Female.Path[j] = ActiveFish[0].Path[j];
                                }
                            }

                            //if there is only one fish then they are probably at the same position, so give them both the same position
                        } else if(ActiveFish.Count == 1)
                        {
                            for (int j = i; j < Frames && ActiveFish[0].Path[j] != null; j++)
                            {
                                t.Male.Path[j] = ActiveFish[0].Path[j];
                                t.Female.Path[j] = ActiveFish[0].Path[j];
                            }
                        }
                    //if only one fish disappears...
                    } else if(t.Male.Path[i] == null)
                    {
                        //by default give it the position of the other fish temporarily
                        t.Male.Path[i] = t.Female.Path[i];

                        //if ther are other active fish that were not active beforehand, then reassign that fish to the male fish
                        foreach(Fish f in t.FishInTank)
                        {
                            if(f.Path[i - 1] == null && f.Path[i] != null)
                            {
                                for(int j = i; j <f.Path.Length && f.Path[j] != null; j++)
                                {
                                    t.Male.Path[j] = f.Path[j];
                                }
                            }
                        }
                        //the same process, but for the female
                    } else if(t.Female.Path[i] == null)
                    {
                        //again, by default give the female the male's position
                        t.Female.Path[i] = t.Male.Path[i];
                        //if any fish become active, then shift their data into this data
                        foreach (Fish f in t.FishInTank)
                        {
                            if (f.Path[i - 1] == null && f.Path[i] != null)
                            {
                                for (int j = i; j < f.Path.Length && f.Path[j] != null; j++)
                                {
                                    t.Female.Path[j] = f.Path[j];
                                }
                            }                                
                            
                        }
                    }
                }

                //add this tank to the dropdown box
                this.comboBox1.Items.Add(t);
            }
            //update everything
            comboBox1.SelectedIndex = 0;
            UpdateTextBox();
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {



            //erase visuals
            Visuals.Clear(Color.White);
            //ratio of drawing board width to video
            Vector PixelRatio = new Vector(DrawingWindow.Width  *TankCount.x /Dimensions.x, DrawingWindow.Height * TankCount.y / Dimensions.y );

            //the male fish to draw
            Fish male = Tanks[CurrentTankIndex].Male;
            //the female fish to draw
            Fish female = Tanks[CurrentTankIndex].Female;

            //draw the male with a circle for the head and a blue line denoting the direction of the body
            Pen BluePen = new Pen(Brushes.Blue);
            Point Male_P = new Point((int)((male.Path[CurrentFrame].x % (Dimensions.x / TankCount.x)) * PixelRatio.x), (int)((male.Path[CurrentFrame].y % (Dimensions.y / TankCount.y)) * PixelRatio.y));
            Visuals.DrawEllipse(BluePen, Male_P.X - ellipseSize / 2,  Male_P.Y - ellipseSize / 2, ellipseSize, ellipseSize);
            Visuals.DrawLine(BluePen, Male_P.X,Male_P.Y, Male_P.X  + (float)Math.Sin(male.GetVelocity(CurrentFrame).GetMagnitude() * Math.PI/ 180) * 30,Male_P.Y +  (float)Math.Cos(male.GetVelocity(CurrentFrame).GetAngle() * Math.PI / 180) * 30);

            //draw the female with a circle for the head and a pink line denoting the direction of the body
            Point Female_P = new Point((int)((female.Path[CurrentFrame].x % (Dimensions.x / TankCount.x)) * PixelRatio.x), (int)((female.Path[CurrentFrame].y % (Dimensions.y / TankCount.y)) * PixelRatio.y));
            Pen PinkPen = new Pen(Brushes.HotPink);
            Visuals.DrawEllipse(PinkPen, Female_P.X - ellipseSize / 2, Female_P.Y - ellipseSize / 2,  ellipseSize, ellipseSize);
            Visuals.DrawLine(PinkPen, Female_P.X,Female_P.Y, Female_P.X  + (float)Math.Sin(female.GetVelocity(CurrentFrame).GetAngle() * Math.PI / 180) * 30,Female_P.Y +  (float)Math.Cos(female.GetVelocity(CurrentFrame).GetAngle() * Math.PI / 180) * 30);

        }

        //gets the difference of angles between two vector directions
        public float AngleDifference(Vector a, Vector b)
        {
            return Math.Abs(Math.Abs(a.GetAngle()) - b.GetAngle());

        }

        //updates the textboxes
        public void UpdateTextBox()
        {
            Tank NewTank = Tanks[CurrentTankIndex];
            
            Fish fish;
            if (DisplayMale)
            {
                fish = NewTank.Male;
            }
            else
            {
                fish = NewTank.Female;
            }
            String output = "";

            for(int i = 0; i < fish.Path.Length; i++)
            {

                if(fish.Path[i]== null)
                {
                    output += "X:-,Y:-\r\n";
                    continue;
                }
                switch (CurrentPlottingType)
                {
                    case PlottingType.Vector: output += "X:" + fish.Path[i].x + ",Y:" + fish.Path[i].y + "\r\n";
                                                break;
                    case PlottingType.Velocity:
                        output += "X:" + fish.GetVelocity(i).x + ",Y:" + fish.GetVelocity(i).y + "\r\n";
                            break;
                    case PlottingType.Direction:
                       
                        output += fish.GetVelocity(i).GetAngle() + "\r\n";
                        break;
                    case PlottingType.Magnitude:                   
                        output += fish.GetVelocity(i).GetMagnitude() + "\r\n";

                        break;
                            }
            }

            this.textBox1.Text = output;
            
            this.textBox2.Text = NewTank.Behaviours[CurrentFrame].ToString();
            this.textBox3.Text = CurrentFrame.ToString();
        }

        private void InitializeComponent()
        {
            this.OK_Button = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Magnitude = new System.Windows.Forms.RadioButton();
            this.Direction = new System.Windows.Forms.RadioButton();
            this.Velocity = new System.Windows.Forms.RadioButton();
            this.Vector = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Display_Female = new System.Windows.Forms.RadioButton();
            this.Display_Male = new System.Windows.Forms.RadioButton();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.DrawingWindow = new System.Windows.Forms.PictureBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DrawingWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // OK_Button
            // 
            this.OK_Button.AccessibleName = "";
            this.OK_Button.Location = new System.Drawing.Point(24, 323);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(366, 34);
            this.OK_Button.TabIndex = 0;
            this.OK_Button.Text = "OK";
            this.OK_Button.UseVisualStyleBackColor = true;
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.AllowDrop = true;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(24, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(178, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(225, 20);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(165, 224);
            this.textBox1.TabIndex = 7;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(225, 266);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(165, 25);
            this.textBox2.TabIndex = 8;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Magnitude);
            this.panel1.Controls.Add(this.Direction);
            this.panel1.Controls.Add(this.Velocity);
            this.panel1.Controls.Add(this.Vector);
            this.panel1.Location = new System.Drawing.Point(24, 121);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(113, 123);
            this.panel1.TabIndex = 9;
            // 
            // Magnitude
            // 
            this.Magnitude.Appearance = System.Windows.Forms.Appearance.Button;
            this.Magnitude.AutoSize = true;
            this.Magnitude.Location = new System.Drawing.Point(13, 90);
            this.Magnitude.Name = "Magnitude";
            this.Magnitude.Size = new System.Drawing.Size(67, 23);
            this.Magnitude.TabIndex = 3;
            this.Magnitude.Text = "Magnitude";
            this.Magnitude.UseVisualStyleBackColor = true;
            this.Magnitude.CheckedChanged += new System.EventHandler(this.Magnitude_CheckedChanged);
            // 
            // Direction
            // 
            this.Direction.Appearance = System.Windows.Forms.Appearance.Button;
            this.Direction.AutoSize = true;
            this.Direction.Location = new System.Drawing.Point(13, 61);
            this.Direction.Name = "Direction";
            this.Direction.Size = new System.Drawing.Size(59, 23);
            this.Direction.TabIndex = 2;
            this.Direction.Text = "Direction";
            this.Direction.UseVisualStyleBackColor = true;
            this.Direction.CheckedChanged += new System.EventHandler(this.Direction_CheckedChanged);
            // 
            // Velocity
            // 
            this.Velocity.Appearance = System.Windows.Forms.Appearance.Button;
            this.Velocity.AutoSize = true;
            this.Velocity.Location = new System.Drawing.Point(13, 32);
            this.Velocity.Name = "Velocity";
            this.Velocity.Size = new System.Drawing.Size(54, 23);
            this.Velocity.TabIndex = 1;
            this.Velocity.Text = "Velocity";
            this.Velocity.UseVisualStyleBackColor = true;
            this.Velocity.CheckedChanged += new System.EventHandler(this.Velocity_CheckedChanged);
            // 
            // Vector
            // 
            this.Vector.Appearance = System.Windows.Forms.Appearance.Button;
            this.Vector.AutoSize = true;
            this.Vector.Checked = true;
            this.Vector.Location = new System.Drawing.Point(13, 3);
            this.Vector.Name = "Vector";
            this.Vector.Size = new System.Drawing.Size(48, 23);
            this.Vector.TabIndex = 0;
            this.Vector.TabStop = true;
            this.Vector.Text = "Vector";
            this.Vector.UseVisualStyleBackColor = true;
            this.Vector.CheckedChanged += new System.EventHandler(this.Vector_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Display_Female);
            this.panel2.Controls.Add(this.Display_Male);
            this.panel2.Location = new System.Drawing.Point(24, 64);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(164, 34);
            this.panel2.TabIndex = 11;
            // 
            // Display_Female
            // 
            this.Display_Female.AutoSize = true;
            this.Display_Female.Location = new System.Drawing.Point(72, 11);
            this.Display_Female.Name = "Display_Female";
            this.Display_Female.Size = new System.Drawing.Size(75, 17);
            this.Display_Female.TabIndex = 1;
            this.Display_Female.Text = "female fish";
            this.Display_Female.UseVisualStyleBackColor = true;
            this.Display_Female.CheckedChanged += new System.EventHandler(this.Display_Female_CheckedChanged);
            // 
            // Display_Male
            // 
            this.Display_Male.AutoSize = true;
            this.Display_Male.Checked = true;
            this.Display_Male.Location = new System.Drawing.Point(0, 12);
            this.Display_Male.Name = "Display_Male";
            this.Display_Male.Size = new System.Drawing.Size(66, 17);
            this.Display_Male.TabIndex = 0;
            this.Display_Male.TabStop = true;
            this.Display_Male.Text = "male fish";
            this.Display_Male.UseVisualStyleBackColor = true;
            this.Display_Male.CheckedChanged += new System.EventHandler(this.Display_Male_CheckedChanged);
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(423, 334);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(397, 23);
            this.hScrollBar1.TabIndex = 13;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // DrawingWindow
            // 
            this.DrawingWindow.BackColor = System.Drawing.SystemColors.Window;
            this.DrawingWindow.Location = new System.Drawing.Point(435, 13);
            this.DrawingWindow.Name = "DrawingWindow";
            this.DrawingWindow.Size = new System.Drawing.Size(385, 298);
            this.DrawingWindow.TabIndex = 14;
            this.DrawingWindow.TabStop = false;
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(43, 258);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(61, 33);
            this.textBox3.TabIndex = 15;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // Program
            // 
            this.ClientSize = new System.Drawing.Size(827, 385);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.DrawingWindow);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.OK_Button);
            this.Name = "Program";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DrawingWindow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            CurrentTankIndex = comboBox1.SelectedIndex;
            UpdateTextBox();
            UpdateVisuals();

        }

        private void Vector_CheckedChanged(object sender, EventArgs e)
        {

            CurrentPlottingType = PlottingType.Vector;
            UpdateTextBox();

        }

        private void Velocity_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPlottingType = PlottingType.Velocity;
            UpdateTextBox();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Direction_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPlottingType = PlottingType.Direction;
            UpdateTextBox();
        }

        private void Magnitude_CheckedChanged(object sender, EventArgs e)
        {
            CurrentPlottingType = PlottingType.Magnitude;
            UpdateTextBox();
        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            ProcessData();
        }

        private void Display_Male_CheckedChanged(object sender, EventArgs e)
        {
            DisplayMale = true;
            UpdateTextBox();
        }

        private void Display_Female_CheckedChanged(object sender, EventArgs e)
        {
            DisplayMale = false;
            UpdateTextBox();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            CurrentFrame = e.NewValue;
            UpdateVisuals();
            UpdateTextBox();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public enum PlottingType
    {
        Vector,
        Velocity,
        Direction,
        Magnitude
    }
}
