using System;
using System.Collections.Generic;
using System.IO;

namespace KnapsackProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            Element[] Elements;
            Elements = Element.GenerateArray();
            Console.WriteLine(Element.ToString(Elements));
            Population b = new Population(Elements);
            b.Start();
            Console.WriteLine("");
            Console.WriteLine("Solution: ");
            Console.WriteLine(b.ToString());
            Console.WriteLine("Save result to file? \nClick 1 for saving \nClick 2 for ending program ");
            int a = Convert.ToInt32(Console.ReadLine());
            switch (a)
            {
                case 1:
                    Console.WriteLine("Enter file name:");
                    string fileName = Console.ReadLine();
                    b.SaveToFile(fileName);
                    Console.WriteLine($"Result saved to {fileName}");
                    Console.WriteLine($"Set of Elements saved to {"Elements" + fileName}");
                    break;
                case 2:
                    Console.WriteLine("Ending program");
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    return;
            }
        }
    }

    class Element
    {
        public static int elements { get; private set; } = 100;
        public int weight { get; private set; }
        public int cost { get; private set; }
        private Random random = new Random();

        public Element()
        {
            weight = random.Next(1, 6);
            cost = random.Next(2, 11);
        }

        public static Element[] GenerateArray()
        {
            Element[] Elements = new Element[elements];
            for (int i = 0; i < elements; i++)
            {
                Elements[i] = new Element();
            }
            return Elements;
        }

        public static string ToString(Element[] Elements)
        {
            string a = "";
            for (int i = 0; i < elements; i++)
            {
                a += $"{Elements[i].weight} \t {Elements[i].cost} \n";
            }
            return a;
        }
    }

    class Population
    {
        private readonly int populate = 100;
        private readonly int genes = 50;
        private readonly int capacity = 150;
        private readonly int mutationpercents = 10;
        private readonly int iterationsnumber = 1000;

        private Element[] Elements;
        private bool[,] population;
        public Population(Element[] Elements)
        {
            this.Elements = Elements;
            population = new bool[populate, Element.elements];
        }

        public void Start()
        {
            Console.WriteLine($"Itetation \t Weight \t Cost");
            InitPopulation();
            for (int i = 0; i < iterationsnumber; i++)
            {
                int[] changed = Crossing();
                if (changed[0] != -1)
                {
                    Mutatuion(changed[0]);
                    LocalImprovement(changed[0]);
                }
                if (changed[1] != -1)
                {
                    Mutatuion(changed[1]);
                    LocalImprovement(changed[1]);
                }
                if ((i + 1) % 20 == 0)
                {
                    Console.WriteLine($"{i + 1,7 } {WeightOfSetInMartix(SetWithMaxCost()),13} \t {CostOfSetInMartix(SetWithMaxCost()),11}");
                }
            }

        }

        private void InitPopulation()
        {
            for (int i = 0; i < populate; i++)
            {
                for (int j = 0; j < Element.elements; j++)
                {
                    if (i == j)
                        population[i, j] = true;
                    else
                        population[i, j] = false;
                }
            }
        }

        private int[] Crossing()
        {
            int[] changed = { -1, -1 };
            int maxCost = SetWithMaxCost();
            int random = RandomPopulation(maxCost);
            bool[] first = MatrixToSet(maxCost);
            bool[] second = MatrixToSet(random);
            bool[] firstChild = GetChild(first, second);
            bool[] secondChild = GetChild(second, first);
            int firstChildWeight = WeightOfSet(firstChild);
            int secondChildWeight = WeightOfSet(secondChild);
            int less = SetWithMinWeight();
            if (CanInsert(firstChildWeight, less))
            {
                SetToMatrix(firstChild, less);
                changed[0] = less;
            }
            less = SetWithMinWeight();
            if (CanInsert(secondChildWeight, less))
            {
                SetToMatrix(secondChild, less);
                changed[1] = less;
            }

            return changed;
        }

        private bool CanInsert(int weight, int less)
        {
            if (weight <= capacity && weight >= WeightOfSetInMartix(less))
            {
                return true;
            }
            return false;
        }

        private bool[] GetChild(bool[] first, bool[] second)
        {
            bool[] child = new bool[Element.elements];
            for (int i = 0; i < populate; i++)
            {
                if (genes <= i)
                    child[i] = first[i];
                else
                    child[i] = second[i];
            }
            return child;
        }

        private void Mutatuion(int changed)
        {
            Random rand = new Random();
            int mutation = rand.Next(101);
            int gen = rand.Next(Element.elements);
            if (mutation <= mutationpercents)
            {
                if (population[changed, gen])
                    population[changed, gen] = false;
                else
                    population[changed, gen] = true;
            }
            if (WeightOfSetInMartix(changed) <= capacity)
            {
                return;
            }
            else
            {
                if (population[changed, gen])
                    population[changed, gen] = false;
                else
                    population[changed, gen] = true;
            }
        }

        private int SetWithMinWeight()
        {
            int minWeight = int.MaxValue;
            int vertex = -1;
            for (int i = 0; i < populate; i++)
            {
                int tmp = WeightOfSetInMartix(i);
                if (minWeight > tmp)
                {
                    minWeight = tmp;
                    vertex = i;
                }
            }

            return vertex;
        }

        private int WeightOfSet(bool[] ElementSet)
        {
            int weightOfSet = 0;
            for (int i = 0; i < Element.elements; i++)
            {
                if (ElementSet[i] == true)
                {
                    weightOfSet += Elements[i].weight;
                }
            }
            return weightOfSet;
        }

        private int WeightOfSetInMartix(int numOfSet)
        {
            int weightOfSet = 0;
            for (int i = 0; i < Element.elements; i++)
            {
                if (population[numOfSet, i] == true)
                {
                    weightOfSet += Elements[i].weight;
                }
            }
            return weightOfSet;
        }

        private int SetWithMaxCost()
        {
            int maxCost = 0;
            int vertex = -1;
            for (int i = 0; i < populate; i++)
            {
                int tmp = CostOfSetInMartix(i);
                if (maxCost < tmp)
                {
                    maxCost = tmp;
                    vertex = i;
                }
            }
            return vertex;
        }

        private int CostOfSet(bool[] ElementSet)
        {
            int costOfSet = 0;
            for (int i = 0; i < Element.elements; i++)
            {
                if (ElementSet[i] == true)
                {
                    costOfSet += Elements[i].cost;
                }
            }
            return costOfSet;
        }

        private int CostOfSetInMartix(int numOfSet)
        {
            int costOfSet = 0;
            for (int i = 0; i < Element.elements; i++)
            {
                if (population[numOfSet, i] == true)
                {
                    costOfSet += Elements[i].cost;
                }
            }
            return costOfSet;
        }

        private bool[] MatrixToSet(int numOfSet)
        {
            bool[] result = new bool[Element.elements];
            for (int i = 0; i < populate; i++)
            {
                result[i] = population[numOfSet, i];
            }
            return result;
        }

        private int RandomPopulation(int numOfSet)
        {
            int result;
            Random rand = new Random();
            do
            {
                result = rand.Next(populate);
            } while (result == numOfSet);
            return result;
        }

        private void LocalImprovement(int changed)
        {
            int minWeight = int.MaxValue;
            for (int i = 0; i < Element.elements; i++)
            {
                if (population[changed, i] == false)
                {
                    if (Elements[i].weight < minWeight)
                        minWeight = Elements[i].weight;
                }
            }
            Stack<int> maxCostMinWeight = new Stack<int>();
            for (int i = 0; i < Element.elements; i++)
            {
                if (population[changed, i] == false && Elements[i].weight == minWeight)
                    maxCostMinWeight.Push(i);
            }
            int BestVertex = -1;
            int maxCost = 0;

            while (maxCostMinWeight.Count != 0)
            {
                int tmp = maxCostMinWeight.Pop();
                if (Elements[tmp].cost > maxCost)
                {
                    maxCost = Elements[tmp].cost;
                    BestVertex = tmp;
                }
            }
            if (BestVertex != -1)
            {
                bool[] temp = MatrixToSet(changed);
                temp[BestVertex] = true;
                if (WeightOfSet(temp) <= capacity)
                    population[changed, BestVertex] = true;
            }
        }

        private void SetToMatrix(bool[] set, int numOfSet)
        {
            for (int i = 0; i < Element.elements; i++)
            {
                population[numOfSet, i] = set[i];
            }
        }

        public override string ToString()
        {
            bool[] arr = MatrixToSet(SetWithMaxCost());
            string a = "";
            a += $"Weight \t Cost \n";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == true)
                {
                    a += $"{Elements[i].weight,3}  {Elements[i].cost,7} \n";
                }
            }
            a += $"Total result of the weight: {WeightOfSet(arr)} \nTotal result of the cost: {CostOfSet(arr)} \n";
            return a;
        }

        public void SaveToFile(string fileName)
        {
            StreamWriter sw = new StreamWriter(fileName);
            string a = "";
            a += ToString();
            sw.WriteLine(a);
            sw.Close();
            StreamWriter sw2 = new StreamWriter("Elements" + fileName);
            sw2.WriteLine(Element.ToString(Elements));
            sw2.Close();
        }
    }
}

