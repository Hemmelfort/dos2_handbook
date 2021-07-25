using DOS2_Handbook.Model;
using DOS2_Handbook.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DOS2_Handbook.ViewModel
{
    class SkillWindowViewModel:ViewModelBase
    {
       
        //技能树。注意里面包含的成员是各个学派，每个学派里面才是一个个的技能
        public ObservableCollection<SkillTreeNode> SkillTrees { get; set; }

        public SkillWindowViewModel()
        {
            NewInit();
        }


        //当前选中项。用户选中列表中的一项后，要先判断用户选中的是学派还是技能
        private object _selectedItem;

        public object SelectedItem
        {
            get { return _selectedItem; }
            set {
                var node = value as SkillTreeNode;
                if (node.NodeSkill != null)
                {
                    SelectedSkill = node.NodeSkill;
                    Alert(node.Header);
                }
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }



        //当前选中的技能。选中后界面窗口上会显示该技能的详细信息
        private DOS2_SkillModel selectedSkill;

        public DOS2_SkillModel SelectedSkill
        {
            get { return selectedSkill; }
            set { 
                selectedSkill = value;
                if (value.Property.TryGetValue("Description", out string desc))
                    SelectedDescription = desc;
                else
                    SelectedDescription = "";
                RaisePropertyChanged();
            }
        }


        //当玩家选中一个技能后，需要把参数填到技能描述里面
        private string _selectedDescription;

        public string SelectedDescription
        {
            get { return _selectedDescription; }
            set 
            {
                string rawDescription = DOS2_Utils.TranslateString(value);
                if (SelectedSkill.Property.TryGetValue("StatsDescriptionParams", out string paramString))
                {
                    string[] paramList = paramString.Split(';');

                    for(int i=0; i<paramList.Length; i++)
                    {
                        var pattern = $"\\[{i+1}\\]";
                        var param = paramList[i];
                        if (selectedSkill.Property.ContainsKey(param))
                        {
                            if (param.EndsWith("Radius"))
                                param = $" {selectedSkill.Property[param]}m ";
                            else
                                param = $" {selectedSkill.Property[param]} ";
                            
                        }
                        else
                        {
                            param = $" [{param}] ";
                        }

                        rawDescription = Regex.Replace(rawDescription, pattern, param);
                    }

                }

                _selectedDescription = rawDescription;
                RaisePropertyChanged(); 
            }
        }


        #region test only
        #endregion


        void NewInit()
        {
            var allSkills = DOS2_Utils.GetAllSkills();
            var allSchools = new Dictionary<string, SkillTreeNode>();

            SkillTrees = new ObservableCollection<SkillTreeNode>();
            
            allSkills.ForEach(s =>
            {
                ///先获取学派，再把技能放到该学派对应的List中去
                var abl = s.GetProperty("Ability", "Unknown");
                var node = new SkillTreeNode() 
                { 
                    Header = s.Name,
                    NodeSkill = s
                    //技能和学派都是SkillTreeNode类型，
                    //区别在于技能的SubNodes为空，学派的NodeSkill为null。
                };

                if (allSchools.ContainsKey(abl))
                {
                    allSchools[abl].SubNodes.Add(node);
                }
                else
                {
                    allSchools[abl] = new SkillTreeNode()
                    {
                        Header = abl,
                        SubNodes = new List<SkillTreeNode>() { node }
                        //学派的NodeSkill保持为null
                    };
                }
            });

            //给学派排序
            var allSchoolsOrdered = from school in allSchools orderby school.Key select school;

            ///把学派添加到技能树上（添加学派即可，学派下的各个技能是包含在
            ///学派里面的，也就是SubNodes里）
            foreach(KeyValuePair<string, SkillTreeNode> kvp in allSchoolsOrdered)
            {
                SkillTrees.Add(new SkillTreeNode
                {
                    Header = kvp.Key,
                    SubNodes = kvp.Value.SubNodes
                });
            }
            //foreach (var abl in allSchools.Keys)
            //{
            //    SkillTrees.Add(new SkillTreeNode
            //    {
            //        Header = abl,
            //        SubNodes = allSchools[abl].SubNodes
            //    });
            //}

            //启动后随机显示一个技能
            if (allSkills.Count != 0)
            {
                int randIndex = new Random().Next(allSkills.Count);
                SelectedSkill = allSkills[randIndex];
            }
        }





        //调试信息显示在底部状态栏上
        private string _debugMessage;

        public string DebugMessage
        {
            get { return _debugMessage; }
            set { _debugMessage = value; RaisePropertyChanged(); }
        }

        public void Alert(string msg)
        {
            DebugMessage = msg;
        }

    }
}
