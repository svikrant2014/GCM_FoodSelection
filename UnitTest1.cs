using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GCM_FoodSelection;

namespace FoodSelectionTests
{
    public class morning
    {
        protected Meal sut;
        protected const TimeOfOrder TestTimeOfOrder = TimeOfOrder.Morning;
    }
    public class night
    {
        protected Meal sut;
        protected const TimeOfOrder TestTimeOfOrder = TimeOfOrder.Night;
    }

    [TestClass]
    public class DishTypeTests
    {
        [TestClass]
        public class when_breakfast_is_served : morning
        {
            public when_breakfast_is_served()
            {
                sut = Meal.Create(TestTimeOfOrder, DishType.Entree, DishType.Drink, DishType.Side);
            }
            [TestMethod]
            public void entree_for_eggs()
            {
                Assert.AreEqual(Dish.Eggs, sut.Entree);
            }

            [TestMethod]
            public void side_for_toast()
            {
                Assert.AreEqual(Dish.Toast, sut.Side);
            }

            [TestMethod]
            public void drink_for_coffee()
            {
                Assert.AreEqual(Dish.Coffee, sut.Drink);
            }

        }


        [TestClass]
        public class when_dinner_is_served : night
        {
            public when_dinner_is_served()
            {
                sut = Meal.Create(TestTimeOfOrder, DishType.Entree, DishType.Side, DishType.Drink, DishType.Side, DishType.Dessert);
            }
            [TestMethod]
            public void entree_for_steak()
            {
                Assert.AreEqual(Dish.Steak, sut.Entree);
            }

            [TestMethod]
            public void side_for_potato()
            {
                Assert.AreEqual(Dish.Potato, sut.Side);
            }

            [TestMethod]
            public void drink_for_wine()
            {
                Assert.AreEqual(Dish.Wine, sut.Drink);
            }

            [TestMethod]
            public void then_dessert_for_cake()
            {
                Assert.AreEqual(Dish.Cake, sut.Dessert);
            }
        }

    }

    // --- For Meal Class

    public class given_a_meal_ticket
    {
        protected Meal sut;
        protected List<CourseInfo> sutSummary;

        protected readonly DishType[] Breakfast = new DishType[] { DishType.Entree, DishType.Side, DishType.Drink };
        protected readonly DishType[] Dinner = new DishType[] { DishType.Entree, DishType.Side, DishType.Drink, DishType.Dessert };

        protected readonly SortedList<int, DishType> ExpectedOutputOrder = new SortedList<int, DishType>();
        protected const DishType ErrorFlag = DishType.Indeterminate;
        public given_a_meal_ticket()
        {
            ExpectedOutputOrder.Add(1, DishType.Entree);
            ExpectedOutputOrder.Add(2, DishType.Side);
            ExpectedOutputOrder.Add(3, DishType.Drink);
            ExpectedOutputOrder.Add(4, DishType.Dessert);
        }
    }

    [TestClass]
    public class MealTests
    {
        [TestClass]
        public class when_breakfast_is_ordered : given_a_meal_ticket
        {
            public when_breakfast_is_ordered()
            {
                sut = Meal.Create(TimeOfOrder.Morning, Breakfast);
                sutSummary = sut.GenerateMealSummary().ToList();
            }

            [TestMethod]
            public void then_TimeOfOrder_is_morning()
            {
                Assert.AreEqual(TimeOfOrder.Morning, sut.TimeOfDay);
            }

        }


        [TestClass]
        public class when_dinner_is_ordered : given_a_meal_ticket
        {
            public when_dinner_is_ordered()
            {
                sut = Meal.Create(TimeOfOrder.Night, Dinner);
                sutSummary = sut.GenerateMealSummary().ToList();
            }

            [TestMethod]
            public void then_TimeOfOrder_is_dinner()
            {
                Assert.AreEqual(TimeOfOrder.Night, sut.TimeOfDay);
            }
            

        }
    }

    // -- main method tests

    [TestClass]
    public class ConsoleTests
    {


        [TestMethod]
        public void CommandLineShouldParseTimeOfDay()
        {
            var opts = FoodConsoleOptions.Parse(new[] { "morning" });
            Assert.AreEqual(TimeOfOrder.Morning, opts.TimeOfDay);

            opts = FoodConsoleOptions.Parse(new[] { "night" });
            Assert.AreEqual(TimeOfOrder.Night, opts.TimeOfDay);

        }


        [TestMethod]
        public void ValidInputShouldOutputDishes()
        {
            var opts = FoodConsoleOptions.Parse(new[] { "morning", "1", "2", "3" });
            var meal = Meal.Create(opts.TimeOfDay, opts.FoodOrder.ToArray());
            var output = FoodConsole.GetMealOutput(meal.GenerateMealSummary(), opts);

            const string expected = "eggs, toast, coffee";
            Console.WriteLine(output);
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void InvalidMenuSelectionsOutputError()
        {

            var opts = FoodConsoleOptions.Parse(new[] { "morning", "1", "2", "3", "4" });
            var meal = Meal.Create(opts.TimeOfDay, opts.FoodOrder.ToArray());
            var output = FoodConsole.GetMealOutput(meal.GenerateMealSummary(), opts);

            const string expected = "eggs, toast, coffee, error";
            Console.WriteLine(output);
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void MultipleDishesOutputCount()
        {
            var opts = FoodConsoleOptions.Parse(new[] { "morning", "1", "2", "3", "3", "3" });
            var meal = Meal.Create(opts.TimeOfDay, opts.FoodOrder.ToArray());
            var output = FoodConsole.GetMealOutput(meal.GenerateMealSummary(), opts);

            const string expected = "eggs, toast, coffee(x3)";
            Console.WriteLine(output);
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void OutOfOrderInputOutputsCorrectOrder()
        {
            var opts = FoodConsoleOptions.Parse(new[] { "morning", "2", "1", "3" });
            var meal = Meal.Create(opts.TimeOfDay, opts.FoodOrder.ToArray());
            var output = FoodConsole.GetMealOutput(meal.GenerateMealSummary(), opts);

            const string expected = "eggs, toast, coffee";
            Console.WriteLine(output);
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void FullDinnerOrderOutputsAllDishes()
        {
            var opts = FoodConsoleOptions.Parse(new[] { "night", "1", "2", "3", "4" });
            var meal = Meal.Create(opts.TimeOfDay, opts.FoodOrder.ToArray());
            var output = FoodConsole.GetMealOutput(meal.GenerateMealSummary(), opts);

            const string expected = "steak, potato, wine, cake";
            Console.WriteLine(output);
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void InvalidDinnerOutputsOnlyValidDishes()
        {
            var opts = FoodConsoleOptions.Parse(new[] { "night", "1", "2", "3", "5" });
            var meal = Meal.Create(opts.TimeOfDay, opts.FoodOrder.ToArray());
            var output = FoodConsole.GetMealOutput(meal.GenerateMealSummary(), opts);

            const string expected = "steak, potato, wine, error";
            Console.WriteLine(output);
            Assert.AreEqual(expected, output);
        }


        [TestMethod]
        public void InvalidInputStopsProcessingOutput()
        {
            var opts = FoodConsoleOptions.Parse(new[] { "night", "1", "1", "2", "3", "5" });
            var meal = Meal.Create(opts.TimeOfDay, opts.FoodOrder.ToArray());
            var output = FoodConsole.GetMealOutput(meal.GenerateMealSummary(), opts);

            const string expected = "steak, error";
            Console.WriteLine(output);
            Assert.AreEqual(expected, output);
        }


    }


}
