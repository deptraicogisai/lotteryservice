using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotteryAPI.Models
{
    public class LotteryResult
    {
        public LotteryResult()
        {
            result = new List<string>();
        }
        public string district { get; set; }

        public List<string> result { get; set; }
    }
}