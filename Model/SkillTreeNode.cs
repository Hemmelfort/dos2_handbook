using DOS2_Handbook.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOS2_Handbook.Model
{

    class SkillTreeNode : ViewModelBase
    {
        public string Header { get; set; }
        public List<SkillTreeNode> SubNodes { get; set; }

        public DOS2_SkillModel NodeSkill { get; set; }
        public bool IsSelected { get; set; }

        private bool isExpanded;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; RaisePropertyChanged(); }
        }

    }
}
