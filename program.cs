using System;
using System.Collections.Generic;

namespace Ameisen_Simulation_Fabian_Müller_HHEK_IA121_2022
{
    class Program
    {

        static void Main(string[] args)
        {
            int amountOfAnts = 0;
            int anthillX = 20;
            int anthillY = 20;
            int days = 30;
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;                   //
            System.Console.WindowHeight = 30;                                            // Windows Size and Output Encoding
            System.Console.WindowWidth = 50;                                             //


            Boolean goOn = false; // Go-on, if you wonder
            Boolean decisionDone = false;
            int verticalSelectionIndex = 0;
            
            // Selection and Simulation-options-menu
            while (!decisionDone)
            {
                System.Console.Clear();
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine("Welcome to the Ant Simulator.");
                System.Console.WriteLine("Please use the Arrow-Keys to navigate");
                System.Console.WriteLine("Amount of Ants: " + (verticalSelectionIndex == 0 ? ">" : "") + amountOfAnts);
                System.Console.WriteLine("Anthill X-Pos:  " + (verticalSelectionIndex == 1 ? ">" : "") + anthillX);
                System.Console.WriteLine("Anthill Y-Pos:  " + (verticalSelectionIndex == 2 ? ">" : "") + anthillY);
                System.Console.WriteLine("Days to circle: " + (verticalSelectionIndex == 3 ? ">" : "") + days);



                ConsoleKeyInfo UserInput = System.Console.ReadKey();
                switch (UserInput.Key)
                {
                    case ConsoleKey.RightArrow:
                        if (verticalSelectionIndex == 0){
                            amountOfAnts++;
                        } else if (verticalSelectionIndex == 1)
                        {
                            anthillX++;
                        }
                        else if (verticalSelectionIndex == 2)
                        {
                            anthillY++;
                        }
                        else if (verticalSelectionIndex == 3)
                        {
                            days += 10;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (verticalSelectionIndex == 0)
                        {
                            amountOfAnts = amountOfAnts > 0 ? amountOfAnts-1 : amountOfAnts;
                        }
                        else if (verticalSelectionIndex == 1)
                        {
                            anthillX = anthillX > 0 ? anthillX - 1 : anthillX;
                        }
                        else if (verticalSelectionIndex == 2)
                        {
                            anthillY = anthillY > 0 ? anthillY - 1 : anthillY;
                        }
                        else if (verticalSelectionIndex == 3)
                        {
                            days = days > 0 ? days - 10 : days;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        verticalSelectionIndex--;
                        if (verticalSelectionIndex<0)
                        {
                            verticalSelectionIndex = 3;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        verticalSelectionIndex++;
                        if (verticalSelectionIndex > 3)
                        {
                            verticalSelectionIndex = 0;
                        }
                        break;
                    case ConsoleKey.Enter:
                        goOn = true;
                        decisionDone = true;
                        break;
                    case ConsoleKey.Escape:
                        goOn = false;
                        decisionDone = true;
                        break;
                }
            }



            // Instance of Simulation-Class
            Simulation sim = new Simulation(amountOfAnts, anthillX, anthillY);
            for(int dayCounter = 0; dayCounter < days; dayCounter++) { 
                sim.DoADay();
                System.Threading.Thread.Sleep(25);
            }
            System.Console.WriteLine("Simulation Complete!");
            System.Console.WriteLine("Press any key to close...");
            System.Console.ReadKey();

        }


    }


    class Simulation
    {
        private Random random = new Random();
        private Anthill anthill = new Anthill();

        public Simulation(int amountOfAnts, int anthillXPosition, int anthillYPosition)
        {
            anthill.x = anthillXPosition;         // Anthill positioning
            anthill.y = anthillYPosition;

            anthill.theAntHighness = new Queen(); // Create Queen
            anthill.theAntHighness.x = anthill.x;
            anthill.theAntHighness.y = anthill.y;

            for (int i = 0; i < amountOfAnts; i++) // Initial iteration to create ants
            {
                Ant temporaryAnt = new Ant();
                temporaryAnt.food = random.Next(10, 60); // Generate a random food level for each ant.
                temporaryAnt.AnthillX = anthill.x;       //
                temporaryAnt.AnthillY = anthill.y;       // Ant-positioning
                temporaryAnt.x = anthill.x;              //
                temporaryAnt.y = anthill.y;              //
                anthill.AddAnt(temporaryAnt);
            }


        }


        // Do a day-circle .
        public void DoADay()
        {
            System.Console.Clear();

            anthill.theAntHighness.Breed(); // Make the Queen Breed

            for (int iterator = 0; iterator < anthill.Population; iterator++)
            {
                anthill.GetAntByNumber(iterator).Move();
                anthill.GetAntByNumber(iterator).Draw();
            }

            // If the Queen is pregnant, give birth to a baby
            if (anthill.theAntHighness.isPregnant)
            {
                AddAntToAnthill();
            }
            // Draw The Queen and Anthill
            anthill.theAntHighness.Draw();
            anthill.Draw();
        }

        // Add a Ant to the Ant-Family.
        public void AddAntToAnthill()
        {
            Ant temporaryAnt = new Ant();
            temporaryAnt.food = random.Next(500, 1000);
            this.anthill.AddAnt(temporaryAnt);
        }

    }

    class Entity
    {
        // Initilize Property x (coordinate) with default values
        [System.ComponentModel.DefaultValue(20)]
        public int x { get; set; } = 20;

        [System.ComponentModel.DefaultValue(20)]
        public int y { get; set; } = 20;
        public int food { get; set; } = 50;

        // Just for definition
        public virtual void Move()
        {
            //DO NOTHING
        }

        // Draw the Entity
        public virtual void Draw()
        {
            System.Console.SetCursorPosition(this.x, this.y);
            System.Console.Write("E");
        }

    }



    class Ant : Entity
    {
        // Variables and constants
        private const int SPEED = 1;
        private System.Random randomNumber = new System.Random();
        protected Boolean isHeadingToAnthill = false;

        // Properties
        public int life = 30; // Eine Ameise lebt einen Monat!
        public int AnthillX { get; set; }
        public int AnthillY { get; set; }

        public override void Move()
        {
            life--;
            if (this.food < 3)
            {
                // If the Ant is near the Anthill by 5 Thingys(Pixels, Symbols, whatever), then set it to true and refill food.
                if (Math.Sqrt(Math.Pow((this.AnthillX - this.x), 2) + Math.Pow((this.AnthillY - this.y), 2)) < SPEED + 2)
                {
                    this.isHeadingToAnthill = false;
                    this.food = 50;
                    this.life += 10;
                } else
                {
                    this.isHeadingToAnthill = true;
                    MoveTowardsAntHill();
                }
            } else
            {
                this.isHeadingToAnthill = false;

                // Move to the next random position without leaving the world. (Uses Ternery Operation due to efficency ♥ )
                this.x += this.randomNumber.Next(
                    (this.x - SPEED >= 0 ? -SPEED : SPEED),
                    (this.x + SPEED < System.Console.WindowWidth ? SPEED + SPEED : -SPEED)
                    );

                this.y += this.randomNumber.Next(
                    (this.y - SPEED >= 0 ? -SPEED : SPEED),
                    (this.y + SPEED < System.Console.WindowHeight ? SPEED + SPEED : -SPEED)
                    );

            }
            this.food -= 1;
        }

        // Move starving Ant towards the Anthill. Very wow, very fast!
        public void MoveTowardsAntHill()
        {

            this.x += this.x > AnthillX ? -SPEED : SPEED;
            this.y += this.y > AnthillY ? -SPEED : SPEED;

        }

        // Make the tiny Ant visible.
        public override void Draw()
        {
            if (this.life > 0) { 
                if (this.isHeadingToAnthill)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                } else
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                System.Console.SetCursorPosition(this.x, this.y);
                System.Console.Write("¥");
                }
        }

    }


    // May i introduce: The Queen-Lady - Mama Ant. (Very Handsome)
    class Queen : Ant
    {
        private int pregnancyLevel = 0; // Level of pregnancy. Determines when a baby will be born.

        // A Boolean value which state if big moma is pregnant. If so, a baby will be born and then automatically she in't pregnant anymore.
        public Boolean isPregnant
        {
            get { return pregnancyLevel > 3; }
        }

        // Breed a baby out of nowhere. Who may be the lover?
        public void Breed()
        {
            if (isPregnant)
            {
                pregnancyLevel = 0;
            }
            pregnancyLevel++;
        }

        // Make the old lady visible!
        public override void Draw()
        {
            System.Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.SetCursorPosition(this.x, this.y);
            System.Console.Write("♥♀");
        }
    }


    // Anthill entity to contain Ants and your Highness herself.
    class Anthill : Entity
    {
        public List<Ant> family = new List<Ant>(); // Ant Family/ Ant-Colony
        public Queen theAntHighness { get; set; } // Your Royal Highness, Queen Ant.

        // Population-counter to determine the population of the Anthill(?)
        public int Population
        {
            get { return family.Count; }
        }
        // Returns the required Ant. If not, deliver the Queen. By a total fail, return an empty/fresh ant.
        public Ant GetAntByNumber(int number)
        {
            return family[number]; 
        }

        // Adds an Ant to the Anthill
        public void AddAnt(Ant babyAnt)
        {
            this.family.Add(babyAnt);
        }

        // Draw a nice castle (Anthill) for your Highness, the Queen
        public override void Draw()
        {
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.SetCursorPosition(this.x < 1 ? 0 : this.x - 1, this.y + 1);
            System.Console.Write("▲▬▬▲");
            System.Console.SetCursorPosition(this.x < 1 ? 0 : this.x - 1, this.y + 2);
            System.Console.Write("█▄▄█");
        }
    }
}
