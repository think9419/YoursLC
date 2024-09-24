using System;

namespace Think9.Services.Base
{
    public class Random
    {
        public static string GetRandom1()
        {
            System.Random rd = new System.Random();
            return rd.Next(1, 9).ToString();
        }

        public static string GetRandom10()
        {
            System.Random rd = new System.Random();
            return rd.Next(10, 99).ToString();
        }

        public static string GetRandom100()
        {
            System.Random rd = new System.Random();
            return rd.Next(100, 999).ToString();
        }

        public static string GetRandom1000()
        {
            System.Random rd = new System.Random();
            return rd.Next(1000, 9999).ToString();
        }

        public static string GetRandom10000()
        {
            System.Random rd = new System.Random();
            return rd.Next(10000, 99999).ToString();
        }

        public static DateTime GetRandomDate()
        {
            System.Random rd = new System.Random();
            return DateTime.Today.AddDays(rd.Next(0, 99) * -1);
        }

        public static string GetRandomDEC10()
        {
            System.Random rd = new System.Random();
            return rd.Next(10, 99).ToString() + "." + rd.Next(10, 99).ToString();
        }
    }
}