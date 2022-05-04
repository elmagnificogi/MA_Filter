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
using YamlDotNet.Serialization;

namespace MA_Filter
{
    public partial class Form1 : Form
    {
        //List<ItemFilter> rules = new List<ItemFilter>();
        Dictionary<Item, List<ItemFilter>> sumRules = new Dictionary<Item, List<ItemFilter>>();
        Item curSelectItem;
        List<ItemFilter> curItemFilters;
        ItemFilter curItemFilter;

        public Form1()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
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

            StreamWriter yamlWriter = File.CreateText(file);
            Serializer yamlSerializer = new Serializer();
            yamlSerializer.Serialize(yamlWriter, sumRules);
            yamlWriter.Close();
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
            Item curItem = Items.ItemClassesToChinese.First(q => q.Value == comboBox1.SelectedItem.ToString()).Key;

            Item[] items = Items.ItemClasses[curItem];
            listBox1.Items.Clear();
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
            curItemFilter = new ItemFilter();
            curItemFilters.Add(curItemFilter);
            comboBox11.Items.Add(comboBox11.Items.Count + 1);
            comboBox11.SelectedIndex = comboBox11.Items.Count-1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox11.SelectedIndex == -1)
            {
                MessageBox.Show("请先添加规则");
                return;
            }

            curItemFilter.Ethereal = checkBox1.Checked;

            if(checkBox2.Checked)
            {
                curItemFilter.Sockets = new int[1];
                curItemFilter.Sockets[0] = comboBox5.SelectedIndex;
            }

            curItemFilter.Qualities = new ItemQuality[1];
            curItemFilter.Qualities[0] = (ItemQuality)(comboBox2.SelectedIndex + 2);
            curItemFilters[comboBox11.SelectedIndex] = curItemFilter;
            sumRules[curSelectItem] = curItemFilters;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox11.Items.Clear();
            Item curClass = Items.ItemClassesToChinese.First(q => q.Value == comboBox1.SelectedItem.ToString()).Key;
            Item curItem = Items.ItemClasses[curClass][listBox1.SelectedIndex];
            curSelectItem = curItem;
            if (sumRules.ContainsKey(curSelectItem))
                curItemFilters = sumRules[curSelectItem];
            else
                curItemFilters = new List<ItemFilter>();

            int count = curItemFilters.Count;
            for (int i = 0; i < count; i++)
                comboBox11.Items.Add(i+1);
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
            curItemFilter = curItemFilters[comboBox11.SelectedIndex];

            if(curItemFilter.Ethereal != null)
                checkBox1.Checked = (bool)curItemFilter.Ethereal;

            if(curItemFilter?.Sockets?.Length > 0)
            {
                checkBox2.Checked = true;
                comboBox5.SelectedIndex = curItemFilter.Sockets[0];
            }
            if(curItemFilter?.Qualities?.Length > 0)
                comboBox2.SelectedIndex = (int)(curItemFilter.Qualities[0]-2);
        }
    }
}
