using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOS2_Handbook.Model
{
    public class DOS2_SkillModel
    {

        public string Name;
        public Dictionary<string, string> Property { get; set; }
        public DOS2_SkillModel UsingSkill;   //Base on other skill

        public DOS2_SkillModel()
        {
            Property = new Dictionary<string, string>();
        }

        public string GetProperty(string name)
        {
            Property.TryGetValue(name, out string value);

            return value;
        }

        public string GetProperty(string name, string defaultValue)
        {
            if (Property.ContainsKey(name))
                return Property[name];
            else
                return defaultValue;
        }

/*        public SkillModel ToModel()
        {
            var skillModel = new SkillModel();
            if (Property.TryGetValue("Ability", out string abl))
                skillModel.Ability = abl;
            return skillModel;
        }*/
    }



}
