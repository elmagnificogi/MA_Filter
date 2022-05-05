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
        List<ItemFilter> curItemFilters = new List<ItemFilter>();
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

            checkBox1.Checked = false;
            comboBox2.SelectedIndex = -1;
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

            MessageBox.Show("导出成功，请替换MapAssist文件夹下的ItemFilter.yaml，并重启MapAssist验证");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // add all class data to ui
            foreach(var item in Items.ItemClassesToChinese)
            {
                comboBox1.Items.Add(item.Value);
            }

            // add all skill

            // add all affix stats

            // load all json file
            Localization.LoadLocalizationFile();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Item curItem = Items.ItemClassesToChinese.First(q => q.Value == comboBox1.SelectedItem.ToString()).Key;

            //// add a global filter
            //if(curItem < Item.ClassRings)
            //{
            //    listBox1.Items.Add(curItem);
            //}

            Item[] items = Items.ItemClasses[curItem];
            foreach(var item in items)
            {
                string key = Items._ItemCodes[(uint)item];

                listBox1.Items.Add(Items.LocalizedItems[key].zhTW);
            }
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
                if (comboBox2.SelectedIndex == 0)
                    curItemFilter.Ethereal = false;
                if (comboBox2.SelectedIndex == 1)
                    curItemFilter.Ethereal = true;
            }

            if(checkBox2.Checked)
            {
                curItemFilter.Sockets = new int[checkedListBox2.CheckedItems.Count];
                int index = 0;
                for(int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    if (checkedListBox2.GetItemChecked(i))
                        curItemFilter.Sockets[index++] = i;
                }
            }

            if (checkBox3.Checked)
            {
                curItemFilter.Qualities = new ItemQuality[checkedListBox1.CheckedItems.Count];
                int index = 0;
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.GetItemChecked(i))
                        curItemFilter.Qualities[index++] = (ItemQuality)(i +2);
                }
            }

            curItemFilters[comboBox11.SelectedIndex] = curItemFilter;
            sumRules[curSelectItem] = curItemFilters;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox11.Items.Clear();
            Item curItem;
            Item curClass;
            //if (comboBox1.SelectedIndex<32)
            //{
            //    curClass = Items.ItemClassesToChinese.First(q => q.Value == comboBox1.SelectedItem.ToString()).Key;
            //    if(listBox1.SelectedIndex == 0)
            //    {
            //        curItem = (Item)((uint)Item.ClassAxes + comboBox1.SelectedIndex);
            //    }
            //    else
            //    {
            //        curItem = Items.ItemClasses[curClass][listBox1.SelectedIndex];
            //    }
            //}
            //else
            //{
                curClass = Items.ItemClassesToChinese.First(q => q.Value == comboBox1.SelectedItem.ToString()).Key;
                curItem = Items.ItemClasses[curClass][listBox1.SelectedIndex];
            //}

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
                    checkedListBox1.SetItemChecked((int)(s) - 2,true);
                }
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
            label2.Text = "规则个数：" + curItemFilters.Count;
        }
    }
}
