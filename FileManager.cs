﻿using System.Text;
using System;
using System.IO;
using System.Diagnostics;

namespace MapAssist.Files
{
    public class FileManager
    {
        private string _filePathRelative;
        private string _fullPath;

        public FileManager(string fullpath)
        {
            _filePathRelative = Path.GetFileName(fullpath);
            _fullPath = fullpath;//System.IO.Directory.GetCurrentDirectory() + _filePathRelative.Substring(1);
        }

        public string GetPath() { return _filePathRelative; }
        public string GetAbsolutePath() { return _fullPath; }
        public bool FileExists()
        {
            return System.IO.File.Exists(_fullPath);
        }

        public void CreateFile()
        {
            if (FileExists())
            {
                throw new Exception($"Trying to create {_fullPath} even though file exists..");
            }

            try
            {
                System.IO.File.Create(_fullPath).Close();
            }
            catch (Exception e)
            {
                throw new Exception($"Trying to create {_fullPath} : {e.Message}");
            }
        }

        public void DeleteFile()
        {
            try
            {
                if (FileExists())
                {
                    System.IO.File.Delete(_fullPath);
                    Debug.WriteLine($"Removed {_fullPath}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e, $"Tried to remove {_fullPath} but got error.");
            }
        }

        public string ReadFile()
        {
            var sb = new StringBuilder();
            try
            {
                // Open the file to read from.
                using (StreamReader sr = File.OpenText(GetPath()))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        sb.Append($"{s}\n");
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Debug.WriteLine(e, $"The file {_filePathRelative} could not be read.");
            }
            if (sb.ToString().Length == 0)
            {
                Debug.WriteLine($"The file {_filePathRelative} was empty...");
            }

            return sb.ToString();
        }

        public bool WriteFile(string content)
        {
            try
            {
                using (StreamWriter sw = File.CreateText(_filePathRelative))
                {
                    sw.WriteLine(content);
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Debug.WriteLine(e, $"The file {_filePathRelative} could not be written.");
                return false;
            }
            return true;
        }
    }
}
