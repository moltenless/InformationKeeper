using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace InformationKeeper
{
    partial class AddDataControl : UserControl
    {
        Account account;
        MainForm form;
        internal AddDataControl(Account account, MainForm form)
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            this.account = account;
            this.form = form;
        }

        private void ChangeCheckState_Click(object sender, EventArgs e)
        {
            foreach (var el in Controls)
                if (el is CheckBox box)
                    box.Checked = false;
            (sender as CheckBox).Checked = true;

            PanelInfo.Controls.Clear();
            Controls.RemoveByKey("add");
            Controls.RemoveByKey("remove");

            AddTextBoxAndLabel("Name:", "name", false);
            AddTextBoxAndLabel("Description:", "description", true, DateTime.Now.ToString());
            switch ((string)(sender as CheckBox).Tag)
            {
                case "account info":
                    LabelDataType.Text = "Account information";
                    for (int i = 1; ; i++)
                        if (!InformationKeeper.DataExists(account, (d) => d.Name == "account" + i))
                        {
                            PanelInfo.Controls["name"].Text = "account" + i;
                            break;
                        }

                    AddTextBoxAndLabel("Name:", "login", false);
                    AddTextBoxAndLabel("Password:", "password", false);
                    AddTextBoxAndLabel("Email:", "email", false);
                    AddTextBoxAndLabel("Phone number:", "number", false);
                    break;

                case "note":
                    LabelDataType.Text = "Note";
                    for (int i = 1; ; i++)
                        if (!InformationKeeper.DataExists(account, (d) => d.Name == "Note" + i))
                        {
                            PanelInfo.Controls["name"].Text = "Note" + i;
                            break;
                        }
                    AddTextBoxAndLabel("Content:", "content", true);
                    break;

                case "album":
                    LabelDataType.Text = "Picture album";
                    for (int i = 1; ; i++)
                        if (!InformationKeeper.DataExists(account, (d) => d.Name == "album" + i))
                        {
                            PanelInfo.Controls["name"].Text = "album" + i;
                            break;
                        }

                    Button buttonRemove = new Button { Name = "remove", Text = "Del. im.", FlatStyle = FlatStyle.Flat, Enabled = false, Font = new Font("Tahoma", 7, FontStyle.Regular), Location = new Point(PanelInfo.Controls["name"].Location.X + PanelInfo.Controls["name"].Width / 2 + 40, Height - 90), Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Height = 25, Width = 85 };
                    PanelInfo.Controls["name"].SizeChanged += (sender2, e2) => { buttonRemove.Location = new Point(PanelInfo.Controls["name"].Location.X + PanelInfo.Controls["name"].Width / 2 + 40, Height - 90); };
                    buttonRemove.Click += RemovePictureBox_Click;
                    Controls.Add(buttonRemove);

                    Button buttonAdd = new Button { Name = "add", Text = "Add. im.", FlatStyle = FlatStyle.Flat, Font = new Font("Tahoma", 7, FontStyle.Regular), Location = new Point(PanelInfo.Controls["name"].Location.X + PanelInfo.Controls["name"].Width / 2 + 130, Height - 90), Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Height = 25, Width = 85 };
                    PanelInfo.Controls["name"].SizeChanged += (sender2, e2) => { buttonAdd.Location = new Point(PanelInfo.Controls["name"].Location.X + PanelInfo.Controls["name"].Width / 2 + 130, Height - 90); };
                    buttonAdd.Click += AddPictureBox_Click;
                    Controls.Add(buttonAdd);
                    break;
            }
            LabelDataType.Visible = true;
            ButtonSave.Visible = true;
        }

        void AddPictureBox_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Title = "Choose an image", Filter = "Image Files (*.png; *.jpg; *.bmp)|*.png; *.jpg; *.bmp" };
            fileDialog.ShowDialog();

            if (File.Exists(fileDialog.FileName))
            {
                PictureBox pictureBox = new PictureBox { Tag = Image.FromFile(fileDialog.FileName), Height = 400, Width = PanelInfo.Controls["name"].Width, Image = Image.FromFile(fileDialog.FileName), SizeMode = PictureBoxSizeMode.AutoSize, Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top };
                pictureBox.Click += (sender2, e2) =>
                {
                    foreach (Control control in PanelInfo.Controls)
                        if (control is PictureBox picture)
                            picture.BorderStyle = BorderStyle.None;
                    if ((sender2 as PictureBox).BorderStyle == BorderStyle.None)
                    {
                        (sender2 as PictureBox).BorderStyle = BorderStyle.FixedSingle;
                        Controls["remove"].Enabled = true;
                    }
                    else
                    {
                        (sender2 as PictureBox).BorderStyle = BorderStyle.None;
                        Controls["remove"].Enabled = false;
                    }
                };

                if (pictureBox.Image.Height > 1000 || pictureBox.Image.Width > 1000)
                {
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox.Size = new Size(PanelInfo.Controls["name"].Width, 1000);
                }

                foreach (Control control in PanelInfo.Controls)
                    if (control.Location.Y + control.Height > pictureBox.Location.Y)
                        pictureBox.Location = new Point(PanelInfo.Controls["name"].Location.X, control.Location.Y + control.Height + 30);

                PanelInfo.Controls.Add(pictureBox);
                PanelInfo.ScrollControlIntoView(pictureBox);
            }
        }

        void RemovePictureBox_Click(object sender, EventArgs e)
        {
            foreach (Control control in PanelInfo.Controls)
                if (control is PictureBox picture && picture.BorderStyle == BorderStyle.FixedSingle)
                {
                    int index = PanelInfo.Controls.IndexOf(picture);
                    PanelInfo.Controls.Remove(picture);

                    for (int i = index; i < PanelInfo.Controls.Count; i++)
                            PanelInfo.Controls[i].Location = new Point(PanelInfo.Controls[i].Location.X, PanelInfo.Controls[i].Location.Y - picture.Height - 30);

                    Controls["remove"].Enabled = false;
                    return;
                }
        }

        void AddTextBoxAndLabel(string labelText, string textBoxName, bool multiLine, string textBoxText = "")
        {
            TextBox textBox = new TextBox { Name = textBoxName, Text = textBoxText, Font = new Font("Tahoma", 9.75F, FontStyle.Regular), BorderStyle = BorderStyle.FixedSingle, BackColor = SystemColors.Control, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Multiline = multiLine, Height = multiLine ? 112 : 23 };
            Label label = new Label { Text = labelText, Location = new Point(18, 47), Font = new Font("Tahoma", 14.25F, FontStyle.Regular), AutoSize = true };

            foreach (Control control in PanelInfo.Controls)
                if (control.Location.Y + control.Height > label.Location.Y)
                {
                    label.Location = new Point(18, control.Location.Y + control.Height + 30);
                }

            textBox.Tag = label;
            PanelInfo.Controls.AddRange(new Control[] { label, textBox });

            label.BringToFront();
            textBox.BringToFront();

            foreach (Control control in PanelInfo.Controls)
                if (control is TextBox)
                {
                    control.Location = new Point((control.Tag as Label).Location.X + (control.Tag as Label).Width + 5, (control.Tag as Label).Location.Y + 3);
                    control.Width = PanelInfo.Width - control.Location.X - 20;
                }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            string name = PanelInfo.Controls["name"].Text;
            string description = PanelInfo.Controls["description"].Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || InformationKeeper.DataExists(account, (data) => data.Name == name))
            {
                MessageBox.Show("Укажите имя и описание, имя не должно совпадать с другими", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                if (CheckBoxAccountInfo.Checked)
                {
                    AccountInfo data = new AccountInfo(name, description, PanelInfo.Controls["login"].Text, PanelInfo.Controls["password"].Text, PanelInfo.Controls["email"].Text, PanelInfo.Controls["number"].Text);
                    InformationKeeper.SaveData(account, data);
                    form.LoadDataList();
                    form.LoadDataInfo(data);
                }
                else if (CheckBoxNote.Checked)
                {
                    Note data = new Note(name, description, PanelInfo.Controls["content"].Text);
                    InformationKeeper.SaveData(account, data);
                    form.LoadDataList();
                    form.LoadDataInfo(data);
                }
                else if (CheckBoxPhotoAlbum.Checked)
                {
                    List<Image> images = new List<Image>();

                    foreach (Control control in PanelInfo.Controls)
                        if (control is PictureBox pictureBox)
                            images.Add(control.Tag as Image);

                    Album data = new Album(PanelInfo.Controls["name"].Text, PanelInfo.Controls["description"].Text, images.ToArray());
                    InformationKeeper.SaveData(account, data);
                    form.LoadDataList();
                    form.LoadDataInfo(data);
                }
            }
        }
    }
}
