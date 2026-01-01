using System;
using System.Collections.Generic;
using System.Text;

namespace CardsAgainstHumanity.Application.Extensions
{
    public static class RandomX
    {
        private static readonly Random Instance = new Random();

        public static int Get(int upper = 999999) => Instance.Next(upper);
    }
}
