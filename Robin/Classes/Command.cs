using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robin
{
    public class Command : RelayCommand
    {
        public string Content { get; set; }

        public string ToolTip { get; set; }

        //
        // Summary:
        //     Initializes a new instance of the Command class that can always execute.
        //
        // Parameters:
        //   execute:
        //     The execution logic. IMPORTANT: Note that closures are not supported at the moment
        //     due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/).
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     If the execute argument is null.
        public Command(Action execute) : base(execute)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the RelayCommand class.
        //
        // Parameters:
        //   execute:
        //     The execution logic. IMPORTANT: Note that closures are not supported at the moment
        //     due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/).
        //
        //   canExecute:
        //     The execution status logic.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     If the execute argument is null. IMPORTANT: Note that closures are not supported
        //     at the moment due to the use of WeakActions (see http://stackoverflow.com/questions/25730530/).
        public Command(Action execute, Func<bool> canExecute) : base(execute, canExecute)
        {
        }

        public Command(Action execute, string content = null, string toolTip = null) : base(execute)
        {
            Content = content;
            ToolTip = toolTip;
        }

        public Command(Action execute, Func<bool> canExecute, string content = null, string toolTip = null) : base(execute, canExecute)
        {
            Content = content;
            ToolTip = toolTip;
        }
    }
}
