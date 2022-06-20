using MapAssist.Helpers;
using MapAssist.Settings;
using MapAssist.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.ObjectGraphVisitors;

namespace MA_Filter
{
    public partial class Form1 : Form
    {
        //List<ItemFilter> rules = new List<ItemFilter>();
        Dictionary<Item, List<ItemFilter>> sumRules = new Dictionary<Item, List<ItemFilter>>();
        Item curSelectItem = new Item();
        Item curSelectClassItem = new Item();
        List<ItemFilter> curItemFilters = new List<ItemFilter>();
        List<ItemFilter> curClassFilters = new List<ItemFilter>();
        ItemFilter curItemFilter = new ItemFilter();

        public Form1()
        {
            InitializeComponent();
        }

        private void resetUI()
        {
            checkBox3.Checked = false;
            foreach (int item in checkedListBox1.CheckedIndices)
            {
                checkedListBox1.SetItemChecked(item, false);
            }


            checkBox2.Checked = false;
            foreach (int item in checkedListBox2.CheckedIndices)
            {
                checkedListBox2.SetItemChecked(item, false);
            }

            checkBox3.Checked = false;
            foreach (int item in checkedListBox3.CheckedIndices)
            {
                checkedListBox3.SetItemChecked(item, false);
            }

            checkBox13.Checked = false;
            //checkBox12.Checked = false;
            checkBox1.Checked = false;
            comboBox2.SelectedIndex = -1;
            //comboBox12.SelectedIndex = -1;

            List<ComboBox> affix_combox = new List<ComboBox>
            {
                comboBox4,comboBox8,comboBox9,comboBox6,comboBox3,comboBox5,comboBox7
            };
            List<TextBox> affix_combox_value = new List<TextBox>
            {
                textBox1,textBox2,textBox3,textBox5,textBox4,textBox7,textBox6
            };

            List<CheckBox> affix_check = new List<CheckBox>
            {
                checkBox5,checkBox6,checkBox7,checkBox8,checkBox11,checkBox10,checkBox9
            };

            foreach(var ui in affix_combox)
            {
                ui.SelectedIndex = -1;
            }
            foreach (var ui in affix_combox_value)
            {
                ui.Text = "";
            }
            foreach (var ui in affix_check)
            {
                ui.Checked = false;
            }



        }

        private void button4_Click(object sender, EventArgs e)
        {
            string file = "";
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "请选择路径";     //弹窗的标题
            dialog.InitialDirectory = Application.StartupPath;       //默认打开的文件夹的位置
            dialog.Filter = "过滤文件(*.yaml)|*.yaml|所有文件(*.*)|*.*";       //筛选文件

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
            }
            else
            {
                return;
            }

            StreamWriter yamlWriter = File.CreateText(file);
            Serializer yamlSerializer = new Serializer();
            //var s = new SerializerBuilder().WithObjectGraphTraversalStrategyFactory( new DefaultExclusiveObjectGraphVisitor()).Build();
            //var serializer1 = new SerializerBuilder().WithNamingConvention(new CamelCaseNamingConvention())
            //    .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
            //    .Build();
            yamlSerializer.Serialize(yamlWriter, sumRules);
            yamlWriter.Close();

            MessageBox.Show("导出成功，请替换Wakuwaku文件夹下的ItemFilter.yaml，并重启Wakuwaku验证");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // load all json file
            Localization.LoadLocalizationFile();

            // add all class data to ui
            foreach (var item in Items.ItemClassesToChinese)
            {
                comboBox1.Items.Add(item.Value);
            }

            // add all affix
            foreach(var item in Affix.ItemAffix)
            {
                comboBox4.Items.Add(item.Value);
                comboBox8.Items.Add(item.Value);
                comboBox9.Items.Add(item.Value);
            }

            // add all global skill
            foreach (var item in Affix.SkillAffix)
            {
                comboBox6.Items.Add(item.Key);
                comboBox3.Items.Add(item.Key);
                comboBox5.Items.Add(item.Key);
                comboBox7.Items.Add(item.Key);
            }

            // add all normal skill
            int count = 0;
            foreach(var skilltree in SkillExtensions.SkillTreeToSkillDict)
            {
                foreach(var skill in skilltree.Value)
                {
                    // translate skill is not enough,only 205,in fact is 210
                    string skillName = skill.ToString();
                    string localName = string.Concat(skillName.Select((x, j) => j > 0 && char.IsUpper(x) ? " " + x.ToString() : x.ToString()));
                    LocalizedObj chinese = Affix.LocalizedSkills.FirstOrDefault(x => x.Value.enUS.Equals(localName)).Value;
                    if (chinese != null)
                    {
                        Debug.WriteLine(chinese.zhTW);
                        count++;
                        comboBox6.Items.Add(chinese.zhTW);
                        comboBox3.Items.Add(chinese.zhTW);
                        comboBox5.Items.Add(chinese.zhTW);
                        comboBox7.Items.Add(chinese.zhTW);
                        comboBox10.Items.Add(chinese.zhTW);
                        Affix.SkillAffix.Add(chinese.zhTW, skillName);
                    }
                    else
                    {
                        Debug.WriteLine(localName);
                    }
                }
            }
            Debug.WriteLine(count);

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // display sub items
            listBox1.Items.Clear();
            Item curItem = Items.ItemClassesToChinese.First(q => q.Value == comboBox1.SelectedItem.ToString()).Key;
            if (curItem < Item.ClassRings)
                curSelectClassItem = curItem;
            else
                curSelectClassItem = Item.xBases;

            Item[] items = Items.ItemClasses[curItem];
            foreach (var item in items)
            {
                /* it's a bug that Classboots name is same as boots 
                 * but MapAssit author wouldnt change it
                 * so have to jump the boots item
                 * https://github.com/OneXDeveloper/MapAssist/issues/479
                 */
                if (item == Item.Boots) continue;

                string key = Items._ItemCodes[(uint)item];

                listBox1.Items.Add(Items.LocalizedItems[key].zhTW);
            }

            // display class filters
            comboBox12.Items.Clear();
            Item curClass;
            curClass = Items.ItemClassesToChinese.First(q => q.Value == comboBox1.SelectedItem.ToString()).Key;

            curSelectClassItem = curClass;
            if (sumRules.ContainsKey(curSelectClassItem))
                curClassFilters = sumRules[curSelectClassItem];
            else
                curClassFilters = new List<ItemFilter>();

            int count = curClassFilters.Count;
            for (int i = 0; i < count; i++)
                comboBox12.Items.Add(i + 1);
            label9.Text = "规则个数：" + curClassFilters.Count;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
                checkBox2.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("先选择物品");
                return;
            }
            
            curItemFilter = new ItemFilter();
            curItemFilters.Add(curItemFilter);
            comboBox11.Items.Add(comboBox11.Items.Count + 1);
            comboBox11.SelectedIndexChanged -= new System.EventHandler(this.comboBox11_SelectedIndexChanged);
            comboBox11.SelectedIndex = comboBox11.Items.Count-1;
            comboBox11.SelectedIndexChanged += new System.EventHandler(this.comboBox11_SelectedIndexChanged);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            curItemFilter = new ItemFilter();

            if (comboBox11.SelectedIndex == -1)
            {
                MessageBox.Show("请先添加规则");
                return;
            }

            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("先选择物品");
                return;
            }

            if (checkBox1.Checked)
            {
                if (comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("有形无形必须选一种");
                    return;
                }
                if (comboBox2.SelectedIndex == 0)
                    curItemFilter.Ethereal = false;
                if (comboBox2.SelectedIndex == 1)
                    curItemFilter.Ethereal = true;
            }

            if(checkBox2.Checked)
            {
                if (checkedListBox2.CheckedItems.Count == 0)
                {
                    MessageBox.Show("必须选一个孔数");
                    return;
                }
                curItemFilter.Sockets = new int[checkedListBox2.CheckedItems.Count];
                int index = 0;
                for(int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    if (checkedListBox2.GetItemChecked(i))
                        curItemFilter.Sockets[index++] = i;
                }
            }

            if (checkBox14.Checked)
            {
                curItemFilter.CheckVendor = true;
            }
            else
            {
                curItemFilter.CheckVendor = false;
            }

            if (checkBox3.Checked)
            {
                if(checkedListBox1.CheckedItems.Count==0)
                {
                    MessageBox.Show("必须选一个质量");
                    return;
                }

                curItemFilter.Qualities = new ItemQuality[checkedListBox1.CheckedItems.Count];
                int index = 0;
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.GetItemChecked(i))
                        curItemFilter.Qualities[index++] = (ItemQuality)(i +1);
                }
            }

            if (checkBox5.Checked)
                if (comboBox4.SelectedIndex != -1)
                {
                    string affix = Affix.ItemAffix.First(q => q.Value == comboBox4.SelectedItem.ToString()).Key;
                    if(textBox1.Text.Trim() == "")
                    {
                        MessageBox.Show("词缀必须填数值");
                        return;
                    }
                    curItemFilter.GetType().GetProperty(affix).SetValue(curItemFilter,int.Parse(textBox1.Text));
                }

            if (checkBox6.Checked)
                if (comboBox8.SelectedIndex != -1)
                {
                    string affix = Affix.ItemAffix.First(q => q.Value == comboBox8.SelectedItem.ToString()).Key;
                    if (textBox2.Text.Trim() == "")
                    {
                        MessageBox.Show("词缀必须填数值");
                        return;
                    }
                    curItemFilter.GetType().GetProperty(affix).SetValue(curItemFilter, int.Parse(textBox2.Text));
                }

            if (checkBox7.Checked)
                if (comboBox9.SelectedIndex != -1)
                {
                    string affix = Affix.ItemAffix.First(q => q.Value == comboBox9.SelectedItem.ToString()).Key;
                    if (textBox3.Text.Trim() == "")
                    {
                        MessageBox.Show("词缀必须填数值");
                        return;
                    }
                    curItemFilter.GetType().GetProperty(affix).SetValue(curItemFilter, int.Parse(textBox3.Text));
                }

            // reset all
            curItemFilter.ClassSkills.Clear();
            curItemFilter.SkillTrees.Clear();
            curItemFilter.Skills.Clear();

            if(checkBox8.Checked)
                if (!addSkillFromUI(comboBox6, textBox5, ref curItemFilter))
                    return;
            if (checkBox11.Checked)
                if (!addSkillFromUI(comboBox3, textBox4, ref curItemFilter))
                    return;
            if(checkBox10.Checked)
                if (!addSkillFromUI(comboBox5, textBox7, ref curItemFilter))
                    return;
            if(checkBox9.Checked)
                if (!addSkillFromUI(comboBox7, textBox6, ref curItemFilter))
                    return;

            if(checkBox4.Checked)
                if (comboBox10.SelectedIndex != -1)
                {
                    // normal skill
                    // skill tree
                    string name = Affix.SkillAffix.First(q => q.Key == comboBox10.SelectedItem.ToString()).Value;
                    Skill playerClass = (Skill)Enum.Parse(typeof(Skill), name);
                    curItemFilter.SkillCharges.Add(playerClass, 1);
                }


            curItemFilters[comboBox11.SelectedIndex] = curItemFilter;
            sumRules[curSelectItem] = curItemFilters;
        }

        private bool addSkillFromUI(ComboBox comboBox,TextBox textBox,ref ItemFilter itemFilter)
        {
            if (comboBox.SelectedIndex != -1)
            {
                if (textBox.Text.Trim() == "")
                {
                    MessageBox.Show("技能必须填数值");
                    return false;
                }
                if (comboBox.SelectedItem.ToString().Contains("_"))
                {
                    // class skill
                    string name = Affix.SkillAffix.First(q => q.Key == comboBox.SelectedItem.ToString()).Value;
                    if (name.Equals("Any_C"))
                        name = "Any";

                    PlayerClass playerClass = (PlayerClass)Enum.Parse(typeof(PlayerClass), name);
                    itemFilter.ClassSkills.Add(playerClass, int.Parse(textBox.Text));
                }
                else if (comboBox.SelectedItem.ToString().Contains("-"))
                {
                    // skill tree
                    string name = Affix.SkillAffix.First(q => q.Key == comboBox.SelectedItem.ToString()).Value;
                    if (name.Equals("Any_T"))
                        name = "Any";

                    SkillTree playerClass = (SkillTree)Enum.Parse(typeof(SkillTree), name);
                    itemFilter.SkillTrees.Add(playerClass, int.Parse(textBox.Text));
                }
                else
                {
                    // normal skill
                    // skill tree
                    string name = Affix.SkillAffix.First(q => q.Key == comboBox.SelectedItem.ToString()).Value;
                    if (name.Equals("Any_S"))
                        name = "Any";

                    Skill playerClass = (Skill)Enum.Parse(typeof(Skill), name);
                    itemFilter.Skills.Add(playerClass, int.Parse(textBox.Text));
                }
            }

            return true;
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox11.Items.Clear();
            Item curItem;
            Item curClass;
            curClass = Items.ItemClassesToChinese.First(q => q.Value == comboBox1.SelectedItem.ToString()).Key;
            if(curClass == Item.ClassBoots)
                curItem = Items.ItemClasses[curClass][listBox1.SelectedIndex+1];
            else
                curItem = Items.ItemClasses[curClass][listBox1.SelectedIndex];

            curSelectItem = curItem;
            if (sumRules.ContainsKey(curSelectItem))
                curItemFilters = sumRules[curSelectItem];
            else
                curItemFilters = new List<ItemFilter>();

            int count = curItemFilters.Count;
            for (int i = 0; i < count; i++)
                comboBox11.Items.Add(i+1);
            label2.Text = "规则个数：" + curItemFilters.Count;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string file = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;      //该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";     //弹窗的标题
            dialog.InitialDirectory = Application.StartupPath;       //默认打开的文件夹的位置
            dialog.Filter = "过滤文件(*.yaml)|*.yaml|所有文件(*.*)|*.*";       //筛选文件

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
            }
            else
            {
                return;
            }
            LootLogConfiguration.Load(file);
            sumRules = LootLogConfiguration.Filters;
            label4.Text = "总规则数:" + sumRules.Count.ToString();
            MessageBox.Show("已导入" + "总规则数:" + sumRules.Count.ToString());
            label5.Text = "当前导入规则文件：" + Path.GetFileNameWithoutExtension(file);
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox11.Items.Count == 0)
                return;

            curItemFilter = curItemFilters[comboBox11.SelectedIndex];
            resetUI();

            if (curItemFilter.Ethereal != null)
            {
                checkBox1.Checked = true;
                if((bool)curItemFilter.Ethereal)
                    comboBox2.SelectedIndex = 1;
                else
                    comboBox2.SelectedIndex = 0;
            }

            if(curItemFilter.CheckVendor)
            {
                checkBox14.Checked = true;
            }
            else
            {
                checkBox14.Checked = false;
            }

            if (curItemFilter?.Sockets?.Length > 0)
            {
                checkBox2.Checked = true;
                foreach(var s in curItemFilter.Sockets)
                {
                    checkedListBox2.SetItemChecked(s, true);
                }
            }

            if (curItemFilter?.Qualities?.Length > 0)
            {
                checkBox3.Checked = true;
                foreach (var s in curItemFilter.Qualities)
                {
                    checkedListBox1.SetItemChecked((int)(s) - 1,true);
                }
            }

            List<ComboBox> affix_combox = new List<ComboBox>
            {
                comboBox4,comboBox8,comboBox9
            };
            List<TextBox> affix_combox_value = new List<TextBox>
            {
                textBox1,textBox2,textBox3
            };

            List<CheckBox> affix_check = new List<CheckBox>
            {
                checkBox5,checkBox6,checkBox7
            };

            int count = 0;
            foreach (var affix in curItemFilter.GetType().GetProperties())
            {
                if(Affix.ItemAffix.ContainsKey(affix.Name))
                {
                    if (affix.GetValue(curItemFilter, null) == null)
                        continue;
                    affix_combox[count].SelectedIndex = comboBox4.Items.IndexOf(Affix.ItemAffix[affix.Name]);
                    affix_combox_value[count].Text = (affix.GetValue(curItemFilter, null)).ToString();
                    affix_check[count++].Checked = true;
                }
            }

            affix_combox = new List<ComboBox>
            {
                comboBox6,comboBox3,comboBox5,comboBox7
            };
            affix_combox_value = new List<TextBox>
            {
                textBox5,textBox4,textBox7,textBox6
            };

            affix_check = new List<CheckBox>
            {
                checkBox8,checkBox11,checkBox10,checkBox9
            };

            count = 0;
            foreach (var affix in curItemFilter.ClassSkills)
            {
                string key = affix.Key.ToString();
                if (key.Equals("Any"))
                    key = key+"_C";

                affix_combox[count].SelectedIndex = comboBox6.Items.IndexOf(Affix.SkillAffix.First(x => x.Value.Equals(key)).Key.ToString());
                affix_combox_value[count].Text = affix.Value.ToString();
                affix_check[count++].Checked = true;
            }

            foreach (var affix in curItemFilter.SkillTrees)
            {
                string key = affix.Key.ToString();
                if (key.Equals("Any"))
                    key += "_T";

                affix_combox[count].SelectedIndex = comboBox6.Items.IndexOf(Affix.SkillAffix.First(x => x.Value.Equals(key)).Key.ToString());//;
                affix_combox_value[count].Text = affix.Value.ToString();
                affix_check[count++].Checked = true;
            }

            foreach (var affix in curItemFilter.Skills)
            {
                string key = affix.Key.ToString();
                if (key.Equals("Any"))
                    key += "_S";

                affix_combox[count].SelectedIndex = comboBox6.Items.IndexOf(Affix.SkillAffix.First(x => x.Value.Equals(key)).Key.ToString());//;
                affix_combox_value[count].Text = affix.Value.ToString();
                affix_check[count++].Checked = true;
            }

            if(curItemFilter.SkillCharges.Count> 0)
            {
                comboBox10.SelectedIndex = comboBox10.Items.IndexOf(Affix.SkillAffix.First(x => x.Value.Equals(curItemFilter.SkillCharges.First().Key.ToString())).Key.ToString());//;
                checkBox4.Checked = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox11.SelectedIndex == -1)
            {
                MessageBox.Show("请先选择规则");
                return;
            }

            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("先选择物品");
                return;
            }
            curItemFilters.RemoveAt(comboBox11.SelectedIndex);
            if (curItemFilters.Count == 0)
                sumRules.Remove(curSelectItem);
            else
                sumRules[curSelectItem] = curItemFilters;

            comboBox11.Items.RemoveAt(comboBox11.SelectedIndex);
            for (int i = 0; i < comboBox11.Items.Count; i++)
            {
                comboBox11.Items[i] = (i + 1).ToString();
            }
            label2.Text = "规则个数：" + curItemFilters.Count;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
           resetUI();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Dictionary<Item, List<ItemFilter>> rules1 = new Dictionary<Item, List<ItemFilter>>();

            Dictionary<Item, List<ItemFilter>> rules2 = new Dictionary<Item, List<ItemFilter>>();

            string file = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;      //该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";     //弹窗的标题
            dialog.InitialDirectory = Application.StartupPath;       //默认打开的文件夹的位置
            dialog.Filter = "过滤文件(*.yaml)|*.yaml|所有文件(*.*)|*.*";       //筛选文件

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = dialog.FileName;
                Debug.WriteLine(dialog.FileNames);
            }
            else
            {
                return;
            }

            if(dialog.FileNames.Length != 2)
            {
                MessageBox.Show("请选择2个文件进行合并");
            }

            LootLogConfiguration.Load(dialog.FileNames[0]);
            rules1 = LootLogConfiguration.Filters;

            LootLogConfiguration.Load(dialog.FileNames[1]);
            rules2 = LootLogConfiguration.Filters;

            foreach(var rule in rules1)
            {
                if(rules2.ContainsKey(rule.Key))
                {
                    rules2[rule.Key].AddRange(rule.Value);
                }
                else
                {
                    rules2.Add(rule.Key, rule.Value);
                }
            }
            sumRules = rules2;
            label4.Text = "总规则数:" + sumRules.Count.ToString();
            MessageBox.Show("已合并" + "总规则数:" + sumRules.Count.ToString());
            label5.Text = "当前导入规则文件：" + Path.GetFileNameWithoutExtension(dialog.FileNames[0])+
                " + " + Path.GetFileNameWithoutExtension(dialog.FileNames[1]);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = "总规则数:" + sumRules.Count.ToString();
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // add new clas filter
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("先选择物品种类");
                return;
            }

            if(comboBox1.SelectedIndex >31)
            {
                MessageBox.Show("该物品种类不具备过滤能力");
                return;
            }

            curItemFilter = new ItemFilter();
            curClassFilters.Add(curItemFilter);
            comboBox12.Items.Add(comboBox12.Items.Count + 1);
            comboBox12.SelectedIndexChanged -= new System.EventHandler(this.comboBox12_SelectedIndexChanged);
            comboBox12.SelectedIndex = comboBox12.Items.Count - 1;
            comboBox12.SelectedIndexChanged += new System.EventHandler(this.comboBox12_SelectedIndexChanged);
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            // change class filter
            if (comboBox12.Items.Count == 0)
                return;

            if (comboBox12.SelectedIndex == -1)
                return;

            curItemFilter = curClassFilters[comboBox12.SelectedIndex];
            resetUI();

            if (curItemFilter.Ethereal != null)
            {
                checkBox1.Checked = true;
                if ((bool)curItemFilter.Ethereal)
                    comboBox2.SelectedIndex = 1;
                else
                    comboBox2.SelectedIndex = 0;
            }

            if (curItemFilter.CheckVendor)
            {
                checkBox14.Checked = true;
            }
            else
            {
                checkBox14.Checked = false;
            }

            if (curItemFilter?.Tiers?.Length > 0)
            {
                checkBox13.Checked = true;
                foreach (var s in curItemFilter.Tiers)
                {
                    checkedListBox3.SetItemChecked((int)s, true);
                }
            }

            if (curItemFilter?.Sockets?.Length > 0)
            {
                checkBox2.Checked = true;
                foreach (var s in curItemFilter.Sockets)
                {
                    checkedListBox2.SetItemChecked(s, true);
                }
            }

            if (curItemFilter?.Qualities?.Length > 0)
            {
                checkBox3.Checked = true;
                foreach (var s in curItemFilter.Qualities)
                {
                    checkedListBox1.SetItemChecked((int)(s) - 1, true);
                }
            }

            List<ComboBox> affix_combox = new List<ComboBox>
            {
                comboBox4,comboBox8,comboBox9
            };
            List<TextBox> affix_combox_value = new List<TextBox>
            {
                textBox1,textBox2,textBox3
            };

            List<CheckBox> affix_check = new List<CheckBox>
            {
                checkBox5,checkBox6,checkBox7
            };

            int count = 0;
            foreach (var affix in curItemFilter.GetType().GetProperties())
            {
                if (Affix.ItemAffix.ContainsKey(affix.Name))
                {
                    if (affix.GetValue(curItemFilter, null) == null)
                        continue;
                    affix_combox[count].SelectedIndex = comboBox4.Items.IndexOf(Affix.ItemAffix[affix.Name]);
                    affix_combox_value[count].Text = (affix.GetValue(curItemFilter, null)).ToString();
                    affix_check[count++].Checked = true;
                }
            }

            affix_combox = new List<ComboBox>
            {
                comboBox6,comboBox3,comboBox5,comboBox7
            };
            affix_combox_value = new List<TextBox>
            {
                textBox5,textBox4,textBox7,textBox6
            };

            affix_check = new List<CheckBox>
            {
                checkBox8,checkBox11,checkBox10,checkBox9
            };

            count = 0;
            foreach (var affix in curItemFilter.ClassSkills)
            {
                string key = affix.Key.ToString();
                if (key.Equals("Any"))
                    key = key + "_C";

                affix_combox[count].SelectedIndex = comboBox6.Items.IndexOf(Affix.SkillAffix.First(x => x.Value.Equals(key)).Key.ToString());
                affix_combox_value[count].Text = affix.Value.ToString();
                affix_check[count++].Checked = true;
            }

            foreach (var affix in curItemFilter.SkillTrees)
            {
                string key = affix.Key.ToString();
                if (key.Equals("Any"))
                    key += "_T";

                affix_combox[count].SelectedIndex = comboBox6.Items.IndexOf(Affix.SkillAffix.First(x => x.Value.Equals(key)).Key.ToString());//;
                affix_combox_value[count].Text = affix.Value.ToString();
                affix_check[count++].Checked = true;
            }

            foreach (var affix in curItemFilter.Skills)
            {
                string key = affix.Key.ToString();
                if (key.Equals("Any"))
                    key += "_S";

                affix_combox[count].SelectedIndex = comboBox6.Items.IndexOf(Affix.SkillAffix.First(x => x.Value.Equals(key)).Key.ToString());//;
                affix_combox_value[count].Text = affix.Value.ToString();
                affix_check[count++].Checked = true;
            }

            if (curItemFilter.SkillCharges.Count > 0)
            {
                comboBox10.SelectedIndex = comboBox10.Items.IndexOf(Affix.SkillAffix.First(x => x.Value.Equals(curItemFilter.SkillCharges.First().Key.ToString())).Key.ToString());//;
                checkBox4.Checked = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // save class filter
            curItemFilter = new ItemFilter();

            if (comboBox12.SelectedIndex == -1)
            {
                MessageBox.Show("请先添加规则");
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("先选择物品种类");
                return;
            }

            if (checkBox13.Checked)
            {
                if (checkedListBox3.CheckedItems.Count == 0)
                {
                    MessageBox.Show("必须选一个物品级别");
                    return;
                }
                curItemFilter.Tiers = new ItemTier[checkedListBox3.CheckedItems.Count];
                int index = 0;
                for (int i = 0; i < checkedListBox3.Items.Count; i++)
                {
                    if (checkedListBox3.GetItemChecked(i))
                        curItemFilter.Tiers[index++] = (ItemTier)i;
                }
            }
            else
            {
                MessageBox.Show("必须设置物品级别");
                return;
            }

            if (checkBox1.Checked)
            {
                if (comboBox2.SelectedIndex == -1)
                {
                    MessageBox.Show("有形无形必须选一种");
                    return;
                }
                if (comboBox2.SelectedIndex == 0)
                    curItemFilter.Ethereal = false;
                if (comboBox2.SelectedIndex == 1)
                    curItemFilter.Ethereal = true;
            }

            if (checkBox14.Checked)
            {
                curItemFilter.CheckVendor = true;
            }
            else
            {
                curItemFilter.CheckVendor = false;
            }

            if (checkBox2.Checked)
            {
                if (checkedListBox2.CheckedItems.Count == 0)
                {
                    MessageBox.Show("必须选一个孔数");
                    return;
                }
                curItemFilter.Sockets = new int[checkedListBox2.CheckedItems.Count];
                int index = 0;
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    if (checkedListBox2.GetItemChecked(i))
                        curItemFilter.Sockets[index++] = i;
                }
            }

            if (checkBox3.Checked)
            {
                if (checkedListBox1.CheckedItems.Count == 0)
                {
                    MessageBox.Show("必须选一个质量");
                    return;
                }

                curItemFilter.Qualities = new ItemQuality[checkedListBox1.CheckedItems.Count];
                int index = 0;
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.GetItemChecked(i))
                        curItemFilter.Qualities[index++] = (ItemQuality)(i + 1);
                }
            }

            if (checkBox5.Checked)
                if (comboBox4.SelectedIndex != -1)
                {
                    string affix = Affix.ItemAffix.First(q => q.Value == comboBox4.SelectedItem.ToString()).Key;
                    if (textBox1.Text.Trim() == "")
                    {
                        MessageBox.Show("词缀必须填数值");
                        return;
                    }
                    curItemFilter.GetType().GetProperty(affix).SetValue(curItemFilter, int.Parse(textBox1.Text));
                }

            if (checkBox6.Checked)
                if (comboBox8.SelectedIndex != -1)
                {
                    string affix = Affix.ItemAffix.First(q => q.Value == comboBox8.SelectedItem.ToString()).Key;
                    if (textBox2.Text.Trim() == "")
                    {
                        MessageBox.Show("词缀必须填数值");
                        return;
                    }
                    curItemFilter.GetType().GetProperty(affix).SetValue(curItemFilter, int.Parse(textBox2.Text));
                }

            if (checkBox7.Checked)
                if (comboBox9.SelectedIndex != -1)
                {
                    string affix = Affix.ItemAffix.First(q => q.Value == comboBox9.SelectedItem.ToString()).Key;
                    if (textBox3.Text.Trim() == "")
                    {
                        MessageBox.Show("词缀必须填数值");
                        return;
                    }
                    curItemFilter.GetType().GetProperty(affix).SetValue(curItemFilter, int.Parse(textBox3.Text));
                }

            // reset all
            curItemFilter.ClassSkills.Clear();
            curItemFilter.SkillTrees.Clear();
            curItemFilter.Skills.Clear();

            if (checkBox8.Checked)
                if (!addSkillFromUI(comboBox6, textBox5, ref curItemFilter))
                    return;
            if (checkBox11.Checked)
                if (!addSkillFromUI(comboBox3, textBox4, ref curItemFilter))
                    return;
            if (checkBox10.Checked)
                if (!addSkillFromUI(comboBox5, textBox7, ref curItemFilter))
                    return;
            if (checkBox9.Checked)
                if (!addSkillFromUI(comboBox7, textBox6, ref curItemFilter))
                    return;

            if (checkBox4.Checked)
                if (comboBox10.SelectedIndex != -1)
                {
                    // normal skill
                    // skill tree
                    string name = Affix.SkillAffix.First(q => q.Key == comboBox10.SelectedItem.ToString()).Value;
                    Skill playerClass = (Skill)Enum.Parse(typeof(Skill), name);
                    curItemFilter.SkillCharges.Add(playerClass, 1);
                }


            curClassFilters[comboBox12.SelectedIndex] = curItemFilter;
            sumRules[curSelectClassItem] = curClassFilters;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // delete class filter
            if (comboBox12.SelectedIndex == -1)
            {
                MessageBox.Show("请先选择规则");
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("先选择物品种类");
                return;
            }

            curClassFilters.RemoveAt(comboBox12.SelectedIndex);
            if (curClassFilters.Count == 0)
                sumRules.Remove(curSelectClassItem);
            else
                sumRules[curSelectClassItem] = curClassFilters;

            comboBox12.Items.RemoveAt(comboBox12.SelectedIndex);
            for(int i=0;i<comboBox12.Items.Count;i++)
            {
                comboBox12.Items[i] = (i+1).ToString();
            }

            label9.Text = "规则个数：" + curClassFilters.Count;
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox12.Checked)
            {
                listBox1.Enabled = false;
                checkBox13.Checked = true;
            }
            else
            {
                listBox1.Enabled = true;
                checkBox13.Checked = false;
            }
        }
    }
}
