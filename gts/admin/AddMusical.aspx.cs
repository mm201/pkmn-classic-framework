using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;

namespace PkmnFoundations.GTS.admin
{
    public partial class AddMusical : System.Web.UI.Page
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
            if (data.Length < 0x248)
            {
                Fail(); return;
            }

            if (data[0x04] != 0x09 ||
                data[0x05] != 0x52 ||
                data[0x06] != 0x00 ||
                data[0x07] != 0x00)
            {
                Fail(); return;
            }

            int results = BitConverter.ToInt32(data, 0x08);
            if (data.Length != 12 + 572 * results)
            {
                Fail(); return;
            }

            int added = 0;

            for (int x = 0; x < results; x++)
            {
                int pid = BitConverter.ToInt32(data, 12 + 572 * x);
                long serial = BitConverter.ToInt64(data, 16 + 572 * x);
                if (serial == 0) continue;

                byte[] result = new byte[560];
                Array.Copy(data, 24 + 572 * x, result, 0, 560);

                MusicalRecord5 record = new MusicalRecord5(pid, serial, result);
                if (DataAbstract.Instance.MusicalUpload5(record) != 0) added++;
            }

            litMessage.Text = "Added " + added.ToString() + " musical photos to the database.";
        }

        private void Fail()
        {
            litMessage.Text = "There was an error with the data.";
        }
    }
}