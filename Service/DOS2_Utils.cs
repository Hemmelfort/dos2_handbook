using DOS2_Handbook.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;

namespace DOS2_Handbook.Service
{
    class DOS2_Utils
    {
        #region 单例模式
        private static readonly Lazy<DOS2_Utils> lazy =
            new Lazy<DOS2_Utils>(() => new DOS2_Utils());
        public static DOS2_Utils Instance
        {
            get { return lazy.Value; }
        }
        private DOS2_Utils()
        {
            LoadIconModels();   //Load icon metadata
            LoadImages();       //Unpack images from zip file
            LoadStrings();
        }
        #endregion


        // ################################################ //
        #region Images and Icons
        private Dictionary<string, DOS2_IconModel> IconModels;
        private BitmapImage SkillsAtlasImage { get; set; }

        private void LoadIconModels()
        {
            string jsonString = FileServices.GetTextFileContent("Stats/Ability_Skill_Status_Icons.json");
            var serializer = new JavaScriptSerializer();
            var serialist = serializer.Deserialize<List<Dictionary<string, string>>>(jsonString);
            IconModels = new Dictionary<string, DOS2_IconModel>();
            serialist.ForEach(d =>
            {
                IconModels.Add(d["Key"], new DOS2_IconModel
                {
                    Name = d["Key"],
                    TextureAtlasPath = d["TextureAtlasPath"],
                    X1 = int.Parse(d["X1"]),
                    X2 = int.Parse(d["X2"]),
                    Y1 = int.Parse(d["Y1"]),
                    Y2 = int.Parse(d["Y2"])
                });
            });
        }

        private DOS2_IconModel GetIconModel(string iconName)
        {
            IconModels.TryGetValue(iconName, out DOS2_IconModel iconModel);
            return iconModel;
        }

        private void LoadImages()
        {
            var bitmap = FileServices.GetImageAsBitmapImage("Images/Ability_Skill_Status_Icons.PNG");
            if (bitmap != null)
                SkillsAtlasImage = bitmap;
        }

        public static BitmapSource GetSkillIcon(string iconName)
        {
            Instance.IconModels.TryGetValue(iconName, out DOS2_IconModel icon);

            var cbm = new CroppedBitmap();
            cbm.BeginInit();
            cbm.Source = Instance.SkillsAtlasImage;
            if (icon == null)
            {
                cbm.SourceRect = new Int32Rect(384, 0, 64, 64);
            }
            else
            {
                cbm.SourceRect = new Int32Rect(icon.X1, icon.Y1, icon.X2 - icon.X1, icon.Y2 - icon.Y1);
            }
            cbm.EndInit();

            return cbm;


            //var rect = new Int32Rect(icon.X1, icon.Y1, icon.X2 - icon.X1, icon.Y2 - icon.Y1);
            //int stride = (Instance.SkillsAtlasImage.Format.BitsPerPixel * rect.Width + 7) / 8;
            //byte[] data = new byte[stride * rect.Height];
            //Instance.SkillsAtlasImage.CopyPixels(rect, data, stride, 0);
            //return BitmapSource.Create(rect.Width, rect.Height, 96, 96,
            //    System.Windows.Media.PixelFormats.Bgra32, null, data, stride);
        }
        #endregion


        // ################################################ //
        #region Text and Strings

        private Dictionary<string, string> ChineseDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> AbilityDictionary;

        //从文件加载翻译的文本
        void LoadStrings()
        {
            //加载技能属性翻译
            var xmlString = FileServices.GetTextFileContent("Strings/SkillProperties.xml");
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);
            var ls = xml.SelectNodes("/GameData/LocalizedText//Row");

            foreach (XmlNode item in ls)
            {
                var tag = item.Attributes.GetNamedItem("Tag").Value;
                var text = item.SelectSingleNode("./Text").InnerText;
                ChineseDictionary.Add(tag, text);
                //Console.WriteLine(tag + ": " +  text);
            }

            //加载词条
            var jsonString = FileServices.GetTextFileContent("Strings/StatsStrings.json");
            var serializer = new JavaScriptSerializer();
            var dic = serializer.Deserialize<Dictionary<string, string>>(jsonString);
            ChineseDictionary = ChineseDictionary.Concat(dic).ToDictionary(k => k.Key, v => v.Value);

            //加载学派名
            jsonString = FileServices.GetTextFileContent("Strings/AbilityStrings.json");
            serializer = new JavaScriptSerializer();
            AbilityDictionary = serializer.Deserialize<Dictionary<string, string>>(jsonString);
        }

        /// <summary>
        /// 对词条进行翻译
        /// </summary>
        /// <param name="name">要翻译的词条（如“Jump_CloakAndDagger_DisplayName”）</param>
        /// <returns>翻译后的词条（如“金蝉脱壳”）</returns>
        public static string TranslateString(string stringName)
        {
            if (Instance.ChineseDictionary.ContainsKey(stringName))
            {
                return Instance.ChineseDictionary[stringName];
            }
            return stringName;
        }

        /// <summary>
        /// 翻译学派名称。如果传进来的不是学派而是技能，那就把技能名修饰一下。
        /// </summary>
        /// <param name="ability">学派名称（如“Water”）</param>
        /// <returns>翻译后的学派名称，如“水占学派”</returns>
        public static string TranslateAbilitySchoolName(string ability)
        {
            if(string.IsNullOrWhiteSpace(ability))
            {
                return "空白";
            }

            if (Instance.AbilityDictionary.ContainsKey(ability))
            {
                return Instance.AbilityDictionary[ability];
            }
            else
            {
                var index = ability.IndexOf('_');
                if (index != -1 && index + 1 < ability.Length)
                    return ability.Substring(index + 1);
            }
            return ability;
        }
        #endregion


        // ################################################ //
        #region Skills

        public static List<DOS2_SkillModel> GetAllSkills()
        {
            //string fpath = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Stats\Skills.json";
            //string jsonString = File.ReadAllText(fpath);
            string jsonString = FileServices.GetTextFileContent("Stats/Skills.json");
            var serializer = new JavaScriptSerializer();
            var serialist = serializer.Deserialize<List<Dictionary<string, string>>>(jsonString);
            var ls = new List<DOS2_SkillModel>();
            serialist.ForEach(d =>
            {
                var skill = new DOS2_SkillModel();
                ls.Add(new DOS2_SkillModel()
                {
                    Name = d["EntryName"],
                    Property = d
                });
            });
            return ls;
        }
        #endregion

    }
}
