using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{

    public class FileManager
    {
        private string currentDirectory =
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private ListView listView;
        public FileManager(ListView listView)
        {
            this.listView = listView;
            LoadFilesAndFolders();
        }
        private void LoadFilesAndFolders()
        {
            listView.Items.Clear();
            var files = Directory.GetFiles(currentDirectory, "*", SearchOption.AllDirectories);
            var directories = Directory.GetDirectories(currentDirectory, "*",
            SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                listView.Items.Add(new ListViewItem(
                new[] { fileInfo.Name, fileInfo.LastWriteTime.ToString(), fileInfo.Length.ToString()
                }));
            }
            foreach (var dir in directories)
            {
                var dirInfo = new DirectoryInfo(dir);
                listView.Items.Add(new ListViewItem(
                new[] { dirInfo.Name, dirInfo.LastWriteTime.ToString(), "Папка" })
                { ForeColor = System.Drawing.Color.Blue });
            }
        }
        public void CreateFile()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.InitialDirectory = currentDirectory;
                dialog.Title = "Создать файл";
                dialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.Create(dialog.FileName).Dispose();
                    LoadFilesAndFolders();
                }
            }
        }
        public void DeleteFile()
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Сначала выберите файл или папку для удаления.");
                return;
            }
            var selectedItem = listView.SelectedItems[0];
            var path = Path.Combine(currentDirectory, selectedItem.Text);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            LoadFilesAndFolders();
        }
        public void RenameFile()
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Сначала выберите файл или папку для переименования.");
                return;
            }
            var selectedItem = listView.SelectedItems[0];
            var oldPath = Path.Combine(currentDirectory, selectedItem.Text);
            using (var dialog = new SaveFileDialog())
            {
                dialog.InitialDirectory = currentDirectory;
                dialog.Title = "Переименовать";
                dialog.Filter = "*.*|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var newPath = dialog.FileName;
                    if (File.Exists(oldPath))
                    {
                        File.Move(oldPath, newPath);
                    }
                    else if (Directory.Exists(oldPath))
                    {
                        Directory.Move(oldPath, newPath);
                    }
                    LoadFilesAndFolders();
                }
            }
        }
        public void SortByDate()
        {
            var files = Directory.GetFiles(currentDirectory, "*", SearchOption.AllDirectories)
            .OrderBy(f => new FileInfo(f).LastWriteTime)
            .ToList();
            var directories = Directory.GetDirectories(currentDirectory, "*",
            SearchOption.AllDirectories)
            .OrderBy(d => new DirectoryInfo(d).LastWriteTime)
            .ToList();
            listView.Items.Clear();
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                listView.Items.Add(new ListViewItem(
                new[] { fileInfo.Name, fileInfo.LastWriteTime.ToString(), fileInfo.Length.ToString()
                }));
            }
            foreach (var dir in directories)
            {
                var dirInfo = new DirectoryInfo(dir);
                listView.Items.Add(new ListViewItem(
                new[] { dirInfo.Name, dirInfo.LastWriteTime.ToString(), "Папка" })
                { ForeColor = System.Drawing.Color.Blue });
            }
        }
        public void SortByName()
        {
            var files = Directory.GetFiles(currentDirectory, "*", SearchOption.AllDirectories)
            .OrderBy(f => Path.GetFileName(f))
            .ToList();
            var directories = Directory.GetDirectories(currentDirectory, "*",
            SearchOption.AllDirectories)
            .OrderBy(d => Path.GetFileName(d))
            .ToList();
            listView.Items.Clear();
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                listView.Items.Add(new ListViewItem(
                new[] { fileInfo.Name, fileInfo.LastWriteTime.ToString(), fileInfo.Length.ToString()
                }));
            }
            foreach (var dir in directories)
            {
                var dirInfo = new DirectoryInfo(dir);
                listView.Items.Add(new ListViewItem(
                new[] { dirInfo.Name, dirInfo.LastWriteTime.ToString(), "Папка" })
                { ForeColor = System.Drawing.Color.Blue });
            }
        }
    }
    public class FileManagementForm : Form
    {
        private FileManager fileManager;
        private ListView listView;
        private Button createFileButton;
        private Button deleteButton;
        private Button renameButton;
        private Button sortByDateButton;
        private Button sortByNameButton;
        public FileManagementForm()
        {
            this.Text = "Управление файлами и папками";
            this.Width = 800;
            this.Height = 600;
            CreateControls();
        }
        private void CreateControls()
        {
            listView = new ListView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(760, 400),
                View = View.Details,
                FullRowSelect = true
            };
            listView.Columns.Add("Имя", 200);
            listView.Columns.Add("Дата изменения", 150);
            listView.Columns.Add("Размер/Тип", 150);
            createFileButton = new Button
            {
                Location = new System.Drawing.Point(10, 420),
                Text = "Создать файл",
                Size = new System.Drawing.Size(100, 25)
            };
            createFileButton.Click += CreateFileButton_Click;
            deleteButton = new Button
            {
                Location = new System.Drawing.Point(120, 420),
                Text = "Удалить",
                Size = new System.Drawing.Size(100, 25)
            };
            deleteButton.Click += DeleteButton_Click;
            renameButton = new Button
            {
                Location = new System.Drawing.Point(230, 420),
                Text = "Переименовать",
                Size = new System.Drawing.Size(100, 25)
            };
            renameButton.Click += RenameButton_Click;
            sortByDateButton = new Button
            {
                Location = new System.Drawing.Point(340, 420),
                Text = "Сортировать по дате",
                Size = new System.Drawing.Size(120, 25)
            };
            sortByDateButton.Click += SortByDateButton_Click;
            sortByNameButton = new Button
            {
                Location = new System.Drawing.Point(470, 420),
                Text = "Сортировать по имени",
                Size = new System.Drawing.Size(120, 25)
            };
            sortByNameButton.Click += SortByNameButton_Click;
            this.Controls.Add(listView);
            this.Controls.Add(createFileButton);
            this.Controls.Add(deleteButton);
            this.Controls.Add(renameButton);
            this.Controls.Add(sortByDateButton);
            this.Controls.Add(sortByNameButton);
            fileManager = new FileManager(listView);
        }
        private void CreateFileButton_Click(object sender, EventArgs e)
        {
            fileManager.CreateFile();
        }
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            fileManager.DeleteFile();
        }
        private void RenameButton_Click(object sender, EventArgs e)
        {
            fileManager.RenameFile();
        }
        private void SortByDateButton_Click(object sender, EventArgs e)
        {
            fileManager.SortByDate();
        }
        private void SortByNameButton_Click(object sender, EventArgs e)
        {
            fileManager.SortByName();
        }
        }
    }
