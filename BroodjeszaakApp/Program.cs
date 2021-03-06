﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BroodjeszaakLib;

namespace BroodjeszaakApp {
    static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            //Check argument info
            var pricelist = new PriceList();
            if (args.Length < 1 || !pricelist.LoadFromFile(args[0])) {
                var result = MessageBox.Show("Something went wrong while reading the file...\n\nContinue?",
                                             "Warning",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Warning);
                if (result == DialogResult.Abort || result == DialogResult.Cancel || result == DialogResult.No)
                    return;
            }

            // Show form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(pricelist));
        }
    }
}
