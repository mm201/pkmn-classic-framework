using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Structures;
using PkmnFoundations.Data;
using PkmnFoundations.Web;

namespace PkmnFoundations.GTS.admin
{
    public partial class AddBoxes : System.Web.UI.Page
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
            if (data[0x04] != 0x09 ||
                data[0x05] != 0x52 ||
                data[0x06] != 0x00 ||
                data[0x07] != 0x00)
            {
                Fail(); return;
            }

            int results = BitConverter.ToInt32(data, 0x08);
            if (data.Length != 12 + 556 * results)
            {
                Fail(); return;
            }

            int added = 0;

            for (int x = 0; x < results; x++)
            {
                int pid = BitConverter.ToInt32(data, 12 + 556 * x);
                BoxLabels4 label = (BoxLabels4)BitConverter.ToInt32(data, 16 + 556 * x);
                ulong serial = BitConverter.ToUInt64(data, 20 + 556 * x);
                if (serial == 0) continue;

                byte[] result = new byte[540];
                Array.Copy(data, 28 + 556 * x, result, 0, 540);

                BoxRecord4 record = new BoxRecord4(pid, label, serial, result);
                if (Database.Instance.BoxUpload4(record) != 0) added++;
            }

            litMessage.Text = "Added " + added.ToString() + " boxes to the database.";
        }

        private void Fail()
        {
            litMessage.Text = "There was an error with the data.";
        }
    }
}