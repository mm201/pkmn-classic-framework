using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using PkmnFoundations.Structures;
using PkmnFoundations.Data;

namespace PkmnFoundations.GTS.test
{
    public partial class AddDressup : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            litMessage.Text = "";
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            byte[] data = fuBox.FileBytes;
            if (data.Length < 0xf8)
            {
                Fail(); return;
            }

            Common.CryptMessage(data);
            if (data[0x04] != 0x21 || 
                data[0x05] != 0x4e ||
                data[0x06] != 0x00 ||
                data[0x07] != 0x00)
            {
                Fail(); return;
            }

            int results = BitConverter.ToInt32(data, 0x08);
            if (data.Length != 12 + 236 * results)
            {
                Fail(); return;
            }

            int added = 0;

            for (int x = 0; x < results; x++)
            {
                int pid = BitConverter.ToInt32(data, 12 + 236 * x);
                ulong serial = BitConverter.ToUInt64(data, 16 + 236 * x);
                if (serial == 0) continue;

                byte[] result = new byte[224];
                Array.Copy(data, 24 + 236 * x, result, 0, 224);

                DressupRecord4 record = new DressupRecord4(pid, serial, result);
                if (DataAbstract.Instance.DressupUpload4(record) != 0) added++;
            }

            litMessage.Text = "Added " + added.ToString() + " dressup photos to the database.";
        }

        private void Fail()
        {
            litMessage.Text = "There was an error with the data.";
        }
    }
}
