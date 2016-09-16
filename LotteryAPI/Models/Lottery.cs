using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryAPI.Models
{
    public class Lottery
    {
        public Lottery()
        {
            result = new List<LotteryResult>();
        }
        public string title { get; set; }

        public List<LotteryResult> result { get; set; }
    }
}