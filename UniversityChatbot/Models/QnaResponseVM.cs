using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class QnaResponseVM
    {
            public List<QnaAnswersVM> Answers { get; set; }
         
    }
    
public class QnaAnswersVM
{
    public List<string> Questions { get; set; }
    public string Answer { get; set; }
    public double Score { get; set; }
    public List<QnaMetadataVM> metadata { get; set; }
    public int Id { get; set; }
    public string Source { get; set; }
}
public class QnaMetadataVM
{
    public string Name { get; set; }
    public string Value { get; set; }
}
}
