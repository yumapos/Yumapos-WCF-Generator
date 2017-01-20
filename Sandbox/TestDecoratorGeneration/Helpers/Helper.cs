using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDecoratorGeneration.Helpers
{
    public class Helper
    {
        public bool CanIHelpYou()
        {
            return true;
        }

        public Task GetTask()
        {
            return new Task(()=> {});
        }
    }
}
