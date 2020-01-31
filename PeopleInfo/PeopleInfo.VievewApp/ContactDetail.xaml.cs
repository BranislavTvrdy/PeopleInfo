using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PeopleInfo.Database;
using QRCoder;

namespace PeopleInfo.VievewApp
{
    /// <summary>
    /// Interaction logic for ContactDetail.xaml
    /// </summary>
    public partial class ContactDetail : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public ContactDetail(Person paPerson, string paDepartment, string paRoom)
        {
            InitializeComponent();
            LFirstName.Content = paPerson.FirstName;
            LLastName.Content = paPerson.LastName;
            LPhone.Content = paPerson.Phone;
            LEmail.Content = paPerson.Email;
            LDepartment.Content = paDepartment;
            LRoom.Content = paRoom;

            PayloadGenerator.ContactData generator = new PayloadGenerator.ContactData(PayloadGenerator.ContactData.ContactOutputType.VCard3,
                paPerson.FirstName, paPerson.LastName,null,paPerson.Phone,null,null,paPerson.Email);
            string payload = generator.ToString();


            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            var qrCodeAsBitmap = qrCode.GetGraphic(20);

            using (var memory = new MemoryStream())
            {
                qrCodeAsBitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                ImageQR.Source = bitmapImage;
            }

            ImageQR.Stretch = Stretch.Uniform;
            //ImageQR.Source = retval;
        }
    }
}
