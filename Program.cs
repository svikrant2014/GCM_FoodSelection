using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCM_FoodSelection
{
    public class FoodConsoleOptions
    {
        public TimeOfOrder TimeOfDay { get; set; }
        public IEnumerable<DishType> FoodOrder { get; set; }

        public static FoodConsoleOptions Parse(string[] p)
        {
            TimeOfOrder time;
            Enum.TryParse(p[0], true, out time);

            return new FoodConsoleOptions()
            {
                TimeOfDay = time,
                FoodOrder = p.Skip(1)
                .Select(x =>
                {
                    DishType d;
                    Enum.TryParse<DishType>(x, out d);
                    return d;
                })
            };
        }
    }
    public class FoodConsole
    {
        static void Main(string[] args1)
        {
            string[] args = new String[3];
            Console.WriteLine();
            Console.WriteLine("Please input your preferences in the following format: 'Time of Day', Entree, Side, Drink, Dessert'");
            Console.WriteLine("\n");
            Console.WriteLine("For e.g. Morning, 1,2,3");

            var command = Console.ReadLine();
            args = command.ToString().Split(',');
            //args = new string[3] {"morning", "1", "2"};
            FoodConsoleOptions options = null;
            var sanitized = args.Select(x => x.Replace(",", string.Empty)).ToArray();
            try
            {
                options = FoodConsoleOptions.Parse(sanitized);
            }
            catch (Exception ex)
            {

                Console.WriteLine("There is some issue with the inputs. Please check:");
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(-1);

            }
            Debug.Assert(options != null);
            var meal = Meal.Create(options.TimeOfDay, options.FoodOrder.ToArray());
            var summary = meal.GenerateMealSummary();
            var outputText = GetMealOutput(summary, options);
            Console.WriteLine(outputText);

            Console.ReadKey();

        }

        public static string GetMealOutput(IEnumerable<CourseInfo> mealSummary, FoodConsoleOptions options)
        {
            var output = new List<string>();
            //var timeOfDayString = string.Format("{0}, ", options.TimeOfDay.ToString().ToLowerInvariant());
            //output.Add(timeOfDayString);
            foreach (var item in mealSummary)
            {
                output.Add(item.ToString());
            }
            return string.Join(", ", output);
        }


    }
}
