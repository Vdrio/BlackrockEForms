using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Security;
using Xamarin.Forms;

namespace VdrioEForms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            byte[] iv = Convert.FromBase64String(AESEncryptor.CreateInitializor());
            string encryptedString = AESEncryptor.EncryptTo64String("This is a test string being encrypted", iv);
            Debug.WriteLine("Encrypted String:" + encryptedString);
            MyLabel.Text = encryptedString;
            string decryptedString = AESEncryptor.Decrypt(Convert.FromBase64String(encryptedString), iv);
            MyLabel.Text += "Decrypted: " + decryptedString;
            
        }
    }
}
