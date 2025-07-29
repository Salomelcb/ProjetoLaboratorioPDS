using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GestaoAvaliacoes.Model
{
    public class NavMenuItem
    {
        public string Icon { get; set; }
        public string ToolTip { get; set; }
        public Page TargetView { get; set; }
    }
}

